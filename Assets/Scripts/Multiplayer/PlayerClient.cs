using System;
using Assets.Scripts.Interfaces.Multiplayer;
using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Multiplayer.PopupSystem;
using Multiplayer.Utils;
using Steamworks;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

namespace Multiplayer.PlayerSystem
{
    public struct PlayerInfoData
    {
        public string Username;
        public ulong SteamID;

        public PlayerInfoData(string username, ulong steamID)
        {
            Username = username;
            SteamID = steamID;
        }
    }

    public class PlayerClient : BaseNetworkBehaviour, IRegisterEvents
    {
        public static Action<PlayerClient> OnStartClient;
        public static Action<bool> OnIsReady;
        public static Action<PlayerClient> C_OnSetPosition;
        
        
        public readonly SyncVar<PlayerInfoData> PlayerInfo = new SyncVar<PlayerInfoData>();
        public readonly SyncVar<bool> IsReady = new SyncVar<bool>();

        [FormerlySerializedAs("mesh")]
        //[Header("Controller")] 
        //[SerializeField] private GameObject contoller;
        [SerializeField] private Behaviour[] componentsToEnable;
        
        //got lazy so decided to have the UI in the party a worldspace Canvas
        [Header("Party NameTag")] 
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private TMP_Text _isReadyText;

        private PlayerController _characterController;
        
        public override void RegisterEvents()
        {
            PlayerConnectionManager.Instance.AllClients.Add(this);
            PlayerInfo.OnChange += OnPlayerDataChange;
            IsReady.OnChange += OnIsReadyChange;

            _characterController = GetComponent<PlayerController>();
            
            //if owned by/is local player
            if (IsOwner)
            {
                OnStartClient?.Invoke(this);
                PopupManager.Popup_Close();
            }
            
            Cmd_UpdatePlayerInfo(SteamClient.SteamId, SteamClient.Name);
        }
        
        public override void UnregisterEvents()
        {
            PlayerConnectionManager.Instance.AllClients.Remove(this);
            
            OnStartClient?.Invoke(null);
            PlayerInfo.OnChange -= OnPlayerDataChange;
            IsReady.OnChange -= OnIsReadyChange;
        }

        [ObserversRpc]
        public void Rpc_ToggleController(bool value)
        {
            
            if(!IsOwner) return;

            Cursor.lockState = CursorLockMode.Locked;
            
            //contoller.SetActive(value);

            foreach (var component in componentsToEnable)
            {
                component.enabled = value;
            }
        }

        [Server]
        public void S_SetPosition(Vector3 pos, Quaternion rot, bool toggleController)
        {
            TRpc_SetPosition(Owner, pos, rot);
            Debug.Log("2");

            if (toggleController)
                Rpc_ToggleController(true);
        }

        [TargetRpc]
        private void TRpc_SetPosition(NetworkConnection conn, Vector3 pos, Quaternion rot)
        {
            _characterController.enabled = false;
            
            transform.position = pos;
            transform.rotation = rot;
            
            _characterController.enabled = true;

            C_OnSetPosition?.Invoke(this);
        }

        #region SyncVar Hooks

        private void OnPlayerDataChange(PlayerInfoData prev, PlayerInfoData currentData, bool asserver)
        {
            _usernameText.text = currentData.Username;
        }

        //Trusting client with this data since its a coop game
        [ServerRpc]
        private void Cmd_UpdatePlayerInfo(ulong steamId, string username)
        {
            PlayerInfo.Value = new PlayerInfoData(username, steamId);
        }
        
        
        private void OnIsReadyChange(bool prev, bool value, bool asserver)
        {
            _isReadyText.text = value ? "Ready" : "Not Ready";
            
            if (IsOwner)
            {
                OnIsReady?.Invoke(value);
            }
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void Cmd_ReadyUp()
        {
            IsReady.Value = !IsReady.Value;
        }

#endregion
   
    }
}