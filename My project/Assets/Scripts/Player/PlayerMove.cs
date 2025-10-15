using Bingyan;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

/// <summary>
/// 玩家移动的脚本
/// </summary>
public class PlayerMove : MonoBehaviour, IDisposable
{
    public static PlayerMove Instance { get; private set; }

    [SerializeField] Material Hit;
    [SerializeField] Material Default;
    //自身组件引用
    Animator Ani;
    Rigidbody2D Rb;
    CapsuleCollider2D Cod;
    LayerTrigger GroundTrigger;
    Vector3 InitScale;
    SpriteRenderer SR;

    [Header("跳跃参数")]
    [Title("一段跳跃速度")][SerializeField] float JumpVelocity1 = 3f;
    [Title("二段跳跃速度")][SerializeField] float JumpVelocity2 = 2f;
    [Title("上升重力规模")][SerializeField] float UpGravity = 4.5f;
    [Title("下降重力规模")][SerializeField] float DownGravity = 6.5f;
    [Title("下降速度限制")][SerializeField] float DownMaxVelocity = 10f;
    selfUpdateInt JumpTime;
    selfUpdateInt CoyoteTime;
    selfUpdateInt JumpCD;
    bool OnGround = false;
    bool IfJumped = false;
    float PreYVelocity = 0f;
    float StandPoint = 0f;
    int JumpCount = 2;


    [Header("移动参数")]
    [Title("启动时间")][SerializeField] float StartTime;
    [Title("移动速度")][SerializeField] float MoveForce;
    [Title("冲刺速度")][SerializeField] float RushForce;
    [Title("蹲下速度")][SerializeField] float CrouchForce;
    float MoveInput = 0f;
    bool IfRush = false;
    bool IfCrouch = false;
    public float BanTime = 0f;

    [Header("冲刺参数")]
    [Title("维持时间")][SerializeField] float DashDuration;
    [Title("冲刺速度")][SerializeField] float DashSpeed;
    [Title("冷却时间")][SerializeField] float DashCDs;
    [Title("无敌时间")][SerializeField] float DashInv;
    selfUpdateInt DashCD;
    selfUpdateInt DashDur;
    bool ifDashed = false;

    [Header("外部引用")]
    [Title("生成点")][SerializeField] Transform SpawnPoint;

    [Header("碰撞交互相关")]
    [Title("生命值")][SerializeField] int Health;
    [Title("无敌时间")][SerializeField] float InvulnerableTime;
    public selfUpdateInt InvTime;
    bool IfDead = false;

    #region 公共属性

    /// <summary>
    /// 跳跃倍率
    /// </summary>
    public float HeightMultiply { get; set; } = 1f;

    /// <summary>
    /// 移动倍率
    /// </summary>
    public float MoveMultiply { get; set; } = 1f;

    [Title("能否疾跑")][SerializeField] private bool canRush = false;
    /// <summary>
    /// 能否疾跑
    /// </summary>
    public bool CanRush { get => canRush; set { canRush = value; } }

    [Title("能否二段跳")][SerializeField] private bool canDoubleJump = false;
    /// <summary>
    /// 能否二段跳
    /// </summary>
    public bool CanDoubleJump { get => canDoubleJump; set { canDoubleJump = value; } }

    private int healthMax = 1;
    /// <summary>
    /// 生命值上限
    /// </summary>
    public int HealthMax
    {
        get { return healthMax; }
        set { healthMax = value; Health = healthMax; }
    }

    [Title("能否冲刺")][SerializeField] private bool canDash = false;
    /// <summary>
    /// 能否冲刺
    /// </summary>
    public bool CanDash { get => canDash; set { canDash = value; } }

    public int CurHealth => Health;

    #endregion

