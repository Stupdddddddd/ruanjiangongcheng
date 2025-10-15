using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class UIAvoidEureka : MonoBehaviour
{
    [SerializeField] private AnimationCurve moveCurve;
    [SerializeField] private float moveSpeed = 1;
    [SerializeField] private RectTransform target;
    [SerializeField] private Vector3 direction;
    private float moveFactor;
    private float MoveFactor
    {
        get => moveFactor;
        set => moveFactor = Mathf.Clamp(value, 0f, 1f);
    }
    private void FixedUpdate()
    {
        MoveFactor = UIManager.Instance.Eureka.Spread ? MoveFactor - moveSpeed * Time.fixedDeltaTime : MoveFactor + moveSpeed * Time.fixedDeltaTime;
        target.localPosition = Vector3.Lerp(direction, Vector3.zero, moveCurve.Evaluate(MoveFactor));
    }
}
