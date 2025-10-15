using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/致命节奏")]
public class EurekaDeadlyRhythm : AbstractEurekaConfig
{
    [SerializeField, Title("最大层数")] private int maxCnt = 3;
    public int ActCnt
    {
        get
        {
            int actCnt = Cnt / targetCnt;
            return actCnt < maxCnt ? actCnt : maxCnt;
        }
    }
    [SerializeField, Title("叠加因子"), Tooltip("效果计算公式：Factor^Cnt")] private float factor = 1.14f;
    [SerializeField, Title("目标攻击性物体数")] private int targetCnt = 4;
    private int Cnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().Attacktive);
    public override string Condition => condition.Replace("x", Cnt.ToString()).Replace("y", ActCnt.ToString());
    public override bool Meet()
    {
        PlayerMove.Instance.MoveMultiply = 1;
        return ActCnt > 0;
    }
    protected override void Act()
    {
        PlayerMove.Instance.MoveMultiply = Mathf.Pow(factor, ActCnt);
    }
}
