using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using w4ndrv.Controller;
using w4ndrv.Master;

namespace w4ndrv.UI
{
    public class View_Controller : UIView
    {
        public static View_Controller Instance;
        public JoystickCustom MovementJoystick;
        //button
        [Header("Button")]
        public List<AbilitySlot> AbilitySlots = new();


        public void Start() {
           
            if(Instance == null)
                Instance = this;
        }


       
    }

}
