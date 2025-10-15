using Bingyan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/有刺无刺")]
public class EurekaTogenashiTogeari : AbstractEurekaConfig
{
    [SerializeField, Title("提升生命值")] private int hp = 2;
    private Func<KeyValuePair<GameObject, UISlot>, bool> select = x =>
            x.Key.GetComponent<BaseItem>() is Spike && x.Key.GetComponentInParent<Circle>() != null;
    public override string Condition => condition.Replace("x", EditManager.Instance.ItemRecord.Count(select).ToString());
    public override bool Meet()
    {
        return EditManager.Instance.ItemRecord.Any(select);
    }
    protected override void Act()
    {
        EurekaManager.Instance.HpBuffer += hp;

    }
}
