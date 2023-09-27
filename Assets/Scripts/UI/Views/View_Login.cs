using System.Collections;
using System.Collections.Generic;

namespace w4ndrv.UI
{
    using TMPro;
    using UnityEngine;
    using UnityEngine.UI;
    using w4ndrv.Core;
    using w4ndrv.Sound;
    using FishNet.Example;
    public class View_Login : UIView
    {
        [SerializeField] private NetworkHudCanvases _networkHud;
        [SerializeField] private TMP_InputField _inputName;
        [SerializeField] private Button _btnPlay;

        // Start is called before the first frame update
        void Start()
        {
            _btnPlay.onClick.AddListener(() =>
            {
                if (_inputName.text.Length > 3 && _inputName.text.Length<=6)
                {
                    User.SetUserName(_inputName.text);
                    _networkHud.OnClick_Client();
                    UIManager.Instance.ToggleView(ViewName.Controller);
                    SoundManager.Instance.PlayBackground(EBackgroundType.InGame);
                }
                else
                    UIManager.Instance.ShowAlert("Your name length must be greater than 3 and lower than 7 character!", AlertType.Warning);
               
            });
        }

    }

}
