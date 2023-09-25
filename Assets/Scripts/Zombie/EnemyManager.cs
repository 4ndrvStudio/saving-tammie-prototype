using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Enemy
{
    using FishNet;
    using FishNet.Object;

    public class EnemyManager : NetworkBehaviour
    {
        [Header("Stage And Time")]
        [SerializeField] private NetworkObject _zombiePrefab;
        [SerializeField] private List<Transform> _spawnPoints = new();
        [SerializeField] private float _timeStage = 10f;
        [SerializeField] private float _spawnTime = 1f;
        [SerializeField] private float _targetSpawnCount = 5f;
        private float _timeStageCount;
        private float _spawnTimeCount;
        private int _spawnCount;

        [Header("Object Pool")]
        [SerializeField] private int _objectPoolInstantiateCount = 50;
        [SerializeField] private bool _canDisableAllZombie;
        private List<NetworkObject> _lstZombieSpawned = new();



        private void Start()
        {

        }

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (IsServer == false)
                return;

            _timeStageCount = 8f;
            //Instantiate object Pool
            InstanceFinder.NetworkManager.CacheObjects(_zombiePrefab, _objectPoolInstantiateCount, IsServer);
        }

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
            //Check client count
            //true : Spawn enemy with state and Time
            //false : Disable all
            if (InstanceFinder.ServerManager.Clients.Count > 0)
            {
                _canDisableAllZombie = true;
                _timeStageCount += (float)InstanceFinder.TimeManager.TickDelta;
               
                if (_timeStageCount >= _timeStage)
                {

                    _spawnTimeCount += (float)InstanceFinder.TimeManager.TickDelta;
                    if (_spawnTimeCount >= _spawnTime)
                    {
                        _spawnTimeCount = 0;
                        _spawnCount++;
                        SpawnEnemy();
                        if (_spawnCount >= _targetSpawnCount)
                        {
                            _spawnCount = 0;
                            _spawnTimeCount = 0;
                            _timeStageCount = 0;
                        }
                    }
                }
            }
            else
            {
                if (_canDisableAllZombie)
                {
                    DisableAllEnemy();
                }
            }

        }

        private void SpawnEnemy()
        {
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            Vector3 randomOffset = new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            NetworkObject zombie = InstanceFinder.ServerManager.NetworkManager.GetPooledInstantiated(_zombiePrefab, spawnPoint.position + randomOffset, Quaternion.identity, true);
            InstanceFinder.ServerManager.Spawn(zombie);
            _lstZombieSpawned.Add(zombie);

        }

        private void DisableAllEnemy()
        {
            if (IsServer == false)
                return;
            _lstZombieSpawned.ForEach(item =>
            {
                Despawn(item, DespawnType.Pool);
            });
            _lstZombieSpawned.Clear();
            _canDisableAllZombie = false;

        }


    }

}
