using Bingyan;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Purchasing;
using UnityEngine.UI;
public class LevelManager : AbstractFSM
{
    private static LevelManager instance;
    public static LevelManager Instance
    {
        get
        {
            if (instance != null) return instance;
            throw new System.NullReferenceException("找不到LevelManager的实例，请检查场景。");
        }
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(instance.gameObject);
            Debug.LogWarning("LevelManager的实例已变更！");
        }
        instance = this;

        Round = 1;

        InitManagers();
        InitStates();
        SwitchState<EditState>();

        GameSystem.Input.InGame.Disable();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        instance = null;
    }
    private void InitManagers()
    {
        foreach (var manager in GetComponentsInChildren<AbstractManager>()) manager.Init();
    }
    private void InitStates()
    {
        AddState<EditState>(new EditState());
        AddState<EurekaState>(new EurekaState());
        AddState<PlayState>(new PlayState());
        AddState<SettleState>(new SettleState());
    }
    public void Refresh()
    {
        EditManager.Instance.RewindRecord();
        GameSystem.ClearBall();
        PlayerMove.Instance.Revive();
        SwitchState<EditState>();
    }
    public bool Echo()
    {
        var profiles = GameSystem.ReadProfiles();
        if (profiles != null && profiles.Count == 0) return false;
        var profile = profiles[Random.Range(0, profiles.Count - 1)];
        StartCoroutine(EchoCoroutine(profile.Saying));
        EditManager.Instance.RewindRecord(profile);
        return true;
    }
    private bool clicked;
    private void Click(InputAction.CallbackContext cbk) => clicked = true;
    public bool Win { get; set; }
    public bool Defeat { get; set; }
    private IEnumerator EchoCoroutine(string saying)
    {
        var profile = GameSystem.ReadProfile();

        clicked = Win = Defeat = false;
        GameSystem.Input.InGame.Disable();
        GameSystem.Input.InEdit.Enable();
        GameSystem.Input.InEdit.Confirm.started += Click;
        UIManager.Instance.Echo.SetActive(true);
        UIManager.Instance.Echo.transform.GetComponentInChildren<Text>().text = saying;
        GameSystem.ClearBall();
        PlayerMove.Instance.Revive();
        yield return new WaitUntil(() => clicked);
        SwitchState<EditState>();
        GameSystem.Input.InEdit.Confirm.started -= Click;
        UIManager.Instance.Echo.SetActive(false);

        yield return new WaitUntil(() => Win || Defeat);
        EditManager.Instance.RewindRecord(profile);
        if (Win) EurekaManager.Instance.HpBase++;
        if (Defeat)
        {
            Round++;
            GameSystem.ClearBall();
            PlayerMove.Instance.Revive();
            SwitchState<EditState>();
        }
    }
    public void Next()
    {
        if (EurekaManager.Instance.ChaosEcho && Echo()) return;
        EditManager.Instance.UpdateRecord();
        Round++;
        PlayerMove.Instance.Revive();
        SwitchState<EditState>();
    }

    public int Round { get; set; }
    public class EditState : AbstractStates
    {
        public override void OnEnter()
        {
            MapManager.Instance.RandomizeQuadrant();
            UIManager.Instance.ShowEdit();
            GameSystem.Input.InEdit.Enable();
            EditManager.Instance.ConnectAll();

            UIManager.Instance.ShowEureka();
            EurekaManager.Instance.DestroyIcons();
            GameSystem.SaveProfile("111");

            GameSystem.Input.InEdit.Giveup.started += UIManager.Instance.GiveUpOn;
            GameSystem.Input.InEdit.Giveup.canceled += UIManager.Instance.GiveUpOff;
        }
        public override void OnExit()
        {
            UIManager.Instance.HideEdit();
            EditManager.Instance.DropAll();
            EditManager.Instance.ConnectAll();
            GameSystem.Input.InEdit.Disable();

            GameSystem.Input.InEdit.Giveup.started -= UIManager.Instance.GiveUpOn;
            GameSystem.Input.InEdit.Giveup.canceled -= UIManager.Instance.GiveUpOff;
        }
    }
    public class EurekaState : AbstractStates
    {
        public override void OnEnter()
        {
            EurekaManager.Instance.Refresh();
            EurekaManager.Instance.Settle();

            GameSystem.Input.InEureka.Enable();
        }
        public override void OnExit()
        {
            GameSystem.Input.InEureka.Disable();
        }
    }
    public class PlayState : AbstractStates
    {
        public override void OnEnter()
        {
            GameSystem.Input.InGame.Enable();
            EditManager.Instance.ItemRecord.Keys.ForEach(x => x.GetComponent<BaseItem>().SetTriggered());
            UIManager.Instance.ShowPlay();

            GameSystem.Input.InGame.Restart.started += UIManager.Instance.RestartOn;
            GameSystem.Input.InGame.Giveup.started += UIManager.Instance.GiveUpOn;
            GameSystem.Input.InGame.Restart.canceled += UIManager.Instance.RestartOff;
            GameSystem.Input.InGame.Giveup.canceled += UIManager.Instance.GiveUpOff;

            UIManager.Instance.ResetMask();
        }
        public override void OnExit()
        {
            GameSystem.Input.InGame.Disable();
            EditManager.Instance.ItemRecord.Keys.Select(x => x.GetComponent<BaseItem>())
                .ForEach(x => { if (x is Circle) x.SetDisabled(); });
            EditManager.Instance.ItemRecord.Keys.Select(x => x.GetComponent<BaseItem>())
                .ForEach(x => { if (x is not Circle) x.SetDisabled(); });
            UIManager.Instance.HidePlay();

            GameSystem.Input.InGame.Restart.started -= UIManager.Instance.RestartOn;
            GameSystem.Input.InGame.Giveup.started -= UIManager.Instance.GiveUpOn;
            GameSystem.Input.InGame.Restart.canceled -= UIManager.Instance.RestartOff;
            GameSystem.Input.InGame.Giveup.canceled -= UIManager.Instance.GiveUpOff;
        }
    }
    public class SettleState : AbstractStates
    {
        public override void OnEnter()
        {
            UIManager.Instance.ShowSettle();
        }
        public override void OnExit()
        {
            UIManager.Instance.HideSettle();
        }
    }
}
