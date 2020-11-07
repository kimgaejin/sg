using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMoving : MonoBehaviour
{
    public void SetCamera(Transform target)
    {
        Camera.main.transform.position = target.position;
        Camera.main.transform.rotation = target.rotation;
    }
}
