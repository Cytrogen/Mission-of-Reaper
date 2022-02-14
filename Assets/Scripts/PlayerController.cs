using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
 
public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rig;//刚体
    private Animator Anim;//角色的Animator
 
    [Header("Layers")]
    public LayerMask groundLayer;//用来开启layer
    public LayerMask WhatIsEnemies;
 
    [Space]
    [Header("Speed")]
    [SerializeField] private float moveSpeed=250f;//移动速度
 
    [Space]
    [Header("Force")]
    private float moveForce=175f;//移动力
    private float jumpForce=575f;//跳跃力
 
    [Space]
    [Header("Frequency")]
    private int jumpMax=1;//跳跃次数的上线
    [SerializeField] private int jumpNum=0;//当前跳跃的次数
 
    [Space]
    [Header("Booleans")]
    private bool falling = false;//用来标记是否是下落状态
    private bool onJumping;//是否正在跳跃中
    private bool onGround;//是否正在地上
    private bool onHurt;//是否正在地上
 
    [Space]
    [Header("Collision")]
    private float collisionRadius = 0.15f;//碰撞半径
    public Vector2 bottomOffset, rightOffset, leftOffset;//下左右相对于角色中心的二维向量
    private Collider2D coll;//角色的碰撞器
 
    [Space]
    [Header("UI")]
    private Text HpNumberText;
    [Space]
    private float face;//记录角色朝向
    private float HP;//角色血量
 

    void Start()
    {
        rig = GetComponent<Rigidbody2D>();//获取主角刚体组件
        Anim = GetComponent<Animator>();//获取主角动画组件
        coll = GetComponent<Collider2D>();//获取角色碰撞器
        HpNumberText= GameObject.Find("HpNumber").GetComponent<Text>();//获取ui对于的text

        groundLayer = 1 << 8;//开启Ground的layer层，Ground在layer8

        onJumping = false;//初始不正在跳跃
        onGround = true;//初始在地上
        onHurt = false;

        face = 1;//初始朝向向右边
        moveSpeed = 250f;//移动速度
        HP = 100f;

        bottomOffset = new Vector2(0,-0.27f);
        rightOffset = new Vector2(0.13f,-0.23f);
        leftOffset = new Vector2(-0.31f,-0.23f);
    }


    void Update()
    {
        if (HP <= 0)
        {
            SceneManager.LoadScene("Death");
        }
    }

 
    void FixedUpdate()
    {
        Movement();
        changeAnimator();
    }
 
    //控制移动
    void Movement()
    {
        float moveMultiple = Input.GetAxis("Horizontal");
        float faceDirection = Input.GetAxisRaw("Horizontal");
        
        if (onGround)//在地上
        {
            move(moveMultiple, faceDirection);//左右移动
            if (Input.GetButtonDown("Jump"))
            {
                onGround = false;
                onJumping = true;
                Jump();
            }
        }
        else if (onJumping)//在空中跳跃
        {
            if (Input.GetButtonDown("Jump"))//二段跳
            {
                Jump();
            }
        }
    }


    bool isGround()//判断是否碰地
    {
        return Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, collisionRadius, groundLayer);//判断是否碰到地面
    }
    

    void move(float moveMultiple,float faceDirection)//移动代码
    {
        //角色左右移动
        if (moveMultiple != 0)
        {
            //velocity表示速度，Vector表示向量
            rig.velocity = new Vector2(moveMultiple * moveSpeed * Time.deltaTime, rig.velocity.y);//输入x，y向量，数值*方向
        }
        //角色朝向修改
        if (faceDirection != 0)
        {
            //Scale，代表缩放，Localscale代表的是当前物体相对于父物体的缩放，通过正负不修改大小来实现左右朝向修改
            transform.localScale = new Vector2(faceDirection, 1);
            face = faceDirection;
        }
    }

    
    void Flip()
    {
        face = (face == 1) ? -1 : 1;//在墙上跳出的话，动画要和原来相反
        transform.Rotate(0f,180f,0f);//这样枪口也会一起转向
    }


    void Jump()//跳跃代码
    {
        if(jumpNum < jumpMax)
        {
            rig.velocity = new Vector2(rig.velocity.x, jumpForce * Time.deltaTime);
            jumpNum++;
        }   
    }


    void AccordingDirectionFlip(Collision2D collision)//根据敌人方向，安排玩家转向
    {
        if (collision != null)//如果玩家出现视野中
        {
            int direction;
            if (collision.transform.position.x < transform.position.x)
            {
                direction = -1;//玩家在敌人的左边
            }
            else
            {
                direction = 1;//玩家在敌人的右边
            }
            if (direction != face)//表示玩家朝向和敌人相对方位不一致
            {
                //Debug.Log(direction);
                Flip();
            }
        }
    }


    void changeAnimator()//动画切换
    {
    if (onGround)
        {
            Anim.SetFloat("speed", Mathf.Abs(rig.velocity.x));//速度是向量
            if (onHurt)
            {
                Anim.SetBool("injured", true);
                //Anim.SetBool("ground", false);
                onGround = false;
            }
        }
        if (onJumping)
        {
            if (onHurt)
            {
                Anim.SetBool("injured", true);
                jumpNum = 0;//当前跳跃次数清零
                falling = false;
                onJumping = false;
            }
            else if (Anim.GetBool("ground"))
            {
                falling = false;
                Anim.SetBool("ground", false);
            }
            else 
            {
                if (falling&&isGround())
                {
                    Anim.SetBool("ground", true);
                    jumpNum = 0;//落地的话，当前跳跃次数清零
                    falling = false;
                    onJumping = false;
                    onGround = true;
                }
                else if(rig.velocity.y < 0)
                {
                    falling = true;
                }
            }
 
        }
        if (onHurt)
        {
            if (falling && isGround())
            {
                Anim.SetBool("injured", false);
                Anim.SetBool("ground", true);
                falling = false;
                onGround = true;
                onHurt = false;
            }
            else if (rig.velocity.y < 0)
            {
                falling = true;
            }
        }
    }


    void Hurt(Collision2D collision)//受伤代码入口
    {
        onHurt = true;
        AccordingDirectionFlip(collision);
        rig.velocity = new Vector2(face * moveForce * Time.deltaTime, jumpForce * Time.deltaTime);//反弹
    }


    void OnDrawGizmos()//绘制辅助线
    {
        Gizmos.color = Color.red;//辅助线颜色
        //绘制圆形辅助线
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + rightOffset, collisionRadius);
        Gizmos.DrawWireSphere((Vector2)transform.position + leftOffset, collisionRadius);
    }


    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            HP -= 10f;//碰到一次敌人减去25血量
            HpNumberText.text = HP.ToString();
            Hurt(collision);
        }
    }
}