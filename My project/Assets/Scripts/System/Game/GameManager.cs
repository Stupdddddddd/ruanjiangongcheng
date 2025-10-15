using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class GameManager : AbstractFSM
{
    private static GameManager instance;
    public static GameManager Instance
    {
        get
        {
            if (instance != null) return instance;
            throw new System.NullReferenceException("找不到GameManager的实例，请检查场景。");
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            Debug.LogWarning("GameManager的实例已变更！");
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
        InitManagers();
        InitStates();
    }
    private void InitManagers()
    {
        foreach (var manager in GetComponentsInChildren<AbstractManager>()) manager.Init();
    }
    private void InitStates()
    {

    }
}
