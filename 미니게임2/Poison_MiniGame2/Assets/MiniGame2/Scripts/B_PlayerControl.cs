﻿using System.Collections;
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
    // 속도 변수
    public float MaxSpeed = 10f;
    public float JumpPower = 600;
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
    // 주 입력 축
    public string HorzAxis = "Horizontal";
    public string JumpButton = "Jump";
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
    
    bool isFall = false;

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
        Collider2D[] HitColliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 1.2f, transform.position.y - 1.2f),
            new Vector2(transform.position.x, transform.position.y), GroundLayer);
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

    // 왼쪽 대각선 점프를 처리한다.
    public void Jump_L()
    {
        if (!isGrounded || !CanJump)
            return;
        if (Facing == FaceDirection.FaceLeft)
            FlipDirection();
        // 점프한다.
        ThisBody.AddForce(new Vector2(-JumpPower*0.6f, JumpPower));
        CanJump = false;
        Invoke("ActivateJump", JumpTimeOut);
    }

    // 오른쪽 대각선 점프를 처리한다.
    public void Jump_R()
    {
        if (!isGrounded || !CanJump)
            return;
        if (Facing == FaceDirection.FaceRight)
            FlipDirection();
        // 점프한다.
        ThisBody.AddForce(new Vector2(JumpPower*0.6f, JumpPower));
        CanJump = false;
        Invoke("ActivateJump", JumpTimeOut);
    }

    // 이단 점프를 방지하기 위해 점프 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateJump()
    {
        CanJump = true;
    }

    // 이동 처리
    public void LeftMove()
    {
        if (ThisBody.velocity.x < 0)
        {
            handsAnimator.SetBool("walk", false);
            bodyAnimator.SetBool("walk", false);
            ThisBody.velocity = new Vector2(0, 0);
        }
        else if (ThisBody.velocity.x >= 0)
        {
            handsAnimator.SetBool("walk", true);
            bodyAnimator.SetBool("walk", true);
            if (Facing == FaceDirection.FaceLeft)
                FlipDirection();
            ThisBody.velocity = new Vector2(-MaxSpeed, 0);
        }
    }
    public void RightMove()
    {
        if (ThisBody.velocity.x > 0)
        {
            ThisBody.velocity = new Vector2(0, 0);
        }
        else if (ThisBody.velocity.x <= 0)
        {
            handsAnimator.SetBool("walk", true);
            bodyAnimator.SetBool("walk", true);
            if (Facing == FaceDirection.FaceRight)
                FlipDirection();
            ThisBody.velocity = new Vector2(+MaxSpeed, 0);
        }
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
        //점프
        isGrounded = GetGrounded();
        float Horz = CrossPlatformInputManager.GetAxis(HorzAxis);

        // 속도를 제한한다.
        ThisBody.velocity = new Vector2(Mathf.Clamp(ThisBody.velocity.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(ThisBody.velocity.y, -Mathf.Infinity, JumpPower));

        // 걷는 모션
        if (handsAnimator.GetBool("walk") && ThisBody.velocity.x == 0)
        {
            handsAnimator.SetBool("walk", false);
            bodyAnimator.SetBool("walk", false);
        }
       /* if (!handsAnimator.GetBool("walk") && ThisBody.velocity != Vstop)
        {
            handsAnimator.SetBool("walk", true);
            bodyAnimator.SetBool("walk", true);
        }*/

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
        if ((Horz < 0f && Facing != FaceDirection.FaceLeft) || (Horz > 0f && Facing != FaceDirection.FaceRight))
            FlipDirection();
            
    }

    // 말탄환에 맞거나 enemy와 충돌했을 경우
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (Health != 0 && HitFlag && !shield.isActiveAndEnabled)
        {
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
            }

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
                Invoke("HitFlagOn", 1f);
            }
        }

        // 클리어 후 문에 닿으면 방향 전환
        if (collision.tag.Equals("door0") || collision.tag.Equals("door1"))
        {
            FlipDirection();
        }
    }

    private void HitFlagOn()
    {
        HitFlag = true;
    }

    // 아이템 관련
    public void ItemOn()
    {
        UIM.isItem = true;
        HourglassScript.doFlip = true;
        isItem = true;
        Invoke("ItemOff", 10f);
        print("ItemOn");
    }

    private void ItemOff()
    {
        UIM.isItem = false;
        HourglassScript.doFlip = true;
        isItem = false;
        print("ItemOff");
    }

    // 플레이어를 죽이는 함수
    static void Die()
    {
        Destroy(B_PlayerControl.PlayerInstance.gameObject);
    }

}
