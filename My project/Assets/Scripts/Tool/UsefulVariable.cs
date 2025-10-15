using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class selfUpdateInt:ILifeCycle
{
    [SerializeField]
    /// <summary>
    /// 计时器
    /// </summary>
    private int frameCount = 0;

    [HideInInspector]
    /// <summary>
    /// 根引用
    /// </summary>
    public GameObject owner = null;

    //构造函数注册
    public selfUpdateInt(GameObject owner)
    {   
        LifeCycleManager.Instance.register(this);
        frameCount = 0;
        this.owner = owner;
    }

    //按帧更新
    public void FixedUpdate() { if (frameCount > 0) frameCount--; }

    public bool ifZero() { return frameCount <= 0; }

    public void SetValue(int value)=>frameCount = value;

    public int GetValue() { return frameCount; }

    public bool ifDelete() { return owner == null; }

    public void Start() { }

    public void Update() { }
}

public class delegateInt : ILifeCycle
{
    //计时器
    [SerializeField] private int frameCount;

    //在计时为0时该干的事
    public System.Action doWhat;

    //根引用
    public GameObject owner;

    //构造函数注册
    public delegateInt(GameObject owner,Action doWhat)
    {
        frameCount = 0;
        this.owner = owner;
        this.doWhat = doWhat;
    }

    public delegateInt(GameObject owner,int frameCount,Action doWhat)
    {
        frameCount = 0;
        this.owner = owner;
    }

    public int Value
    {
        get => frameCount;
    }

    //按帧更新
    public void FixedUpdate()
    {
        frameCount--;
        if(frameCount == 0) doWhat?.Invoke();
    }

    public void SetValue(int value)
    {
        LifeCycleManager.Instance.register(this);
        frameCount = value;
    }

    public bool ifDelete() { return frameCount < 0 || owner == null; }

    /// <summary>
    /// 提前触发
    /// </summary>
    public void Trigger()
    {
        LifeCycleManager.Instance.unregister(this);
        doWhat?.Invoke();
        frameCount = 0;
    }

    /// <summary>
    /// 取消触发
    /// </summary>
    public void Cancel()
    {
        LifeCycleManager.Instance.unregister(this);
        frameCount = 0;
    }

    public void Start() { }

    public void Update() { }
}