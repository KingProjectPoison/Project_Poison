using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_OnTrigger : MonoBehaviour {

    public B_EnemyMovement target = null;
    public Transform Camera = null;
    public AudioClip soundEffect = null;
    private Animator myAnimator;
    private string ThisTag;
    private AudioSource ThisAudio;
    bool switchA = false;

    // Use this for initialization
    void Awake () {
        ThisAudio = GetComponent<AudioSource>();
        myAnimator = GetComponent<Animator>();
        ThisTag = transform.tag;
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("letterbullet") && ThisTag.Equals("Shield") && !switchA)
        {
            collision.gameObject.SetActive(false);
            switchA = true;
            ThisAudio.Play();
            myAnimator.SetTrigger("break");
            Invoke("ActiveOff", 1f);
        }
        else if ((collision.CompareTag("enemy1") || collision.CompareTag("enemy2")) && ThisTag.Equals("Shield") && !switchA)
        {
            switchA = true;
            ThisAudio.Play();
            myAnimator.SetTrigger("break");
            Invoke("ActiveOff", 1f);
        }
        else if (collision.CompareTag("Player") && ThisTag.Equals("door0"))
        {
            B_SoundManager.instance.PlaySingle(soundEffect);
            Camera.position = new Vector3(Camera.position.x, 0f, Camera.position.z);
            collision.GetComponent<B_PlayerControl>().Door0();
        }
        else if (collision.CompareTag("Player") && ThisTag.Equals("door1"))
        {
            B_SoundManager.instance.PlaySingle(soundEffect);
            B_UIManager.instance.Result();
        }
        else if(collision.CompareTag("floor_middle"))
        {
            if (target != null && target.mode == B_UIManager.enemyMode.normal)
                target.Change_i();
        }

    }

    private void ActiveOff()
    {
        gameObject.SetActive(false);
        switchA = false;
    }

}
