using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightHint : MonoBehaviour {
    public Transform playerPos;
    Vector3 lightPos;

    public float speed;

	// Use this for initialization
	void OnEnable(){
        lightPos = this.GetComponent<Transform>().position;
        if (playerPos.position.y < 1.3 || playerPos.position.y > 4)
                {
            lightPos.x = -6.5f;
            speed = Mathf.Abs(speed);
        }
                else if (playerPos.position.y < 3.1)
                {
            lightPos.x = 20.5f;
            speed = -Mathf.Abs(speed);
        }

        this.GetComponent<Transform>().position = lightPos; 
    }

    // Update is called once per frame
    void Update () {
        this.transform.Translate(speed * Time.deltaTime, 0, 0);
	}
    
    //void SpawnLight()
    //{
    //    playerPos = GameObject.Find("player").GetComponent<Transform>().position;
    //    lightPos = this.GetComponent<Transform>().position;
    //    lightPos.x = playerPos.x - 3f;
    //    this.GetComponent<Transform>().position = lightPos;
    //}
}
