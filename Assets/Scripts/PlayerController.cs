using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    public AudioSource jumpAudio, hurtAudio,cherryAudio;
    public Collider2D coll;
    public Collider2D DisColl;
    public Transform CellingCheck;
    public float speed;
    public float JumpForce;
    public LayerMask ground;
    public int Cherry = 0;

    public Text CherryNum;
    private bool isHurt; //默认是false

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!isHurt)
        {
            Movement();
        }
        SwitchAnim();
    }
    //移动 
    void Movement()
    {
        float horizontalMove = Input.GetAxis("Horizontal");
        float facedirection = Input.GetAxisRaw("Horizontal");

        if (horizontalMove != 0)
        {
            rb.velocity = new Vector2(horizontalMove * speed * Time.fixedDeltaTime, rb.velocity.y);
            anim.SetFloat("running", Mathf.Abs(facedirection));
        }
        if (facedirection != 0)
        {
            transform.localScale = new Vector3(facedirection, 1, 1);
        }
        //角色跳跃
        if (Input.GetButtonDown("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, JumpForce * Time.fixedDeltaTime);
            jumpAudio.Play();
            anim.SetBool("jumping", true);
            
        }

        Crouch();
    }
    //切换动画
    void SwitchAnim()
    {
        //anim.SetBool("idle", false);
        if(rb.velocity.y < 0.1f && !coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", true);
        }
        if (anim.GetBool("jumping"))
        {
            if (rb.velocity.y < 0) //速度小于0的时候下落
            {
                anim.SetBool("jumping", false);
                anim.SetBool("falling", true);
            }
        }
        
        else if (coll.IsTouchingLayers(ground))
        {
            anim.SetBool("falling", false);
            //anim.SetBool("idle", true);
        }

        if (isHurt)
        {
            anim.SetBool("hurt", true);
            anim.SetFloat("running", 0);
            if (Mathf.Abs(rb.velocity.x) < 0.1f)
            {
                anim.SetBool("hurt", false);
                anim.SetBool("idle", true);
                isHurt = false;
            }
        }
    }
    //碰撞触发器
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //收集物品
        if (collision.tag == "Collections")
        {
            cherryAudio.Play();
            Destroy(collision.gameObject);
            Cherry += 1;
            CherryNum.text = Cherry.ToString();
        }
        if (collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            Invoke(nameof(Restart),2f);
        }
    }
    //笔记： OnTriggerEnter2D 和 OnCollisionEnter2D 所调用的函数不一样 
    //消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.tag == "Enimy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))  //所以要获取这个gameobject的tag
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x,JumpForce * Time.deltaTime);
                anim.SetBool("jumping", true);
            }
            else if (transform.position.x<collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-60, 30);
                hurtAudio.Play();
                isHurt = true;

            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(60, 30);
                hurtAudio.Play();
                isHurt = true;
            }
        }
    }

    void Crouch()
    {
        if (!Physics2D.OverlapCircle(CellingCheck.position,0.2f,ground))
        {
            if (Input.GetButton("Crouch"))
            {
                anim.SetBool("Crouching", true);
                DisColl.enabled = false;
            }
            else
            {
                anim.SetBool("Crouching", false);
                DisColl.enabled = true;
            }
        } 
    }

    void Restart()
    {
        //死亡重置
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);  
    }
}
