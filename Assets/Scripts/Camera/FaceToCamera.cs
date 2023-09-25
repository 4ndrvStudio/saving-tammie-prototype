using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using w4ndrv.Core;

public class FaceToCamera : MonoBehaviour
{
    private void LateUpdate()
    {
        transform.LookAt(transform.position +CameraManager.Instance.MainCamera.transform.rotation * Vector3.forward,CameraManager.Instance.MainCamera.transform.rotation * Vector3.up);
    }
}