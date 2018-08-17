using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_DestroyInTime : MonoBehaviour {
    
    public int power = 30;
    Vector2 Dir;
    float speed;
    string ThisName;
    Rigidbody2D thisbody;

    // Use this for initialization
    void Awake () {
        thisbody = GetComponent<Rigidbody2D>();
        ThisName = transform.tag;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("wall"))
            Delete();
        else if (collision.CompareTag("waterbullet") && ThisName.Equals("letterbullet"))
            Delete();
        else if ((collision.CompareTag("Player")) && ThisName.Equals("item"))
        {
            collision.GetComponent<B_PlayerControl>().ItemOn();
            Delete();
        }
    }

    public void MoveBullet(Vector2 Dir, float speed, float delay)
    {
        this.Dir = Dir;
        this.speed = speed;
        Invoke("Move", delay);
    }

    private void Move()
    {
        thisbody.AddForce(Dir.normalized * speed);
    }

    private void Delete()
    {
        gameObject.SetActive(false);
    }

}
