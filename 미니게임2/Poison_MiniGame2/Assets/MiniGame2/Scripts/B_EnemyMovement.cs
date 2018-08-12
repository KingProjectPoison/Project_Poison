using System.Collections;
using UnityEngine;

public class B_EnemyMovement : MonoBehaviour {

    private string ThisName;
    // 공격 패턴
    public B_UIManager.enemyMode mode = B_UIManager.enemyMode.normal;
    // 바라보는 방향
    public enum FaceDirection { FaceLeft = -1, FaceRight = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;
    // 이 객체에 대한 참조
    private Rigidbody2D ThisBody = null;
    private Transform ThisTransform = null;
    // 속도 변수
    public float Speed = 10f;
    public float bulletSpeed = 500;
    [SerializeField]
    public float[] bulletTimeOut;
    // 공격/방어를 위한
    public Transform markX_L, markX_R;
    public Transform LastBarrelCenter;
    public Transform[] LastBarrel1, LastBullet1, LastBarrel2, LeftFloor, RightFloor;
    private Transform tempFloor = null;
    public Transform BarrelCenter = null;
    public Transform[] barrel, bullet;
    private B_DestroyInTime[] Bullets = null, LastBullets1 = null;
    private int[] LastBullets2_index = {27, 14, 15, 16, 21, 22, 23};
    public float destroyTime = 1f;
    public B_PlayerControl target;
    private Transform targetTransform;
    private bool enemy1_CanAttack = true;
    private bool enemy2_CanAttack = true;
    // 애니메이션을 위한
    private Animator[] bulletAnimator;
    public GameObject[] bulletObject;
    public Animator headAnimator, wingsAnimator, bodyAnimator;
    public GameObject waterball, Ihead;
    public SpriteRenderer wingL, wingR;
    public Sprite VwingL, VwingR;
    // 이동 관련
    [SerializeField]
    public Transform[] rotationCenter;
    [SerializeField]
    public Transform[] Path_UpTogether;
    public float rotationRadius = 2f, angularSpeed = 2f;
    float posX, posY, angle = -1f, digree;
    int i = 0, page = 1, count = 0;
    bool flag = true, flip = true, clear = false, switchA = false, switchB = false, switchC = false;
    Vector2 targetDir, Dir;
    Vector3 UpMiddle = new Vector3(0, 16.7f, 0);
    //  체력 5칸
    int Health;
    public B_UIManager UIM;
    // 클리어 관련
    public GameObject ClearEnemy, HitMessage, item1, item2;
    private SpriteRenderer HitMsg;
    public Transform barrelPoint;
    // 효과음
    public AudioClip enemyHit, modeChange;
    private AudioSource ThisAudio;

    // Use this for initialization
    private void Awake()
    {
        // 이 객체의 정보들을 담는다.
        ThisAudio = GetComponent<AudioSource>();
        HitMsg = HitMessage.GetComponent<SpriteRenderer>();
        Health = UIM.enemyMaxHP;
        ThisBody = GetComponent<Rigidbody2D>();
        ThisTransform = GetComponent<Transform>();
        ThisName = ThisTransform.tag;
        targetTransform = target.GetComponent<Transform>();
        Bullets = new B_DestroyInTime[bullet.Length];
        LastBullets1 = new B_DestroyInTime[LastBullet1.Length];
        bulletAnimator = new Animator[bulletObject.Length];
        for (int i = 0; i < bullet.Length; i++)
        {
            bullet[i].gameObject.SetActive(false);
            Bullets[i] = bullet[i].GetComponent<B_DestroyInTime>();
        }
        for (int i = 0; i < LastBullet1.Length; i++)
        {
            LastBullet1[i].gameObject.SetActive(false);
            LastBullets1[i] = LastBullet1[i].GetComponent<B_DestroyInTime>();
        }
        for (int i = 0; i < bulletAnimator.Length; i++)
        {
            bulletAnimator[i] = bulletObject[i].GetComponent<Animator>();
        }
        waterball.SetActive(false);
        ClearEnemy.SetActive(false);
        HitMessage.SetActive(false);
    }

