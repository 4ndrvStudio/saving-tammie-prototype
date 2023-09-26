using System.Collections;
using System.Collections.Generic;

namespace w4ndrv.Master
{
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using UnityEngine;
    using w4ndrv.UI;

    public class MasterDamageable : NetworkBehaviour
    {

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SyncVar(WritePermissions = WritePermission.ServerOnly, OnChange = nameof(on_health))]
        public int HP = 100;

        private bool canDamage = true;

        public override void OnStartClient()
        {
            base.OnStartClient();
            
            if(IsServer)
            {
                HP = 100;
                canDamage = true;
            }
               
        }

        private void on_health(int prev, int next, bool asServer)
        {
            //interact in owner
            if (IsOwner && canDamage == true)
            {
                if(HP > 0)
                {
                    View_Controller.Instance.UpdateHP((float)next / 100f);
                } else
                {
                    View_Controller.Instance.UpdateHP(0);
                    _animator.SetBool("isDeath", true);
                }
            }
            //disable interact both server and client
            if (HP <= 0) {
                _controller.enabled = false;
                canDamage = false;
            } 
        }

        public void TakeDamage(int damge)
        {
            if (IsServer == false)
                return;
            HP-= damge;
        }

        
    }

  
    
}
