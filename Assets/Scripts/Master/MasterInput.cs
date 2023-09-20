using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Master
{
    public class MasterInput : MonoBehaviour
    {
        public bool PlayLockTarget;

        // Update is called once per frame
        void Update()
        {
            PlayLockTarget = Input.GetKey(KeyCode.Tab);

        }
    }

}
