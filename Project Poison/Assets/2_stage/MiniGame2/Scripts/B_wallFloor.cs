using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_wallFloor : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            collision.GetComponent<Transform>().position = new Vector3(0, 6.8f, 0);
    }

}
