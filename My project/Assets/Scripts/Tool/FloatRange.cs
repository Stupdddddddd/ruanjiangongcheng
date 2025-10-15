using System;
using UnityEngine;
using Rand = UnityEngine.Random;
[Serializable]
public struct FloatRange
{
    public float Max;
    public float Min;
    public static implicit operator float(FloatRange fr) => Rand.Range(fr.Min, fr.Max);
}
