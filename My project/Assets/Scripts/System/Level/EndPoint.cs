using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(BoxCollider2D))]
public class EndPoint : MonoBehaviour
{
    private bool triggered;
    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        triggered = false;
    }
    private void FixedUpdate()
    {
        if (triggered)
        {
            triggered = false;
            LevelManager.Instance.Win = true;
            LevelManager.Instance.Next();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player")) return;
        AudioManager.Instance.Play("NextLevel", PlayerMove.Instance.gameObject);
        triggered = true;
    }
}
