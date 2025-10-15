using Bingyan;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
public abstract class AbstractEurekaConfig : ScriptableObject
{
    [SerializeField, Title("名称")] private string title;
    public string Title => title;
    [SerializeField, Title("描述")] private string description;
    public string Description => "描述：" + description;
    [SerializeField, Title("条件")] protected string condition;
    public virtual string Condition => condition;
    [SerializeField, Title("图标")] private Sprite icon;
    public Sprite Icon => icon;
    [SerializeField, Title("侧栏")] private Sprite bar;
    public Sprite Bar => bar;
    [SerializeField, Title("减益")] private bool debuff;
    public bool Debuff => debuff;
    public bool Triggered { get; set; }
    public bool Activated { get; private set; }
    /// <summary>
    /// 满足条件
    /// </summary>
    public abstract bool Meet();
    /// <summary>
    /// 执行
    /// </summary>
    protected abstract void Act();
    public void Settle()
    {
        Activated = false;
        if (Meet())
        {
            Act();
            Activated = true;
        }
    }
}
