using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Weapon
{
    using FishNet.Object;
    using Enemy;

    public class Weapon : NetworkBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (IsOwner == false)
                return;

            if (other.CompareTag("Enemy"))
                if (other.transform.root.TryGetComponent<ZombieDamageable>(out var zombieDamageable))
                {
                    HitEnemy(zombieDamageable);
                }
        }

        [ServerRpc]
        public void HitEnemy(ZombieDamageable zombieDamageable)
        {
            zombieDamageable.TakeDamage();
        }
    }
}


