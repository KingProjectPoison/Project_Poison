using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
// using UnityEngine.UI;

public class B_PlayerControl : MonoBehaviour {

    // 플레이어가 바라보는 방향
    public enum FaceDirection { FaceLeft = -1, FaceRight = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;
    // 바닥 태그가 지정된 오브젝트
    public LayerMask GroundLayer;
    // rigidbody에 대한 참조
    private Rigidbody2D ThisBody = null;
    // transform에 대한 참조
    private Transform ThisTransform = null;
    // 착지 여부
    public bool isGrounded = false;
    // 이동 여부
    public bool drag = false;
    // 속도 변수
    public float MaxSpeed = 10f, Speed = 5f;
    public float JumpPower = 500;
    public float JumpTimeOut = 1f;
    public float bulletSpeed = 500f;
    public float bulletTimeOut = 3f;
    public float shieldTimeOut = 3f;
    // 현재 점프/공격/방어할 수 있는지 여부
    private bool CanJump = true;
    private bool CanAttack = true;
    private bool CanShield = true;
    // 플레이어를 조종할 수 있는지 여부
    public bool CanControl = true;
    public static B_PlayerControl PlayerInstance = null;
    // 공격/방어를 위한
    public Transform barrel;
    public Transform bullet;
    public int AttackLimit = 15;
    public Collider2D shield;
    public int ShieldLimit = 5;
    public GameObject playerShield;
    // 애니메이션을 위한
    public Animator faceAnimator;
    public Animator handsAnimator;
    public Animator bodyAnimator;
    // UIManager관련
    public B_UIManager UIM;
    // 체력 체크
    public int Health = 600;
    bool HitFlag = true;
    // 아이템
    public bool isItem = false;
    public B_FlipHourglass HourglassScript;
    // 필요 변수 선언
    bool isFall = false, DragFlag = false;
    float dirX;

    // Use this for initialization
    private void Awake()
    {
        // 이 객체의 정보들을 담는다.
        ThisBody = GetComponent<Rigidbody2D>();
        ThisTransform = GetComponent<Transform>();
        playerShield.SetActive(false);
        bullet.gameObject.SetActive(false);
        FlipDirection();
        
    }

