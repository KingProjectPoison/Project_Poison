using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Floor : MonoBehaviour {
    
    public GameObject targetFloor;
    public bool switchA = false;
    public bool flag = false;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (collision.GetComponent<Rigidbody2D>().velocity.y == 0)
            {
                flag = true;
                if (!switchA && !targetFloor.activeSelf)
                {
                    switchA = true;
                    B_UIManager.instance.FloorSound();
                    targetFloor.SetActive(true);
                }
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switchA = false;
        }
    }

}
