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

        private void Start()
        {
            Debug.Log("IsServer " + IsServer);
        }

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
            if (IsServer == true && IsPlayer == false)
            {
                if (other.CompareTag("Player"))
                {
                    if (other.transform.TryGetComponent<MasterDamageable>(out var masterDamageable))
                    {
                        Debug.Log("Hit Player");
                        Debug.Log(masterDamageable);
                    }
                }

                  
            }

        }

        [ServerRpc]
        public void HitEnemy(ZombieDamageable zombieDamageable)
        {
            zombieDamageable.TakeDamage(1);
        }
        [ServerRpc]
        public void HitPlayer(MasterDamageable masterDamageable)
        {
            masterDamageable.TakeDamage(5);

        }

    }
}


