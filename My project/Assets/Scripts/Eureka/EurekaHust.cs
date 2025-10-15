using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/你能做的，岂止于此？")]
public class EurekaHust : AbstractEurekaConfig
{
    [SerializeField, Title("目标比例")] private float target = 3;
    [SerializeField, Title("减缓程度")] private float factor = .5f;
    private int BaseCnt => EditManager.Instance.ItemRecord.Count;
    private int AtkCnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().Attacktive);
    public override string Condition => condition.Replace("x", (BaseCnt - AtkCnt).ToString()).Replace("y", AtkCnt.ToString());
    public override bool Meet()
    {
        return AtkCnt * target < BaseCnt;
    }
    protected override void Act()
    {
        EurekaManager.Instance.JumpBuffer *= factor;
    }
}
