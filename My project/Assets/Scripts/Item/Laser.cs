using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : BaseItem
{
    [Title("检测层")][SerializeField] LayerMask DetectedLayer;
    [Title("最大长度")][SerializeField] float MaxLength;
    [Title("最大反射次数")][SerializeField] int MaxTry = 10;
    LineRenderer lineRenderer;
    static int count = 0;

    public override bool Droppable =>
        base.Droppable &&
        !EditManager.Instance.Outside(CurCell + Orient[rotate]) &&
        MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y] &&
        MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y].GetComponent<BaseItem>().IfRoot;
    public override void OnDrop()
    {
        base.OnDrop();
        transform.parent = MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y].transform;
    }

    public override void SetTriggered()
    {
        base.SetTriggered();
        if (count == 0) AudioManager.Instance.Play("Laser", AudioManager.Instance.gameObject);
        count++;
    }


    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        if (!isTriggered) return;

        //设置起始点
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, transform.position);

        Vector2 point = transform.position;
        Vector2 direction = transform.right;


        RaycastHit2D hit = Physics2D.Raycast(point, direction, MaxLength, DetectedLayer);
        while (hit)
        {
            //如果有点，则计算新的点和方向
            point = hit.point;
            direction = Vector2.Reflect(direction, hit.normal);

            lineRenderer.positionCount++;
            lineRenderer.SetPosition(lineRenderer.positionCount - 1, point);

            //如果照到玩家
            if (hit.collider.gameObject.layer == 6)
            {
                AudioManager.Instance.Play("Hit_Laser", AudioManager.Instance.gameObject);
                PlayerMove.Instance.OnHurt();
            }
            //不能超过最大次数
            if (lineRenderer.positionCount == MaxTry - 1) return;

            //重新发射
            hit = Physics2D.Raycast(point + direction * 0.1f, direction, MaxLength, DetectedLayer);
        }

        //添加最后一次
        lineRenderer.positionCount++;
        lineRenderer.SetPosition(lineRenderer.positionCount - 1, point + direction * MaxLength);
    }
    protected override void Execute()
    {

    }

    protected override void ResetState()
    {
        lineRenderer.positionCount = 0;
        count--;
        if (count == 0) AudioManager.Instance.Stop("Laser", AudioManager.Instance.gameObject);
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        AudioManager.Instance.Stop("Laser", AudioManager.Instance.gameObject);
    }
}
