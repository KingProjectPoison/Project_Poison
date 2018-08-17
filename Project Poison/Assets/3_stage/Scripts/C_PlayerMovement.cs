
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class C_PlayerMovement : MonoBehaviour {

    public C_ScoreUpdate UIM;

    // 플레이어가 바라보는 방향
    public enum FaceDirection { FaceRight = -1, FaceLeft = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;

    //이동
    float dirX, dirY;

    //속도
    public float MaxSpeed = 10f, Speed = 5f;

    bool isWalk = false;

    //체력
    public int maxHealth = 600;
    int health;
    bool isDie = false;

    //아이템
    bool isItem = false;
    //public GameObject[] phone;
    public Transform[] PathCenter;
    public GameObject item;
    public Transform[] itemarr;

    private Transform ThisTransform = null;
    int i = 1;
    public float PathCenterposX = 5f;
    public GameObject[] Enemy;
    private C_EnemyMove[] EnemyScript = null;
    int EnemyIndex = 0;
    bool isEnemyIndex = false;

    //충돌
    Rigidbody2D rigid;

    //애니메이터
    Animator animator;

    // 애니메이션을 위한
    public Animator faceAnimator;
    public Animator handsAnimator;
    public Animator bodyAnimator;

    public SpriteRenderer[] spriteRenderer;

    //충돌시 깜빡거림
    bool isUnBeatTime = false;

    bool flag = true;

    Vector3 movement;
    public float speed = 20; //속도

    // Use this for initialization
    void Awake() {
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
        ThisTransform = GetComponent<Transform>();

        health = maxHealth;
        EnemyScript = new C_EnemyMove[Enemy.Length];
        for(int i = 0; i < EnemyScript.Length; i++)
        {
            EnemyScript[i] = Enemy[i].GetComponent<C_EnemyMove>();
        }
        for (i = 0; i < PathCenter.Length; i++)
        {
            item.transform.position = PathCenter[i].transform.position;
            item.SetActive(false);
        }

        StartCoroutine("ItemTime");
    }
    void SetItemOn()
    {
        isItem = false;
    }
 
    public void Shuffle()
    {
        for(int i = 0; i< itemarr.Length; i++)
        {
            int random = Random.Range(0, itemarr.Length);
            Transform temp = itemarr[random];
            itemarr[random] = itemarr[i];
            itemarr[i] = temp;
        }
    }
    IEnumerator ItemTime()
    {
        while (true)
        {
            Shuffle();
            //Instantiate(생성시킬 오브젝트, 생성될 위치, 생성됐을때 회전값);
            //
            for(int i = 0; i<itemarr.Length; i++)
            {
                item.transform.position = itemarr[i].position;
                item.SetActive(true);
                yield return new WaitForSeconds(5f);
                item.SetActive(false);
            }
           
        }

    }

    void FixedUpdate()
    {
        dirX = CrossPlatformInputManager.GetAxis("Horizontal");
        dirY = CrossPlatformInputManager.GetAxis("Vertical");

        rigid.velocity = new Vector2(dirX * speed, dirY * speed);

        // 필요한 경우 방향을 바꾼다.
        if (((dirX < 0f && Facing == FaceDirection.FaceRight) || (dirX > 0f && Facing == FaceDirection.FaceLeft)))
            FlipDirection();
        // 속도를 제한한다.
        rigid.velocity = new Vector2(Mathf.Clamp(rigid.velocity.x, -MaxSpeed, MaxSpeed), Mathf.Clamp(rigid.velocity.y, -MaxSpeed, MaxSpeed));

        // 이동 여부 확인
        isWalk = handsAnimator.GetBool("walk");

        // 걷는 모션
        if (isWalk && rigid.velocity.x == 0 && rigid.velocity.y ==0)
        {
            handsAnimator.SetBool("walk", false);
            bodyAnimator.SetBool("walk", false);
        }
        if (!isWalk && (rigid.velocity.x != 0 || rigid.velocity.y !=0))
        {
            handsAnimator.SetBool("walk", true);
            bodyAnimator.SetBool("walk", true);
        }

    }

    // 캐릭터 방향을 바꾼다.
    public void FlipDirection()
    {
        Facing = (FaceDirection)((int)Facing * -1f);
        Vector3 LocalScale = ThisTransform.localScale;
        LocalScale.x *= -1f;
        ThisTransform.localScale = LocalScale;
    }

    void Die()
    {
        //Die Flag On
        isDie = true;

        rigid.velocity = Vector2.zero;

        //Die Motion
        animator.Play("Die");

    }
    void RestartStage()
    {
        GameManager.RestartStage();
    }

    // Update is called once per frame
    //moving
   

    void OnTriggerEnter2D(Collider2D other)
    {
        //rigidBody가 무언가와 충돌할때 호출되는 함수
         //Collider2D other로 부딪힌 객체를 받아온다.
        //적과 충돌
         if (other.CompareTag("enemy") && !isUnBeatTime)
         {
            if (C_ScoreUpdate.instance.isItem)
            {
                if(!isEnemyIndex)
                {
                    isEnemyIndex = true;
                    EnemyIndex = other.GetComponent<C_EnemyMove>().PathCenterIndex;
                }
                float enemyX = other.transform.position.x;
                Rigidbody2D enemyBody = other.GetComponent<Rigidbody2D>();
                if (enemyX - transform.position.x > 0)
                {
                    enemyBody.velocity = Vector3.right * 20;
                }
                else
                {
                    enemyBody.velocity = Vector3.left * 20;
                }
            }
            else if (!isUnBeatTime)
            {
                health -= 25;
                UIM.HitPlayer(25);
                if (health > 0)
                {
                    isUnBeatTime = true;
                    StartCoroutine("UnBeatTime");
                }
            }
         }

         //아이템충돌
        else if (other.CompareTag("Item"))
        {
            item.SetActive(false);
            isItem = true;
            C_ScoreUpdate.instance.ScoreSet();
        }

    }

    public void CheckItem()
    {
        StartCoroutine("Itemact");
    }

    
    // 크기 조정
    IEnumerator Itemact()
    {
        C_ScoreUpdate.instance.isItem = true;
        Vector3 ThisSize = transform.localScale;
        while (true)
        {
            if (Mathf.Abs(ThisSize.x) == 30)
            {
                if (ThisSize.x < 0)
                {
                    ThisSize.x = -30;
                    ThisSize.y = 30;
                }
                else
                    ThisSize.x = ThisSize.y = 30;
                transform.localScale = ThisSize;
                break;
            }
            else
            {
                if (ThisSize.x < 0)
                    ThisSize.x -= 1;
                else
                    ThisSize.x += 1;
                ThisSize.y = Mathf.Abs(ThisSize.x);
                transform.localScale = ThisSize;
                yield return new WaitForSeconds(0.1f);
            }


        }
        float keeptime = C_ScoreUpdate.instance.accumulate * 1.5f;
        C_ScoreUpdate.instance.ScoreReset();
        yield return new WaitForSeconds(keeptime);

        while (true)
        {
            if (Mathf.Abs(ThisSize.x) == 17)
            {
                if (ThisSize.x < 0)
                {
                    ThisSize.x = -17;
                    ThisSize.y = 17;
                }
                else
                    ThisSize.x = ThisSize.y = 17;
                transform.localScale = ThisSize;
                break;
            }
            else
            {
                if (ThisSize.x < 0)
                    ThisSize.x += 1;
                else
                    ThisSize.x -= 1;
                ThisSize.y = Mathf.Abs(ThisSize.x);
                transform.localScale = ThisSize;
                yield return new WaitForSeconds(0.1f);
            }

        }
        C_ScoreUpdate.instance.isItem = false;
        yield return new WaitForSeconds(3f);
        for (int i = 0; i < Enemy.Length; i++)
        {
            Enemy[i].SetActive(false);
        }
        yield return new WaitForSeconds(1f);
        for (int i = 0; i<Enemy.Length; i++)
        {
            EnemyScript[i].InitPosition(EnemyIndex);
            Enemy[i].SetActive(true);
            EnemyScript[i].isDrag = false;
        }
    }

    //깜박거리기
    IEnumerator UnBeatTime()
    {
            int countTime = 0;
            while (countTime < 10)
            {
                for (int i = 0; i < spriteRenderer.Length; i++)
                {
                    if (countTime % 2 == 0)
                        spriteRenderer[i].color = new Color32(255, 255, 255, 90);
                    else
                        spriteRenderer[i].color = new Color32(255, 255, 255, 180);
                }
                yield return new WaitForSeconds(0.2f);
                    countTime++;
            }
        for (int i = 0; i < spriteRenderer.Length; i++)
            spriteRenderer[i].color = new Color32(255, 255, 255, 255);
    
        isUnBeatTime = false;
    }

}


