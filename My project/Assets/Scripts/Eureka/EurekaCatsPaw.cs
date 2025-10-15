using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/火中取栗")]
public class EurekaCatsPaw : AbstractEurekaConfig
{
    [SerializeField, Title("目标攻击性物体数")] private int targetCnt = 3;
    private int Cnt => EditManager.Instance.ItemRecord.Count(x => x.Key.GetComponent<BaseItem>().Attacktive);
    public override string Condition => condition.Replace("x", Cnt.ToString());
    public override bool Meet()
    {
        PlayerMove.Instance.CanDoubleJump = false;
        return Cnt >= targetCnt;
    }
    protected override void Act()
    {
        PlayerMove.Instance.CanDoubleJump = true;
    }
}
