using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.UI
{
    public enum PopupName
    {
        None,
        Death
    }

    public class UIPopup : MonoBehaviour
    {
        [SerializeField] private PopupName _popupName = PopupName.None;
        public PopupName PopupName => _popupName;


        protected Dictionary<string, object> _customProperties;

        public virtual void Show(Dictionary<string, object> customProperties)
        {
            this._customProperties = customProperties;
            gameObject.SetActive(true);
        }

        public virtual void Hide()
        {
            _customProperties = null;
            gameObject.SetActive(false);
        }
    }

}

