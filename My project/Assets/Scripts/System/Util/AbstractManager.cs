using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public abstract class AbstractManager : MonoBehaviour
{
    public abstract void Init();
}
public abstract class AbstractManager<T> : AbstractManager where T : AbstractManager<T>
{
    private static T instance;
    public static T Instance
    {
        get
        {
            if (instance != null) return instance;
            throw new System.NullReferenceException(string.Format("找不到{0}的实例，请检查场景与实例化顺序！", typeof(T).Name));
        }
    }
    public override void Init()
    {
        if (instance)
        {
            Destroy(instance.gameObject);
            Debug.LogWarning(string.Format("{0}的实例已变更！", typeof(T).Name));
        }
        instance = this as T;
    }
}
