using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_DestroyInTime : MonoBehaviour {

    [SerializeField]
    float destroyTime = 1f;
    public int power = 30;
    Vector2 Dir;
    float speed, delay;
    Rigidbody2D thisbody;

    // Use this for initialization
    void Start () {
        Invoke("Delete", destroyTime);
        thisbody = GetComponent<Rigidbody2D>();
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((collision.tag.Equals("floor") || collision.tag.Equals("floor_middle")) && transform.tag.Equals("waterbullet"))
            Delete();
        if(collision.tag.Equals("wall"))
            Delete();
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