    #region 初始化和GC
    void Awake()
    {
        Rb = GetComponent<Rigidbody2D>();
        Cod = GetComponent<CapsuleCollider2D>();
        Ani = GetComponent<Animator>();
        SR = GetComponent<SpriteRenderer>();
        InitScale = transform.localScale;

        Instance = this;

        //绑定跳跃触发器
        GroundTrigger = GetComponentInChildren<LayerTrigger>();
        GroundTrigger.doWhenIn = () =>
        {
            OnGround = true;
            IfJumped = false;
            JumpCD.SetValue(2);
            JumpCount = CanDoubleJump ? 2 : 1;
        };
        GroundTrigger.doWhenOut = () =>
        {
            OnGround = false;
            CoyoteTime.SetValue(8);
            if (IfCrouch) Crouch_Cancel(new());
        };

        //绑定输入
        GameSystem.Input.InGame.XMove.performed += XMove_Performed;
        GameSystem.Input.InGame.XMove.canceled += XMove_Canceled;
        GameSystem.Input.InGame.Rush.started += Rush_Start;
        GameSystem.Input.InGame.Rush.canceled += Rush_Cancel;
        GameSystem.Input.InGame.Crouch.started += Crouch_Start;
        GameSystem.Input.InGame.Crouch.canceled += Crouch_Cancel;
        GameSystem.Input.InGame.Jump.started += Jump;
        GameSystem.Input.InGame.Dash.started += Dash;

        //绑定自更新变量
        JumpTime = new selfUpdateInt(gameObject);
        JumpCD = new selfUpdateInt(gameObject);
        InvTime = new selfUpdateInt(gameObject);
        CoyoteTime = new selfUpdateInt(gameObject);
        DashCD = new selfUpdateInt(gameObject);
        DashDur = new selfUpdateInt(gameObject);
    }

    /// <summary>
    /// 垃圾回收
    /// </summary>
    public void Dispose()
    {
        GameSystem.Input.InGame.XMove.performed -= XMove_Performed;
        GameSystem.Input.InGame.XMove.canceled -= XMove_Canceled;
        GameSystem.Input.InGame.Rush.started -= Rush_Start;
        GameSystem.Input.InGame.Rush.canceled -= Rush_Cancel;
        GameSystem.Input.InGame.Crouch.started -= Crouch_Start;
        GameSystem.Input.InGame.Crouch.canceled -= Crouch_Cancel;
        GameSystem.Input.InGame.Jump.started -= Jump;
        GameSystem.Input.InGame.Dash.started -= Dash;
    }

    #endregion

    #region 更新
    void FixedUpdate()
    {
        Jump();
        Move();
        Float();
    }

    void Update()
    {
        if (Health <= 0 && !IfDead) OnDeath();
        Anime();
    }

    /// <summary>
    /// 控制动画
    /// </summary>
    void Anime()
    {
        //处理转向
        if (Rb.velocity.x > 0) InitScale.x = Mathf.Abs(InitScale.x);
        else if (Rb.velocity.x < 0) InitScale.x = -Mathf.Abs(InitScale.x);
        transform.localScale = InitScale;

        Ani.SetFloat("YSpeed", Rb.velocity.y);
        Ani.SetBool("IfCrouch", IfCrouch);
        Ani.SetFloat("XSpeed", Mathf.Abs(Rb.velocity.x));
    }

    #endregion

    #region 移动控制
    /// <summary>
    /// 悬浮控制
    /// </summary>
    private void Float()
    {
        //顶部测试
        float newYVelocity = Rb.velocity.y;
        if (PreYVelocity > 0 && newYVelocity < 0)
        {
            Debug.Log("JumpMax: " + (transform.position.y - StandPoint));
        }
        PreYVelocity = newYVelocity;

        //重力更改
        if (Rb.velocity.y < 0)
        {
            Rb.gravityScale = DownGravity;
        }
        else if (Rb.velocity.y > 0)
        {
            Rb.gravityScale = UpGravity;
        }


        //最大速度
        if (Rb.velocity.y < -DownMaxVelocity)
        {
            Rb.velocity = new Vector2(Rb.velocity.x, -DownMaxVelocity);
        }

        //位置过低似了
        if (!IfDead && transform.position.y < -15) OnDeath();
    }

