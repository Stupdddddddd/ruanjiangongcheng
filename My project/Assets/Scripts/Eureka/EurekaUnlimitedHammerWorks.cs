using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/无限锤制")]
public class EurekaUnlimitedHammerWorks : AbstractEurekaConfig
{
    [SerializeField, Title("每层提升概率")] private float amountPerLayer = .25f;
    [SerializeField, Title("目标钢体数量")] private int targetCnt = 3;
    private int Cnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().IfSteel);
    public override string Condition => condition.Replace("x", Cnt.ToString());
    private static float amount;
    public static float Amount
    {
        get => amount;
        set => amount = value > 1 ? 1 : value;
    }
    public override bool Meet()
    {
        int cnt = Cnt / targetCnt;
        amount = amountPerLayer * cnt;
        return cnt > 0;

    }
    protected override void Act()
    {
        EurekaManager.Instance.UHW = true;
    }
}
