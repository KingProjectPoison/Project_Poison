using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_OnTrigger : MonoBehaviour {

    public B_EnemyMovement target;
    public Transform Camera = null;
    private Animator myAnimator;
    string ThisTag;

    // Use this for initialization
    void Start () {
        myAnimator = GetComponent<Animator>();
        ThisTag = transform.tag;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.tag;

        if (tag.Equals("floor_middle"))
        {
            target.Change_i();
        }

        if (tag.Equals("letterbullet") && ThisTag.Equals("Shield"))
        {
            collision.gameObject.SetActive(false);
            myAnimator.SetTrigger("break");
            Invoke("ActiveOff", 1f);
        }

        if (tag.Equals("Player") && ThisTag.Equals("door0"))
        {
            collision.GetComponent<Transform>().position = new Vector2(-6.404f, -4.6f);
            Camera.position = new Vector3(Camera.position.x, 0f, Camera.position.z);
            collision.GetComponent<B_PlayerControl>().Door0();
        }

        if (tag.Equals("Player") && ThisTag.Equals("door1"))
        {
            print("*** Clear!!! ***");
        }

    }

    private void ActiveOff()
    {
        gameObject.SetActive(false);
    }

}
