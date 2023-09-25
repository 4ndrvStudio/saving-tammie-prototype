using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Enemy
{
    using System.Linq;
    using UnityEngine.AI;
    using w4ndrv.Master;
    using FishNet;
    using FishNet.Object;
    using DG.Tweening;
    using w4ndrv.Civilian;

    public class Zombie : NetworkBehaviour
    {
        [SerializeField] private NavMeshAgent _navMeshAgent;
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private LayerMask _obstacleMask;
        [SerializeField] private Animator _animator;
        [SerializeField] private float _speedWalk = 1.5f;
        [SerializeField] private float _speedRun = 4f;
        [SerializeField] private float _viewRadius = 5f;
        [SerializeField] private List<Collider> _playersInRange;
        [SerializeField] private Collider _selectedPlayer;

        public bool IsDeath;

        private void Awake()
        {
            InstanceFinder.TimeManager.OnUpdate += TimeManager_OnUpdate;
                        _animator.SetFloat("speed", 1);

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

            if (IsServer == false)
                return;

            IsDeath = _animator.GetCurrentAnimatorStateInfo(1).IsTag("Death");
            if (!IsDeath)
                CheckPlayerInRange();
            else
                Stop(transform);
        }
        private void Follow(Transform targetPos)
        {
            _animator.SetFloat("speed", 1);
            _animator.SetBool("isAttack", false);
            _navMeshAgent.SetDestination(targetPos.position);
            _navMeshAgent.speed = _speedWalk;
            _navMeshAgent.isStopped = false;

        }
        private void Stop(Transform target)
        {
            if (!IsDeath)
            {
                _animator.SetFloat("speed", 0);
                _animator.SetBool("isAttack", true);
                //aim rotate
                Vector3 from = target.transform.position - transform.position;
                Vector2 to = transform.forward;
                from.y = 0;
                var rotation = Quaternion.LookRotation(from);
                transform.DORotateQuaternion(rotation, 0f);
            }

            _navMeshAgent.speed = 0;
            _navMeshAgent.isStopped = true;



        }

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, _viewRadius);
        }

        private void FollowAndAttack(Transform target)
        {
            float dstToTarget = Vector3.Distance(transform.position, target.position);

            if (dstToTarget > 1.5f)
            {
                Follow(target);
            }
            else
            {
                Stop(target);
            }
        }

        private void CheckPlayerInRange()
        {
            Collider[] rangeTemp = Physics.OverlapSphere(transform.position, _viewRadius, _playerMask);

            _playersInRange = rangeTemp.ToList();
            if (_playersInRange.Count <= 0)
            {
                _selectedPlayer = null;
            }
            else
            {
                for (int i = 0; i < _playersInRange.Count; i++)
                {
                    Transform player = _playersInRange[i].transform;
                    Vector3 dirToPlayer = (player.position - transform.position).normalized;
                    MasterState masterState = player.GetComponent<MasterState>();

                    if (_selectedPlayer != null && masterState.IsDeath)
                        _selectedPlayer = null;

                    if (_selectedPlayer == null && !masterState.IsDeath)
                        _selectedPlayer = _playersInRange[i];

                    if (Vector3.Distance(_selectedPlayer.transform.position, transform.position) > _viewRadius)
                        _selectedPlayer = null;
                }

            }

            if (_selectedPlayer != null)
            {
                Vector3 dirToPlayer = (_selectedPlayer.transform.position - transform.position).normalized;
                float dstToPlayer = Vector3.Distance(transform.position, _selectedPlayer.transform.position);
                // Check Player In Range
                if (!Physics.Raycast(transform.position, dirToPlayer, dstToPlayer, _obstacleMask))
                {
                    FollowAndAttack(_selectedPlayer.transform);
                }
                else
                {
                    FollowAndAttack(Tammie.Instance.transform);
                }
            }
            else
                FollowAndAttack(Tammie.Instance.transform);
        }

    }

}
