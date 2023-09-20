using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Master
{
    using FishNet;
    using FishNet.Object;
    using DG.Tweening;
    public class MasterAimAssist : NetworkBehaviour
    {
        [SerializeField] private LayerMask _playerMask;
        [SerializeField] private List<Collider> _nearestToAttack;
        [SerializeField] private float _nearestTemp = Mathf.Infinity;
        [SerializeField] private float _assistantRangeAngle = 120f;
        [SerializeField] private float _attackMoveDuration = 0.1f;
        [SerializeField] private float _attackMoveDis = 0.5f;
        [SerializeField] private LayerMask _obstacleMask;

        public Collider SelectedNearest;

       
        private void Awake()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
            }
        }

        private void TimeManager_OnTick()
        {
            if (IsOwner == true || IsServer == true)
            {
                NearestCheck();
            }
        }
         private void NearestCheck()
        {
            _nearestToAttack = new List<Collider>(Physics.OverlapSphere(transform.position, 1f, _playerMask));

            if (_nearestToAttack.Count > 0)
            {
                float temp = Mathf.Infinity;
                _nearestToAttack.ForEach(targetAttack =>
                {
                
                        Transform enemy = targetAttack.transform;
                        Vector3 dirToEnemy = (enemy.position - transform.position).normalized;

                        if (Vector3.Angle(transform.forward, dirToEnemy) < _assistantRangeAngle / 2)
                        {
                            Vector3 from = targetAttack.transform.position - transform.position;
                            Vector2 to = transform.forward;
                            float angle = Vector3.Angle(from, to);
                            if (angle < temp)
                            {
                                SelectedNearest = targetAttack;
                                temp = angle;
                                _nearestTemp = temp;
                            }
                        }
                        else
                        {
                            if (SelectedNearest == targetAttack) SelectedNearest = null;
                        };
                    
                });
            }
            else
            {
                _nearestTemp = 360;
                SelectedNearest = null;
            };

        }
           public void ExecuteAimSupport()
        {
            if (IsOwner == true || IsServer == true)
            {
                if (SelectedNearest != null)
                {
                    Vector3 from = SelectedNearest.transform.position - transform.position;
                    Vector2 to = transform.forward;
                    from.y = 0;
                    var rotation = Quaternion.LookRotation(from);
                    transform.DORotateQuaternion(rotation, 0f);

                    //translate support
                    Vector3 targetPos = SelectedNearest.transform.position + ((transform.position - SelectedNearest.transform.position).normalized * 1.2f);
                    targetPos.y = transform.position.y;
                    //transform.DOMove(targetPos, _attackMoveDuration);
                    StartCoroutine(SmoothAimming(targetPos, _attackMoveDuration));

                }
                else
                {
                    RaycastHit hit;
                    if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, _attackMoveDis, _obstacleMask))
                    {
                        // transform.DOMove(transform.position + transform.forward * _attackMoveDis, _attackMoveDuration);

                        StartCoroutine(SmoothAimming(transform.position + transform.forward * _attackMoveDis, _attackMoveDuration));

                    }
                }
            }
        }
          private IEnumerator SmoothAimming(Vector3 targetPosition, float duration)
        {

            Vector3 initialPosition = transform.position;
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                // Calculate the percentage of completion based on time
                float t = elapsedTime / duration;

                // Smoothly interpolate between the initial position and the target position
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);

                // Increment the elapsed time by the delta time
                elapsedTime += base.TimeManager.Tick;

                yield return null;
            }

            // Ensure the final position is exactly the target position
            transform.position = targetPosition;

        }
    }

}
