using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_EnemyMove : MonoBehaviour {

    public enum FaceDirection { FaceRight = -1, FaceLeft = 1 };
    public FaceDirection Facing = FaceDirection.FaceRight;

    public bool isDrag = false;

    private Transform ThisTransform = null;
    private Rigidbody2D ThisBody = null;

    [SerializeField]
    Transform[] PathCenter;

    [SerializeField]
    float moveSpeed = 2f;
    
    public int PathCenterIndex = 0;
    Animator animator;

    float dirX;

    Vector3 past;
    Vector3 next;

    // Use this for initialization
    void Awake () {
        ThisBody = GetComponent<Rigidbody2D>();
        transform.position = PathCenter [PathCenterIndex].transform.position;
        past = transform.position;
    }

    // Update is called once per frame
    void Update () {
        if (!isDrag)
        {
            Move();
            Vector3 scale = transform.localScale;

            next = transform.position;


            if (next.x - past.x > 0)
            {
                if (scale.x < 0)
                {
                    scale.x *= -1;
                }

                transform.localScale = scale;
            }

            else
            {
                if (scale.x > 0)

                    scale.x *= -1;
                transform.localScale = scale;
            }

            past = next;
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

    void Move()
    {
        
            transform.position = Vector2.MoveTowards(transform.position, PathCenter[PathCenterIndex].transform.position, moveSpeed * Time.deltaTime);

            if (transform.position == PathCenter[PathCenterIndex].transform.position)
            {
                PathCenterIndex += 1;
            }
            if (PathCenterIndex == PathCenter.Length)
                PathCenterIndex = 0;


    }
    

}

