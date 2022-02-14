using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 

public class EnemyController : MonoBehaviour
{
    private Rigidbody2D rig;
    private Animator Anim;

    [Header("Layers")]
    public LayerMask playerLayer;
    [Space]
    [Header("Collision")]
    private Collider2D coll;
    [SerializeField] private float collisionRadius = 5f;
    Vector2 beg;
    Vector2 down = new Vector2(0, -1);
    [SerializeField] private float radialLength = 1.1f;
    [Space]
    [Header("Speed")]
    private float moveSpeed = 100f;
    [SerializeField] private float face;
    

    void Start () {
        rig = GetComponent<Rigidbody2D>();
        Anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
        face = -1;
        playerLayer = 1 << 9;
    }
 

    void FixedUpdate()
    {
        beg = transform.position;
        Collider2D playerColl = isPlayerView();
        if (isBorder())
        {
            Debug.Log("hello");
            rig.velocity = new Vector2(0,0);
            AccordingDirectionFlip(playerColl);
        }
        else
        {
            AccordingDirectionFlip(playerColl);
            Move();
        }
    }
 

    void AccordingDirectionFlip(Collider2D playerColl)
    {
        if (playerColl != null)
        {
            int direction;
            if (playerColl.transform.position.x < transform.position.x)
            {
                direction = -1;
            }
            else
            {
                direction = 1;
            }
            if (direction != face)
            {
                Flip();
            }
        }
    }


    void Flip()
    {
        face = (face == 1) ? -1 : 1;
        transform.localScale = new Vector2(face*(-1), 1);
    }
 

    bool isBorder()
    {
        if (!Physics2D.Raycast(beg,new Vector2(face,0)+down, radialLength, LayerMask.GetMask("Ground")))
        {
            return true;
        }
        return false;
    }
 

    Collider2D isPlayerView()
    {
        return Physics2D.OverlapCircle((Vector2)transform.position, collisionRadius,playerLayer);
    }
 

    void Move()
    {
        rig.velocity = new Vector2(face*moveSpeed * Time.deltaTime, rig.velocity.y);
    }
 

    void ChangeAnimator()
    {
        Anim.SetFloat("speed", Mathf.Abs(rig.velocity.x));
    }
 

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere((Vector2)transform.position, collisionRadius);
        Gizmos.DrawLine((Vector2)transform.position, (Vector2)transform.position+(new Vector2(face, 0) + down)*radialLength);
    }
}