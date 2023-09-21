using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace w4ndrv.Master
{
    using w4ndrv.UI;
    using DG.Tweening;
    using UnityEngine.UI;
    using System;

    public enum EAbilitySlot { Slot1, Slot2, Slot3, Slot4 };
    
    [Serializable]
    public class AbilitySlot
    {
        public EAbilitySlot SlotIndex;
        public Image ImgReload;
        public float ReloadTime;
        public bool CanUse = true;
        public Button BtnAbility;
        
        public void Reload()
        {
            if (ImgReload == null)
                return;
            CanUse = false;
            DOTween.To(() => ImgReload.fillAmount = 1,
                            x => ImgReload.fillAmount = x, 0, ReloadTime)
                            .OnComplete(() => CanUse = true);
        }
    }

    public class MasterInput : MonoBehaviour
    {
        public Vector2 Movement;
        public bool PlayAttack;
        public bool PlaySkill1;
        public bool PlaySkill2;

        public List<AbilitySlot> AbilitySlots = new();

        void Start()
        {
            SetupController();
        }

        void SetupController()
        {
            AbilitySlots = UIController.Instance.AbilitySlots;

            AbilitySlots.ForEach(abilitySlot =>
            {
                abilitySlot.BtnAbility.onClick.AddListener(() =>
                {
                    if (abilitySlot.CanUse)
                    {
                        UseButton(abilitySlot.SlotIndex);
                        abilitySlot.Reload();
                    }
                });
            });
        }

        public void UseButton(EAbilitySlot slotIndex)
        {
            switch (slotIndex)
            {
                case EAbilitySlot.Slot1:
                    PlayAttack = true;
                    break;
                case EAbilitySlot.Slot2:
                    PlaySkill1 = true;
                    break;
                case EAbilitySlot.Slot3:
                    PlaySkill2 = true;
                    break;
            }
        }

        // Update is called once per frame
        void Update()
        {
            float horizontal = Input.GetAxisRaw("Horizontal") + UIController.Instance.MovementJoystick.Direction.x;
            float vertical = Input.GetAxisRaw("Vertical") + UIController.Instance.MovementJoystick.Direction.y;
            Movement.x = horizontal;
            Movement.y = vertical;
        }


    }

}
