using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/诸武精通")]
public class EurekaWeaponMaster : AbstractEurekaConfig
{
    [SerializeField, Title("目标攻击性物体数")] private int targetCnt = 3;
    private int Cnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().Attacktive);
    public override string Condition => condition.Replace("x", Cnt.ToString());
    public override bool Meet()
    {
        Hammer.Eureka = false;
        bool haveHammer = false;
        int cnt = 0;
        foreach (var obj in EditManager.Instance.ItemRecord.Keys)
        {
            if (obj.TryGetComponent(out BaseItem item))
            {
                if (item.Attacktive) cnt++;
                if (item is Hammer) haveHammer = true;
                if (cnt >= targetCnt && haveHammer) return true;
            }
        }
        return false;
    }
    protected override void Act()
    {
        Hammer.Eureka = true;
    }
}
