using System;
using System.Collections.Generic;
using System.Linq;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Multiplayer.PlayerSystem;
using Multiplayer.Utils;
using Steamworks;
using UnityEngine;

namespace Multiplayer
{
    public enum EServerState
    {
        MainMenu, InGame
    }

    public class PlayerConnectionManager : BaseMonoBehaviour
    {
        public static PlayerConnectionManager Instance;
        
        public static Action<NetworkConnection> S_OnConnect;
        public static Action<NetworkConnection> S_OnDisconnect;

        public static List<ulong> PlayerSteamIds = new List<ulong>();
        public static List<PlayerClient> AllClients = new List<PlayerClient>();
        
        private ServerManager _serverManager;

        protected override void RegisterEvents()
        {
            Instance = this;
            
            _serverManager = InstanceFinder.ServerManager;
            
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }

        protected override void UnregisterEvents()
        {
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState -= OnRemoteConnectionState;        
        }

        private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            ulong id = SteamClient.SteamId.Value;
            if (remoteConnectionStateArgs.ConnectionState == RemoteConnectionState.Started)
            {
                
                
                bool alreadyClient = PlayerSteamIds.Any(x => x == id);
                if (!alreadyClient)
                {
                    PlayerSteamIds.Add(id);
                    S_OnConnect?.Invoke(connection);
                }
            }
            else
            {
                PlayerSteamIds.Remove(id);
                var clientToRemove = AllClients.SingleOrDefault(x => x.PlayerInfo.Value.SteamID == id);
                AllClients.Remove(clientToRemove);
                S_OnDisconnect?.Invoke(connection);
            }
        }
    }   
}