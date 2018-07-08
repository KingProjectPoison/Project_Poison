using UnityEngine;

public class B_CameraMove : MonoBehaviour {

    public Transform target;

    public float smoothSpeed = 0.125f;

    private void LateUpdate()
    {
        float y = target.position.y;
        if (y >= 0 && y <= 11.5)
        {
            transform.position = new Vector3(0, y, -15);
        }
    }

}
