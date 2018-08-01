using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_EnemyMovement : MonoBehaviour {

    private string ThisName;
    // 바라보는 방향
    public enum FaceDirection { FaceLeft = -1, FaceRight = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;
    // 이 객체에 대한 참조
    private Rigidbody2D ThisBody = null;
    private Transform ThisTransform = null;
    private Collider2D ThisCollider = null;
    // 속도 변수
    public float Speed = 10f;
    public float bulletSpeed = 500;
    [SerializeField]
    public float[] bulletTimeOut;
    // 공격/방어를 위한
    public Transform[] barrel;
    public Transform[] bullet;
    public float destroyTime = 1f;
    public B_PlayerControl target;
    private Transform targetTransform;
    private bool enemy1_CanAttack = true;
    private bool enemy2_CanAttack = true;
    // 애니메이션을 위한
    public Animator headAnimator, wingsAnimator, bodyAnimator;
    public GameObject waterball;
    // 이동 관련
    [SerializeField]
    public Transform[] rotationCenter;
    public float rotationRadius = 2f, angularSpeed = 2f;
    float posX, posY, angle = -1f, digree;
    int i = 0;
    bool flag = true, flip = true, clear = false;
    Vector2 targetDir, Dir;
    //  체력 5칸
    public int Health = 5;
    public B_UIManager UIM;
    // 클리어 관련
    public GameObject ClearEnemy, HitMessage, item1, item2;

    // Use this for initialization
    private void Awake()
    {
        // 이 객체의 정보들을 담는다.
        ThisBody = GetComponent<Rigidbody2D>();
        ThisTransform = GetComponent<Transform>();
        ThisCollider = GetComponent<Collider2D>();
        ThisName = ThisTransform.tag;
        targetTransform = target.GetComponent<Transform>();
        for (int i = 0; i < bullet.Length; i++)
        {
            bullet[i].gameObject.SetActive(false);
        }
        waterball.SetActive(false);
        ClearEnemy.SetActive(false);
        HitMessage.SetActive(false);
    }

    // 캐릭터 방향을 바꾼다.
    private void FlipDirection()
    {
        Facing = (FaceDirection)((int)Facing * -1f);
        Vector3 LocalScale = ThisTransform.localScale;
        LocalScale.x *= -1f;
        ThisTransform.localScale = LocalScale;
        HitMessage.GetComponent<SpriteRenderer>().flipX = flip;
        flip = !flip;
    }

    // Update is called once per frame
    void FixedUpdate () {
        // enemy1의 이동 기본 패턴
        if (ThisName.Equals("enemy1"))
        {
            if (flag)
            {
                posX = rotationCenter[i].position.x - Mathf.Cos(angle) * rotationRadius;
                posY = rotationCenter[i].position.y - Mathf.Sin(angle) * rotationRadius;
                transform.position = new Vector2(posX, posY);
                angle = angle + Time.deltaTime * angularSpeed;
            }
        }
        // enemy2의 이동 기본 패턴
        else if (ThisName.Equals("enemy2"))
        {
            if (flag)
            {
                posX = rotationCenter[0].position.x - Mathf.Cos(angle) * rotationRadius;
                posY = rotationCenter[0].position.y - Mathf.Sin(angle) * rotationRadius;
                transform.position = new Vector2(posX, posY);
                angle = angle + Time.deltaTime * angularSpeed;
            }

            if (enemy2_CanAttack)
            {
                enemy2_CanAttack = false;
                StartCoroutine(Enemy2_Attack());
            }
        }

        // target(플레이어)가 있는 방향으로 고개를 돌림
        if (targetTransform.position.x - transform.position.x > 0)
        {
            if (Facing == FaceDirection.FaceLeft)
                FlipDirection();
        }
        else
        {
            if (Facing == FaceDirection.FaceRight)
                FlipDirection();
        }

    }

    // enemy2 공격 함수
    IEnumerator Enemy2_Attack()
    {
        flag = false;
        // 상하좌우+대각선4방향으로 말탄환을 날린다.
        for (int k = 0; k < 8; k++)
        {
            targetDir = barrel[k].position;
            Dir = targetDir - (Vector2)transform.position;
            bullet[k].gameObject.SetActive(true);
            bullet[k].position = barrel[k].position;
            bullet[k].rotation = barrel[k].rotation;
            yield return new WaitForSecondsRealtime(0.1f);
            bullet[k].GetComponent<B_DestroyInTime>().MoveBullet(Dir, bulletSpeed, 1.5f - 0.025f*k);
        }
        headAnimator.SetTrigger("attack");
        wingsAnimator.SetTrigger("attack");
        yield return new WaitForSecondsRealtime(1f);
        Invoke("setFlag", 0.8f);
        Invoke("ActivateBullets", bulletTimeOut[Random.Range(0, bulletTimeOut.Length)]);
    }

    // enemy2 공격 딜레이
    private void ActivateBullets()
    {
        enemy2_CanAttack = true;
    }

    // enemy1의 이동 경로 조작. 네 개의 원의 교차점에서는 잠시 멈춰 공격을 날리고 다른 원을 따라 움직이는 것을 반복한다.
    public void Change_i()
    {
        flag = false;
        i = (i + 1) % 4;
        angle -= 1.47f;
        Enemy1_Attack();
        Invoke("setFlag", 0.8f);
    }

    private void setFlag()
    {
        if(!waterball.activeSelf)
            flag = true;
    }

    // enemy1 공격 함수
    public void Enemy1_Attack()
    {
        if (!enemy1_CanAttack)
            return;
        
        enemy1_CanAttack = false;
        headAnimator.SetTrigger("attack");
        wingsAnimator.SetTrigger("attack");
        // target(플레이어) 방향으로 말탄환을 날린다.
        targetDir = targetTransform.position;
        Dir = targetDir - (Vector2) transform.position;
        if (Facing == FaceDirection.FaceLeft)
            digree = Mathf.Atan2(barrel[0].position.y - targetDir.y, barrel[0].position.x - targetDir.x) * 180f / Mathf.PI;
        else
            digree = - Mathf.Atan2(barrel[0].position.y - targetDir.y, barrel[0].position.x - targetDir.x) * 180f / Mathf.PI + 180;
        barrel[0].Rotate(0, 0, digree);
        int k = Random.Range(0, bullet.Length);
        bullet[k].gameObject.SetActive(true);
        bullet[k].GetComponent<B_DestroyInTime>().Invoke("Delete", destroyTime);
        bullet[k].position = barrel[0].position;
        bullet[k].rotation = barrel[0].rotation;
        bullet[k].GetComponent<Rigidbody2D>().AddForce(Dir.normalized * bulletSpeed);
        // 공격 딜레이
        Invoke("ActivateBullet", bulletTimeOut[0]);
    }

    // enemy1 공격 딜레이. 제한 시간이 지나야 CanJump 변수를 활성화한다.
    private void ActivateBullet()
    {
        enemy1_CanAttack = true;
        barrel[0].Rotate(0, 0, -digree);
    }

    // 공격 당했을 시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("waterbullet") && Health != 0)
        {
            Health--;
            headAnimator.SetTrigger("hit");
            wingsAnimator.SetTrigger("hit");
            bodyAnimator.SetTrigger("hit");
            flag = false;
            if (transform.tag.Equals("enemy1"))
                UIM.HitEnemy1();
            else if(transform.tag.Equals("enemy2"))
                UIM.HitEnemy2();
            waterball.SetActive(true);
            HitMessage.SetActive(true);
            if(Health > 0)
                Invoke("SetFlagAndBall", 2f);
            else
                Invoke("SetFlagAndBall", 1.5f);
        }
    }

    private void SetFlagAndBall()
    {
        waterball.SetActive(false);
        HitMessage.SetActive(false);
        if (Health > 0)
            flag = true;
        else
            Clear();
    }

    // 적이 클리어된 경우
    public void Clear()
    {
        if (!clear)
        {
            print("이동");
            ClearEnemy.GetComponent<Transform>().position = new Vector2(transform.position.x, transform.position.y - 1.2f);
            if (gameObject.tag.Equals("enemy1"))
                UIM.flag1 = false;
            else if (gameObject.tag.Equals("enemy2"))
                UIM.flag2 = false;
            gameObject.SetActive(false);
            ClearEnemy.SetActive(true);
            if (!UIM.flag1 && !UIM.flag2)
            {
                item1.SetActive(false);
                item2.SetActive(false);
            }
        }
        clear = true;
    }

}
