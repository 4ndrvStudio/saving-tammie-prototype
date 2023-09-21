using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.UI
{
    using UnityEngine.EventSystems;
    public class UIButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
    {

        public bool IsPressed = false;
        public GameObject _joyOb;
        public void OnPointerDown(PointerEventData eventData)
        {
            IsPressed = true;
            _joyOb.SetActive(true);
            this.gameObject.SetActive(false);
        }
        public void OnPointerUp(PointerEventData eventData)
        {

            IsPressed = false;
        }
    }
}
