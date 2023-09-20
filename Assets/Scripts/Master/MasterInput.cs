using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using w4ndrv.UI;

namespace w4ndrv.Master
{
    public class MasterInput : MonoBehaviour
    {
        public Vector2 Movement;
        public bool PlayAttack;
        public bool PlaySkill1;

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal") + UIController.Instance.MovementJoystick.Direction.x;
            float vertical = Input.GetAxisRaw("Vertical")+ UIController.Instance.MovementJoystick.Direction.y;
            Movement.x = horizontal;
            Movement.y = vertical;

            PlayAttack = Input.GetMouseButton(0); 
            PlaySkill1 = Input.GetMouseButton(1); 
        }
    }

}
