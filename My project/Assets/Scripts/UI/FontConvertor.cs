using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FontConvertor : MonoBehaviour
{
    [SerializeField] private Font font;
    private void Awake()
    {
        foreach (var text in GetComponentsInChildren<Text>())
        {
            text.font = font;
        }
    }
}
