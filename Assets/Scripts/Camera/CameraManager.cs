using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace w4ndrv.Core
{
    public class CameraManager : MonoBehaviour
    {
        public static CameraManager Instance;

        [SerializeField] private Camera _mainCam;
        public Camera MainCamera => _mainCam;

        [SerializeField] private CinemachineVirtualCamera _cameraVT;

        // Start is called before the first frame update
        void Start()
        {
            if(Instance == null)
                Instance = this;
        }

        // Update is called once per frame
        void Update()
        {

        }
        public void SetAimTarget(Transform body)
        {
            _cameraVT.Follow = body;
        }

        public Vector3 GetMainCamEuler() => _mainCam.transform.eulerAngles;

        public Transform GetTransform() => _mainCam.transform;
    }

}
