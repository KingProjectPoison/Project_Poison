using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class B_PlayerControl : MonoBehaviour {

    // 플레이어가 바라보는 방향
    public enum FaceDirection { FaceRight = -1, FaceLeft = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;
    // 바닥 태그가 지정된 오브젝트
    public LayerMask GroundLayer;
    // player에 대한 참조
    private Rigidbody2D ThisBody = null, bulletBody = null;
    private Transform ThisTransform = null;
    private CapsuleCollider2D ThisCollider = null;
    // 착지 여부
    public bool isGrounded = false;
    // 이동 여부
    public bool isDrag = false;
    // 속도 변수
    public float MaxSpeed = 10f, Speed = 5f;
    public float JumpPower = 500;
    public float JumpTimeOut = 1f;
    public float bulletSpeed = 500f;
    public float AttackTimeOut = 3f;
    public float shieldTimeOut = 5f;
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
    public Collider2D shield;
    int ShieldLimit;
    public GameObject playerShield;
    // 애니메이션을 위한
    public Animator faceAnimator;
    public Animator handsAnimator;
    public Animator bodyAnimator;
    // UIManager관련
    public B_UIManager UIM;
    public GameObject OK;
    // 체력 체크
    private int Health;
    bool HitFlag = true;
    // 아이템
    public bool isItem = false;
    public B_FlipHourglass HourglassScript;
    // 필요 변수 선언
    bool isFall = false, DragFlag = false, isFloor = false, isWalk, switchB = false, healFlag = true;
    float dirX;
    // 효과음
    public AudioClip playerJump, playerAttack, createShield, playerHit, itemSound;
    private AudioSource ThisAudio;

    // Use this for initialization
    private void Awake()
    {
        // 이 객체의 정보들을 담는다.
        ThisAudio = GetComponent<AudioSource>();
        ThisBody = GetComponent<Rigidbody2D>();
        ThisTransform = GetComponent<Transform>();
        ThisCollider = GetComponent<CapsuleCollider2D>();
        bulletBody = bullet.GetComponent<Rigidbody2D>();
        playerShield.SetActive(false);
        bullet.gameObject.SetActive(false);
        FlipDirection();
        Health = UIM.playerMaxHP;
        ShieldLimit = UIM.shieldLimit;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        // 이동
        if (!isDrag)
        {
            dirX = CrossPlatformInputManager.GetAxis("Horizontal");
            ThisBody.velocity = new Vector2(dirX * Speed, ThisBody.velocity.y);
        }

        // 땅에 서 있는지의 여부 체크
        isGrounded = GetGrounded();
        // (땅에 서 있는데 이동불가인 상태)*는 0.4초간만 지속된다. (공격 당했을 시 + 페이크 문에 닿았을 시)*
        if (isGrounded && isDrag && !DragFlag)
        {
            DragFlag = true;
            Invoke("DragOff", 0.4f);
        }
        // 점프
        if (CrossPlatformInputManager.GetButtonDown("Jump"))
            Jump();

        // 속도를 제한한다.
        ThisBody.velocity = new Vector2(Mathf.Clamp(ThisBody.velocity.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(ThisBody.velocity.y, -Mathf.Infinity, JumpPower));

        // 이동 여부 확인
        isWalk = handsAnimator.GetBool("walk");

        // 걷는 모션
        if (isWalk && ThisBody.velocity.x == 0)
        {
            handsAnimator.SetBool("walk", false);
            bodyAnimator.SetBool("walk", false);
        }
        if (!isWalk && ThisBody.velocity.x != 0)
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
        else if (isFall && isGrounded)
        {
            isFall = false;
            handsAnimator.SetTrigger("land");
        }

        // 필요한 경우 방향을 바꾼다.
        if (!isDrag && ((dirX < 0f && Facing == FaceDirection.FaceRight) || (dirX > 0f && Facing == FaceDirection.FaceLeft)))
            FlipDirection();

        if (switchB && Facing == FaceDirection.FaceLeft)
            FlipDirection();

    }

    // 플레이어가 착지 상태인지 여부를 반환한다.
    private bool GetGrounded()
    {
        // 바닥을 확인한다
        Vector3 ThisPosition = ThisTransform.position;
        Collider2D[] HitColliders = Physics2D.OverlapAreaAll(new Vector2(ThisPosition.x - 0.05f, ThisPosition.y - 1f),
            new Vector2(ThisPosition.x + 0.05f, ThisPosition.y - 0.8f), GroundLayer);
        if (HitColliders.Length > 0)
            return true;
        return false;
    }

    // 캐릭터 방향을 바꾼다.
    public void FlipDirection()
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
        B_SoundManager.instance.PlaySingle(playerJump);
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
        if (!CanAttack)
            return;

        ThisAudio.clip = playerAttack;
        ThisAudio.Play();
        CanAttack = false;
        faceAnimator.SetTrigger("attack");
        handsAnimator.SetTrigger("attack");
        bullet.gameObject.SetActive(true);
        bullet.position = barrel.position;
        bullet.rotation = barrel.rotation;
        bulletBody.AddForce(barrel.up * bulletSpeed);
        UIM.Attack();
        Invoke("ActivateBullet", AttackTimeOut);
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
            B_SoundManager.instance.PlaySingle(createShield);
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

    // 벽, 바닥에 옆면에 붙어있기 금지
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
        {
            if (!isGrounded)
            {
                isDrag = true;
                if (Facing == FaceDirection.FaceLeft)
                    ThisBody.AddForce(new Vector2(1, -1) * 5);
                else if(Facing == FaceDirection.FaceRight)
                    ThisBody.AddForce(new Vector2(-1, -1) * 5);
            }
            else
                isDrag = false;
        }
        else if (collision.CompareTag("side"))
        {
            bool fc = false;
            float playerX = ThisTransform.position.x;
            float collisionX = collision.transform.position.x;
            if ((collisionX < playerX) && Facing == FaceDirection.FaceLeft)
                fc = true;
            else if ((collisionX > playerX) && Facing == FaceDirection.FaceRight)
                fc = true;
            if (!isGrounded && fc)
            {
                if (!isFloor)
                {
                    isDrag = true;
                    if (Facing == FaceDirection.FaceLeft)
                        ThisBody.AddForce(new Vector2(1, -1) * 5);
                    else if (Facing == FaceDirection.FaceRight)
                        ThisBody.AddForce(new Vector2(-1, -1) * 5);
                }
            }
            else
                isDrag = false;
        }
    }

    private void OffIsFloor()
    {
        isFloor = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("floor"))
            isFloor = false;
    }

    // 말탄환에 맞거나 enemy와 충돌했을 경우
    private void OnTriggerEnter2D(Collider2D collision)
    {
        // 점프 시 발판에 방해받지 않게 하기
        if (collision.CompareTag("floor"))
        {
            Vector2 playerXY = ThisTransform.position;
            Vector2 collisionXY = collision.transform.position;
            if (collisionXY.y > playerXY.y - 0.5f)
            {
                isFloor = true;
                Invoke("OffIsFloor", 0.5f);
                ThisCollider.isTrigger = true;
            }
            else
                ThisCollider.isTrigger = false;
        }

        if (Health != 0 && HitFlag && !shield.isActiveAndEnabled)
        {
            // 말탄환에 맞았을 경우
            if (collision.CompareTag("letterbullet"))
            {
                B_SoundManager.instance.PlaySingle(playerHit);
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
                    if (Health >= 600)
                        Health = 600;
                }
                faceAnimator.SetTrigger("hit");
                handsAnimator.SetTrigger("hit");
                Invoke("HitFlagOn", 1f);
                // 맞은 방향 반대편으로 밀려나는 효과
                isDrag = true;
                float playerX = ThisTransform.position.x;
                float collisionX = collision.transform.position.x;
                if (collisionX < playerX)
                {
                    ThisBody.velocity = Vector2.right * 2;
                }
                else if (collisionX > playerX)
                {
                    ThisBody.velocity = Vector2.left * 2;
                }
                Invoke("DragOff", 1f);
            }

            // 적과 충돌한 경우
            if (collision.CompareTag("enemy1") || collision.CompareTag("enemy2"))
            {
                B_SoundManager.instance.PlaySingle(playerHit);
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
                    if (Health >= 600)
                        Health = 600;
                }
                faceAnimator.SetTrigger("hit");
                handsAnimator.SetTrigger("hit");
                Invoke("HitFlagOn", 0.5f);
                // 맞은 방향 반대편으로 밀려나는 효과
                isDrag = true;
                float playerX = ThisTransform.position.x;
                float collisionX = collision.transform.position.x;
                if (collisionX < playerX)
                {
                    ThisBody.velocity = Vector2.right * 4;
                }
                else if (collisionX > playerX)
                {
                    ThisBody.velocity = Vector2.left * 4;
                }
                Invoke("DragOff", 0.5f);
            }
        }

        if (collision.CompareTag("door0"))
        {
            switchB = true;
            FlipDirection();
        }

    }

    private void DragOff()
    {
        isDrag = false;
        DragFlag = false;
    }

    private void HitFlagOn()
    {
        HitFlag = true;
    }

    // Door0에 닿으면 왼쪽 아래로 이동하는데, 이 때 앞으로 튕겨져 나오는 듯한 효과 주기
    public void Door0()
    {
        isDrag = true;
        ThisTransform.position = new Vector2(-6.4f, -4f);
        ThisBody.velocity = Vector2.right * 6;
        Invoke("DragOff", 0.5f);
        Invoke("SwitchBOff", 0.5f);
    }

    private void SwitchBOff()
    {
        switchB = false;
    }

    // 아이템 관련
    public void ItemOn()
    {
        UIM.HealPlayer(100);
        Health += 100;
        if (Health > 600)
            Health = 600;
        ThisAudio.clip = itemSound;
        ThisAudio.Play();
        UIM.isItem = true;
        HourglassScript.doFlip = true;
        isItem = true;
        Invoke("ItemOff", 10f);
    }

    private void ItemOff()
    {
        ThisAudio.clip = itemSound;
        ThisAudio.Play();
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
