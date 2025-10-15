using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UISettle : MonoBehaviour
{
    [SerializeField] private InputField input;
    [SerializeField] private Text round;
    public void Init()
    {
        round.text = LevelManager.Instance.Round.ToString();
    }
    public void Restart()
    {
        GameSystem.SaveProfiles(input.text);
        SceneManager.LoadScene(1);
    }
    public void Exit()
    {
        GameSystem.SaveProfiles(input.text);
        SceneManager.LoadScene(0);
    }
}
