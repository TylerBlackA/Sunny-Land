using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Frog : Enemy
{
    // Start is called before the first frame update
    private Rigidbody2D rb;
    //private Animator Anim;
    private Collider2D Coll;
    public LayerMask Ground;

    public Transform leftpoint, rightpoint;
    public float Speed,JumpForce;
    private float leftx, rightx; 

    private bool Faceleft = true;
    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        Coll = GetComponent<Collider2D>();
        transform.DetachChildren();
        leftx = leftpoint.position.x;
        rightx = rightpoint.position.x;
        Destroy(leftpoint.gameObject);
        Destroy(rightpoint.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        /*Movement();*/
        SwitchAnim();
    }

    void Movement()
    { 
        if(Faceleft)//面朝左
        {
            if (Coll.IsTouchingLayers(Ground))
            {
                Anim.SetBool("jumping", true);
                rb.velocity = new Vector2(-Speed, JumpForce);//跳跃
            }
            if(transform.position.x < leftx)
            {
                transform.localScale = new Vector3(-1, 1, 1); //如果超过左边界就转向
                Faceleft = false;
            }
        }
        else//面朝右
        {
            if (Coll.IsTouchingLayers(Ground))
            {
                Anim.SetBool("jumping", true);
                rb.velocity = new Vector2(Speed, JumpForce);//跳跃
            }
            if (transform.position.x > rightx)//超过右边点就转向
            {
                transform.localScale = new Vector3(1, 1, 1);
                Faceleft = true;
            }
        }
    }

    void SwitchAnim()
    {
        if(Anim.GetBool("jumping"))
        {
            if(rb.velocity.y < 0.1)
            {
                Anim.SetBool("jumping", false);
                Anim.SetBool("falling", true);
            }
        }
        if(Coll.IsTouchingLayers(Ground) && Anim.GetBool("falling"))
        {
            Anim.SetBool("falling", false);
        }
    }

    
}
