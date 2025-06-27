using Multiplayer.FriendSystem;
using Multiplayer.Utils;
using Steamworks;
using Steamworks.Data;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LobbyList : MonoBehaviour
{
    [SerializeField] private GameObject _itemListPrefab;
    [SerializeField] private Transform _itemListContainer;
    private void Start()
    {
        ReloadFriendsListAsync();
    }

    public async void ReloadFriendsListAsync()
    {
        try
        {
            List<Lobby> lobbies = await SteamExtension.GetLobbies();
            foreach (Lobby lobby in lobbies)
            {
                GameObject item = Instantiate(_itemListPrefab, _itemListContainer);
                item.GetComponent<LobbyListItem>().Setup(lobby);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogException(ex);
        }
    }
}
