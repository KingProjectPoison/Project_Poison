using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_FloorReset : MonoBehaviour {

    public GameObject[] floors;
    public B_Floor initialFloor;
    public GameObject door_active;
    public bool activeScript = true;
    private bool switchA = false;

    private void Awake()
    {
        for (int i = floors.Length - 1; i >= 0; i--)
        {
            floors[i].SetActive(false);
        }
    }

    public void CreateFloors()
    {
        for (int i = 0; i < floors.Length; i++)
        {
            floors[i].SetActive(true);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (!switchA && activeScript)
        {
            if (collision.CompareTag("Player"))
            {
                if (collision.GetComponent<Rigidbody2D>().velocity.y == 0)
                {
                    switchA = true;
                    StartCoroutine("ResetFloor");
                    initialFloor.flag = false;
                }
            }
        }

        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }

    }

    IEnumerator ResetFloor()
    {
        yield return new WaitForSecondsRealtime(1f);
        for (int i = floors.Length - 1; i >= 0; i--)
        {
            if (initialFloor.flag)
                break;
            if (floors[i].activeSelf)
            {
                floors[i].SetActive(false);
                yield return new WaitForSecondsRealtime(0.6f);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            switchA = false;
            collision.GetComponent<CapsuleCollider2D>().isTrigger = false;
        }
    }

}
