using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Ladder1 : MonoBehaviour {
    
    public BoxCollider2D Floor = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D body = collision.GetComponent<Rigidbody2D>();
        if (collision.tag == "Player")
        {
            Floor.isTrigger = false;
        }

    }

}
