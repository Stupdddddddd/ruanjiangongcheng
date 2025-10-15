using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIEdit : MonoBehaviour
{
    [SerializeField] private GameObject slotPrefab;
    [SerializeField] private GameObject slotSpawn;
    [SerializeField] private GameObject masks;
    [SerializeField] private Text curCnt;
    [SerializeField] private Text tarCnt;
    private UISlot[] slots;
    private Image[] mask;
    public bool Ready
    {
        get
        {
            int cnt = 0;
            foreach (var slot in slots) if (slot.Used) cnt++;
            return cnt == EditManager.Instance.TargetCnt;
        }
    }
    public void Next()
    {
        if (Ready && !EditManager.Instance.CurItem) LevelManager.Instance.SwitchState<LevelManager.EurekaState>();
    }
    private List<List<GameObject>> roundItemRecord;
    private void Awake()
    {
        roundItemRecord = new();

        originPos = itemBar.transform.localPosition;
        moveFactor = 0;
        mask = masks.GetComponentsInChildren<Image>();
    }
    public void Init()
    {
        while (roundItemRecord.Count < LevelManager.Instance.Round)
            roundItemRecord.Add(GameSystem.Spawner.Draw(EditManager.Instance.ItemCnt));
        var items = roundItemRecord[LevelManager.Instance.Round - 1];
        slots = slotSpawn.GetComponentsInChildren<UISlot>();
        for (int i = 0; i < slots.Length; i++) Destroy(slots[i].gameObject);
        slots = new UISlot[items.Count];
        for (int i = 0; i < items.Count; i++)
        {
            slots[i] = Instantiate(slotPrefab, slotSpawn.transform).GetComponent<UISlot>();
            (slots[i].transform as RectTransform).sizeDelta = (slotSpawn.transform as RectTransform).rect.height * .8f * Vector2.one;
            slots[i].Img.sprite = items[i].GetComponent<SpriteRenderer>().sprite;
            slots[i].Item = items[i];
        }

        grid.material.SetInt("_Col", MapManager.Instance.Map.GetLength(0));
        grid.material.SetInt("_Row", MapManager.Instance.Map.GetLength(1));

        for (int i = 0; i < mask.Length; i++)
            mask[i].gameObject.SetActive(i + 1 == MapManager.Instance.DisabledQuadrant);

        curCnt.text = "0";
        tarCnt.text = EditManager.Instance.TargetCnt.ToString();
    }

    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private RectTransform itemBar;
    private float moveFactor;
    private Vector3 originPos;
    private float MoveFactor
    {
        get => moveFactor;
        set => moveFactor = Mathf.Clamp(value, 0f, 1f);
    }
    private const int distance = 168;
    private void FixedUpdate()
    {
        MoveFactor = UIManager.Instance.Eureka.Spread ? MoveFactor - moveSpeed * Time.fixedDeltaTime : MoveFactor + moveSpeed * Time.fixedDeltaTime;
        itemBar.localPosition = Vector3.Lerp(originPos, originPos + distance * Vector3.down, moveCurve.Evaluate(MoveFactor));

        int cnt = 0;
        foreach (var slot in slots) if (slot.Used) cnt++;
        curCnt.text = cnt.ToString();
    }

    [SerializeField] private Image grid;
    public Image Grid => grid;
}