    // 캐릭터 방향을 바꾼다.
    private void FlipDirection()
    {
        Facing = (FaceDirection)((int)Facing * -1f);
        Vector3 LocalScale1 = ThisTransform.localScale;
        LocalScale1.x *= -1f;
        ThisTransform.localScale = LocalScale1;
        Vector3 LocalScale2 = barrelPoint.localScale;
        LocalScale2.x *= -1f;
        barrelPoint.localScale = LocalScale2;
        HitMsg.flipX = flip;
        flip = !flip;
    }

    // 모드를 확인한다. (모드는 UIManager가 관리한다.)
    private void CheckMode()
    {
        mode = UIM.mode;
    }

    // Update is called once per frame
    private void FixedUpdate() {
        
        // 모드 확인
        CheckMode();
        // 모드에 따라 코드 진행
        switch (mode) {
            /* normal 모드 일 때 */
            case B_UIManager.enemyMode.normal:
                // enemy1의 이동 기본 패턴
                if (ThisName.Equals("enemy1"))
                {
                    if (flag)
                    {
                        posX = rotationCenter[i].position.x - Mathf.Cos(angle) * rotationRadius;
                        posY = rotationCenter[i].position.y - Mathf.Sin(angle) * rotationRadius;
                        ThisTransform.position = new Vector2(posX, posY);
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
                        ThisTransform.position = new Vector2(posX, posY);
                        angle = angle + Time.deltaTime * angularSpeed;
                    }

                    if (enemy2_CanAttack)
                    {
                        enemy2_CanAttack = false;
                        StartCoroutine(Enemy2_Attack1());
                    }
                }
                break;

            /* UpTogether 모드 일 때 */
            case B_UIManager.enemyMode.UpTogether:
                // enemy1의 UpTogether 모드 이동 패턴
                if (ThisName.Equals("enemy1"))
                {
                    if (page == 1)
                    {
                        if (!switchA)
                        {
                            switchA = true;
                            StartCoroutine(ModeMotion());
                        }
                        if (Path_UpTogether[0].position == ThisTransform.position)
                        {
                            ThisBody.velocity = new Vector2(0, 0);
                            page = 2;
                        }
                        else
                            ThisBody.velocity = (Path_UpTogether[0].position - ThisTransform.position) * 5f;
                    }
                    else if (UIM.enemy2_page2)
                    {
                        if (Path_UpTogether[1].position == ThisTransform.position)
                        {
                            page = 1;
                            ThisBody.velocity = new Vector2(0, 0);
                        }
                        else
                            ThisBody.velocity = (Path_UpTogether[1].position - ThisTransform.position) * 5f;
                    }

                    if (enemy1_CanAttack)
                        Enemy1_Attack();

                }
                // enemy2의 UpTogether 모드 이동 패턴
                else if (ThisName.Equals("enemy2"))
                {
                    if (page == 1)
                    {
                        if (!switchA)
                        {
                            switchA = true;
                            StartCoroutine(ModeMotion());
                        }
                        if (Path_UpTogether[0].position == ThisTransform.position)
                        {
                            page = 2;
                            ThisBody.velocity = new Vector2(0, 0);
                        }
                        else
                            ThisBody.velocity = (Path_UpTogether[0].position - ThisTransform.position) * 5f;
                    }
                    else if (page == 2)
                    {
                        if (count == 0)
                        {
                            count += 1;
                            StartCoroutine(Enemy2_Attack2());
                        }
                        UIM.enemy2_page2 = true;
                        if (Path_UpTogether[1].position == ThisTransform.position)
                        {
                            page = 1;
                            ThisBody.velocity = new Vector2(0, 0);
                        }
                        else
                            ThisBody.velocity = (Path_UpTogether[1].position - ThisTransform.position) * 5f;
                    }
                }
                break;

            /* LastPang 모드 일 때 */
            case B_UIManager.enemyMode.LastPang:
                if (!switchC)
                {
                    switchC = true;
                    // 플레이어를 향해 말탄환 생성 및 발사
                    float gap = 0.3f;
                    for (int k = 0; k < 7; k++)
                    {
                        int index = LastBullets2_index[k];
                        switch (k)
                        {
                            case 0:
                                targetDir = targetTransform.position + new Vector3(0, gap*3, 0);
                                break;
                            case 1:
                                targetDir = targetTransform.position + new Vector3(-gap*3, 0, 0);
                                break;
                            case 2:
                                targetDir = targetTransform.position + new Vector3(-gap*2, gap, 0);
                                break;
                            case 3:
                                targetDir = targetTransform.position - new Vector3(-gap, gap*2, 0);
                                break;
                            case 4:
                                targetDir = targetTransform.position - new Vector3(gap, gap*2, 0);
                                break;
                            case 5:
                                targetDir = targetTransform.position + new Vector3(gap*2, gap, 0);
                                break;
                            case 6:
                                targetDir = targetTransform.position + new Vector3(gap*3, 0, 0);
                                break;
                            }
                        Dir = targetDir - (Vector2)LastBarrel2[k].position;
                        digree = Mathf.Atan2(LastBarrel2[k].position.y - targetDir.y, LastBarrel2[k].position.x - targetDir.x) * 180f / Mathf.PI;
                        LastBarrel2[k].Rotate(0, 0, digree);
                        LastBullet1[index].gameObject.SetActive(true);
                        LastBullet1[index].position = LastBarrel2[k].position;
                        LastBullet1[index].rotation = LastBarrel2[k].rotation;
                        LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                        LastBarrel2[k].Rotate(0, 0, -digree);
                    }
                    UIM.HealEnemy();
                }
                if (!switchB)
                {
                    if (ThisTransform.position == UpMiddle)
                    {
                        switchB = true;
                        ThisBody.velocity = new Vector2(0, 0);
                        StartCoroutine(ModeMotion());
                        StartCoroutine(LastAttack());
                    }
                    else
                        ThisBody.velocity = (UpMiddle - ThisTransform.position) * 5f;
                }
                break;
        }

        // target(플레이어)가 있는 방향으로 고개를 돌림
        if (targetTransform.position.x - ThisTransform.position.x > 0)
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

    // 모드 바뀔 때 모션
    IEnumerator ModeMotion()
    {
        headAnimator.ResetTrigger("hit");
        headAnimator.ResetTrigger("attack");
        bodyAnimator.ResetTrigger("hit");
        wingsAnimator.ResetTrigger("hit");
        wingsAnimator.ResetTrigger("attack");
        if (mode == B_UIManager.enemyMode.UpTogether)
            yield return new WaitForSecondsRealtime(1.5f);
        else
        {
            wingL.sprite = VwingL;
            wingR.sprite = VwingR;
        }
        bodyAnimator.SetTrigger("mode");
        yield return new WaitForSecondsRealtime(0.1f);
        wingsAnimator.ResetTrigger("hit");
        wingsAnimator.ResetTrigger("attack");
        wingsAnimator.SetTrigger("mode");
        if (mode == B_UIManager.enemyMode.LastPang)
            Ihead.SetActive(true);
        yield return new WaitForSecondsRealtime(0.05f);
        ThisAudio.clip = modeChange;
        ThisAudio.Play();
    }

    // LastPang 모드 공격 패턴 (이 모드에선 적은 제자리에서 공격을 반복함.)
    IEnumerator LastAttack()
    {
        yield return new WaitForSecondsRealtime(2f);
        // 참고 - 준비된 말탄환 28개
        int index = 0;
        while (Health > 0)
        {
            /* 패턴 1 => 말탄환 좌우로 마구마구 공격*/
            for (int count = 0; count <= 7; count++)
            {
                // 5방향으로 말탄환을 날린다. (0, 2, 4, 6, 8)
                for (int k = 0; k < 9; k += 2)
                {
                    index = k + (count%3) * 9;
                    targetDir = LastBarrel1[k].position;
                    Dir = targetDir - (Vector2)ThisTransform.position;
                    LastBullet1[index].gameObject.SetActive(true);
                    LastBullet1[index].position = LastBarrel1[k].position;
                    LastBullet1[index].rotation = LastBarrel1[k].rotation;
                    LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                }
                yield return new WaitForSecondsRealtime(0.25f);
                if (count % 4 == 0 || count % 4 == 3)
                    LastBarrelCenter.Rotate(new Vector3(0, 0, 3f));
                else
                    LastBarrelCenter.Rotate(new Vector3(0, 0, -3f));
                // 4방향으로 말탄환을 날린다. (1, 3, 5, 7)
                for (int k = 1; k < 9; k += 2)
                {
                    index = k + (count%3)* 9;
                    targetDir = LastBarrel1[k].position;
                    Dir = targetDir - (Vector2)ThisTransform.position;
                    LastBullet1[index].gameObject.SetActive(true);
                    LastBullet1[index].position = LastBarrel1[k].position;
                    LastBullet1[index].rotation = LastBarrel1[k].rotation;
                    LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                }
                yield return new WaitForSecondsRealtime(0.25f);
                // 공격 범위 조정을 위해 LastBarrel 좌우로 회전
                if (count % 4 == 0 || count % 4 == 3)
                    LastBarrelCenter.Rotate(new Vector3(0, 0, 3f));
                else
                    LastBarrelCenter.Rotate(new Vector3(0, 0, -3f));
            }
            // 공격 딜레이
            yield return new WaitForSecondsRealtime(2f);

            /* 패턴 2 => LeftFloor[5] 중 한 곳 + RightFloor[5] 중 한 곳, 총 두 곳을 향해 말탄환을 동시에 3개씩 발사하고 플레이어를 향해 한 발 발사한다 */
            FloorShuffle();
            for (int i = 0; i < 5; i++)
            {
                // 타겟 Floor 표시
                markX_L.gameObject.SetActive(true);
                markX_R.gameObject.SetActive(true);
                markX_L.position = LeftFloor[i].position;
                markX_R.position = RightFloor[i].position;
                // 말탄환 애니메이션
                for(int k = 0; k < 7; k++)
                {
                    bulletObject[k].SetActive(true);
                    bulletAnimator[k].SetTrigger("Ready");
                    yield return new WaitForSecondsRealtime(0.1f);
                }
                yield return new WaitForSecondsRealtime(0.3f);
                markX_L.gameObject.SetActive(false);
                markX_R.gameObject.SetActive(false);
                // 애니메이션 끝나면 말탄환 생성 및 발사 (오른쪽+왼쪽)
                for (int barrel_index = 1; barrel_index <= 3; barrel_index++)
                {
                    // 왼쪽
                    index = LastBullets2_index[barrel_index];
                    targetDir = LeftFloor[i].position;
                    Dir = targetDir - (Vector2) LastBarrel2[barrel_index].position;
                    digree = Mathf.Atan2(LastBarrel2[barrel_index].position.y - targetDir.y, LastBarrel2[barrel_index].position.x - targetDir.x) * 180f / Mathf.PI;
                    LastBarrel2[barrel_index].Rotate(0, 0, digree);
                    bulletObject[barrel_index].SetActive(false);
                    LastBullet1[index].gameObject.SetActive(true);
                    LastBullet1[index].position = LastBarrel2[barrel_index].position;
                    LastBullet1[index].rotation = LastBarrel2[barrel_index].rotation;
                    LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                    LastBarrel2[barrel_index].Rotate(0, 0, -digree);
                    // 오른쪽
                    index = LastBullets2_index[barrel_index+3];
                    targetDir = RightFloor[i].position;
                    Dir = targetDir - (Vector2)LastBarrel2[barrel_index+3].position;
                    digree = Mathf.Atan2(LastBarrel2[barrel_index+3].position.y - targetDir.y, LastBarrel2[barrel_index+3].position.x - targetDir.x) * 180f / Mathf.PI;
                    LastBarrel2[barrel_index+3].Rotate(0, 0, digree);
                    bulletObject[barrel_index+3].SetActive(false);
                    LastBullet1[index].gameObject.SetActive(true);
                    LastBullet1[index].position = LastBarrel2[barrel_index+3].position;
                    LastBullet1[index].rotation = LastBarrel2[barrel_index+3].rotation;
                    LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                    LastBarrel2[barrel_index+3].Rotate(0, 0, -digree);
                    yield return new WaitForSecondsRealtime(0.1f);
                }
                // 플레이어를 향해 말탄환 생성 및 발사
                index = LastBullets2_index[0];
                targetDir = targetTransform.position;
                Dir = targetDir - (Vector2)LastBarrel2[0].position;
                digree = Mathf.Atan2(LastBarrel2[0].position.y - targetDir.y, LastBarrel2[0].position.x - targetDir.x) * 180f / Mathf.PI;
                LastBarrel2[0].Rotate(0, 0, digree);
                bulletObject[0].SetActive(false);
                LastBullet1[index].gameObject.SetActive(true);
                LastBullet1[index].position = LastBarrel2[0].position;
                LastBullet1[index].rotation = LastBarrel2[0].rotation;
                LastBullets1[index].MoveBullet(Dir, bulletSpeed, 0);
                LastBarrel2[0].Rotate(0, 0, -digree);
                // 공격 딜레이
                yield return new WaitForSecondsRealtime(2f);
            }
            // 공격 딜레이
            yield return new WaitForSecondsRealtime(3f);
        }
    }

    // LeftFloor[5]와 RightFloor[5]를 랜덤 정렬한다.
    public void FloorShuffle()
    {
        for(int i = 0; i < LeftFloor.Length; i++)
        {
            int random = Random.Range(0, LeftFloor.Length);
            tempFloor = LeftFloor[random];
            LeftFloor[random] = LeftFloor[i];
            LeftFloor[i] = tempFloor;
        }
        for (int i = 0; i < RightFloor.Length; i++)
        {
            int random = Random.Range(0, RightFloor.Length);
            tempFloor = RightFloor[random];
            RightFloor[random] = RightFloor[i];
            RightFloor[i] = tempFloor;
        }
    }

    // enemy2 공격 함수1
    IEnumerator Enemy2_Attack1()
    {
        flag = false;
        headAnimator.SetTrigger("attack");
        wingsAnimator.SetTrigger("attack");
        // 상하좌우+대각선4방향으로 말탄환을 날린다.
        for (int k = 0; k < 8; k++)
        {
            targetDir = barrel[k].position;
            Dir = targetDir - (Vector2) ThisTransform.position;
            bullet[k].gameObject.SetActive(true);
            bullet[k].position = barrel[k].position;
            bullet[k].rotation = barrel[k].rotation;
            Bullets[k].MoveBullet(Dir, bulletSpeed, 0);
            yield return new WaitForSecondsRealtime(0.1f);
        }
        Invoke("SetFlag", 0.8f);
        Invoke("ActivateBullets", bulletTimeOut[Random.Range(0, bulletTimeOut.Length)]);
    }

    // enemy2 공격 함수2
    IEnumerator Enemy2_Attack2()
    {
        int index = 0;
        Quaternion[] R = {barrel[2].rotation, barrel[3].rotation, barrel[4].rotation, barrel[5].rotation, barrel[6].rotation};
        // 좌우하+대각선2방향으로 말탄환을 날린다.
        for (int i = 0; i < 5; i++) {
            headAnimator.SetTrigger("attack");
            wingsAnimator.SetTrigger("attack");
            for (int k = 2; k <= 6; k++)
            {
                barrel[k].rotation = R[k-2];
                targetDir = barrel[k].position;
                Dir = targetDir - (Vector2) ThisTransform.position;
                bullet[index].gameObject.SetActive(true);
                bullet[index].position = barrel[k].position;
                bullet[index].rotation = barrel[k].rotation;
                Bullets[index].MoveBullet(Dir, bulletSpeed, 0);
                yield return new WaitForSecondsRealtime(0.2f);
                index += 1;
                index %= 15;
            }
            yield return new WaitForSecondsRealtime(1f);
            if (i % 2 == 0)
                BarrelCenter.Rotate(new Vector3(0, 0, 22.5f));
            else
                BarrelCenter.Rotate(new Vector3(0, 0, -22.5f));
            headAnimator.SetTrigger("attack");
            wingsAnimator.SetTrigger("attack");
            for (int k = 6; k >= 2; k--)
            {
                targetDir = barrel[k].position;
                Dir = targetDir - (Vector2) ThisTransform.position;
                bullet[index].gameObject.SetActive(true);
                bullet[index].position = barrel[k].position;
                bullet[index].rotation = barrel[k].rotation;
                Bullets[index].MoveBullet(Dir, bulletSpeed, 0);
                yield return new WaitForSecondsRealtime(0.2f);
                index += 1;
                index %= 15;
            }
            yield return new WaitForSecondsRealtime(1f);
            if (i % 2 == 1)
                BarrelCenter.Rotate(new Vector3(0, 0, 22.5f));
            else
                BarrelCenter.Rotate(new Vector3(0, 0, -22.5f));
        }
        yield return new WaitForSecondsRealtime(1f);
        Invoke("ActivateBullets", 0.5f);
        count = 0;
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
        Invoke("SetFlag", 0.8f);
    }

    private void SetFlag()
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
        Dir = targetDir - (Vector2) ThisTransform.position;
        if (Facing == FaceDirection.FaceLeft)
            digree = Mathf.Atan2(barrel[0].position.y - targetDir.y, barrel[0].position.x - targetDir.x) * 180f / Mathf.PI;
        else
            digree = - Mathf.Atan2(barrel[0].position.y - targetDir.y, barrel[0].position.x - targetDir.x) * 180f / Mathf.PI + 180;
        barrel[0].Rotate(0, 0, digree);
        int k = Random.Range(0, bullet.Length);
        bullet[k].gameObject.SetActive(true);
        bullet[k].position = barrel[0].position;
        bullet[k].rotation = barrel[0].rotation;
        Bullets[k].MoveBullet(Dir, bulletSpeed, 0);
        // 공격 딜레이
        if (mode == B_UIManager.enemyMode.normal)
        {
            Bullets[k].Invoke("Delete", destroyTime);
            Invoke("ActivateBullet", bulletTimeOut[0]);
        }
        else if (mode == B_UIManager.enemyMode.UpTogether)
        {
            Bullets[k].Invoke("Delete", 1.5f);
            Invoke("ActivateBullet", 3f);
        }
    }

