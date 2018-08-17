using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class B_UIManager : MonoBehaviour {

    private Transform ThisTransform;
    public Camera mainCame;
    public Transform playerTarget, enemy1Target, enemy2Target, clear1Target, clear2Target;
    public GameObject door1, door2;
    public RectTransform attackImage, enemy1_bar, enemy2_bar;
    public Text attackLimitText1, attackLimitText2;
    public Text shieldLimitText1, shieldLimitText2;
    private Scrollbar enemy1_scroll, enemy2_scroll;
    public Slider hourglassA, hourglassB, hourglassC;
    private Transform transformC;
    public Animator BreakHourglass;
    public int shieldLimit = 5;
    public int playerMaxHP = 600;
    int playerHP;
    public bool moveSand = false, enemy2_page2 = false;
    // 적 공격 패턴
    public enum enemyMode { normal = -1, UpTogether = 0, LastPang = 1, End = 2};
    public enemyMode mode = enemyMode.normal;
    // 적이 클리어되면 false가 된다.
    public bool flag1 = true, flag2 = true, doflag = false, isItem = false, breakHourglass = false;
    [SerializeField]
    public int enemyMaxHP = 5;
    int enemy1HP, enemy2HP;
    float diffHP;
    float PositionZ;
    bool switchA = false, switchB = false;
    // 애니메이션을 위한
    public Image attackButton;
    public Sprite[] attackDelay;
    public Image shieldButton;
    public Sprite[] shieldDelay;
    public Transform ground;
    private B_FloorReset groundScript;
    // 효과음
    public AudioClip breakGlass, activeDoor, enemyClear, floorOn, modeWait, buttonClick, healthUP;
    private AudioSource ThisAudio;
    // 클리어 후에 없어져야 하는 것
    public GameObject[] RemoveItem;
    public GameObject RemoveCanvas;
    // UIScreen
    public GameObject TimeStop_Screen, GameOver_Screen, Result_Screen, Good, Bad;
    // static
    public static B_UIManager instance = null;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        ThisAudio = GetComponent<AudioSource>();
        ThisTransform = GetComponent<Transform>();
        groundScript = ground.GetComponent<B_FloorReset>();
        transformC = hourglassC.GetComponent<Transform>();
        enemy1_scroll = enemy1_bar.GetComponent<Scrollbar>();
        enemy2_scroll = enemy2_bar.GetComponent<Scrollbar>();
        playerHP = playerMaxHP;
        enemy1HP = enemyMaxHP;
        enemy2HP = enemyMaxHP;
        door1.SetActive(false);
        door2.SetActive(false);
        hourglassA.value = playerHP;
        hourglassB.value = playerMaxHP - playerHP;
        hourglassC.value = 0;
        PositionZ = ThisTransform.position.z;
    }

    // Update is called once per frame
    void Update () {
        if (flag1)
            enemy1_bar.position = mainCame.WorldToScreenPoint(new Vector3(enemy1Target.position.x, enemy1Target.position.y, PositionZ));
        else
        {
            enemy1_bar.position = mainCame.WorldToScreenPoint(new Vector3(clear1Target.position.x - 0.35f, clear1Target.position.y + 0.85f, PositionZ));
        }
        if (flag2)
            enemy2_bar.position = mainCame.WorldToScreenPoint(new Vector3(enemy2Target.position.x, enemy2Target.position.y, PositionZ));
        else
        {
            enemy2_bar.position = mainCame.WorldToScreenPoint(new Vector3(clear2Target.position.x - 0.25f, clear2Target.position.y + 0.7f, PositionZ));
        }
        
        // 모래시계 UI 관련
        if (playerHP > 0)
        {
            if (moveSand)
            {
                moveSand = false;
                StartCoroutine("MoveSand");
            }
            // 플레이어 체력 양과 모래시계 양이 차이가 나면 diffHP씩 맞춰간다.
            if (hourglassA.value > playerHP && !isItem)
            {
                diffHP = (hourglassA.value - playerHP) * 0.1f;
                hourglassA.value -= diffHP;
                hourglassB.value += diffHP;
            }
            else if (hourglassA.value < playerHP && isItem)
            {
                diffHP = (playerHP - hourglassA.value) * 0.1f;
                hourglassA.value += diffHP;
                hourglassB.value -= diffHP;
            }
        }
        // 플레이어 체력이 0이 되면 모래시계 깨지는 효과가 나오게 한다. (1회 실행)
        else if(!breakHourglass)
        {
            breakHourglass = true;
            BreakHourglass.SetTrigger("break");
            Invoke("BreakSound", 1.5f);
            Invoke("GameOver", 3f);
        }

        // 적 하나의 체력이 절반이하로 떨어지면 mode를 UpTogether로 바꾼다.
        if ((enemy2HP <= enemyMaxHP * 0.5f || enemy1HP <= enemyMaxHP * 0.5f) && !switchA)
        {
            switchA = true;
            Invoke("ChangeMode", 1.5f);
        }

        // 적 하나가 clear 상태가 될 시 mode를 LastPang으로 바꾼다.
        if ((!flag1 || !flag2) && !switchB)
        {
            switchB = true;
            Invoke("LastMode", 1.5f);
        }

        // 적이 모두 clear되면 문을 활성화 시킨다. (1회 실행)
        if (!flag1 && !flag2 && !doflag)
        {
            doflag = true;
            for(int i = 0; i < RemoveItem.Length; i++)
            {
                RemoveItem[i].SetActive(false);
            }
            Invoke("DoorActivate", 1f);
        }

    }

    private void DoorActivate()
    {
        ThisAudio.clip = activeDoor;
        ThisAudio.Play();
        door1.SetActive(true);
        door2.SetActive(true);
        mode = enemyMode.End;
        Vector3 pos = ground.position;
        pos.y -= 11f;
        ground.position = pos;
        for (int i = 0; i < RemoveItem.Length; i++)
        {
            RemoveItem[i].SetActive(false);
        }
    }

    private void ChangeMode()
    {
        ThisAudio.clip = modeWait;
        ThisAudio.Play();
        mode = enemyMode.UpTogether;
    }

    private void LastMode()
    {
        ThisAudio.clip = modeWait;
        ThisAudio.Play();
        mode = enemyMode.LastPang;
        groundScript.activeScript = false;
        groundScript.CreateFloors();
        Vector3 pos = ground.position;
        pos.y += 11f;
        ground.position = pos;
    }

    private void BreakSound()
    {
        ThisAudio.clip = breakGlass;
        ThisAudio.Play();
    }

    public void ClearSound()
    {
        ThisAudio.clip = enemyClear;
        ThisAudio.Play();
    }

    //모래 쏟아져 내리는 효과 관련 (hourglassC가 모래 막대 부분임)
    IEnumerator MoveSand()
    {
        for (int i = 1; i <= 10; i++)
        {
            hourglassC.value += 0.1f;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        Vector3 tempV = transformC.localScale;
        tempV.y *= -1;
        if (hourglassC.direction == Slider.Direction.TopToBottom)
        {
            hourglassC.SetDirection(Slider.Direction.BottomToTop, true);
            transformC.localScale = tempV;
        }
        else
        {
            hourglassC.SetDirection(Slider.Direction.TopToBottom, true);
            transformC.localScale = tempV;
        }
        for (int i = 1; i <= 10; i++)
        {
            hourglassC.value -= 0.1f;
            yield return new WaitForSecondsRealtime(0.02f);
        }
        tempV.y *= -1;
        if (hourglassC.direction == Slider.Direction.TopToBottom)
        {
            hourglassC.SetDirection(Slider.Direction.BottomToTop, true);
            transformC.localScale = tempV;
        }
        else
        {
            hourglassC.SetDirection(Slider.Direction.TopToBottom, true);
            transformC.localScale = tempV;
        }
    }

    // 플레이어 체력 관련
    public void HitPlayer(int power)
    {
        moveSand = true;
        playerHP -= power;
    }

    public void HealPlayer(int power)
    {
        if (playerHP <= playerMaxHP)
        {
            moveSand = true;
            playerHP += power;
        }

        if (playerHP > playerMaxHP)
            playerHP = playerMaxHP;
    }

    // 적 체력 관련
    public void HitEnemy1()
    {
        enemy1HP--;
        if(enemy1HP > 0)
            enemy1_scroll.size = (float) enemy1HP / enemyMaxHP;
        else
            enemy1_scroll.size = 0;
    }

    public void HitEnemy2()
    {
        enemy2HP--;
        if(enemy2HP > 0)
            enemy2_scroll.size = (float) enemy2HP / enemyMaxHP;
        else
            enemy2_scroll.size = 0;
    }

    // LastPang 모드 진입 시 enemy 체력 +1
    public void HealEnemy()
    {
        B_SoundManager.instance.PlaySingle(healthUP);
        // enemy1이 살아남은 경우
        if (enemy1HP > 0 && enemy1HP < enemyMaxHP)
        {
            enemy1HP += 2;
            enemy1_scroll.size = (float) enemy1HP / enemyMaxHP;
        }
        else if (enemy2HP > 0 && enemy2HP < enemyMaxHP)
        {
            enemy2HP += 2;
            enemy2_scroll.size = (float) enemy2HP / enemyMaxHP;
        }
    }

    // 공격 딜레이 애니메이션 관련
    public void Attack()
    {
        StartCoroutine("AttackAnimation");
    }

    IEnumerator AttackAnimation()
    {
        int init = attackDelay.Length - 1;
        for (int i = init - 1; i > 0; i--)
        {
            attackButton.sprite = attackDelay[i];
            yield return new WaitForSecondsRealtime(0.9f);
        }
        attackButton.sprite = attackDelay[0];
        yield return new WaitForSecondsRealtime(0.25f);
        attackButton.sprite = attackDelay[init];
    }

    private void AttackLimitOver()
    {
        if (flag1 || flag2)
            GameOver();
    }

    // 방어 개수 제한 + 방어 딜레이 애니메이션 관련
    public void Shield()
    {
        shieldLimit--;
        shieldLimitText1.text = "" + shieldLimit;
        shieldLimitText2.text = "" + shieldLimit;
        StartCoroutine("ShieldAnimation");
    }

    IEnumerator ShieldAnimation()
    {
        int init = shieldDelay.Length - 1;
        for (int i = init - 1; i > 0; i--)
        {
            shieldButton.sprite = shieldDelay[i];
            yield return new WaitForSecondsRealtime(0.9f);
        }
        shieldButton.sprite = shieldDelay[0];
        yield return new WaitForSecondsRealtime(0.25f);
        shieldButton.sprite = shieldDelay[init];
    }

    // 발판 생성 효과음
    public void FloorSound()
    {
        if (!(ThisAudio.clip == modeWait && ThisAudio.isPlaying))
        {
            ThisAudio.clip = floorOn;
            ThisAudio.Play();
        }
    }

    // 게임 오버 창 뜨게 하기
    public void GameOver()
    {
        RemoveCanvas.SetActive(false);
        mode = enemyMode.End;
        Time.timeScale = 0;
        GameOver_Screen.SetActive(true);
    }

    // 게임 결과 창 뜨게 하기
    public void Result()
    {
        RemoveCanvas.SetActive(false);
        mode = enemyMode.End;
        Time.timeScale = 0;
        Result_Screen.SetActive(true);
        if(playerHP >= 250)
        {
            Good.SetActive(true);
            Bad.SetActive(false);
        }
        else
        {
            Good.SetActive(false);
            Bad.SetActive(true);
        }
    }
    
    // 게임 일시 정지
    public void TimeStop()
    {
        RemoveCanvas.SetActive(false);
        ThisAudio.clip = buttonClick;
        ThisAudio.Play();
        if (!TimeStop_Screen.activeSelf)
        {
            TimeStop_Screen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    public void HealthUP_Sound()
    {
        ThisAudio.clip = healthUP;
        ThisAudio.Play();
    }

    // 일시 정지 취소
    public void TimeGo()
    {
        ThisAudio.clip = buttonClick;
        ThisAudio.Play();
        TimeStop_Screen.SetActive(false);
        Result_Screen.SetActive(false);
        GameOver_Screen.SetActive(false);
        Time.timeScale = 1;
        RemoveCanvas.SetActive(true);
    }

    // 메인화면으로 가기 (지금은 임시로 재시작 기능으로 구현)
    public void Restart()
    {
        TimeGo();
        SceneManager.LoadScene("MiniGame2");
    }

    // 결과 화면에서 메인화면으로 가기 (지금은 임시로 게임종료 기능으로 구현)
    public void Quit()
    {
        TimeGo();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit(); // 종료
#endif
    }

}
