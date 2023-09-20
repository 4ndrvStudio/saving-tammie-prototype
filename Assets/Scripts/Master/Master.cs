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
        public string HunterName { get; [ServerRpc(RunLocally = true)] set; }

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

            //if (string.IsNullOrEmpty(PVPUI.Instance.UserName))
            //{
            //    HunterName = "Hunter " + ObjectId;
            //}
            //else
            //{
            //    HunterName = PVPUI.Instance.UserName;
            //}


            gameObject.name = "Owner";


            //var targetLookPoint = Resources.Load<GameObject>("Camera/LookPoint");
            //GameObject lookPoint = Instantiate(targetLookPoint);
            //lookPoint.GetComponent<TargetLookPoint>().TargetFollow = _body.transform;

            CameraManager.Instance.SetAimTarget(_body.transform);

            //Cursor.lockState = CursorLockMode.Locked;
            //Cursor.visible = true;

        }
    }

}
