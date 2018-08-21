using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class C_ScoreUpdate : MonoBehaviour {
    public int score = 0;
    public Text scoreLabel;
    public Image ScoreBar ;
    public int accumulate = 0;
    public static C_ScoreUpdate instance = null;
    public bool isItem = false;

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
}
