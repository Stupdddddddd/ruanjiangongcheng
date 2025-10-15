using Bingyan;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIEureka : MonoBehaviour
{
    [Header("Arrow")]
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private RectTransform arrow;
    public bool Spread { get; private set; } = false;
    private float moveFactor = 0;
    private float MoveFactor
    {
        get => moveFactor;
        set => moveFactor = Mathf.Clamp(value, 0f, 1f);
    }
    private const int distance = 1728;
    public void Arrow()
    {
        Spread = !Spread;
        for (int i = 0; i < configs.Length; i++)
            configs[i].SetConfig(EurekaManager.Instance.Configs[i + (page - 1) * 6]);
    }
    private void FixedUpdate()
    {
        MoveFactor = Spread ? MoveFactor + moveSpeed * Time.fixedDeltaTime : MoveFactor - moveSpeed * Time.fixedDeltaTime;
        transform.localPosition = Vector3.Lerp(distance * Vector3.left, Vector3.zero, moveCurve.Evaluate(MoveFactor));
        arrow.rotation = Quaternion.Euler((MoveFactor > .5 ? 0 : 180) * Vector2.one);
    }

    [Header("Page")]
    [SerializeField] private Text currentPage;
    private int page;
    private UIEurekaConfig[] configs;
    public void Init()
    {
        page = 1;
        configs = GetComponentsInChildren<UIEurekaConfig>();
        for (int i = 0; i < configs.Length; i++)
            configs[i].SetConfig(EurekaManager.Instance.Configs[i]);
        currentPage.text = "1";
    }
    public void ChangePage(int page)
    {
        if (page == this.page) return;
        this.page = page;
        for (int i = 0; i < configs.Length; i++)
            configs[i].SetConfig(EurekaManager.Instance.Configs[i + (page - 1) * 6]);
        currentPage.text = page.ToString();
    }

    [Header("Bar")]
    [SerializeField] private GameObject iconPrefab;
    [SerializeField] private GameObject iconSpawn;
    [SerializeField, Title("尤里卡展示时间")] private float eurekaTime = .1f;
    public void Settle() => StartCoroutine(SettleCoroutine());
    public void DestroyIcons()
    {
        for (int i = 0; i < iconSpawn.transform.childCount; i++)
            Destroy(iconSpawn.transform.GetChild(i).gameObject);
        foreach (var config in EurekaManager.Instance.Configs)
            config.Triggered = false;
    }
    private void Click(InputAction.CallbackContext cbk) => skip = true;
    private bool skip;
    private IEnumerator SettleCoroutine()
    {
        ChangePage(1);
        if (EurekaManager.Instance.Configs.Any(x => x.Activated))
        {
            float time = eurekaTime;
            Spread = true;
            skip = false;
            GameSystem.Input.InEureka.Confirm.started += Click;
            yield return new WaitForSeconds(1 / moveSpeed);
            for (int i = 0; i < EurekaManager.Instance.Configs.Count; i++)
            {
                if (i == 6) ChangePage(2);
                if (!EurekaManager.Instance.Configs[i].Activated) continue;
                if (skip) time = 0;

                EurekaManager.Instance.Configs[i].Triggered = true;
                if (!skip)
                {
                    configs[i - (page - 1) * 6].SetConfig(EurekaManager.Instance.Configs[i]);
                    AudioManager.Instance.Play(EurekaManager.Instance.Configs[i].Debuff ? "Bad" : "Good", gameObject);
                }

                var current = Instantiate(iconPrefab, iconSpawn.transform);
                current.transform.GetComponent<Image>().sprite = EurekaManager.Instance.Configs[i].Bar;
                (current.transform as RectTransform).sizeDelta = (iconSpawn.transform as RectTransform).rect.width * .72f * Vector2.one;

                yield return new WaitForSeconds(time);
            }
            Spread = false;
            GameSystem.Input.InEureka.Confirm.started -= Click;
            yield return new WaitForSeconds(1 / moveSpeed);
        }
        LevelManager.Instance.SwitchState<LevelManager.PlayState>();
    }
}
