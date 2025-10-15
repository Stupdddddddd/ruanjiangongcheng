using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[Serializable]
public struct ItemInfo
{
    public string Title { get; set; }
    public int Orient { get; set; }
    public float[] Position { get; set; }
}
/// <summary>
/// ´æµµ
/// </summary>
[Serializable]
public class Profile
{
    public string Saying { get; set; }
    public int Record { get; set; }
    public ItemInfo[] Items { get; set; }
}
