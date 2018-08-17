using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_EnemyMove : MonoBehaviour {

    public enum FaceDirection { FaceRight = -1, FaceLeft = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;

    // 애니메이션을 위한
    public Animator faceAnimator;
    public Animator handsAnimator;
    public Animator bodyAnimator;

    public bool isDrag = false;

    private Transform ThisTransform = null;
    private Rigidbody2D ThisBody = null;

    [SerializeField]
    Transform[] PathCenter;
    int[] index1 = { 2, 3, 7, 6, 10, 9, 8, 4, 0, 1, 2, 6, 5, 4, 8, 9, 10, 6, 2, 1, 0, 4, 5, 6, 10, 11, 7, 3, 2, 1, 0, 4, 8, 9, 10, 6, 7, 3, 2, 1, 0, 4, 5, 1, 0, 4, 5, 1, 2, 3, 7, 11, 10, 9, 8, 4, 0, 1, 2, 3, 7, 11, 10, 6, 2, 3, 7, 11, 10, 9, 8, 4, 0, 1, 2, 3, 7, 11, 10, 6, 2, 3, 7, 11, 10, 6, 2, 3, 7, 11, 10, 9, 8 };
    int[] index2 = { 9, 8, 4, 5, 1, 2, 3, 7, 6, 5, 9, 10, 11, 7, 3, 2, 1, 0, 4, 8, 9, 10, 11, 7, 3, 2, 1, 0, 4, 5, 9, 10, 11, 7, 3, 2, 1, 5, 4, 8, 9, 10, 11, 7, 6, 2, 3, 7, 6, 10, 9, 8, 4, 0, 1, 2, 6, 5, 9, 10, 6, 5, 9, 8, 4, 0, 1, 5, 6, 2, 1, 5, 6, 7, 11, 10, 9, 5, 4, 0, 1, 5, 9, 8, 4, 0, 1, 5, 9, 8, 4, 0, 1, 2 };
    [SerializeField]
    float moveSpeed = 2f;

    public int PathCenterIndex = 0;

    public int i = 0;
    public bool indexpattern = false;

    // Use this for initialization
    void Awake() {
        ThisBody = GetComponent<Rigidbody2D>();
        transform.position = PathCenter[PathCenterIndex].transform.position;
        
        handsAnimator.SetBool("walk", true);
        bodyAnimator.SetBool("walk", true);

        StartCoroutine("Move");


    }

    // Update is called once per frame
    void Update() {
        if (!isDrag)
        {
            Vector3 scale = transform.localScale;

            if (indexpattern)
            {
                if (i != 0 && Mathf.Abs(index1[i - 1] - index1[i]) == 4)
                {

                }
                else if (i != 0 && index1[i - 1] - index1[i] == -1)
                {
                    if (scale.x > 0)
                    {
                        scale.x *= -1;
                    }

                    transform.localScale = scale;
                }

                else
                {
                    if (scale.x < 0)

                        scale.x *= -1;
                    transform.localScale = scale;
                }
            }
            else
            {
                if (i != 0 && Mathf.Abs(index2[i - 1] - index2[i]) == 4)
                {

                }
                else if (i != 0 && index2[i - 1] - index2[i] == -1)
                {
                    if (scale.x > 0)
                    {
                        scale.x *= -1;
                    }

                    transform.localScale = scale;
                }

                else
                {
                    if (scale.x < 0)

                        scale.x *= -1;
                    transform.localScale = scale;
                }
            }

        }

}
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Floor"))
        {
            isDrag = true;
            ThisBody.velocity = Vector3.zero;
        }
    }

    public void InitPosition(int index)
    {
        transform.position = PathCenter[index].transform.position;

        if (transform.position == PathCenter[index].transform.position)
        {
            index += 1;
        }
        if (index == PathCenter.Length)
            index = 0;
    }

    IEnumerator Move()
    {
        while (true)
        {
            if(indexpattern)
             transform.position = PathCenter[index1[i]].position;
            else
                transform.position = PathCenter[index2[i]].position;
            yield return new WaitForSeconds(1f);
            i++;
            if (i == index1.Length)
                i = 0;
        }
    }

    

}

