using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Master
{
    using FishNet;
    using FishNet.Object;
    using FishNet.Object.Prediction;
    using FishNet.Transporting;
    using FishNet.Object.Synchronizing;

    public class MasterCombat : NetworkBehaviour
    {
         public struct AttackData : IReplicateData
        {
            public bool AttackButton;
            public bool Skill1Button;

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }
        public struct AttackReconcileData : IReconcileData
        {
            public int AttackCount;
            public int Skill1Count;

            public AttackReconcileData(int attackCount, int skill1Count)
            {
                AttackCount = attackCount;
                Skill1Count = skill1Count;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }
        [SerializeField] private Animator _animator;
        [SerializeField] private MasterInput _masterInput;
        [SerializeField] private MasterState _masterState;
        [SerializeField] private MasterAimAssist _masterAimAssist;


        //Normal Attack
        [SyncVar(WritePermissions = WritePermission.ClientUnsynchronized)]
        public int AttackCount;
        //Normal Attack
        [SyncVar(WritePermissions = WritePermission.ServerOnly)]
        private int _normalAttackIndex;
        [SerializeField] private List<string> _normalAttackList;  

        //Skill Attack
        [SyncVar(WritePermissions = WritePermission.ClientUnsynchronized)]
        public int Skill1Count;
        [SerializeField] private float _skill1Time;


        [SerializeField] private AttackData _clientAttackData;
        private int _lastAttackCount;
        private int _lastSkill1Count;

   

          private void Awake()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnUpdate += TimeManager_OnUpdate;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            _lastAttackCount = AttackCount;
            _lastSkill1Count = Skill1Count;
            _skill1Time = 0;

        }
        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnUpdate -= TimeManager_OnUpdate;
            }
        }

         private void TimeManager_OnTick()
        {

            if (base.IsOwner)
            {
                Reconciliation(default, false);
                CheckInput(out AttackData attackData);
                Combat(attackData, false);
            }
            if (base.IsServer)
            {
                Combat(default, true);
                AttackReconcileData attackReconcileData = new AttackReconcileData();
                Reconciliation(attackReconcileData, true);
            }
        }
           private void CheckInput(out AttackData attackData)
        {
            attackData = default;
            bool canAbilityOrAttack =  _masterState.IsAction || _masterState.IsAbilityCanMove;
            //normal attack
            bool attackButton = canAbilityOrAttack ? false : _masterInput.PlayAttack;
            bool skill1Button = canAbilityOrAttack ? false : _masterInput.PlaySkill1;

            attackData = new AttackData()
            {
                AttackButton = attackButton,
                // DashButton = dashButton,
                Skill1Button = skill1Button,
                // Skill2Button = skill2Button
            };
            
            // if(dashButton)
            // {
            //     _hunterInput.Reload(AbilitySlot.Slot1);
            // }

            // if (skill1Button)
            // {
            //     _hunterInput.Reload(AbilitySlot.Slot2);
            // }
            // if (skill2Button)
            // {
            //     _hunterInput.Reload(AbilitySlot.Slot3);
            // }

            _masterInput.PlayAttack = false;
            _masterInput.PlaySkill1 = false;
            // _hunterInput.PlaySkill2 = false;
            // _hunterInput.PlayDash = false;



        }

        [Replicate]
        private void Combat(AttackData attackData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {

            if (attackData.AttackButton == true)
            {
                AttackCount += 1;
            }
            // if (attackData.DashButton == true)
            // {
            //     DashCount += 1;
            // }

            if (attackData.Skill1Button == true)
            {
                Skill1Count += 1;
            }
            // if (attackData.Skill2Button == true)
            // {
            //     Skill2Count += 1;
            // }


        }

        [Reconcile]
        private void Reconciliation(AttackReconcileData attackReconcileData, bool asServer, Channel channel = Channel.Unreliable)
        {
            //AttackCount = attackReconcileData.AttackCount;

        }
         private void TimeManager_OnUpdate()
        {
            RenderCombat();
            RenderSkillTime();
        }

        private void RenderSkillTime() {
            //skill1 time
            if(_skill1Time > 0) {
                _skill1Time -= (float)InstanceFinder.TimeManager.TickDelta;
            } else
                _animator.SetBool("skill1",false);

        }

        private void RenderCombat() {
            if (AttackCount > _lastAttackCount)
            {
                PlayAttack();
            }
            _lastAttackCount = AttackCount;

             if (Skill1Count > _lastSkill1Count)
            {
                PlaySkill1();
            }
            _lastSkill1Count = Skill1Count;
        }
        
        public void PlayAttack()
        {
            _normalAttackIndex++;

            if (_normalAttackIndex >= _normalAttackList.Count)
                _normalAttackIndex = 0;

            _masterAimAssist.ExecuteAimSupport();

            _animator.Play(_normalAttackList[_normalAttackIndex], 1, 0);
        }
        
        public void PlaySkill1()
        {
            _animator.SetBool("skill1",true);
            _skill1Time = 1f;
        }



    }

}
