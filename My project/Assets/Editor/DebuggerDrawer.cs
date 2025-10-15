using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Debugger))]
public class DebuggerDrawer : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        Debugger debugger = (Debugger)target;

        GUI.enabled = Application.isPlaying;

        if (GUILayout.Button("激活所有物体", GUILayout.Width(150)))
        {
            foreach(var Go in GameObject.FindObjectsOfType<BaseItem>())
            {
                Go.SetTriggered();
            }
        }

        if (GUILayout.Button("关闭所有物体", GUILayout.Width(150)))
        {
            foreach (var Go in GameObject.FindObjectsOfType<BaseItem>())
            {
                Go.SetDisabled();
            }
        }

        if (GUILayout.Button("存档", GUILayout.Width(150)))
        {
            GameSystem.SaveProfile("Test");
        }

        if (GUILayout.Button("读档", GUILayout.Width(150)))
        {
            debugger.profile = GameSystem.ReadProfile();
        }

        GUI.enabled = true;
    }
}