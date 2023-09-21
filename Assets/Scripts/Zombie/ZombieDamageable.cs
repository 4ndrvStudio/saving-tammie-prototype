using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Enemy
{
    using FishNet;
    using FishNet.Object;
    using FishNet.Object.Prediction;
    using FishNet.Transporting;
    using FishNet.Object.Synchronizing;

    public class ZombieDamageable : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private GameObject _bloodFx;

        [SyncVar(WritePermissions = WritePermission.ServerOnly, OnChange = nameof(on_health))]
        public int HP = 10;

        [SerializeField] private bool _canDamage = true;

        public override void OnStartServer()
        {
            base.OnStartServer();
            _canDamage = true;
        }

        public void TakeDamage()
        {
            if(HP<0) _canDamage = false;

            if (IsServer == false && _canDamage == false)
                return;
            HP--;
        }

        private void on_health(int prev, int next, bool asServer)
        {
            if (HP > 0)
                Debug.Log("getHit");
            else
            {
                _animator.Play("death", 1, 0);
                _collider.enabled = false;
               Invoke("DespawnEnemy",3f);
            }

            if(IsClient) {
                GameObject bloodFx = Instantiate(_bloodFx,transform.position + Vector3.up *1f,_bloodFx.transform.rotation);
                bloodFx.SetActive(true);
                Object.Destroy(bloodFx,0.4f);
            }
        }

        private void DespawnEnemy() => Despawn(this.gameObject);

    }
}