    // 플레이어가 착지 상태인지 여부를 반환한다.
    private bool GetGrounded()
    {
        // 바닥을 확인한다
        Collider2D[] HitColliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 0.6f, transform.position.y - 1.2f),
            new Vector2(transform.position.x+0.6f, transform.position.y - 0.8f), GroundLayer);
        if (HitColliders.Length > 0)
            return true;
        return false;
    }

    // 캐릭터 방향을 바꾼다.
    private void FlipDirection()
    {
        Facing = (FaceDirection)((int)Facing * -1f);
        Vector3 LocalScale = ThisTransform.localScale;
        LocalScale.x *= -1f;
        ThisTransform.localScale = LocalScale;
    }

    // 점프를 처리한다.
    public void Jump()
    {
        if (!isGrounded || !CanJump)
            return;
        // 점프한다.
        ThisBody.AddForce(Vector2.up * JumpPower);
        CanJump = false;
        Invoke("ActivateJump", JumpTimeOut);
    }

    // 이단 점프를 방지하기 위해 점프 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateJump()
    {
        CanJump = true;
    }

    // 공격
    public void Attack()
    {
        if (AttackLimit <= 0 || !CanAttack)
            return;

        if (AttackLimit > 0 && CanAttack)
        {
            CanAttack = false;
            faceAnimator.SetTrigger("attack");
            handsAnimator.SetTrigger("attack");
            new WaitForSeconds(3);
            bullet.gameObject.SetActive(true);
            bullet.position = barrel.position;
            bullet.rotation = barrel.rotation;
            bullet.GetComponent<Rigidbody2D>().AddForce(barrel.up * bulletSpeed);
            AttackLimit--;
            UIM.Attack();
        }
        Invoke("ActivateBullet", bulletTimeOut);
    }

    // 공격 딜레이. 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateBullet()
    {
        CanAttack = true;
    }

    // 방어
    public void Shield()
    {
        if (ShieldLimit <= 0 || !CanShield)
            return;

        if (ShieldLimit > 0 && CanShield)
        {
            CanShield = false;
            playerShield.SetActive(true);
            ShieldLimit--;
            UIM.Shield();
        }
        Invoke("ActivateShield", shieldTimeOut);
    }

    // 방어 딜레이. 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateShield()
    {
        CanShield = true;
        playerShield.SetActive(false);
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // 이동
        if (!drag)
        {
            dirX = CrossPlatformInputManager.GetAxis("Horizontal");
            ThisBody.velocity = new Vector2(dirX * Speed, ThisBody.velocity.y);
        }

        //점프
        isGrounded = GetGrounded();
        if (isGrounded && drag && !DragFlag)
        {
            DragFlag = true;
            Invoke("DragOff", 0.5f);
        }
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
            Jump();

        // 속도를 제한한다.
        ThisBody.velocity = new Vector2(Mathf.Clamp(ThisBody.velocity.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(ThisBody.velocity.y, -Mathf.Infinity, JumpPower));

        // 걷는 모션
        if (handsAnimator.GetBool("walk") && ThisBody.velocity.x == 0)
        {
            handsAnimator.SetBool("walk", false);
            bodyAnimator.SetBool("walk", false);
        }
        if (!handsAnimator.GetBool("walk") && ThisBody.velocity.x != 0)
        {
            handsAnimator.SetBool("walk", true);
            bodyAnimator.SetBool("walk", true);
        }

        // 낙하 모션
        if (!isFall && ThisBody.velocity.y < 0 && !isGrounded)
        {
            isFall = true;
            handsAnimator.SetTrigger("fall");
        }
        else if(isFall && isGrounded)
        {
            isFall = false;
            handsAnimator.SetTrigger("land");
        }

        // 필요한 경우 방향을 바꾼다.
        if (!drag && ((dirX < 0f && Facing == FaceDirection.FaceLeft) || (dirX > 0f && Facing == FaceDirection.FaceRight)))
            FlipDirection();
            
    }

    // 벽, 바닥에 옆면에 붙어있기 금지
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("wall"))
        {
            if (!isGrounded)
            {
                drag = true;
            }
            else
                drag = false;
        }
        else if (collision.tag.Equals("side"))
        {
            bool fc = false;
            if ((collision.transform.position.x < transform.position.x) && Facing == FaceDirection.FaceRight)
                fc = true;
            else if ((collision.transform.position.x > transform.position.x) && Facing == FaceDirection.FaceLeft)
                fc = true;
            if (!isGrounded && fc)
            {
                drag = true;
            }
            else
                drag = false;
        }
    }

    // 말탄환에 맞거나 enemy와 충돌했을 경우
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Health != 0 && HitFlag && !shield.isActiveAndEnabled)
        {
            // 말탄환에 맞았을 경우
            if (collision.tag.Equals("letterbullet"))
            {
                HitFlag = false;
                int damage = collision.GetComponent<B_DestroyInTime>().power;
                // 아이템 x일 시, 체력이 데미지 양만큼 깎인다.
                if (!isItem)
                {
                    Health -= damage;
                    UIM.HitPlayer(damage);
                }
                // 아이템을 먹었다면, 체력이 데미지 양만큼 회복된다.
                else
                {
                    Health += damage;
                    UIM.HealPlayer(damage);
                }
                faceAnimator.SetTrigger("hit");
                handsAnimator.SetTrigger("hit");
                Invoke("HitFlagOn", 1f);
                // 맞은 방향 반대편으로 밀려나는 효과
                drag = true;
                if (collision.transform.position.x < transform.position.x)
                {
                    ThisBody.velocity = Vector2.right * 2;
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    ThisBody.velocity = Vector2.left * 2;
                }
                Invoke("DragOff", 1f);
            }

            // 적과 충돌한 경우
            if (collision.tag.Equals("enemy1") || collision.tag.Equals("enemy2"))
            {
                print("enemy in");
                HitFlag = false;
                // 아이템 x일 시, 체력이 데미지 양만큼 깎인다.
                if (!isItem)
                {
                    Health -= 30;
                    UIM.HitPlayer(30);
                }
                // 아이템을 먹었다면, 체력이 데미지 양만큼 회복된다.
                else
                {
                    Health += 30;
                    UIM.HealPlayer(30);
                }
                faceAnimator.SetTrigger("hit");
                handsAnimator.SetTrigger("hit");
                Invoke("HitFlagOn", 0.5f);
                // 맞은 방향 반대편으로 밀려나는 효과
                drag = true;
                if (collision.transform.position.x < transform.position.x)
                {
                    ThisBody.velocity = Vector2.right * 4;
                }
                else if (collision.transform.position.x > transform.position.x)
                {
                    ThisBody.velocity = Vector2.left * 4;
                }
                Invoke("DragOff", 0.5f);
            }
        }

        // 클리어 후 문에 닿으면 방향 전환
        if (collision.tag.Equals("door0") || collision.tag.Equals("door1"))
        {
            FlipDirection();
        }
    }

    private void DragOff()
    {
        drag = false;
        DragFlag = false;
    }

    private void HitFlagOn()
    {
        HitFlag = true;
    }

    // Door0에 닿으면 왼쪽 아래로 이동하는데, 이 때 앞으로 튕겨져 나오는 듯한 효과 주기
    public void Door0()
    {
        drag = true;
        ThisBody.velocity = Vector2.right * 6;
        Invoke("DragOff", 0.5f);
    }

    // 아이템 관련
    public void ItemOn()
    {
        UIM.isItem = true;
        HourglassScript.doFlip = true;
        isItem = true;
        Invoke("ItemOff", 10f);
    }

    private void ItemOff()
    {
        UIM.isItem = false;
        HourglassScript.doFlip = true;
        isItem = false;
    }

    // 플레이어를 죽이는 함수
    static void Die()
    {
        Destroy(B_PlayerControl.PlayerInstance.gameObject);
    }

}
