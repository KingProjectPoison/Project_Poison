using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HintManager : MonoBehaviour {

    GameObject[] lightHints = new GameObject[4];
    //Vector3[] lightPos = new Vector3[4];

    //light position based on player
    public Transform player;


    // Use this for initialization
    void Start () {
        for (int i = 0; i < 4; i++)
              {
                  lightHints[i] = GameObject.Find("Mask_" + (i+1));
                  //lightPos[i] = lightHints[i].GetComponent<Transform>().position;
              }

        for (int i = 0; i < 4; i++)
        {
            lightHints[i].SetActive(false);
        }

        StartCoroutine("MyUpdate");
    }

    IEnumerator LightStart()
    {
        yield return new WaitForSeconds(15);
        MaskOff();
        yield return new WaitForSeconds(7);
        MaskOn();
    }

    IEnumerator MyUpdate()
    {
        float timer = 0f;
        float time = 5f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        MaskOn();
    }

    void MaskOn()
    {
        for (int i = 0; i < 4; i++)
        {
            lightHints[i].gameObject.SetActive(true);
        }
        StartCoroutine("LightStart");
    }

    void MaskOff()
    {
        for (int i = 0; i < 4; i++)
        {
            lightHints[i].gameObject.SetActive(false);
        }
    }

    ////setting light based on player
    //void SetPos()
    //{
    //    Vector3 playerCoor = player.position;
    //    float speed = lightHints[1].
    //    if (playerCoor.y < 2)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            lightPos[i].x = -6.5f;
                
    //        }
    //    }
    //    else if (playerCoor.y < 4.1)
    //    {
    //        for (int i = 0; i < 4; i++)
    //        {
    //            lightPos[i].x = 6.5f;
    //        }
    //    }
    //}
}
