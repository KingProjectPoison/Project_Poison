
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PlayerMovement : MonoBehaviour {

    //이동
    public float movePower = 6f;
    public bool inputLeft = false;
    public bool inputRight = false;
    public bool inputUp = false;
    public bool inputDown = false;

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

    //bool isWalk = false;

    SpriteRenderer spriteRenderer;

    //충돌시 깜빡거림
    bool isUnBeatTime = false;

    bool flag = true;

    Vector3 movement;
    public float speed = 20; //속도

    // Use this for initialization
    void Awake() {
        rigid = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponentInChildren<Animator>();
        spriteRenderer = gameObject.GetComponentInChildren<SpriteRenderer>();

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
        //Check Health
        if (health == 0)
            return;
        Move();
       
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
    void Move()
    {
        Vector3 moveVelocity = Vector3.zero;
        Vector3 ThisScale = transform.localScale;
        if ((!inputRight && !inputLeft && !inputUp && !inputDown))
        {
            animator.SetBool("isMoving", false);
        }
        else if (inputLeft)
        {
            animator.SetBool("isMoving", true);
            moveVelocity = Vector3.left;
            if(ThisScale.x < 0)
                ThisScale.x *= -1;
            transform.localScale = ThisScale;
        }
        else if (inputRight)
        {
            animator.SetBool("isMoving", true);
            moveVelocity = Vector3.right;
            if (ThisScale.x > 0)
                ThisScale.x *= -1;
            transform.localScale = ThisScale;
        }
        else if (inputUp)
        {
            animator.SetBool("isMoving", true);
            moveVelocity = Vector3.up;
        }
        else if (inputDown)
        {
            animator.SetBool("isMoving", true);
            moveVelocity = Vector3.down;
        }

        Vector3 pos = transform.position;
        if(pos.x > 9.6) //오른쪽으로 나감
        {
            inputRight = false;
        }
        else if(pos.x< -7.9)
        {
            inputLeft = false;
        }
        if (pos.y > 5.5)
        {
            inputUp = false;
        }
        else if (pos.y < -0.01)
        {
            inputDown = false;
        }
        transform.position += moveVelocity * movePower * Time.deltaTime * 10;
    }

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
                if (countTime % 2 == 0)
                    spriteRenderer.color = new Color32(255, 255, 255, 90);
                else
                    spriteRenderer.color = new Color32(255, 255, 255, 180);
                yield return new WaitForSeconds(0.2f);
                countTime++;
            }
            spriteRenderer.color = new Color32(255, 255, 255, 255);

            isUnBeatTime = false;
    }

    public void LeftDown()
    {
        inputLeft = true;
    }
    public void LeftUp()
    {
        inputLeft = false;
    }
    public void RightDown()
    {
        inputRight = true;
    }
    public void RightUp()
    {
        inputRight = false;
    }
    public void DownDown()
    {
        inputDown = true;
    }
    public void DownUp()
    {
        inputDown = false;
    }
    public void UpDown()
    {
        inputUp = true;
    }
    public void UpUp()
    {
        inputUp = false;
    }
}


