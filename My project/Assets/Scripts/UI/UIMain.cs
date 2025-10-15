using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIMain : MonoBehaviour
{
    [SerializeField] private GameObject tips;
    [SerializeField] private GameObject main;
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private Vector3 direction;
    public void Enter() => SceneManager.LoadScene(1);
    public void Exit() => Application.Quit();
    public void Tips(bool open) => StartCoroutine(TipsCoroutine(open));
    private IEnumerator TipsCoroutine(bool open)
    {
        float factor = 0;
        while (factor < 1)
        {
            Move(open ? main.transform : tips.transform, factor);
            factor += moveSpeed * Time.deltaTime;
            yield return null;
        }
        factor = 0;
        while (factor < 1)
        {
            Move(open ? tips.transform : main.transform, 1 - factor);
            factor += moveSpeed * Time.deltaTime;
            yield return null;
        }
    }
    private void Move(Transform target, float factor) =>
        target.localPosition = Vector3.Lerp(Vector3.zero, direction, moveCurve.Evaluate(factor));
}
