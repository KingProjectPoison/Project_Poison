using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class B_UIManager : MonoBehaviour {
    
    public Camera mainCame;
    public Transform target;
    public RectTransform attackImage;
    public Text attackLimitText1;
    public Text attackLimitText2;
    public Text shieldLimitText1;
    public Text shieldLimitText2;
    public int attackLimit = 15;
    public int shieldLimit = 5;
    // 애니메이션을 위한
    private Animator myAnimator1;
    private Animator myAnimator2;
    public Button attackButton;
    public Button shieldButton;

    private void Awake()
    {
        myAnimator1 = attackButton.GetComponent<Animator>();
        myAnimator2 = shieldButton.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update () {
        attackImage.position = mainCame.WorldToScreenPoint(new Vector3(target.position.x, target.position.y, transform.position.z));
	}

    public void Attack()
    {
        attackLimit--;
        attackLimitText1.text = "" + attackLimit;
        attackLimitText2.text = "" + attackLimit;
        myAnimator1.SetTrigger("Attack");
    }

    public void Shield()
    {
        shieldLimit--;
        shieldLimitText1.text = "" + shieldLimit;
        shieldLimitText2.text = "" + shieldLimit;
        myAnimator2.SetTrigger("Shield");
    }

}
