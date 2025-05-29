using Multiplayer.Utils;
using Steamworks;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Multiplayer.FriendSystem
{
    public class FriendListManager : MonoBehaviour
    {
        [SerializeField] private GameObject _friendListItem;
        [SerializeField] private Transform _friendListContainer;
        private void Start()
        {
            ReloadFriendsListAsync();
        }

        public async void ReloadFriendsListAsync()
        {
            try
            {
                IEnumerable<Friend> friends = SteamFriends.GetFriends();
                foreach (Friend friend in friends.OrderByDescending(x => x.IsOnline))
                {
                    GameObject friendItem = Instantiate(_friendListItem, _friendListContainer);
                    var image = await SteamFriends.GetLargeAvatarAsync(friend.Id);
                    var imageTexture = SteamExtension.GetTextureFromImage(image.Value);
                    friendItem.GetComponent<FriendListItem>().Setup(friend, imageTexture);
                }
            }
            catch (System.Exception ex)
            {
                Debug.LogException(ex);
            }
        }
    }
}