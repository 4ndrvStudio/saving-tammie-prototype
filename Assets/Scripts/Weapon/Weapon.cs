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

            if(IsServer == true)
            {

                if (other.CompareTag("Player"))
                    if (other.transform.root.TryGetComponent<IMasterDamageable>(out var masterDamageable))
                    {
                        
                    }
            }
           
        }

        [ServerRpc]
        public void HitEnemy(ZombieDamageable zombieDamageable)
        {
            zombieDamageable.TakeDamage(1);
        }

        //[ServerRpc]
        //public void HitEnemy(ZombieDamageable zombieDamageable)
        //{
        //    zombieDamageable.TakeDamage(1);
        //}

    }
}


