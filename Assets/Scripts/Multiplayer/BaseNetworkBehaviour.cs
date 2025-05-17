using Assets.Scripts.Interfaces.Multiplayer;
using FishNet.Object;

namespace Multiplayer.Utils
{
    public abstract class BaseNetworkBehaviour : NetworkBehaviour, IRegisterEvents
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

        public abstract void RegisterEvents();
        public abstract void UnregisterEvents();
    }
}