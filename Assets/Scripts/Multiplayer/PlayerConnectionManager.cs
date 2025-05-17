using System;
using System.Collections.Generic;
using Assets.Scripts.Interfaces.Multiplayer;
using FishNet;
using FishNet.Connection;
using FishNet.Managing.Server;
using FishNet.Transporting;
using Multiplayer.PlayerSystem;
using UnityEngine;

namespace Multiplayer
{
    public enum EServerState
    {
        MainMenu, InGame
    }

    public class PlayerConnectionManager : MonoBehaviour, IRegisterEvents
    {
        public static PlayerConnectionManager Instance;
        
        public static Action<NetworkConnection> S_OnConnect;
        public static Action<NetworkConnection> S_OnDisconnect;

        public List<PlayerClient> AllClients = new List<PlayerClient>();
        
        private ServerManager _serverManager;
        
        public void RegisterEvents()
        {
            Instance = this;
            
            _serverManager = InstanceFinder.ServerManager;
            
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState += OnRemoteConnectionState;
        }
        
        public void UnregisterEvents()
        {
            if (_serverManager != null)
                _serverManager.OnRemoteConnectionState -= OnRemoteConnectionState;        
        }

        private void OnRemoteConnectionState(NetworkConnection connection, RemoteConnectionStateArgs remoteConnectionStateArgs)
        {
            if(remoteConnectionStateArgs.ConnectionState == RemoteConnectionState.Started)
                S_OnConnect?.Invoke(connection);
            else
            {
                S_OnDisconnect?.Invoke(connection);
            }
        }
    }   
}