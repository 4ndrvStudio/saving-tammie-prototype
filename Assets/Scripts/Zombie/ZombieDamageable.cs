using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace w4ndrv.Enemy
{
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using DG.Tweening;
    public class ZombieDamageable : NetworkBehaviour
    {
        [SerializeField] private Animator _animator;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private GameObject _bloodFx;

        [SyncVar(WritePermissions = WritePermission.ServerOnly, OnChange = nameof(on_health))]
        public int HP = 10;

        [SerializeField] private bool _canDamage = true;

        [SerializeField] private SkinnedMeshRenderer _bodyMesh;
        [SerializeField] private MeshRenderer _headMesh;
        [SerializeField] private List<Material> _zombieMat = new();


        public override void OnStartServer()
        {
            base.OnStartServer();
            _canDamage = true;
        }

        public override void OnStartClient()
        {
            base.OnStartClient();
            Material[] materials = _bodyMesh.materials;
            _bodyMesh.material = new Material(materials[0]);
            _zombieMat.Add(_bodyMesh.materials[0]);
            Material[] materials2 = _headMesh.materials;
            _headMesh.material = new Material(materials[0]);
            _zombieMat.Add( _headMesh.materials[0]);

        }

        public void TakeDamage(int dame )
        {
            if (HP < 0) _canDamage = false;

            if (IsServer == false && _canDamage == false)
                return;
       
            HP -= dame;
        }
        

        private void on_health(int prev, int next, bool asServer)
        {
            if (HP < 0)
            {
                _animator.Play("death", 1, 0);
                _collider.enabled = false;
                Invoke("DespawnEnemy", 3f);
            }

            if (IsClient)
            {
                GameObject bloodFx = Instantiate(_bloodFx, transform.position + Vector3.up * 1f, _bloodFx.transform.rotation);
                bloodFx.SetActive(true);
                PlayMaterialFX();
                Object.Destroy(bloodFx, 0.4f);
            }
        }

        public void PlayMaterialFX()
        {

            if (IsClient)
            {
                _zombieMat.ForEach(_mat =>
                {
                    _mat.DOColor(Color.white, "_EmissionColor", 0f).OnComplete(() =>
                     {
                         _mat.DOColor(Color.green, "_EmissionColor", 0.1f).OnComplete(() =>
                         {
                             _mat.DOColor(Color.white, "_EmissionColor", 0.01f).OnComplete(() =>
                             {
                                 _mat.DOColor(Color.green, "_EmissionColor", 0.1f).OnComplete(() =>
                                 {
                                     _mat.DOColor(Color.black, "_EmissionColor", 0);
                                 });
                             });
                         });

                     });
                });


            }

        }

        private void DespawnEnemy() => Despawn(this.gameObject);

    }
}

