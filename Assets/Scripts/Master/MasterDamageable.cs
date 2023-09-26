using System.Collections;
using System.Collections.Generic;

namespace w4ndrv.Master
{
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using UnityEngine;
    using UnityEngine.UI;
    using DG.Tweening;

    public class MasterDamageable : NetworkBehaviour, IMasterDamageable
    {

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Image _uiHpBar;
        [SyncVar(WritePermissions = WritePermission.ServerOnly, OnChange = nameof(on_health))]
        public int HP = 10;

        private void on_health(int prev, int next, bool asServer)
        {
            if (IsClient)
            {
               
            }
        }

        public void TakeDamage(int damge)
        {
            if (IsServer == false)
                return;
            HP--;
        }
    }

  
    
}
