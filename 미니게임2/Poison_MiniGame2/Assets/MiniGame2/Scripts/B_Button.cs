using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class B_Button : MonoBehaviour {
    
    Collider2D m_Strite; // UISprite UI관련 컴퍼넌트를 선언합니다.

    void Awake()
    {
        m_Strite = GetComponent<Collider2D>(); // 위젯에 연결된 스프라이트를 변수에 매칭합니다.
    }
    
    void OnClick() // 클릭이벤트가 발생했을때 이 함수를 호출합니다.
    {
        if (m_Strite.tag == "UI_JumpButton")
        {
            
        }
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
