using System;
using FishNet;
using FishNet.Managing.Scened;
using Multiplayer.PopupSystem;
using UnityEngine;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Multiplayer
{
    public enum EScenes
    {
        Boot, MainMenu, Game, Loading
    }
    public class ScenesManager : MonoBehaviour
    {
        public static Action<EScenes> OnSceneLoaded;
        private static ScenesManager _instance;
        
        //private EScenes _currentScene;

        private void Awake()
        {
            _instance = this;
            InstanceFinder.SceneManager.OnLoadStart += HandleLoadStart;
            InstanceFinder.SceneManager.OnLoadEnd += HandleLoadEnd;
        }

        private void HandleLoadStart(SceneLoadStartEventArgs args)
        {
            PopupManager.Popup_Show(new PopupContent("LOADING", "Please Wait..."));
        }

        private void HandleLoadEnd(SceneLoadEndEventArgs args)
        {
            PopupManager.Popup_Close();
        }

        public static void ChangeScene(EScenes scene, bool asServer = false)
        {
            if(_instance == null) return;

            if (asServer)
            {
                SceneLoadData sld = new SceneLoadData(EScenes.Game.ToString());
                sld.ReplaceScenes = ReplaceOption.All;
                InstanceFinder.SceneManager.LoadGlobalScenes(sld);
                
                return;
            }

            SceneManager.LoadScene(scene.ToString());
        }

        public static void ChangeScene(string scene, bool asServer = false)
        {
            if (_instance == null) return;

            if (asServer)
            {
                SceneLoadData sld = new SceneLoadData(scene);
                sld.ReplaceScenes = ReplaceOption.All;
                InstanceFinder.SceneManager.LoadGlobalScenes(sld);
                return;
            }

            SceneManager.LoadSceneAsync(scene);
        }
    }
}