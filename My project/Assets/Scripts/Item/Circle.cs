using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : BaseItem
{
    public override bool Droppable
    {
        get
        {
            if (!base.Droppable) return false;
            for (int i = 0; i < 4; i++)
            {
                var target = CurCell + Orient[i];
                if (EditManager.Instance.Outside(target)) continue;
                if (MapManager.Instance.Map[target.x, target.y] &&
                    MapManager.Instance.Map[target.x, target.y].GetComponent<BaseItem>() is Circle) return false;
            }
            return true;
        }
    }
    //还原至非旋转状态
    protected override void ResetState()
    {
        transform.localRotation = Quaternion.Euler(0f, 0f, 90 * rotate);
    }

    protected override void Execute()
    {
        transform.Rotate(0, 0, 60 * Time.fixedDeltaTime);
    }
    public override void OnDrop()
    {
        base.OnDrop();
        for (int i = 0; i < 4; i++)
        {
            var target = CurCell + Orient[i];
            if (!EditManager.Instance.Outside(target) &&
                MapManager.Instance.Map[target.x, target.y] &&
                MapManager.Instance.Map[target.x, target.y].TryGetComponent(out BaseRootItem item))
            {
                item.Connect();
                MapManager.Instance.Map[target.x, target.y].transform.parent = transform;
            }
        }
    }
}
