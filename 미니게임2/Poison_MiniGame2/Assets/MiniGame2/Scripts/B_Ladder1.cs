using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Ladder1 : MonoBehaviour {
    
    public BoxCollider2D Floor = null;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "PlayerCenter")
        {
            Floor.isTrigger = false;
        }

    }

}
