using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.UI
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance;

        [Header("Views")]
        [SerializeField] private List<UIView> _listView = new List<UIView>();
        [Space]
        [Header("Popups")]
        [SerializeField] private List<UIPopup> _listPopup = new List<UIPopup>();

        [Space]
        [SerializeField] private UITextAlert _textAlert;

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        void OnDestroy()
        {
            if (Instance == this)
            {
                Instance = null;
            }
        }

        public void ShowPopup(PopupName popupName, Dictionary<string, object> customProperties = null)
        {
            UIPopup selectedPopup = _listPopup.Find(popup => popup.PopupName == popupName);
            if (selectedPopup != null) selectedPopup.Show(customProperties);
        }
        public void HidePopup(PopupName popupName)
        {
            UIPopup selectedPopup = _listPopup.Find(popup => popup.PopupName == popupName);
            if (selectedPopup != null) selectedPopup.Hide();
        }
        public void ToggleView(ViewName panelName)
        {
            UIView selectedPanel = _listView.Find(tab => tab.ViewName == panelName);
            if (selectedPanel != null)
            {
                _listView.ForEach(panel => panel.Hide());
                selectedPanel.Show();
            }
        }

        public void ShowAlert(string message, AlertType alertType)
        {
            _textAlert.Show(message, alertType);
        } 
    }

}
