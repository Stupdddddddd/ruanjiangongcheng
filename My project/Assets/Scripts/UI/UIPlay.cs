using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class UIPlay : MonoBehaviour
{
    [SerializeField] private Text round;
    [SerializeField] private RectTransform healthBar;
    [SerializeField] private GameObject healthPrefab;
    [SerializeField] private Sprite healthIconEmpty;
    [SerializeField] private Sprite healthIconFull;
    [SerializeField] private GameObject tips;
    [SerializeField] private GameObject main;
    public void Tips(bool open)
    {
        tips.SetActive(open);
        main.SetActive(!open);
    }
    private Image[] healthIcons;
    private int preHealth;
    public void Init()
    {
        round.text = LevelManager.Instance.Round.ToString();
        healthIcons = healthBar.GetComponentsInChildren<Image>();
        for (int i = 0; i < healthIcons.Length; i++) Destroy(healthIcons[i].gameObject);
        healthIcons = new Image[PlayerMove.Instance.HealthMax];
        for (int i = 0; i < healthIcons.Length; i++)
        {
            healthIcons[i] = Instantiate(healthPrefab, healthBar).GetComponent<Image>();
            (healthIcons[i].transform as RectTransform).sizeDelta = .9f *
                (healthPrefab.transform as RectTransform).rect.height * Vector2.one;
        }
        preHealth = PlayerMove.Instance.HealthMax;
    }
    private void FixedUpdate()
    {
        if (preHealth != PlayerMove.Instance.CurHealth)
        {
            preHealth = PlayerMove.Instance.CurHealth;
            for (int i = 0; i < PlayerMove.Instance.HealthMax; i++)
                healthIcons[i].sprite = i < preHealth ? healthIconFull : healthIconEmpty;
        }
    }
    public void Refresh() => LevelManager.Instance.Refresh();
    public void Return()
    {

    }
}
