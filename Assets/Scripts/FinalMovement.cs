using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FinalMovement : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D Coll;
    public Collider2D DisColl;
    private Animator anim;

    public AudioSource jumpAudio, hurtAudio, cherryAudio;
    public float speed, jumpForce;
    public Transform groundCheck;
    public Transform CellingCheck;
    public LayerMask ground;

    public bool isGround, isJump;

    bool jumpPressed;
    int jumpCount;

    [SerializeField]private int Cherry ;
    private int Gem;

    public Text CherryNum;
    private bool isHurt; //默认是false

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        Coll = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();

    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Jump") && jumpCount > 0)
        {
            jumpPressed = true;
        }
    }

    private void FixedUpdate()
    {
        isGround = Physics2D.OverlapCircle(groundCheck.position, 0.1f , ground);
        if (!isHurt)
        {
            GroundMovement();

            Jump();
        
        }

        SwitchAnim();

        CherryNum.text = Cherry.ToString();
    }

    void GroundMovement()
    {
        float horizontalMove = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(horizontalMove * speed, rb.velocity.y);

        if(horizontalMove != 0)
        {
            transform.localScale = new Vector3(horizontalMove, 1, 1);
        }

        Crouch();
    }

    void Jump()
    {
        if (isGround)
        {
            jumpCount = 2;
            isJump = false;
        }
        if(jumpPressed && isGround)
        {
            isJump = true;
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            jumpCount--;
            jumpPressed = false;
            jumpAudio.Play();
        }
        else if(jumpPressed && jumpCount > 0 && isJump)
        {
            rb.velocity = new Vector2(rb.velocity.x , jumpForce);
            jumpCount--;
            jumpPressed = false;
            jumpAudio.Play();
        }
    }
    
    void SwitchAnim()
    {
        anim.SetFloat("running", Mathf.Abs(rb.velocity.x));

        if(isGround)
        {
            anim.SetBool("falling", false);
            anim.SetBool("idle", true);
        }    
        else if(!isGround && rb.velocity.y > 0)
        {
            anim.SetBool("jumping", true);
        }
        else if(rb.velocity.y < 0)
        {
            anim.SetBool("jumping", false);
            anim.SetBool("falling", true);
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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //收集物品
        if (collision.tag == "Collections")
        {
            cherryAudio.Play();
            //Destroy(collision.gameObject);
            //Cherry += 1;
            collision.GetComponent<Animator>().Play("isGot");
            //CherryNum.text = Cherry.ToString();
        }
        if (collision.tag == "DeadLine")
        {
            GetComponent<AudioSource>().enabled = false;
            Invoke(nameof(Restart), 2f);
        }
    }
    //笔记： OnTriggerEnter2D 和 OnCollisionEnter2D 所调用的函数不一样 
    //消灭敌人
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enimy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if (anim.GetBool("falling"))  //所以要获取这个gameobject的tag
            {
                enemy.JumpOn();
                rb.velocity = new Vector2(rb.velocity.x, jumpForce * Time.fixedDeltaTime);
                anim.SetBool("jumping", true);
            }
            else if (transform.position.x < collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(-7, 5);
                hurtAudio.Play();
                isHurt = true;
            }
            else if (transform.position.x > collision.gameObject.transform.position.x)
            {
                rb.velocity = new Vector2(7, 5);
                hurtAudio.Play();
                isHurt = true;
            }
        }
    }

    void Crouch()
    {
        if (!Physics2D.OverlapCircle(CellingCheck.position, 0.2f, ground))
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

    public void CherryCount()
    {
        Cherry += 1;
    }
}

