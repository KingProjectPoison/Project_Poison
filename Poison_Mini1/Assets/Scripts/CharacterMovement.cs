using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour {

    public float Charc_Speed;
    public Transform groundCheck; //to check if player is touching ground
    public float jumpForce = 10f; //jump height
    public LayerMask whatIsGround;

    //movement related
    public float horizontal;
    public float vertical;
    bool jump = false;

    bool facingRight = true; //shifting image left & right
    bool grounded = false; // for detecting ground
    bool onLadder = false; //for ladder

    private float climbSpeed = 0.6f; //speed when climbing up the ladder
    float groundRadius = 0.01f; //distance from player to ground (when grounded)

    Animator anim;
    private Rigidbody2D rgbd;

    // Use this for initialization
    void Start () {
        rgbd = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
	}

    void Update()
    {
        //바닥에 있는 상태에서 jump키를 누르면 점프하기
        if(grounded && jump)
        {
            anim.SetBool("jump", true);
            rgbd.AddForce(new Vector2(0, jumpForce));
            jump = false;
        }
    }
	
	// Update is called once per frame
	void FixedUpdate () {
        grounded = Physics2D.OverlapCircle(groundCheck.position, groundRadius, whatIsGround);

        anim.SetBool("jump", !grounded);
        anim.SetFloat("vSpeed", rgbd.velocity.y); //how fast we are moving up n down

        //캐릭터 좌우 움직임
        rgbd.velocity = new Vector2(horizontal * Charc_Speed, rgbd.velocity.y);

        anim.SetFloat("walk", Mathf.Abs(horizontal));
        anim.SetFloat("climb", vertical);

        //좌우 버튼에 따라 플레이어 보는 방향 바꾸기
        if (horizontal > 0 && !facingRight)
            Flip();
        else if (horizontal < 0 && facingRight)
            Flip();

        //캐릭터가 사다리에 타고있을때의 움직임
        if (onLadder)
        { 
            rgbd.velocity = new Vector2(0, vertical * climbSpeed);
            rgbd.gravityScale = 0;
        }
	}

    //to move character (connected to buttons)
    public void CharacterMove(float hInput)
    {
        horizontal = hInput;
    }
    
    public void Climb(float vInput)
    {
        vertical = vInput;
    }

    public void Jump()
    {
        if (grounded)
        {
            this.rgbd.velocity += jumpForce * Vector2.up;
            jump = true;
        }

        //사다리에 타고있는 상태로 점프할때 (좌우버튼을 꼭 누르고 있어야함)
        if (onLadder && horizontal != 0)
        {
            this.rgbd.velocity += jumpForce * Vector2.up;
            jump = true;
            onLadder = false;
            anim.SetBool("ladder", false);
            rgbd.gravityScale = 1;
        }

        jump = false; 
    }

    //캐릭터 좌우반전
    void Flip()
    {
        facingRight = !facingRight;

        Vector3 theScale = transform.localScale;

        theScale.x *= -1;
        
        transform.localScale = theScale;
    }

    //사다리 타는 모션
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.tag == "Ladder" && vertical == 1)
        {
            onLadder = true;
            anim.SetBool("ladder", true);
        }

        if (other.tag == "Ladder" && vertical == -1)
        {
            onLadder = true;
            anim.SetBool("ladder", true);

            Debug.Log(other.gameObject.name);

            if(other.gameObject.name == "laddder(3)" || other.gameObject.name == "laddder(2)")
                GameObject.Find("2ndfloor").GetComponent<BoxCollider2D>().isTrigger = true;
            else
                GameObject.Find("3rdfloor").GetComponent<BoxCollider2D>().isTrigger = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if(other.tag == "Ladder")
        {
            onLadder = false;
            anim.SetBool("ladder", false);
            rgbd.gravityScale = 0.9f;
        }
    }
}
