using FishNet;
using FishNet.Managing.Client;
using FishNet.Managing.Server;
using FishNet.Object;
using UnityEngine;
using Multiplayer.PlayerSystem;

namespace Multiplayer.Utils
{
    public static class NetworkExtensions
    {

        public static PlayerClient GetLocalPlayer()
        {
            if (InstanceFinder.ClientManager == null)
            {
                Debug.LogError("Client Manager is Null");
                return null;
            }

            PlayerClient player = null;

            foreach (var obj in InstanceFinder.ClientManager.Connection.Objects)
            {
                if (obj.gameObject.CompareTag("Player"))
                    player = obj.GetComponent<PlayerClient>();
            }

            return player;
        }


        public static T GetPlayer<T>(this NetworkBehaviour behaviour, int objectId) where T : Component
        {

            if (InstanceFinder.IsServerStarted)
            {
                ServerManager serverManager = InstanceFinder.ServerManager;
                if (serverManager == null || !serverManager.Objects.Spawned.ContainsKey(objectId))
                    return default;

                T networkObject = serverManager.Objects.Spawned[objectId].GetComponent<T>();

                return networkObject;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                ClientManager clientManager = InstanceFinder.ClientManager;
                if (clientManager == null || !clientManager.Objects.Spawned.ContainsKey(objectId))
                    return default;

                T networkObject = clientManager.Objects.Spawned[objectId].GetComponent<T>();

                return networkObject;
            }

            return default;
        }

        public static bool TryGetNetworkObjectFromObjectId(this int objectId, out NetworkObject networkObject)
        {
            networkObject = null;
            if (InstanceFinder.IsServerStarted)
            {
                ServerManager serverManager = InstanceFinder.ServerManager;
                if (serverManager == null || !serverManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = serverManager.Objects.Spawned[objectId];
                return true;
            }
            else if (InstanceFinder.IsClientStarted)
            {
                ClientManager clientManager = InstanceFinder.ClientManager;
                if (clientManager == null || !clientManager.Objects.Spawned.ContainsKey(objectId))
                    return false;

                networkObject = clientManager.Objects.Spawned[objectId];
                return true;
            }

            return false;
        }
    }
}