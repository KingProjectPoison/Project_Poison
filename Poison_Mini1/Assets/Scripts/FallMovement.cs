using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallMovement : MonoBehaviour {

    BoxCollider2D ground2;
    BoxCollider2D ground3;
    public Transform player;

    // Use this for initialization
    void Start()
    {
        ground2 = GameObject.Find("2ndfloor").GetComponent<BoxCollider2D>();
        ground3 = GameObject.Find("3rdfloor").GetComponent<BoxCollider2D>();
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Vector3 playerCoor = player.position;
        if (other.tag == "Player")
        {
            if (playerCoor.y < 2)
                ground2.isTrigger = true;
            else if (playerCoor.y < 4.1)
                ground3.isTrigger = true;
        }
    }
}
