using FishNet.Connection;
using FishNet.Object;
using FishNet.Object.Synchronizing;
using Multiplayer.PopupSystem;
using Multiplayer.Utils;
using Steamworks;
using System;
using TMPro;
using UnityEngine;

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

    public class PlayerClient : BaseNetworkBehaviour
    {
        public static Action<PlayerClient> OnStartClient;
        public static Action<bool> OnIsReady;
        public static Action<PlayerClient> C_OnSetPosition;
        
        
        public readonly SyncVar<PlayerInfoData> PlayerInfo = new SyncVar<PlayerInfoData>();
        public readonly SyncVar<bool> IsReady = new SyncVar<bool>();

        [SerializeField] private GameObject[] gameObjectsToDisable;
        [SerializeField] private Behaviour[] componentsToDisable;
        [SerializeField] private GameObject inGameContainer;
        [SerializeField] private GameObject inMenuContainer;

        [Header("Party NameTag")] 
        [SerializeField] private TMP_Text _usernameText;
        [SerializeField] private TMP_Text _usernameTextInGame;
        [SerializeField] private TMP_Text _isReadyText;
        
        protected override void RegisterEvents()
        {
            try
            {
                PlayerConnectionManager.Instance.AllClients.Add(this);
                PlayerInfo.OnChange += OnPlayerDataChange;
                IsReady.OnChange += OnIsReadyChange;

                //if owned by/is local player
                if (IsOwner)
                {
                    OnStartClient?.Invoke(this);
                    PopupManager.Popup_Close();
                }

                Cmd_UpdatePlayerInfo(SteamClient.SteamId, SteamClient.Name);
            }
            catch (Exception ex)
            {
                Debug.LogError(ex);
            }
        }
        
        protected override void UnregisterEvents()
        {
            OnStartClient?.Invoke(null);
            PlayerInfo.OnChange -= OnPlayerDataChange;
            IsReady.OnChange -= OnIsReadyChange;
        }

        [ObserversRpc]
        public void Rpc_ToggleController(bool value)
        {
            inMenuContainer.SetActive(!value);
            inGameContainer.SetActive(value);

            if (!IsOwner)
            {
                foreach (var gameObject in gameObjectsToDisable)
                {
                    gameObject.SetActive(!value);
                }

                foreach (var component in componentsToDisable)
                {
                    component.enabled = !value;
                }
            }

            if(IsOwner)
            {
                Cursor.lockState = CursorLockMode.Locked;
                ViewManager.Instance.Show<HUDView>();
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
            transform.position = pos;
            transform.rotation = rot;
            
            C_OnSetPosition?.Invoke(this);
        }

        #region SyncVar Hooks

        private void OnPlayerDataChange(PlayerInfoData prev, PlayerInfoData currentData, bool asserver)
        {
            try
            {
                _usernameText.text = currentData.Username;
                _usernameTextInGame.text = currentData.Username;
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
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