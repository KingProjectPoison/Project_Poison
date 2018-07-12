using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_OnTrigger : MonoBehaviour {

    public B_EnemyMovement target;
    private Animator myAnimator;

	// Use this for initialization
	void Start () {
        myAnimator = GetComponent<Animator>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag.Equals("floor_middle"))
        {
            target.Change_i();
        }

        if (collision.tag.Equals("letterbullet") && transform.tag.Equals("Shield"))
        {
            collision.gameObject.SetActive(false);
            myAnimator.SetTrigger("break");
            Invoke("ActiveOff", 1f);
        }

    }

    private void ActiveOff()
    {
        transform.gameObject.SetActive(false);
    }

}
