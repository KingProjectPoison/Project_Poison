using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderCollider : MonoBehaviour {

    private BoxCollider2D boxCollider;

    float vertical;
        
	// Use this for initialization
	void Start () {
        boxCollider = GetComponent<BoxCollider2D>();
        
	}
	
	// Update is called once per frame
	void Update () {
        vertical = GameObject.Find("player").GetComponent<CharacterMovement>().vertical;
    }

    //플레이어의 발이 땅 collider에서 나가면서 땅 trigger off
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "GroundCheck" && vertical == 1)
        {
            this.boxCollider.isTrigger = false;
        }
    }

}
