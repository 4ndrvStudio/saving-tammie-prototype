using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using w4ndrv.Controller;
using w4ndrv.Master;

namespace w4ndrv.UI
{
    public class UIController : MonoBehaviour
    {
        public static UIController Instance;
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
