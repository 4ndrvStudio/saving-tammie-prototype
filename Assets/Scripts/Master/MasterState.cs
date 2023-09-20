using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Master
{
    using FishNet;
    using FishNet.Object;
    using FishNet.Object.Synchronizing;

    public class MasterState : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;

        [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
        public bool IsAction { get; [ServerRpc(RunLocally = true)] set; }

        [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
        public bool IsAbilityCanMove { get; [ServerRpc(RunLocally = true)] set; }

        [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner)]
        public bool IsDeath { get; [ServerRpc(RunLocally = true)] set; }

        private void Awake()
        {
            InstanceFinder.TimeManager.OnUpdate += TimeManager_OnUpdate;
        }
        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnUpdate -= TimeManager_OnUpdate;
            }
        }
        private void TimeManager_OnUpdate()
        {
            if (base.IsOwner == false)
                return;

            IsAction = _animator.GetCurrentAnimatorStateInfo(1).IsTag("Action");
            IsAbilityCanMove = _animator.GetCurrentAnimatorStateInfo(1).IsTag("AbilityCanMove");
        }

    }

}
