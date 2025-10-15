using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BaseItem),true,isFallback = false)]
public class BaseItemDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        // 绘制默认 Inspector 面板
        base.OnInspectorGUI();

        float wid = (EditorGUIUtility.currentViewWidth - 30) / 8;

        // 获取目标脚本实例
        BaseItem item = target as BaseItem;
        
        GUILayout.BeginHorizontal();

        GUI.enabled = EditorApplication.isPlaying;
        // 创建按钮
        if (GUILayout.Button("激活",GUILayout.Width(wid * 2)))
        {
            item.SetTriggered();
        }

        GUILayout.Space(wid);

        // 创建按钮
        if (GUILayout.Button("关闭", GUILayout.Width(wid * 2)) && EditorApplication.isPlaying)
        {
            item.SetDisabled();
        }

        GUILayout.Space(wid);

        GUI.enabled = !EditorApplication.isPlaying;
        // 创建按钮
        if (GUILayout.Button("旋转", GUILayout.Width(wid * 2)) && !EditorApplication.isPlaying)
        {
            item.Rotate();
            item.transform.Rotate(0, 0, 90);
        }

        GUI.enabled = true;
        GUILayout.EndHorizontal();
    }
}