using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;

[TrackStatic]
public static class GameSystem
{

    /// <summary>
    /// 存档路径
    /// </summary>
    public static readonly string SavingPath = Application.persistentDataPath + "/Profile.json";
    public static readonly string SavingPaths = Application.persistentDataPath + "/Profiles.json";

    
    private static Player input;
    /// <summary>
    /// 玩家输入表
    /// </summary>
    public static Player Input
    {
        get
        {
            if (input == null) input = new();
            return input;
        }
    }

    /// <summary>
    /// 随机生成器
    /// </summary>
    public static RandomItemSpawner Spawner;


    static GameSystem()
    {
        Dict = Resources.Load<String2Prefab>("String2Prefab");
    }
    /// <summary>
    /// 字符串转预制件字典
    /// </summary>
    public static String2Prefab Dict;

    /// <summary>
    /// 保存存档
    /// </summary>
    /// <param name="saying"></param>
    public static void SaveProfile(string saying)
    {
        var profile = new Profile();
        profile.Saying = saying;
        profile.Record = LevelManager.Instance.Round;
        profile.Items = new ItemInfo[EditManager.Instance.RecordCopy.Count];
        int idx = 0;
        foreach (var item in EditManager.Instance.RecordCopy.Keys)
        {
            profile.Items[idx].Title = item.GetComponent<BaseItem>().Title;
            profile.Items[idx].Orient = item.GetComponent<BaseItem>().rotate;
            profile.Items[idx].Position = new float[2] { item.transform.position.x, item.transform.position.y };
            idx++;
        }

        File.WriteAllText(SavingPath, JsonConvert.SerializeObject(profile, Formatting.Indented));
    }
    public static void SaveProfiles(string saying)
    {
        var profiles = ReadProfiles() ?? new();
        var profile = new Profile();
        profile.Saying = saying;
        profile.Record = LevelManager.Instance.Round;
        profile.Items = new ItemInfo[EditManager.Instance.ItemRecord.Count];
        int idx = 0;
        foreach (var item in EditManager.Instance.ItemRecord.Keys)
        {
            profile.Items[idx].Title = item.GetComponent<BaseItem>().Title;
            profile.Items[idx].Orient = item.GetComponent<BaseItem>().rotate;
            profile.Items[idx].Position = new float[2] { item.transform.position.x, item.transform.position.y };
            idx++;
        }


        profiles.Add(profile);
        File.WriteAllText(SavingPaths, JsonConvert.SerializeObject(profiles, Formatting.Indented));

    }

    public static List<Profile> ReadProfiles()
    {
        if (File.Exists(SavingPaths))
        {
            string JsonS = File.ReadAllText(SavingPaths);
            return JsonConvert.DeserializeObject<List<Profile>>(JsonS);
        }
        else
        {
            if (!Directory.Exists(SavingPaths)) File.Create(SavingPaths).Dispose();
            return null;
        }
    }

    /// <summary>
    /// 读取存档
    /// </summary>
    /// <returns>存在时返回存档，否则返回null</returns>
    public static Profile ReadProfile()
    {
        if (File.Exists(SavingPath))
        {
            string JsonS = File.ReadAllText(SavingPath);
            return JsonConvert.DeserializeObject<Profile>(JsonS);
        }
        else
        {
            if (!Directory.Exists(SavingPath)) File.Create(SavingPath).Dispose();
            return null;
        }
    }

    [VoidStaticMethod]
    public static void ClearBall()
    {
        GameObject[] items = SceneManager.GetActiveScene().GetRootGameObjects();
        foreach (var item in items)
        {
            if (item.TryGetComponent(out Ball _))
            {
                GameObject.Destroy(item, 0.1f);
            }
        }
    }
}
