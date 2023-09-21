using System.Collections;
using System.Collections.Generic;
using FishNet.Object;
using UnityEngine;

namespace w4ndrv.Civilian
{
    public class Tammie : NetworkBehaviour
    {
        public static Tammie Instance;

        public override void OnStartServer()
        {
            base.OnStartServer();
            if (Instance == null)
                Instance = this;
        }
    }

}
