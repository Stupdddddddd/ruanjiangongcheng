using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[Serializable]
public class EnumArray<TEnum, TValue> where TEnum : Enum 
{
    [SerializeField]private TValue[] array;
    static private Dictionary<TEnum, int> CorInd;

    static EnumArray()
    {
        CorInd = new Dictionary<TEnum, int>();
        int ind = 0;
        foreach(TEnum EnumValue in Enum.GetValues(typeof(TEnum)))
        {
            CorInd[EnumValue] = ind;
            ind++;
        }
    }

    public EnumArray()
    {
        int size = Enum.GetValues(typeof(TEnum)).Length;
        array = new TValue[size];
    }

    public TValue this[TEnum _enum]
    {
        get { return array[CorInd[_enum]]; }
        set { array[CorInd[_enum]] = value; }
    }

    
}


public class DEnumArray<TEnum1, TEnum2, TValue>
where TEnum1 : Enum
where TEnum2 : Enum
{
    private TValue[] array;

    public DEnumArray()
    {
        int size1 = Enum.GetValues(typeof(TEnum1)).Length;
        int size2 = Enum.GetValues(typeof(TEnum2)).Length;
        array = new TValue[size1 * size2];
    }

    public TValue this[TEnum1 enum1, TEnum2 enum2]
    {
        get { return array[CalculateIndex(enum1, enum2)]; }
        set { array[CalculateIndex(enum1, enum2)] = value; }
    }

    private int CalculateIndex(TEnum1 enum1, TEnum2 enum2)
    {
        int index1 = Convert.ToInt32(enum1);
        int index2 = Convert.ToInt32(enum2);
        int size2 = Enum.GetValues(typeof(TEnum2)).Length;
        return index1 * size2 + index2;
    }
}

