﻿using FishNet.Object;

namespace Multiplayer.Utils
{
    public abstract class BaseNetworkBehaviour : NetworkBehaviour
    {

        public override void OnStartClient()
        {
            base.OnStartClient();
            RegisterEvents();
        }
        
        public override void OnStopClient()
        {
            base.OnStopClient();
            UnregisterEvents();
        }

        protected abstract void RegisterEvents();
        protected abstract void UnregisterEvents();
    }
}