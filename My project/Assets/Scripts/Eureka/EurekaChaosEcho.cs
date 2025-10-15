using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/混沌回响")]
public class EurekaChaosEcho : AbstractEurekaConfig
{
    [SerializeField, Title("目标轮次")] private int round = 8;
    [SerializeField, Title("最多次数")] private int maxCnt = 3;
    [SerializeField, Title("进入概率")] private float probability = .05f;
    private int cnt;
    public void Awake()
    {
        cnt = 0;
    }
    public override bool Meet()
    {
        return cnt < maxCnt && LevelManager.Instance.Round >= round && Random.Range(0, 1f) < probability;
    }
    protected override void Act()
    {
        cnt++;
        EurekaManager.Instance.ChaosEcho = true;
    }
}
