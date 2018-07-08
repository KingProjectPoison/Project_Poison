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
    public Rigidbody2D bullet;
    public int AttackLimit = 15;
    public Collider2D shield;
    public int ShieldLimit = 5;
    public Transform center;
    private Collider2D ThisCollider;
    public GameObject playerShield;
    // 애니메이션을 위한
    private Animator myAnimator;
    // UIAttackManager관련
    public B_UIManager UIM;

    // 체력 체크
    public static float Health
    {
        get
        {
            return _Health;
        }

        set
        {
            _Health = value;
            // 캐릭터가 죽은 경우 게임을 끝낸다.
            if(_Health <= 0)
            {
                Die();
            }
        }
    }

    [SerializeField]
    private static float _Health = 100f;
    // Use this for initialization
    private void Awake()
    {
        // 이 객체의 정보들을 담는다.
        ThisBody = GetComponent<Rigidbody2D>();
        ThisTransform = GetComponent<Transform>();
        myAnimator = GetComponent<Animator>();
        ThisCollider = GetComponent<Collider2D>();

        playerShield.SetActive(false);

        // 정적 인스턴스를 설정한다.
        PlayerInstance = this;
    }

    // 플레이어가 착지 상태인지 여부를 반환한다.
    private bool GetGrounded()
    {
        // 바닥을 확인한다
        Collider2D[] HitColliders = Physics2D.OverlapAreaAll(new Vector2(transform.position.x - 1f, transform.position.y - 1f),
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

    // 이단 점프를 방지하기 위해 점프 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateJump()
    {
        CanJump = true;
    }

    // 이동 처리
    public void LeftMove()
    {
        if (ThisBody.velocity.x < 0)
            ThisBody.velocity = new Vector2(0, 0);
        else if (ThisBody.velocity.x >= 0)
        {
            if (Facing != FaceDirection.FaceLeft)
                FlipDirection();
            ThisBody.velocity = new Vector2(-MaxSpeed, 0);
        }
    }
    public void RightMove()
    {
        if (ThisBody.velocity.x > 0)
            ThisBody.velocity = new Vector2(0, 0);
        else if (ThisBody.velocity.x <= 0)
        {
            if (Facing != FaceDirection.FaceRight)
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
            myAnimator.SetTrigger("Attack");
            new WaitForSeconds(3);
            var waterBullet = Instantiate(bullet, barrel.position, barrel.rotation);
            waterBullet.AddForce(barrel.up * bulletSpeed);
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
        // 캐릭터를 조종할 수 없으면 종료한다.
        if (!CanControl || Health <= 0f)
            return;

        // 점프
        isGrounded = GetGrounded();
        float Horz = CrossPlatformInputManager.GetAxis(HorzAxis);
        ThisBody.AddForce(Vector2.right * Horz * MaxSpeed);

        // 속도를 제한한다.
        ThisBody.velocity = new Vector2(Mathf.Clamp(ThisBody.velocity.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(ThisBody.velocity.y, -Mathf.Infinity, JumpPower));

        // 필요한 경우 방향을 바꾼다.
        if ((Horz < 0f && Facing != FaceDirection.FaceLeft) || (Horz > 0f && Facing != FaceDirection.FaceRight))
            FlipDirection();
            
    }

    private void OnDestroy()
    {
        PlayerInstance = null;
    }

    // 플레이어를 죽이는 함수
    static void Die()
    {
        Destroy(B_PlayerControl.PlayerInstance.gameObject);
    }

    // 플레이어를 기본 상태로 재설정한다.
    public static void Reset()
    {
        Health = 100f;
    }

}
