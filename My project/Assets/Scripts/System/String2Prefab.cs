using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "", menuName = "ScriptableObjects/String2Prefab", order = 1)]
/// <summary>
/// 从字符串到预制件的转换
/// </summary>
public class String2Prefab : ScriptableObject
{
    [Serializable]
    public class Pair
    {
        public string Name;
        public GameObject Prefab;
    }
    public List<Pair> pairs = new List<Pair>();
    public Dictionary<string, GameObject> GetDict()
    {
        Dictionary<string, GameObject> dict = new();
        pairs.ForEach(x => dict.Add(x.Name, x.Prefab));
        return dict;
    }
}
