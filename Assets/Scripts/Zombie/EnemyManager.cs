using System.Collections;
using System.Collections.Generic;
using FishNet;
using FishNet.Object;
using UnityEngine;

namespace w4ndrv.Enemy
{
    public class EnemyManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _zombiePrefab;
        [SerializeField] private List<Transform> _spawnPoints = new();
        [SerializeField] private float _timeState;
        [SerializeField] private float _spawnTime;
        [SerializeField] private int _spawnCount;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (IsServer == false)
                return;

            _timeState = 8f;
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
            _timeState+= (float)InstanceFinder.TimeManager.TickDelta;
            if(_timeState >= 10) {
                _spawnTime+= (float)InstanceFinder.TimeManager.TickDelta;
                if(_spawnTime >=1f) {
                    _spawnTime = 0;
                    _spawnCount++;
                    SpawnEnemy();
                    if(_spawnCount >=5f) {
                        _spawnCount = 0;
                        _spawnTime = 0;
                        _timeState = 0;
                    }
                }
            }
        }

        private void SpawnEnemy() {
            Debug.Log("Spawn");
            Transform spawnPoint = _spawnPoints[Random.Range(0, _spawnPoints.Count)];
            Vector3 randomOffset = new Vector3(Random.Range(-10,10),0,Random.Range(-10,10));
            GameObject zombie = Instantiate(_zombiePrefab, spawnPoint.position + randomOffset, Quaternion.identity);
            InstanceFinder.ServerManager.Spawn(zombie);
        }


    }

}
