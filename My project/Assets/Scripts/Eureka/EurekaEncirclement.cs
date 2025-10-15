using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/天罗地网")]
public class EurekaEncirclement : AbstractEurekaConfig
{
    [SerializeField, Title("目标旋转激光数")] private int targetCnt = 3;
    private int Cnt => EditManager.Instance.ItemRecord.Count(x =>
            x.Key.GetComponent<BaseItem>() is Laser && x.Key.GetComponentInParent<Circle>() != null
        );
    public override string Condition => condition.Replace("x", Cnt.ToString());
    public override bool Meet()
    {
        PlayerMove.Instance.CanDash = false;
        return Cnt >= targetCnt;
    }
    protected override void Act()
    {
        PlayerMove.Instance.CanDash = true;
    }
}
