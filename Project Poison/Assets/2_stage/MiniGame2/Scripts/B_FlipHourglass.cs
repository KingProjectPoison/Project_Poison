using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_FlipHourglass : MonoBehaviour {

    public bool doFlip = false;
    public Slider hourglassC;
    private Transform ThisTransform, transformC;

    private void Awake()
    {
        ThisTransform = GetComponent<Transform>();
        transformC = hourglassC.GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update () {
        // 아이템 먹을 시, 모래시계 뒤집기
        if (doFlip)
        {
            doFlip = false;
            StartCoroutine("FlipHourglass");
        }
    }

    IEnumerator FlipHourglass()
    {
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
        float initY = ThisTransform.localScale.y;
        if (initY > 0)
        {
            for (int i = 1; i <= 28; i++)
            {
                ThisTransform.localScale = new Vector3(0.7f, 0.7f - i*0.05f, 1);
                yield return new WaitForSecondsRealtime(0.015f);
            }
        }
        else
        {
            for (int i = 1; i <= 28; i++)
            {
                ThisTransform.localScale = new Vector3(0.7f, -0.7f + i * 0.05f, 1);
                yield return new WaitForSecondsRealtime(0.015f);
            }
        }
    }

}
