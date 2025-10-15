using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : BaseItem
{
    [Title("移动速度")][SerializeField] float Speed;
    [Title("检测层级")][SerializeField] LayerMask Layer;
    [Title("旋转帧数")][SerializeField] int RotateFrame;

    float Radius;
    Vector3 InitPos;                //初始位置
    Quaternion InitRot;             //初始旋转
    selfUpdateInt RotateT;          //旋转计时
    bool NegativeRotate = false;    //旋转方向
    bool IfEndRotate = false;       //是否结束旋转
    int RuntimeRotate = 0;          //运行时旋转状态
    Vector3 TargetDirection;        //目标方向

    public override bool Droppable =>
        base.Droppable &&
        !EditManager.Instance.Outside(CurCell + Orient[rotate]) &&
        MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y] &&
        MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y].TryGetComponent(out BaseItem item) &&
        item.IfRoot && item is not Spider;
    public override void OnDrop()
    {
        base.OnDrop();
        if (MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y])
            transform.parent = MapManager.Instance.Map[(CurCell + Orient[rotate]).x, (CurCell + Orient[rotate]).y].transform;
        InitPos = transform.position;
        InitRot = transform.rotation;
    }

    protected override void Awake()
    {
        base.Awake();
        Radius = GetComponent<CircleCollider2D>().radius;
        RotateT = new(gameObject);
    }

    public override void SetTriggered()
    {
        base.SetTriggered();
        FetchLand();
        RuntimeRotate = rotate;
    }

    protected override void Execute()
    {
        if (!RotateT.ifZero()) Runtime_Rotate();
        else Runtime_Move();
    }

    /// <summary>
    /// 运行时旋转
    /// </summary>
    private void Runtime_Rotate()
    {
        IfEndRotate = true;

        transform.Rotate(0, 0, 90f / RotateFrame * (NegativeRotate ? -1 : 1));
        if (!NegativeRotate) transform.position += (2 * Radius + 0.1f) * TargetDirection / (float)RotateFrame;

    }

    /// <summary>
    /// 运行时移动
    /// </summary>
    private void Runtime_Move()
    {
        //从旋转状态恢复过来
        if (IfEndRotate)
        {
            IfEndRotate = false;
            NegativeRotate = false;

            transform.localRotation = Quaternion.Euler(0, 0, 90f * RuntimeRotate);
            //if(!NegativeRotate) transform.position += (2 * Radius + 0.05f) * -transform.right;
            FetchLand();
        }

        //抓地和前方障碍检测
        RaycastHit2D[] downHit = Physics2D.RaycastAll(transform.position + transform.right * Radius, -transform.up, 0.8f, Layer);
        RaycastHit2D[] leftHit = Physics2D.RaycastAll(transform.position, -transform.right, Radius + 0.02f, Layer);
        bool downGet = false;
        bool leftGet = false;

        Transform root = transform.parent;
        while (root && root.parent != null) root = root.parent;

        //筛选出正确的Transform
        foreach (var hit in downHit)
        {
            Transform hitRoot = hit.transform;
            while (hitRoot && hitRoot.parent != null) hitRoot = hitRoot.parent;

            if (root != hitRoot) continue;
            else
            {
                downGet = true;
                break;
            }
        }

        //筛选出正确的Transform
        foreach (var hit in leftHit)
        {
            Transform hitRoot = hit.transform;
            while (hitRoot && hitRoot.parent != null) hitRoot = hitRoot.parent;

            if (root != hitRoot) continue;
            else
            {
                leftGet = true;
                break;
            }
        }

        //如果踩空
        if (!downGet)
        {
            RotateT.SetValue(RotateFrame);
            NegativeRotate = false;
            RuntimeRotate = (RuntimeRotate + 1) % 4;
            TargetDirection = RuntimeRotate switch
            {
                0 => new(-1, 0, 0),
                1 => new(0, -1, 0),
                2 => new(1, 0, 0),
                3 => new(0, 1, 0),
                _ => new(0, 0, 0)
            };
        }
        //如果前方有阻碍
        else if (downGet && leftGet)
        {
            RotateT.SetValue(RotateFrame);
            NegativeRotate = true;
            RuntimeRotate = (RuntimeRotate - 1) % 4;
            TargetDirection = RuntimeRotate switch
            {
                0 => new(-1, 0, 0),
                1 => new(0, -1, 0),
                2 => new(1, 0, 0),
                3 => new(0, 1, 0),
                _ => new(0, 0, 0)
            };
        }
        //否则正常移动
        else
        {
            transform.position += -transform.right * Time.fixedDeltaTime * Speed;

        }
    }

    /// <summary>
    /// 抓地
    /// </summary>
    private void FetchLand()
    {
        Transform root = transform.parent;
        //再次进行射线检测
        RaycastHit2D[] downHit = Physics2D.RaycastAll(transform.position + transform.right * Radius, -transform.up, 0.8f, Layer);

        Transform hitRoot = transform.parent;
        while (hitRoot && hitRoot.parent != null) hitRoot = hitRoot.parent;

        //筛选出正确的Transform
        foreach (var hit in downHit)
        {
            if (root != hitRoot) continue;
            else
            {
                transform.position -= transform.up * hit.distance;
                transform.position += transform.up * Radius;
                break;
            }
        }
    }


    protected override void ResetState()
    {
        transform.position = InitPos;
        transform.rotation = InitRot;
        RotateT.SetValue(0);
        NegativeRotate = false;
        IfEndRotate = false;
    }
}
