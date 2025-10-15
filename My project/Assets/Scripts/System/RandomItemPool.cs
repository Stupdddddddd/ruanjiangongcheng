using Bingyan;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pool", menuName = "ScriptableObjects/Pool", order = 1)]
/// <summary>
/// 随机对象池
/// </summary>
public class RandomItemPool : ScriptableObject
{
    [Serializable]
    public struct Selection
    {
        [Title("预制件")] public GameObject Prefab;
        [Title("数量")] public int Count;
    }

    /// <summary>
    /// 池内的物体
    /// </summary>
    public List<Selection> PoolItems;
    
}
