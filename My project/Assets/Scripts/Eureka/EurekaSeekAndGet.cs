using Bingyan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/求仁得仁")]
public class EurekaSeekAndGet : AbstractEurekaConfig
{
    [SerializeField, Title("起跳速度变化")] private float factor = 1.25f;
    private Func<KeyValuePair<GameObject, UISlot>, bool> select = x => x.Key.TryGetComponent<Hammer>(out _);
    public override string Condition => condition.Replace("x", EditManager.Instance.ItemRecord.Count(select).ToString());
    public override bool Meet()
    {
        return EditManager.Instance.ItemRecord.Any(select);
    }
    protected override void Act()
    {
        EurekaManager.Instance.JumpBuffer *= factor;
    }
}
