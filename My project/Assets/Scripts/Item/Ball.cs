using Bingyan;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Ball : BaseItem
{
    [Title("移动速度")][SerializeField] float speed = 5;
    [Title("施加的速度")][SerializeField] float force = 10;
    [Title("持续时间")][SerializeField] float duration = 0.5f;
    Rigidbody2D Rb;
    int BounceCount = 3;
    selfUpdateInt CD;
    [Title("反弹层")][SerializeField]LayerMask bounceMask;

    protected override void Awake()
    {
        base.Awake();
        CD = new(gameObject);
        SetTriggered();

        Rb = GetComponent<Rigidbody2D>();
        Rb.velocity = new Vector2(transform.parent.right.x, transform.parent.right.y) * speed;
        transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        transform.SetParent(null);
        Rb.rotation = 0f;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!CD.ifZero()) return;

        //玩家反冲
        if (collision.gameObject.layer == 14 && PlayerMove.Instance.InvTime.ifZero())
        {
            AudioManager.Instance.Play("Hit_Ball", AudioManager.Instance.gameObject);
            var playerRb = collision.gameObject.transform.parent.GetComponent<Rigidbody2D>();
            Debug.Log(Rb.velocity);
            playerRb.velocity = Rb.velocity.normalized * force;
            PlayerMove.Instance.BanTime = duration;
            CD.SetValue(30);
        }

        
        //反弹次数计算
        --BounceCount;
        if (BounceCount <= 0) Destroy(gameObject);

        //通过射线检测进行反射
        RaycastHit2D hit = Physics2D.Raycast(transform.position,
            Rb.velocity, 0.6f, bounceMask);
        Rb.velocity = speed * Vector2.Reflect(Rb.velocity, hit.normal);
        transform.position += (Vector3)Rb.velocity.normalized * 0.2f;
    }

    protected override void Execute() 
    {
        Rb.velocity = Rb.velocity.normalized * speed;
    }

    protected override void ResetState() { }

    
}