    /// <summary>
    /// 移动函数
    /// </summary>
    private void Move()
    {

        float TargetVelocity = MoveInput;
        if (IfRush) TargetVelocity *= RushForce;
        else if (IfCrouch) TargetVelocity *= CrouchForce;
        else TargetVelocity *= MoveForce;

        TargetVelocity *= MoveMultiply;

        float XVelocity = Rb.velocity.x;
        float AbsXVelocity = Mathf.Abs(XVelocity);

        if (BanTime > 0) BanTime -= Time.fixedDeltaTime;

        if (BanTime < 0) BanTime = 0;
        else if (BanTime > 0) return;

        //冲刺处理
        if (OnGround && ifDashed && DashCD.ifZero()) ifDashed = false;
        if (!DashDur.ifZero())
        {
            Rb.velocity = transform.localScale.x * new Vector2(DashSpeed, 0);
            return;
        }

        //无输入马上停下来
        if (MoveInput == 0)
        {
            Rb.velocity = new Vector2(0, Rb.velocity.y);
        }
        if (MoveInput != 0)
        {
            //差距大则Lerp
            if (Mathf.Abs(XVelocity - TargetVelocity) < 0.01f)
            {
                Rb.velocity = new Vector2(Rb.velocity.x + Mathf.Lerp(0, TargetVelocity, StartTime), Rb.velocity.y);
            }
            //差距小则赋值
            else
            {
                Rb.velocity = new Vector2(TargetVelocity, Rb.velocity.y);
            }
        }



    }

