using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EurekaManager : AbstractManager<EurekaManager>
{
    public void Refresh()
    {
        ChaosEcho = false;
        UHW = false;
        HpBuffer = HpBase;
        JumpBuffer = 1;
        Bomb = false;
    }
    public int HpBase { get; set; } = 1;
    public bool ChaosEcho { get; set; }
    public bool UHW { get; set; }
    public bool Bomb { get; set; }
    public int HpBuffer { get; set; }
    public float JumpBuffer { get; set; }
    [SerializeField] private List<AbstractEurekaConfig> configs;
    public List<AbstractEurekaConfig> Configs => configs;
    public void Settle()
    {
        foreach (var eureka in configs) eureka.Settle();
        PlayerMove.Instance.HealthMax = HpBuffer;
        PlayerMove.Instance.HeightMultiply = JumpBuffer;
        UIManager.Instance.Eureka.Settle();
    }
    public void DestroyIcons() => UIManager.Instance.Eureka.DestroyIcons();
}
