using Steamworks;
using System;
using UnityEngine;

public class SteamClientInitializer : MonoBehaviour
{
    private void Awake()
    {
        if (!SteamClient.IsValid)
        {
            try
            {
                SteamClient.Init(480);
            }
            catch (Exception e)
            {
                // Something went wrong - it's one of these:
                //
                //     Steam is closed?
                //     Can't find steam_api dll?
                //     Don't have permission to play app?
                //

                Debug.LogException(e);
                Application.Quit();
            }
        }
    }
}
