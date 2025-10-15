using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//一个用于检测对象和某一层之间碰撞的对象
public class LayerTrigger:MonoBehaviour
{   
    public LayerMask targetLayer;           //指定检测的层
    public Action doWhenIn;
    public Action doWhenOut;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.callbackLayers & targetLayer) != 0) { doWhenIn?.Invoke(); }
    }
    public void OnTriggerExit2D(Collider2D collision)
    {
        if ((collision.callbackLayers & targetLayer) != 0) { doWhenOut?.Invoke(); }
    }
}
