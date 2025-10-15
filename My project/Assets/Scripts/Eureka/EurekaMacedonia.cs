using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/马其顿方阵")]
public class EurekaMacedonia : AbstractEurekaConfig
{
    [SerializeField, Title("目标大摆锤数")] private int hammerCnt = 2;
    [SerializeField, Title("目标钢体数")] private int steelCnt = 5;
    private int HammerCnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>() is Hammer);
    private int SteelCnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().IfSteel);
    public override string Condition => condition.Replace("x", HammerCnt.ToString()).Replace("y", SteelCnt.ToString());
    public override bool Meet()
    {
        Hammer.Destructive = false;
        return HammerCnt >= hammerCnt && SteelCnt >= steelCnt;
    }
    protected override void Act()
    {
        Hammer.Destructive = true;
    }
}
