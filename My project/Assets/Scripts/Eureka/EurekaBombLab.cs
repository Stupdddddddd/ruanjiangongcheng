using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(menuName = "Eureka/都让开，要炸了！！！")]
public class EurekaBombLab : AbstractEurekaConfig
{
    public override string Condition => condition.Replace("x", Type.ToString());
    private int type = 0;
    private int Type
    {
        get
        {
            type = 0;
            bool[] item = new bool[9];
            foreach (var go in EditManager.Instance.ItemRecord.Keys)
            {
                string it = go.GetComponent<BaseItem>().Title;
                if (it.Contains("平台")) item[0] = true;
                if (it.Contains("直角")) item[1] = true;
                if (it.Contains("蜘蛛")) item[2] = true;
                if (it.Contains("方块")) item[3] = true;
                if (it.Contains("激光")) item[4] = true;
                if (it.Contains("弹力")) item[5] = true;
                if (it.Contains("旋转")) item[6] = true;
                if (it.Contains("锤")) item[7] = true;
                if (it.Contains("刺")) item[8] = true;

            }

            //结果检测
            foreach (var bos in item)
            {
                if (bos) type++;
            }

            return type;
        }
    }

    public override bool Meet()
    {
        return (Type == 9);
    }
    protected override void Act()
    {
        EurekaManager.Instance.Bomb = true;
    }
}
