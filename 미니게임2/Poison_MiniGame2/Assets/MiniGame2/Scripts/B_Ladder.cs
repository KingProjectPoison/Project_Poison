using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Ladder : MonoBehaviour {
    
    public float speed = 5;
    public BoxCollider2D Floor = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        Rigidbody2D body = collision.GetComponent<Rigidbody2D>();
        if (collision.tag == "Player" && body.velocity.x == 0)
        {
            Floor.isTrigger = true;
            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        }
        else if(collision.tag == "Player" && body.velocity.y < 0)
        {
            collision.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        }

    }

}
