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
    using System.Threading.Tasks;

    public class MasterCombat : NetworkBehaviour
    {
        public struct AttackData : IReplicateData
        {
            public bool AttackButton;
            public bool Skill1Button;
            public bool Skill2Button;

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }
        public struct AttackReconcileData : IReconcileData
        {
            public int AttackCount;
            public int Skill1Count;
            public int Skill2Count;

            public AttackReconcileData(int attackCount, int skill1Count, int skill2Count)
            {
                AttackCount = attackCount;
                Skill1Count = skill1Count;
                Skill2Count = skill2Count;
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

        //Skill 1 Attack
        [SyncVar(WritePermissions = WritePermission.ClientUnsynchronized)]
        public int Skill1Count;
        [SerializeField] private float _skill1Time;

        //Skill 2 Attack
        [SyncVar(WritePermissions = WritePermission.ClientUnsynchronized)]
        public int Skill2Count;
        [SerializeField] private GameObject _fxStartSkill2;
        [SerializeField] private GameObject _fxBoomSkill2;
        [SerializeField] private GameObject _fxSelectSkill2;
        [SerializeField] private LayerMask _enemyMask;

        [SerializeField] private AttackData _clientAttackData;
        private int _lastAttackCount;
        private int _lastSkill1Count;
        private int _lastSkill2Count;

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
            _lastSkill2Count = Skill2Count;
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
            bool canAbilityOrAttack = _masterState.IsAction || _masterState.IsAbilityCanMove;
            //normal attack
            bool attackButton = canAbilityOrAttack ? false : _masterInput.PlayAttack;
            bool skill1Button = canAbilityOrAttack ? false : _masterInput.PlaySkill1;
            bool skill2Button = canAbilityOrAttack ? false : _masterInput.PlaySkill2;

            attackData = new AttackData()
            {
                AttackButton = attackButton,
                Skill1Button = skill1Button,
                Skill2Button = skill2Button
            };

            _masterInput.PlayAttack = false;
            _masterInput.PlaySkill1 = false;
            _masterInput.PlaySkill2 = false;

        }

        [Replicate]
        private void Combat(AttackData attackData, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {

            if (attackData.AttackButton == true)
            {
                AttackCount += 1;
            }

            if (attackData.Skill1Button == true)
            {
                Skill1Count += 1;
            }
            if (attackData.Skill2Button == true)
            {
                Skill2Count += 1;
            }


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

        private void RenderSkillTime()
        {
            //skill1 time
            if (_skill1Time > 0)
            {
                _skill1Time -= (float)InstanceFinder.TimeManager.TickDelta;
            }
            else
                _animator.SetBool("skill1", false);

        }

        private void RenderCombat()
        {
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

            if (Skill2Count > _lastSkill2Count)
            {
                PlaySkill2();
            }
            _lastSkill2Count = Skill2Count;
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
            _animator.SetBool("skill1", true);
            _skill1Time = 1f;
        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, 9f);
        }
        public async void PlaySkill2()
        {
            _animator.Play("skill2", 1, 0);
            Collider[] enemyCollider = Physics.OverlapSphere(transform.position, 9f, _enemyMask);

            if (IsClient)
            {
                GameObject fxStart = Instantiate(_fxStartSkill2, transform.position, Quaternion.identity);
                fxStart.SetActive(true);
                Destroy(fxStart, 2f);
                for (int i = 0; i < enemyCollider.Length; i++)
                {
                    GameObject fxSelect = Instantiate(_fxSelectSkill2, enemyCollider[i].transform.position + Vector3.up *1f, Quaternion.identity);
                    fxSelect.SetActive(true);
                    Destroy(fxSelect, 1.5f);
                }
            }


            if (IsServer)
            {
                for (int i = 0; i < enemyCollider.Length; i++)
                {
                    await Task.Delay(500);
                    GameObject fxBoom = Instantiate(_fxBoomSkill2, enemyCollider[i].transform.position, Quaternion.identity);
                    InstanceFinder.ServerManager.Spawn(fxBoom);
                    fxBoom.SetActive(true);
                    Destroy(fxBoom, 2f);
                }
            }

        }



    }

}
