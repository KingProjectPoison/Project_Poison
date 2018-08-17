using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class C_ScoreUpdate : MonoBehaviour {
    public int score = 0;
    public Text scoreLabel;
    public Image ScoreBar ;
    public int accumulate = 0;
    public static C_ScoreUpdate instance = null;
    public bool isItem = false;

    //모래시계
    public Slider hourglassA, hourglassB, hourglassC;
    public int playerMaxHP = 600;
    int playerHP;
    public Animator BreakHourglass;
    public bool moveSand = false, breakHourglass = false;
    float diffHP;
    private Transform transformC;

    // UIScreen
    public GameObject TimeStop_Screen, GameOver_Screen;


    void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        playerHP = playerMaxHP;
        hourglassA.value = playerHP;
        hourglassB.value = playerMaxHP - playerHP;
        hourglassC.value = 0;
        transformC = hourglassC.GetComponent<Transform>();

    }

    public void ScoreReset()
    {
        accumulate = 0;
        ScoreBar.fillAmount = 0;
    }
    public void ScoreSet()
    {
        accumulate += 1;
        if (!isItem)
        {
            ScoreBar.fillAmount += 1 / 60f;
        }
        score += 50;
        scoreLabel.text = score.ToString();
    }
    void Update()
    {
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
        else if (!breakHourglass)
        {
            breakHourglass = true;
            BreakHourglass.SetTrigger("break");
            Invoke("BreakSound", 1.5f);
            Invoke("GameOver", 3f);
        }
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

    // 게임 일시 정지
    public void TimeStop()
    {
        if (!TimeStop_Screen.activeSelf)
        {
            TimeStop_Screen.SetActive(true);
            Time.timeScale = 0;
        }
    }

    // 일시 정지 취소
    public void TimeGo()
    {
        TimeStop_Screen.SetActive(false);
        GameOver_Screen.SetActive(false);
        Time.timeScale = 1;
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
