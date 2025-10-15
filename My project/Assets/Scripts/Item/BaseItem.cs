using Bingyan;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.Image;



/// <summary>
/// 放置到场景中的物体的基类
/// </summary>
public abstract class BaseItem : MonoBehaviour
{

    [Title("名字")] public string Title;
    [Title("描述")] public string Desciption;
    [Title("相对坐标")] public Vector2Int[] Slots;
    [Title("是否依赖平台")] public bool IfRely = false;
    [Title("是否为钢制")] public bool IfSteel = false;
    [Title("是否可带动物体")] public bool IfRoot = false;
    [Title("是否为攻击性物体")] public bool Attacktive = false;
    [SerializeField][Title("编辑时是否可旋转")] private bool rotatableInEdit = true;
    [HideInInspector] public int rotate = 0;
    protected bool isTriggered = false;
    Vector2Int[] originSlots;
    public Vector2Int[] Orient { get; private set; } =
        new Vector2Int[4] { Vector2Int.down, Vector2Int.right, Vector2Int.up, Vector2Int.left };

    public virtual bool Droppable
    {
        get
        {
            foreach (var slot in Slots)
            {
                if (EditManager.Instance.Outside(CurCell + slot)) return false;
                switch (MapManager.Instance.DisabledQuadrant)
                {
                    case 1:
                        if (slot.x + transform.position.x > 0 && slot.y + transform.position.y > 0) return false;
                        break;
                    case 2:
                        if (slot.x + transform.position.x < 0 && slot.y + transform.position.y > 0) return false;
                        break;
                    case 3:
                        if (slot.x + transform.position.x < 0 && slot.y + transform.position.y < 0) return false;
                        break;
                    case 4:
                        if (slot.x + transform.position.x > 0 && slot.y + transform.position.y < 0) return false;
                        break;
                }
                int x = MapManager.Instance.CurCell.x + slot.x;
                int y = MapManager.Instance.CurCell.y + slot.y;
                if (MapManager.Instance.Map[x, y]) return false;
            }
            return true;
        }
    }
    protected virtual void Awake()
    {
        originSlots = new Vector2Int[Slots.Length];
        for (int i = 0; i < Slots.Length; i++) originSlots[i] = Slots[i];
    }

    protected virtual void FixedUpdate()
    {
        if (isTriggered) Execute();
    }

    /// <summary>
    /// 执行逻辑
    /// </summary>
    protected abstract void Execute();

    /// <summary>
    /// 恢复初始的状态
    /// </summary>
    protected abstract void ResetState();


    /// <summary>
    /// 使物体进入运作状态
    /// </summary>
    public virtual void SetTriggered()
    {
        isTriggered = true;
    }

    /// <summary>
    /// 使物体进入停止状态
    /// </summary>
    public void SetDisabled()
    {
        isTriggered = false;
        ResetState();
    }

    /// <summary>
    /// 旋转方向
    /// </summary>
    public void Rotate()
    {
        if (!rotatableInEdit) return;
        transform.Rotate(transform.forward, 90);
        rotate = (rotate + 1) % 4;
        for (int i = 0; i < Slots.Length; i++)
        {
            switch (rotate)
            {
                case 0:
                    Slots[i] = originSlots[i];
                    break;
                case 1:
                    Slots[i] = new Vector2Int(-originSlots[i].y, originSlots[i].x);
                    break;
                case 2:
                    Slots[i] = new Vector2Int(-originSlots[i].x, -originSlots[i].y);
                    break;
                case 3:
                    Slots[i] = new Vector2Int(originSlots[i].y, -originSlots[i].x);
                    break;
            }
        }
    }
    protected Vector2Int CurCell => new(
        (int)(transform.position.x - MapManager.Instance.Origin.x),
        (int)(transform.position.y - MapManager.Instance.Origin.y));
    public virtual void OnDrop()
    {
        foreach (var slot in Slots) MapManager.Instance.Map
                [CurCell.x + slot.x, CurCell.y + slot.y] = gameObject;
    }
    public virtual void OnDestroy()
    {
        if (MapManager.Instance != null)
            Slots.ForEach(v =>
            {
                if (!EditManager.Instance.Outside(v + CurCell))
                    MapManager.Instance.Map[(CurCell + v).x, (CurCell + v).y] = null;
            });
    }

}
public abstract class BaseRootItem : BaseItem
{
    public void Connect()
    {
        if (transform.parent == null) return;
        foreach (var slot in Slots)
        {
            for (int i = 0; i < 4; i++)
            {
                var target = CurCell + slot + Orient[i];
                if (!EditManager.Instance.Outside(target) &&
                    MapManager.Instance.Map[target.x, target.y] &&
                    MapManager.Instance.Map[target.x, target.y].TryGetComponent(out BaseRootItem item) &&
                    item != this && item.transform.parent == null)
                {
                    item.transform.parent = transform.parent;
                    item.Connect();
                }
            }
        }
    }
}