using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/不要停下来啊！")]
public class EurekaKibouNoHana : AbstractEurekaConfig
{
    [SerializeField, Title("目标轮次")] private int round = 10;
    [SerializeField, Title("减少血量")] private int hp = 1;
    public override string Condition => condition.Replace("x", LevelManager.Instance.Round.ToString());
    public override bool Meet()
    {
        return LevelManager.Instance.Round >= round;
    }
    protected override void Act()
    {
        EurekaManager.Instance.HpBuffer -= hp;
    }
}
