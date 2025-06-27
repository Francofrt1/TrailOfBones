using FishNet;
using FishNet.Connection;
using FishNet.Managing.Scened;
using FishNet.Object;
using Multiplayer.PlayerSystem;
using Multiplayer.Utils;
using UnityEngine;

namespace Multiplayer
{
    public class PlayerSpawnManager : BaseMonoBehaviour
    {
        [SerializeField] private GameObject _playerPrefab;
        public static PlayerSpawnManager Instance;
        protected override void RegisterEvents()
        {
            Instance = this;
            PlayerConnectionManager.S_OnConnect += S_OnConnect;

            if (InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoaded;
        }

        public void Unsubscribe()
        {
            UnregisterEvents();
        }

        protected override void UnregisterEvents()
        {
            PlayerConnectionManager.S_OnConnect -= S_OnConnect;

            if (InstanceFinder.SceneManager != null)
                InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoaded;
        }

        private void S_OnConnect(NetworkConnection conn)
        {
            int index = PlayerConnectionManager.Instance.AllClients.Count;

            Transform spawnPoint = SpawnCache.Instance.SpawnPoints[index].transform;

            Debug.Log($"Spawning client: {conn.ClientId} at spawn index: {index} on position: {spawnPoint.position}, rotation: {spawnPoint.rotation}");

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
                Debug.Log($"Spawning player: {clientId} at spawn index: {index} on position: {spawnPoint.position}, rotation: {spawnPoint.rotation}");
                client.S_SetPosition(spawnPoint.position, spawnPoint.rotation, true);
            }
        }

        private void OnSceneLoaded(SceneLoadEndEventArgs obj)
        {
            if (obj.LoadedScenes.Length == 0) return;

            int index = 0;
            foreach (var client in PlayerConnectionManager.Instance.AllClients)
            {
                SpawnPlayer(client.ObjectId, index);
                index++;
            }
        }
    }
}