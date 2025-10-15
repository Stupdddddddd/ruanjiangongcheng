using Bingyan;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
/// <summary>
/// 一个概率项
/// </summary>
public class Probability
{
    public GameObject item;
    public float p;
}

/// <summary>
/// 从池子中随机抽出物体的生成器
/// </summary>
public class RandomItemSpawner : MonoBehaviour
{
    //原始的池子
    RandomItemPool Pool;

    //实际的池子
    List<GameObject> RealPool;

    [Title("直角变体")][SerializeField] List<Probability> RectAngleVari;
    [Title("方块变体")][SerializeField] List<Probability> BlockVari;
    [Title("平台变体")][SerializeField] List<Probability> PlatformVari;
    [Title("炸弹引用")][SerializeField] GameObject BombPrefab;
    [Title("大摆锤引用")][SerializeField] GameObject HammerPrefab;

    public void Awake()
    {
        GameSystem.Spawner = this;

        Pool = Resources.Load<RandomItemPool>("RandomPool");

        Clone();
    }

    /// <summary>
    /// 测试时手动控制生成顺序
    /// </summary>
    public void Init()
    {
        GameSystem.Spawner = this;

        Pool = Resources.Load<RandomItemPool>("RandomPool");

        Clone();
    }

    /// <summary>
    /// 克隆牌库
    /// </summary>
    private void Clone()
    {
        RealPool = new List<GameObject>();
        foreach (var item in Pool.PoolItems)
        {
            for (int i = 0; i < item.Count; i++)
                RealPool.Add(item.Prefab);
        }

        //给牌库随机排序
        System.Random random = new System.Random();
        RealPool = RealPool.OrderBy(x => random.Next()).ToList();
    }

    /// <summary>
    /// 抽取物体
    /// </summary>
    /// <returns>抽取的结果</returns>
    public List<GameObject> Draw(int Count)
    {
        var ls = new List<GameObject>();
        if (RealPool.Count >= Count)
        {
            ls.AddRange(RealPool.GetRange(0, Count));
            RealPool.RemoveRange(0, Count);
        }
        else
        {
            ls.AddRange(RealPool);
            int left = Count - RealPool.Count;

            //重新生成RealPool
            RealPool.Clear();
            Clone();

            ls.AddRange(Draw(left));
        }

        //对于随机平台进行处理
        for (int i = 0; i < ls.Count; ++i)
        {
            //如果是直角
            if (ls[i].TryGetComponent<RightAngle>(out var ra))
            {
                ls[i] = GetRandomVariant(RectAngleVari);
            }
            else if (ls[i].TryGetComponent<Block>(out var block))
            {
                ls[i] = GetRandomVariant(BlockVari);
            }
            else if (ls[i].TryGetComponent<Platform>(out var platform))
            {
                ls[i] = GetRandomVariant(PlatformVari);
            }
        }

        //调整炸弹
        if (EurekaManager.Instance.Bomb)
        {
            int bombCount = 0;
            foreach (var item in ls)
            {
                if (item.GetComponent<Bomb>() != null) bombCount++;
            }
            if (bombCount < 2)
            {
                for (int i = 0; i < ls.Count; ++i)
                {
                    if (bombCount < 2 && ls[i].GetComponent<Bomb>() == null)
                    {
                        ls[i] = BombPrefab;
                        bombCount++;
                    }
                }
            }
        }

        //大摆锤
        if (EurekaManager.Instance.UHW)
        {
            float judge = UnityEngine.Random.value;
            if (judge < EurekaUnlimitedHammerWorks.Amount)
            {
                ls[3] = HammerPrefab;
            }
        }



        return ls;
    }

    public void AllTriggered()
    {
        foreach (var Go in GameObject.FindObjectsOfType<BaseItem>())
        {
            Go.SetTriggered();
        }
    }

    public GameObject GetRandomVariant(List<Probability> plists)
    {
        GameObject re = null;
        float result = UnityEngine.Random.value;
        float acc = 0;
        for (int i = 0; i < plists.Count; ++i)
        {
            if (acc + plists[i].p > result)
            {
                re = plists[i].item;
                break;
            }
            acc += plists[i].p;
        }
        return re;
    }
}
