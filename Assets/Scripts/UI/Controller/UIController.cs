using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using w4ndrv.Controller;

namespace w4ndrv.UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;

        public JoystickCustom MovementJoystick;

        public void Start() {
           
            if(Instance == null)
                Instance = this;
        
        }


       
    }

}
