using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using Multiplayer.PlayerSystem;
using Multiplayer.Utils;
using System;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerSpawnManager : BaseMonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        public static PlayerSpawnManager Instance;
        protected override void RegisterEvents()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }

            PlayerConnectionManager.S_OnConnect += S_OnConnect;
            
            if(InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoaded;
        }

        protected override void UnregisterEvents()
        {
            PlayerConnectionManager.S_OnConnect -= S_OnConnect;
            
            if(InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoaded;
        }
        
        private void S_OnConnect(NetworkConnection conn)
        {
            int index = PlayerConnectionManager.AllClients.Count;

            Transform spawnPoint = SpawnCache.Instance.SpawnPoints[index].transform;
            
            NetworkObject nob = InstanceFinder.NetworkManager.GetPooledInstantiated(_playerPrefab, spawnPoint.position, spawnPoint.rotation, true);
            InstanceFinder.ServerManager.Spawn(nob, conn);
        }

        //spawn players in each scene load
        public static void SpawnPlayer(int clientId, int index)
        {
            if (NetworkExtensions.TryGetNetworkObjectFromObjectId(clientId, out NetworkObject netObj))
            {
                PlayerClient client = netObj.GetComponent<PlayerClient>();
                
                Transform spawnPoint = SpawnCache.Instance.SpawnPoints[index].transform;
                client.S_SetPosition(spawnPoint.position, spawnPoint.rotation, true);
            }
        }
        
        public void OnSceneLoaded(SceneLoadEndEventArgs obj)
        {
            if(obj.LoadedScenes.Length == 0) return;
            
            int index = 0;
            foreach (var client in PlayerConnectionManager.AllClients)
            {
                SpawnPlayer(client.ObjectId, index);
                index++;
            }    
        }
    }
}