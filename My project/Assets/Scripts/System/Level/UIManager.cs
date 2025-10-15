using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIManager : AbstractManager<UIManager>
{
    private static readonly int shaderProcessID = Shader.PropertyToID("_Process");
    private GameObject canvas;
    public override void Init()
    {
        base.Init();

        GameObject.Find("ItemSpawner").GetComponent<RandomItemSpawner>().Init();

        canvas = GameObject.Find("Canvas");
        edit = canvas.transform.Find("Edit").GetComponent<UIEdit>();
        eureka = canvas.transform.Find("Eureka").GetComponent<UIEureka>();
        play = canvas.transform.Find("Play").GetComponent<UIPlay>();
        settle = canvas.transform.Find("Settle").GetComponent<UISettle>();
        Echo = canvas.transform.Find("Echo").gameObject;
        mask = canvas.transform.Find("Mask").GetComponent<Image>();
    }
    [Header("Edit")]
    private UIEdit edit;
    public UIEdit Edit => edit;
    public void HideEdit() => edit.gameObject.SetActive(false);
    public void ShowEdit()
    {
        edit.gameObject.SetActive(true);
        edit.Init();
    }
    [Header("Eureka")]
    private UIEureka eureka;
    public UIEureka Eureka => eureka;
    public void HideEureka() => eureka.gameObject.SetActive(false);
    public void ShowEureka()
    {
        eureka.gameObject.SetActive(true);
        eureka.Init();
    }
    [Header("Play")]
    private UIPlay play;
    public UIPlay Play => play;
    public void HidePlay() => play.gameObject.SetActive(false);
    public void ShowPlay()
    {
        play.gameObject.SetActive(true);
        play.Init();
    }
    [Header("Settle")]
    private UISettle settle;
    public UISettle Settle => settle;
    public void HideSettle() => settle.gameObject.SetActive(false);
    public void ShowSettle()
    {
        settle.gameObject.SetActive(true);
        settle.Init();
    }
    public GameObject Echo { get; private set; }
    [Header("Mask")]
    private Image mask;
    [SerializeField] private float restartTime = 2, giveupTime = 2, fadeTime = 1;
    private float restartTimer, giveupTimer;
    private bool restarted, givenup;
    private void SetMaskProcess(float process) => mask.material.SetFloat(shaderProcessID, process);
    private float Process => Mathf.Max(restartTimer / restartTime, giveupTimer / giveupTime);
    private void FixedUpdate()
    {
        if (restarted)
        {
            restartTimer += Time.fixedDeltaTime;
            if (restartTimer > restartTime)
            {
                LevelManager.Instance.Refresh();
                restarted = false;
            }
        }
        else restartTimer -= Time.fixedDeltaTime * (restartTime / fadeTime);
        if (givenup)
        {
            giveupTimer += Time.fixedDeltaTime;
            if (giveupTimer > giveupTime)
            {
                LevelManager.Instance.SwitchState<LevelManager.SettleState>();
                givenup = false;
            }
        }
        else giveupTimer -= Time.fixedDeltaTime * (giveupTime / fadeTime);
        restartTimer = Mathf.Clamp(restartTimer, 0, restartTime);
        giveupTimer = Mathf.Clamp(giveupTimer, 0, giveupTime);
        SetMaskProcess(Process * Process);
    }
    public void ResetMask()
    {
        restarted = false;
        givenup = false;
        SetMaskProcess(0);
    }
    public void RestartOn(InputAction.CallbackContext cbk) => restarted = true;
    public void GiveUpOn(InputAction.CallbackContext cbk) => givenup = true;
    public void RestartOff(InputAction.CallbackContext cbk) => restarted = false;
    public void GiveUpOff(InputAction.CallbackContext cbk) => givenup = false;
}
