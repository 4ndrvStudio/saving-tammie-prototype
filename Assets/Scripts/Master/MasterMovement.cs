using System.Collections;
using UnityEngine;

namespace w4ndrv.Master
{
    using FishNet;
    using FishNet.Object;
    using FishNet.Object.Prediction;
    using FishNet.Transporting;
    using FishNet.Object.Synchronizing;
    using w4ndrv.Core;
    using TMPro;

    public class MasterMovement : NetworkBehaviour
    {
        public struct MoveData : IReplicateData
        {
            public Vector2 Move;
            public float CameraEulerY;

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;
        }

        //ReconcileData for Reconciliation
        public struct ReconcileData : IReconcileData
        {
            public Vector3 Position;
            public Quaternion Rotation;

            public ReconcileData(Vector3 position, Quaternion rotation)
            {
                Position = position;
                Rotation = rotation;
                _tick = 0;
            }

            private uint _tick;
            public void Dispose() { }
            public uint GetTick() => _tick;
            public void SetTick(uint value) => _tick = value;

        }

        [Header("Component")]
        [SerializeField] private Animator _animator;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private MasterInput _masterInput;
        [SerializeField] private MasterState _masterState;
        private GameObject _mainCamera;

        [Header("Stats")]
        public float MoveSpeed = 5.0f;
        public float SpeedChangeRate = 10.0f;
        public float RotationSmoothTime = 12f;

        private MoveData _clientMoveData;
        private int _animIDSpeed;

        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _verticalVelocity =-9.18f;

        private void Awake()
        {
            InstanceFinder.TimeManager.OnTick += TimeManager_OnTick;
            InstanceFinder.TimeManager.OnUpdate += TimeManager_OnUpdate;

            _controller = GetComponent<CharacterController>();
        }

        private void OnDestroy()
        {
            if (InstanceFinder.TimeManager != null)
            {
                InstanceFinder.TimeManager.OnTick -= TimeManager_OnTick;
                InstanceFinder.TimeManager.OnUpdate -= TimeManager_OnUpdate;
            }
        }


        public override void OnStartClient()
        {
            base.OnStartClient();

            _controller.enabled = (base.IsServer || base.IsOwner);

            if (base.IsOwner)
            {
                _mainCamera = CameraManager.Instance.MainCamera.gameObject;
            }
        }

        public override void OnStartNetwork()
        {
            base.OnStartNetwork();
            AssignAnimationIDs();

        }
        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("movement");
        }


        private void TimeManager_OnTick()
        {
            if (base.IsOwner)
            {
                Reconciliation(default, false);
                CheckInput(out MoveData md);
                Move(md, false);
            }
            if (base.IsServer)
            {
                Move(default, true);
                ReconcileData rd = new ReconcileData(transform.position, transform.rotation);
                Reconciliation(rd, true);
            }
        }

        private void TimeManager_OnUpdate()
        {
            if (base.IsOwner)
            {
                MoveWithData(_clientMoveData, Time.deltaTime);
            }
        }
        [Reconcile]
        private void Reconciliation(ReconcileData rd, bool asServer, Channel channel = Channel.Unreliable)
        {
            transform.position = rd.Position;
            transform.rotation = rd.Rotation;
        }

        private void CheckInput(out MoveData md)
        {
            md = default;


            md = new MoveData()
            {
                Move = _masterInput.Movement,
                CameraEulerY = _mainCamera.transform.eulerAngles.y,
            };


        }

        [Replicate]
        private void Move(MoveData md, bool asServer, Channel channel = Channel.Unreliable, bool replaying = false)
        {
            if (asServer || replaying)
            {
                MoveWithData(md, (float)base.TimeManager.TickDelta);
            }
            else if (!asServer)
                _clientMoveData = md;
        }


        private void MoveWithData(MoveData md, float delta)
        {

            float targetSpeed = MoveSpeed;
            float targetAnimationSpeed = 1f;

            if (md.Move == Vector2.zero)
            {
                targetSpeed = 0.0f;
                targetAnimationSpeed = 0.0f;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetAnimationSpeed, delta * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            Vector3 inputDirection = new Vector3(md.Move.x, 0.0f, md.Move.y).normalized;

            if (md.Move != Vector2.zero && _masterState.IsAction == false)
            {

                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + md.CameraEulerY;

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0.0f, _targetRotation, 0.0f), RotationSmoothTime * delta);
            }

            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;


            if (_masterState.IsAction == false )
            {
                _controller.Move(targetDirection.normalized * (targetSpeed * delta) +
                new Vector3(0.0f, _verticalVelocity, 0.0f) * delta);
            }

            _animator.SetFloat(_animIDSpeed, _animationBlend);


        }

    }
}

