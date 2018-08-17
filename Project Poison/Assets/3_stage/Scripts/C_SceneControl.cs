using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class C_SceneControl : MonoBehaviour {
    Image TimeBar;
    public float maxTime = 2.4f;
    float timeLeft;
    public GameObject timer;

    //public Text timerText;
    //private float startTime;

	// Use this for initialization
	void Start () {
        //startTime = Time.time;
        timer.SetActive(false);
        TimeBar = GetComponent<Image>();
        timeLeft = maxTime;
	}

    // Update is called once per frame
    void Update()
    {
        //float t = Time.time - startTime;

        //string minutes = ((int)t / 60).ToString();
        //string seconds = (t % 60).ToString("f2");

        //timerText.text = minutes + ":" + seconds;

        if (timeLeft > 0)
        {
            timeLeft -= Time.deltaTime;
            TimeBar.fillAmount = timeLeft / maxTime;
        }
        else
        {
            timer.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
