using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class s_collection : MonoBehaviour {
    public enum Collect { one, two, three, four }
    public Collect collectNum;

    private int[] score = new int[3];
    private bool[] grade = new bool[3];
    private bool[] iscollect = new bool[4];

    public GameObject Collection;
    public GameObject rock;
    public GameObject image;

    void Start()
    {
        rock.SetActive(true);
        image.SetActive(false);
        for (int i = 0; i < 4; i++)
        {
            if (i < 3)
            {
                score[i] = 0;
                grade[i] = false;
            }
            iscollect[i] = false;
        }
    }

    void Update() {
        Score();
        Result();
    }
    public void Score()
    {
        //Stage 결과 score에 저장하기

        score[0] = s_variable.score[0];
        score[1] = s_variable.score[1];
        score[2] = s_variable.score[2];

        if (score[0] > 420) grade[0] = true;
        if (score[0] <= 420) grade[0] = false;
        if (score[1] > 250) grade[1] = true;
        if (score[1] <= 250) grade[1] = false;
        if (score[2] > 150) grade[2] = true;
        if (score[2] <= 150) grade[2] = false;

        if (collectNum == Collect.one)
        {
            if ((grade[0] && grade[1] && grade[2]) || (grade[0] && grade[1] && !grade[2]))
                iscollect[0] = true;
        }
        if(collectNum == Collect.two)
        {
            if ((grade[0] && !grade[1] && grade[2]) || (grade[0] && !grade[1] && !grade[2]))
                iscollect[1] = true;
        }
        if(collectNum == Collect.three)
        {
            if ((!grade[0] && grade[1] && grade[2]) || (!grade[0] && grade[1] && !grade[2]))
                iscollect[2] = true;
        }
        if(collectNum == Collect.four)
        {
            if (!(grade[0] && grade[1] && grade[2]) || !(grade[0] && grade[1] && !grade[2]))
                iscollect[3] = true;
        }

    }
    public void Result()
    {  
        //스테이지 여부 따라iscollect 변수값 설정하기//

        if (collectNum == Collect.one && iscollect[0])
        {
            rock.SetActive(false);
        }
        if (collectNum == Collect.two && iscollect[1])
        {
            rock.SetActive(false);
        }
        if (collectNum == Collect.three && iscollect[2])
        {
            rock.SetActive(false);
        }
        if (collectNum == Collect.four && iscollect[3])
        {
            rock.SetActive(false);
        }

    }

    public void collection()
    {
        if (collectNum == Collect.one && iscollect[0])
        {
            image.SetActive(true);
            Collection.SetActive(false);
        }
        if (collectNum == Collect.two && iscollect[1])
        {
            image.SetActive(true);
            Collection.SetActive(false);
        }
        if (collectNum == Collect.three && iscollect[2])
        {
            image.SetActive(true);
            Collection.SetActive(false);
        }
        if (collectNum == Collect.four && iscollect[3])
        {
            image.SetActive(true);
            Collection.SetActive(false);
        }
    }
}
