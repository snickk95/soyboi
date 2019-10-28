using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLerpToTransform : MonoBehaviour {

    public Transform camTarget;
    public float trackingSpeed;
    public float minX;
    public float minY;
    public float maxX;
    public float maxY;

    void FixedUpdate()
    {
        if (camTarget!= null)
        {
            var newPos = Vector2.Lerp(transform.position, camTarget.position, Time.deltaTime * trackingSpeed);

            var CamPosition = new Vector3(newPos.x, newPos.y, -10f);
            var v3 = CamPosition;

            var clampx = Mathf.Clamp(v3.x, minX, maxX);

            var clampy = Mathf.Clamp(v3.y, minY, maxY);

            transform.position = new Vector3(clampx, clampy, -10f);
        }
    }
}