    /// <summary>
    /// 开始下蹲
    /// </summary>
    /// <param name="ctx"></param>
    private void Crouch_Start(InputAction.CallbackContext ctx)
    {
        if (IfRush) return;
        if (IfJumped) return;
        if (!OnGround) return;

        IfCrouch = true;

        //修改位置大小
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.25f, transform.position.z);
        Cod.offset = new Vector2(0, -0.3f);
        Cod.size = new Vector2(1, 1.2f);
    }

    /// <summary>
    /// 冲刺
    /// </summary>
    /// <param name="ctx"></param>
    private void Dash(InputAction.CallbackContext ctx)
    {
        if (!canDash) return;
        if (IfCrouch) Crouch_Cancel(new());
        if (!DashCD.ifZero()) return;
        if (BanTime > 0) return;

        Ani.SetTrigger("Dash");
        //调时间
        ifDashed = true;
        InvTime.SetValue((int)(DashInv * 60));
        DashDur.SetValue((int)(DashDuration * 60));
        DashCD.SetValue((int)(DashCDs * 60));
    }

    /// <summary>
    /// 结束下蹲
    /// </summary>
    /// <param name="ctx"></param>
    private void Crouch_Cancel(InputAction.CallbackContext ctx)
    {
        if (!IfCrouch) return;

        IfCrouch = false;

        transform.position = new Vector3(transform.position.x, transform.position.y + 0.25f, transform.position.z);
        Cod.offset = new Vector2(0, -0.14f);
        Cod.size = new Vector2(1, 1.5f);
    }

    /// <summary>
    /// 跳跃函数
    /// </summary>
    private void Jump()
    {
        //是否在时间次数上允许
        if (JumpCount == 0) return;
        if (JumpTime.ifZero()) return;
        if (!JumpCD.ifZero()) return;

        //不与蹲下兼容
        if (IfCrouch) return;
        if (!DashDur.ifZero()) return;
        if (BanTime > 0) return;

        //进行一段跳
        if (OnGround || (!CoyoteTime.ifZero() && !IfJumped))
        {
            AudioManager.Instance.Play("Jump", gameObject);
            //重置跳跃参数
            JumpTime.SetValue(0);
            JumpCD.SetValue(10);
            IfJumped = true;
            JumpCount--;
            StandPoint = transform.position.y;

            Rb.velocity = new Vector2(Rb.velocity.x, JumpVelocity1 * HeightMultiply);
            Ani.SetTrigger("Jump");
            LifeCycleManager.InsertDelayAction(0.1f, () => Ani.ResetTrigger("Jump"));

            return;
        }

        //判断二段跳
        if (CanDoubleJump && !OnGround)
        {
            AudioManager.Instance.Play("Jump", gameObject);
            //重置跳跃次数
            JumpTime.SetValue(0);
            JumpCD.SetValue(10);
            IfJumped = true;
            JumpCount = 0;
            StandPoint = transform.position.y;

            Rb.velocity = new Vector2(Rb.velocity.x, JumpVelocity2 * HeightMultiply);
            Ani.SetTrigger("Double_Jump");
            LifeCycleManager.InsertDelayAction(0.1f, () => Ani.ResetTrigger("Double_Jump"));
        }
    }

    /// <summary>
    /// 开始冲刺
    /// </summary>
    /// <param name="ctx"></param>
    private void Rush_Start(InputAction.CallbackContext ctx)
    {
        if (IfCrouch) return;
        if (!CanRush) return;

        IfRush = true;
    }
    /// <summary>
    ///  取消冲刺
    /// </summary>
    /// <param name="ctx"></param>
    private void Rush_Cancel(InputAction.CallbackContext ctx)
    {
        if (!IfRush) return;

        IfRush = false;
    }
    /// <summary>
    /// 跳跃
    /// </summary>
    /// <param name="ctx"></param>
    private void Jump(InputAction.CallbackContext ctx) => JumpTime.SetValue(4);

    /// <summary>
    /// 开始移动
    /// </summary>
    /// <param name="ctx"></param>
    private void XMove_Performed(InputAction.CallbackContext ctx) => MoveInput = ctx.ReadValue<float>();

    /// <summary>
    /// 取消移动
    /// </summary>
    /// <param name="ctx"></param>
    private void XMove_Canceled(InputAction.CallbackContext ctx) => MoveInput = 0;

    #endregion

    #region 碰撞交互
    /// <summary>
    /// 死亡
    /// </summary>
    private void OnDeath()
    {
        AudioManager.Instance.Play("Player_Dead", AudioManager.Instance.gameObject);
        IfDead = true;

        //死亡动画
        transform.DOScale(Vector3.zero, 1.5f).SetEase(Ease.OutCubic);

        Rb.constraints = RigidbodyConstraints2D.FreezeAll;

        //延时复活
        LifeCycleManager.InsertDelayAction(1.5f, () => Revive());

        Ani.SetTrigger("Death");

        LevelManager.Instance.Defeat = true;
    }

    /// <summary>
    /// 复活
    /// </summary>
    public void Revive()
    {
        //重置参数
        Health = HealthMax;
        IfDead = false;
        InvTime.SetValue(0);
        JumpCount = 2;
        JumpCD.SetValue(0);
        JumpTime.SetValue(0);
        IfRush = false;

        //重置位移
        Rb.velocity = Vector2.zero;
        transform.position = SpawnPoint.position;
        transform.DOScale(Vector3.one, 1f);
        Rb.constraints = RigidbodyConstraints2D.FreezeRotation;

        //重置动画
        Ani.SetTrigger("Revive");
        Ani.ResetTrigger("Jump");
        OnGround = true;
        if (IfCrouch) Crouch_Cancel(new());


    }

    /// <summary>
    /// 受到伤害
    /// </summary>
    public void OnHurt()
    {
        if (!InvTime.ifZero()) return;

        Health--;
        SR.material = Hit;
        LifeCycleManager.InsertDelayAction(0.2f, () => SR.material = Default);

        if (Health > 0)
        {
            InvTime.SetValue((int)(InvulnerableTime * 60));
        }
        else
        {
            OnDeath();
        }
    }

    /// <summary>
    /// 触发器
    /// </summary>
    /// <param name="collision"></param>
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.layer == 10) OnHurt();
        else if (collision.gameObject.layer == 11) OnDeath();

        //TODO:调用关卡完成函数
    }

    /// <summary>
    /// 碰撞触发
    /// </summary>
    /// <param name="collision"></param>
    public void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == 10) OnHurt();
        else if (collision.gameObject.layer == 11) OnDeath();
    }

    #endregion
}
