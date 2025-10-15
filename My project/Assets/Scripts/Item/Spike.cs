using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spike : BaseItem
{
    protected override void ResetState() { }
    protected override void Execute() { }
    public override bool Droppable
    {
        get
        {
            if (!base.Droppable) return false;
            foreach (var slot in Slots)
            {
                var target = CurCell + slot + Orient[rotate];
                if (EditManager.Instance.Outside(target)) return false;
                if (!MapManager.Instance.Map[target.x, target.y]) return false;
                if (!MapManager.Instance.Map[target.x, target.y].GetComponent<BaseItem>().IfRoot) return false;
            }
            return true;
        }
    }
    public override void OnDrop()
    {
        base.OnDrop();
        foreach (var slot in Slots)
        {
            var target = CurCell + slot + Orient[rotate];
            if (MapManager.Instance.Map[target.x, target.y])
            {
                transform.parent = MapManager.Instance.Map[target.x, target.y].transform;
                break;
            }
        }
    }
}
