using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Controller
{
    using UnityEngine.EventSystems;

    public class JoystickCustom : Joystick
    {
        private Vector3 _rootPos;
         private CanvasGroup _canvasGroup;
        protected override void Start()
        {
            base.Start();
            _canvasGroup  = gameObject.GetComponent<CanvasGroup>();
            _rootPos = background.anchoredPosition;
            _canvasGroup.alpha =0.3f;
        }

        public override void OnPointerDown(PointerEventData eventData)
        {
            background.anchoredPosition = ScreenPointToAnchoredPosition(eventData.position);
             _canvasGroup.alpha =1f;
            base.OnPointerDown(eventData);
        }

        public override void OnPointerUp(PointerEventData eventData)
        {
            background.anchoredPosition = _rootPos;
             _canvasGroup.alpha =0.3f;
            base.OnPointerUp(eventData);
           
        }
    }

}
