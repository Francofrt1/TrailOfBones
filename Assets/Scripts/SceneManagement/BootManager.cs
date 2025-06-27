using UnityEngine;

namespace Multiplayer
{
    public class BootManager : MonoBehaviour
    {
        private void Start()
        {
            ScenesManager.ChangeScene(EScenes.MainMenu);
        }
    }
}