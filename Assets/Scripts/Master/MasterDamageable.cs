using System.Collections;
using System.Collections.Generic;

namespace w4ndrv.Master
{
    using System.Threading.Tasks;
    using DG.Tweening;
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

        [SerializeField] private List<SkinnedMeshRenderer> _lstBodySkinnedMesh = new();
        [SerializeField] private List<MeshRenderer> _lstBodyMesh = new();
        [SerializeField] private List<Material> _lstBodyMat = new();

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

            if(IsServer == false)
            {
                _lstBodySkinnedMesh.ForEach(bodyMesh =>
                {
                    Material[] materials = bodyMesh.materials;
                    bodyMesh.material = new Material(materials[0]);
                    _lstBodyMat.Add(bodyMesh.materials[0]);
                });
                _lstBodyMesh.ForEach(bodyMesh =>
                {
                    Material[] materials = bodyMesh.materials;
                    bodyMesh.material = new Material(materials[0]);
                    _lstBodyMat.Add(bodyMesh.materials[0]);
                });
            } 
                 
        }

        private void on_health(int prev, int next, bool asServer)
        {
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

            if(IsClient == true)
            {
                PlayMaterialFX();
            }

            //disable interact both server and client
            if (HP <= 0) {
                _controller.enabled = false;
                canDamage = false;
            } 

        }

        public void TakeDamage(int damage)
        {
            if (IsServer == false)
                return;
            HP-= damage;
        }

        public async void TakeOutGame()
        {
            await Task.Delay(3000);
            UIManager.Instance.ShowPopup(PopupName.Death);
            await Task.Delay(3000);
            UIManager.Instance.HidePopup(PopupName.Death);
            LeaveRoom(base.Owner);
            UIManager.Instance.ToggleView(ViewName.Login);
            View_Controller.Instance.UpdateHP(1);
            SoundManager.Instance.PlayBackground(EBackgroundType.StartGame);
        }

        public void PlayMaterialFX()
        {
            //excute effect only for client
            if (IsClient)
            {
                _lstBodyMat.ForEach(mat =>
                {
                    mat.DOColor(Color.white, "_EmissionColor", 0f).OnComplete(() =>
                    {
                        mat.DOColor(Color.red, "_EmissionColor", 0.1f).OnComplete(() =>
                        {
                            mat.DOColor(Color.white, "_EmissionColor", 0.01f).OnComplete(() =>
                            {
                                mat.DOColor(Color.red, "_EmissionColor", 0.1f).OnComplete(() =>
                                {
                                    mat.DOColor(Color.black, "_EmissionColor", 0);
                                });
                            });
                        });

                    });
                });

            }
        }

        [ServerRpc]
        public void LeaveRoom(NetworkConnection connection)
        {
            ServerManager.Kick(connection,FishNet.Managing.Server.KickReason.Unset);
        }

        
    }

  
    
}