    // enemy1 공격 딜레이. 제한 시간이 지나야 CanAttack 변수를 활성화한다.
    private void ActivateBullet()
    {
        enemy1_CanAttack = true;
        barrel[0].Rotate(0, 0, -digree);
    }

    // 공격 당했을 시
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("waterbullet") && Health > 0)
        {
            Health--;
            headAnimator.SetTrigger("hit");
            wingsAnimator.SetTrigger("hit");
            bodyAnimator.SetTrigger("hit");
            flag = false;
            if (ThisName.Equals("enemy1"))
                UIM.HitEnemy1();
            else if(ThisName.Equals("enemy2"))
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
        {
            ThisAudio.clip = enemyHit;
            ThisAudio.Play();
            flag = true;
        }
        else
        {
            Clear();
        }
    }

    // 적이 클리어된 경우
    public void Clear()
    {
        if (!clear)
        {
            Vector2 ThisPosition = ThisTransform.position;
            ClearEnemy.GetComponent<Transform>().position = new Vector2(ThisPosition.x, ThisPosition.y - 1.2f);
            gameObject.SetActive(false);
            UIM.ClearSound();
            ClearEnemy.SetActive(true);
            if (ThisName.Equals("enemy1"))
            {
                UIM.flag1 = false;
                item1.SetActive(true);
                item1.GetComponent<B_DestroyInTime>().Invoke("Delete", 5f);
            }
            else if (ThisName.Equals("enemy2"))
            {
                UIM.flag2 = false;
                item2.SetActive(true);
                item2.GetComponent<B_DestroyInTime>().Invoke("Delete", 5f);
            }

            // 적 둘 다 clear 됐을 시에는 아이템이 나오지 않는다.
            if (!UIM.flag1 && !UIM.flag2)
            {
                item1.SetActive(false);
                item2.SetActive(false);
            }
        }
        clear = true;
    }

}
