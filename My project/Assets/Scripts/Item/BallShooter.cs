using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallShooter : BaseItem
{

    [Title("发射间隔")][SerializeField] float space;
    [Title("弹力球预制件")][SerializeField] Transform Ball;
    selfUpdateInt shootTimer;

    protected override void Awake()
    {
        base.Awake();
        shootTimer = new selfUpdateInt(gameObject);
        shootTimer.SetValue(0);
    }

    protected override void Execute()
    {
        //如果到了发射倒计时
        if (shootTimer.ifZero())
        {
            shootTimer.SetValue((int)(60 * space));
            var BallGo = Instantiate(Ball, transform);
            BallGo.transform.position = transform.position + transform.right.normalized * 1.5f;
        }
    }

    protected override void ResetState()
    {
        shootTimer.SetValue(0);
    }

    public override bool Droppable
    {
        get
        {
            if (!base.Droppable) return false;
            var target = CurCell + Orient[rotate];
            return !EditManager.Instance.Outside(target) &&
                MapManager.Instance.Map[target.x, target.y] &&
                MapManager.Instance.Map[target.x, target.y].GetComponent<BaseItem>().IfRoot;
        }
    }
    public override void OnDrop()
    {
        base.OnDrop();
        var target = CurCell + Orient[rotate];
        transform.parent = MapManager.Instance.Map[target.x, target.y].transform;
    }

}
