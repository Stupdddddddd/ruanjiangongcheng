using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIEurekaConfig : MonoBehaviour
{
    [SerializeField] private Text title;
    [SerializeField] private Text description;
    [SerializeField] private Text condition;
    [SerializeField] private Image icon;
    public void SetConfig(AbstractEurekaConfig config)
    {
        title.text = config.Title;
        description.text = config.Description;
        condition.text = "¼¤»îÇé¿ö£º" + config.Condition;
        icon.sprite = config.Triggered ? config.Icon : config.Bar;
    }
}
