using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Ladder : MonoBehaviour {
    
    public float speed = 5;
    public BoxCollider2D Floor = null;
    public Rigidbody2D target;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag.Equals("PlayerCenter") && Mathf.Abs(target.velocity.x) < 3f)
        {
            Floor.isTrigger = true;
            target.velocity = new Vector2(0, speed);
        }
        else if(collision.tag.Equals("PlayerCenter") && target.velocity.y < 0)
        {
            target.GetComponent<Rigidbody2D>().velocity = new Vector2(0, speed);
        }

    }

}
