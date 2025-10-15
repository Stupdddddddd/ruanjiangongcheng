using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EditManager : AbstractManager<EditManager>
{
    public override void Init()
    {
        base.Init();

        RecordCopy = new();
        foreach (var item in MapManager.Instance.Map)
            if (item && item.TryGetComponent(out BaseItem _))
                RecordCopy.TryAdd(item, null);
        ItemRecord = new(RecordCopy);

        GameSystem.Input.InEdit.Cancel.started += Cancel;
        GameSystem.Input.InEdit.Confirm.started += Confirm;
        GameSystem.Input.InEdit.TurnLeft.started += Left;
        GameSystem.Input.InEdit.TurnRight.started += Right;
    }
    private void OnDestroy()
    {
        GameSystem.Input.InEdit.Cancel.started -= Cancel;
        GameSystem.Input.InEdit.Confirm.started -= Confirm;
        GameSystem.Input.InEdit.TurnLeft.started -= Left;
        GameSystem.Input.InEdit.TurnRight.started -= Right;
    }
    private Dictionary<GameObject, UISlot> itemRecord;
    /// <summary>
    /// 记录了放置在场上的物体及其对应的槽位<br/>
    /// 存档建议使用<see cref="RecordCopy"/>
    /// </summary>
    public Dictionary<GameObject, UISlot> ItemRecord
    {
        get
        {
            Dictionary<GameObject, UISlot> newRecord = new();
            foreach (var item in itemRecord.Keys)
                if (item) newRecord.Add(item, itemRecord[item]);
            itemRecord = newRecord;
            return itemRecord;
        }
        set => itemRecord = value;
    }
    /// <summary>
    /// 用于回退的初始记录
    /// </summary>
    public Dictionary<GameObject, UISlot> RecordCopy { get; private set; }
    public void UpdateRecord() => RecordCopy = new(ItemRecord);
    public void RewindRecord()
    {
        var profile = GameSystem.ReadProfile();
        LevelManager.Instance.Round = profile.Record;
        var items = FindObjectsByType<BaseItem>(FindObjectsSortMode.None);
        for (int i = 0; i < items.Length; i++)
            if (items[i]) DestroyImmediate(items[i].gameObject);
        RecordCopy.Clear();
        var dict = GameSystem.Dict.GetDict();
        foreach (var info in profile.Items)
        {
            BaseItem item = Instantiate(dict[info.Title],
                new Vector2(info.Position[0], info.Position[1]),
                Quaternion.identity, null).GetComponent<BaseItem>();
            for (int i = 0; i < info.Orient; i++) item.Rotate();
            RecordCopy.Add(item.gameObject, null);
        }
        MapManager.Instance.Detect();
        ItemRecord = new(RecordCopy);
        DropAll();
    }
    public void RewindRecord(Profile profile)
    {
        var items = FindObjectsByType<BaseItem>(FindObjectsSortMode.None);
        for (int i = 0; i < items.Length; i++)
            if (items[i]) DestroyImmediate(items[i].gameObject);
        RecordCopy.Clear();
        var dict = GameSystem.Dict.GetDict();
        foreach (var info in profile.Items)
        {
            BaseItem item = Instantiate(dict[info.Title],
                new Vector2(info.Position[0], info.Position[1]),
                Quaternion.identity, null).GetComponent<BaseItem>();
            for (int i = 0; i < info.Orient; i++) item.Rotate();
            RecordCopy.Add(item.gameObject, null);
        }
        MapManager.Instance.Detect();
        ItemRecord = new(RecordCopy);
        DropAll();
    }
    public void DropAll()
    {
        foreach (var item in ItemRecord.Keys)
            item.GetComponent<BaseItem>().OnDrop();
    }
    public void ConnectAll()
    {
        foreach (var item in ItemRecord.Keys)
            if (item.TryGetComponent(out BaseRootItem root))
                root.Connect();
    }
    [SerializeField] private int itemCnt;
    public int ItemCnt => itemCnt;
    [SerializeField] private int targetCnt;
    public int TargetCnt => targetCnt;
    [SerializeField] private Color canDrop;
    [SerializeField] private Color cantDrop;
    [SerializeField] private Color hanging;
    private Image Rend => UIManager.Instance.Edit.Grid;
    private BaseItem item;
    private GameObject curItem;
    public GameObject CurItem
    {
        get => curItem;
        set
        {
            curItem = value;
            if (value) item = value.GetComponent<BaseItem>();
        }
    }
    public bool Outside(Vector2Int slot) =>
        slot.x < 0 || slot.x >= MapManager.Instance.CellCnt.x ||
        slot.y < 0 || slot.y >= MapManager.Instance.CellCnt.y;
    private bool IsDragging => CurItem != null;
    private bool Droppable => IsDragging && item.Droppable;

    private Color gridColor;
    private static readonly int shaderGridColorId = Shader.PropertyToID("_GridColor");

    private void Update()
    {
        // Rend.material.SetColor("_GridColor", hanging);
        if (!IsDragging) return;
        CurItem.transform.position = MapManager.Instance.CurPos;
        // Rend.material.SetColor("_GridColor", Droppable ? canDrop : cantDrop);
    }

    private void FixedUpdate()
    {
        if (!IsDragging) gridColor = Color.Lerp(gridColor, hanging, 0.1f);
        else gridColor = Color.Lerp(gridColor, Droppable ? canDrop : cantDrop, 0.1f);
        Rend.material.SetColor(shaderGridColorId, gridColor);
    }

    private void Cancel(InputAction.CallbackContext cbk)
    {
        if (!IsDragging) return;
        ItemRecord[CurItem].Used = false;
        // Rend.material.SetColor("_GridColor", canDrop);
        Destroy(CurItem);
    }
    private void Confirm(InputAction.CallbackContext cbk)
    {
        if (Droppable)
        {
            item.OnDrop();
            CurItem = null;
        }
        else if (MapManager.Instance.CurCell.x >= 0 && MapManager.Instance.CurCell.x < MapManager.Instance.Map.GetLength(0) &&
            MapManager.Instance.CurCell.y >= 0 && MapManager.Instance.CurCell.y < MapManager.Instance.Map.GetLength(1) &&
            MapManager.Instance.Map[MapManager.Instance.CurCell.x, MapManager.Instance.CurCell.y] && !IsDragging &&
            ItemRecord[MapManager.Instance.Map[MapManager.Instance.CurCell.x, MapManager.Instance.CurCell.y]])
        {
            CurItem = MapManager.Instance.Map[MapManager.Instance.CurCell.x, MapManager.Instance.CurCell.y];
            item.OnDestroy();
            ItemRecord.Keys.ForEach(k =>
            {
                if (k.transform.parent && k.transform.parent == CurItem.transform)
                    k.transform.parent = CurItem.transform.parent;
            });
        }
    }
    private void Left(InputAction.CallbackContext cbk) => Turn(true);
    private void Right(InputAction.CallbackContext cbk) => Turn(false);
    private void Turn(bool left)
    {
        if (!IsDragging) return;
        for (int i = 0; i < (left ? 1 : 3); i++) item.Rotate();
    }
}
