using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Weapon
{
    using FishNet.Object;
    using Enemy;
    using w4ndrv.Master;

    public class Weapon : NetworkBehaviour
    {
        [SerializeField] private bool IsPlayer;

        private BoxCollider _boxCollider;

        private void OnTriggerEnter(Collider other)
        {
            if (IsOwner == true)
            {
                if (other.CompareTag("Enemy"))
                    if (other.transform.root.TryGetComponent<ZombieDamageable>(out var zombieDamageable))
                    {
                        HitEnemy(zombieDamageable);
                    }
            }

            if (IsServer == false && IsPlayer == false)
            {
                if (other.CompareTag("Player"))
                {                  
                    if (other.transform.TryGetComponent<MasterDamageable>(out var masterDamageable))
                    {
                        HitPlayer(masterDamageable);
                    }
                }


            }

        }

        [ServerRpc]
        public void HitEnemy(ZombieDamageable zombieDamageable)
        {
            zombieDamageable.TakeDamage(1);
        }
        [ServerRpc(RequireOwnership =false)]
        public void HitPlayer(MasterDamageable masterDamageable)
        {
            masterDamageable.TakeDamage(5);
        }

    }
}


