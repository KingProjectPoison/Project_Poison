using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_DestroyInTime : MonoBehaviour {

    [SerializeField]
    float destroyTime = 2f;

	// Use this for initialization
	void Start () {
        Destroy(gameObject, destroyTime);
	}
}
