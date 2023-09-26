using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace w4ndrv.Master
{
    using FishNet.Object;
    using FishNet.Object.Synchronizing;
    using TMPro;
    using Core;

    public class Master : NetworkBehaviour
    {
        public static Master Instance;

        [Header("Body Parts")]
        [SerializeField] private Transform _body;

        [field: SyncVar(ReadPermissions = ReadPermission.ExcludeOwner, OnChange = nameof(on_name))]
        public string MasterName { get; [ServerRpc(RunLocally = true)] set; }

        [SerializeField] private TextMeshPro _nameText;

        private void on_name(string prev, string next, bool asServer)
        {
            _nameText.text = next;
        }

        public override void OnStartClient()
        {


            base.OnStartClient();
            if (IsOwner == false)
                return;

            if (Instance == null)
                Instance = this;

            if (string.IsNullOrEmpty(User.UserName))
            {
                MasterName = "Master " + ObjectId;
            }
            else
            {
                MasterName = User.UserName;
            }


            gameObject.name = "Owner";

            CameraManager.Instance.SetAimTarget(_body.transform);

        }
    }

}
