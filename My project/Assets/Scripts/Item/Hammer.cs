using Bingyan;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[TrackStatic]
public class Hammer : BaseItem
{
    [EditableStatic]
    /// <summary>
    /// 是否打开伤害
    /// </summary>
    public static bool Eureka { get; set; } = false;

    [EditableStatic]
    /// <summary>
    /// 是否具有破坏性
    /// </summary>
    public static bool Destructive { get; set; } = true;


    [Title("自由落体速度")][SerializeField] float FreeFallSpeed;
    Rigidbody2D Rb;
    float AngleAcc = 0f;
    bool isFreeFall = false;

    public override bool Droppable
    {
        get
        {
            if (!base.Droppable) return false;
            return !EditManager.Instance.Outside(CurCell) &&
                MapManager.Instance.Map[CurCell.x, CurCell.y] &&
                MapManager.Instance.Map[CurCell.x, CurCell.y].GetComponent<BaseItem>().IfRoot;
        }
    }
    public override void OnDrop()
    {
        base.OnDrop();
        transform.parent = MapManager.Instance.Map[CurCell.x, CurCell.y].transform;
    }

    protected override void Awake()
    {
        base.Awake();
        Rb = GetComponent<Rigidbody2D>();
    }

    protected override void Execute()
    {
        if (!isFreeFall) transform.Rotate(0, 0, 60 * Time.fixedDeltaTime);
        gameObject.layer = Eureka ? 10 : 11;

        //判断是否需要自由落体
        AngleAcc += 60 * Time.fixedDeltaTime;
        if (AngleAcc > 360 && Destructive && !isFreeFall) SwitchToFreeFall();

        //销毁自己
        if (transform.position.y < -13) Destroy(gameObject);
    }

    protected override void ResetState()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 90 * rotate);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!Destructive) return;
        if (!isTriggered) return;
        //尝试获取组件
        collision.gameObject.TryGetComponent(out BaseItem baseItem);

        //如果获取到物体且非钢制
        if (baseItem != null && !baseItem.IfSteel)
        {
            //销毁自己的基准点
            if (collision.transform == transform.parent) SwitchToFreeFall();

            AudioManager.Instance.Play("Hammer_Des", gameObject);

            //动画消失
            Destroy(baseItem.gameObject, 1.5f);
            baseItem.gameObject.layer = 0;
            baseItem.transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCubic);

        }
    }

    private void SwitchToFreeFall()
    {
        isFreeFall = true;

        //重新修改位置和速度
        transform.parent = null;
        Destroy(transform.GetChild(0).gameObject);
        Rb.velocity = new Vector2(0, -FreeFallSpeed);
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;
    }
}
