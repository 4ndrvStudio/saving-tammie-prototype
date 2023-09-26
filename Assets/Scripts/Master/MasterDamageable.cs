using System.Collections;
using System.Collections.Generic;

namespace w4ndrv.Master
{
    using System.Threading.Tasks;
    using FishNet.Connection;
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using UnityEngine;
    using w4ndrv.Core;
    using w4ndrv.Sound;
    using w4ndrv.UI;

    public class MasterDamageable : NetworkBehaviour
    {

        [SerializeField] private CharacterController _controller;
        [SerializeField] private Animator _animator;
        [SyncVar(WritePermissions = WritePermission.ServerOnly, OnChange = nameof(on_health))]
        public int HP = 100;

        private bool canDamage = true;

        public override void OnSpawnServer(NetworkConnection connection)
        {
            base.OnSpawnServer(connection);
            canDamage = true;
            HP = 100;
        }
     
        public override void OnStartClient()
        {
            base.OnStartClient();
            canDamage = true;
        }

        private void on_health(int prev, int next, bool asServer)
        {
            Debug.Log("Take Damage!");
            //interact in owner
            if (IsOwner == true && canDamage == true)
            {
                if(HP > 0)
                {
                    View_Controller.Instance.UpdateHP((float)next / 100f);
                } else
                {
                    View_Controller.Instance.UpdateHP(0);
                    _animator.SetBool("isDeath", true);
                    TakeOutGame();
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
            Debug.Log("Take Dame: " + true);
            Debug.Log("Collider: " + _controller.enabled);
            Debug.Log("CanDamage: " + canDamage);

        }

        public async void TakeOutGame()
        {
            await Task.Delay(3000);
            UIManager.Instance.ShowPopup(PopupName.Death);
            await Task.Delay(3000);
            UIManager.Instance.HidePopup(PopupName.Death);
            LeaveRoom(base.Owner);
            UIManager.Instance.ToggleView(ViewName.Login);
            SoundManager.Instance.PlayBackground(EBackgroundType.StartGame);
            
        }

        [ServerRpc]
        public void LeaveRoom(NetworkConnection connection)
        {
            ServerManager.Kick(connection,FishNet.Managing.Server.KickReason.Unset);
        }

        
    }

  
    
}
