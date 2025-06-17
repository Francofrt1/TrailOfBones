using FishNet;
using FishNet.Managing.Scened;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Multiplayer.PopupSystem
{
    public class PopupContent
    {
        public string Title;
        public string Text;
        public UnityAction YesButton;
        public UnityAction NoButton;
        public bool ShowConfirmButton;

        public PopupContent(string title, string text, bool showConfirmButton = false, UnityAction yesButton = null, UnityAction noButton = null)
        {
            Title = title;
            Text = text;
            YesButton = yesButton;
            NoButton = noButton;
            ShowConfirmButton = showConfirmButton;
        }
    }
    
    public class PopupManager : MonoBehaviour
    {
        public static PopupManager Instance;

        [SerializeField] private TMP_Text _textTitle;
        [SerializeField] private TMP_Text _descTitle;
        [SerializeField] private GameObject _popUp;
        [SerializeField] private Button _yesButton;
        [SerializeField] private Button _noButton;
        [SerializeField] private Button _confirmButton;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
            InstanceFinder.SceneManager.OnLoadEnd += OnSceneLoaded;
        }

        private void OnDisable()
        {
            if (InstanceFinder.SceneManager != null)
            {
                InstanceFinder.SceneManager.OnLoadEnd -= OnSceneLoaded;
            }
        }

        private void OnSceneLoaded(SceneLoadEndEventArgs args)
        {

        }

        public static void Popup_Show(PopupContent popupContent)
        {
            try
            {
                Instance._textTitle.text = popupContent.Title;
                Instance._descTitle.text = popupContent.Text;
                Instance._popUp.SetActive(true);

                Instance._yesButton.gameObject.SetActive(popupContent.YesButton != null);
                Instance._noButton.gameObject.SetActive(popupContent.NoButton != null);
                Instance._confirmButton.gameObject.SetActive(popupContent.NoButton == null && popupContent.YesButton == null && popupContent.ShowConfirmButton);


                if (popupContent.YesButton != null)
                {
                    Instance._yesButton.onClick.RemoveAllListeners();
                    Instance._yesButton.onClick.AddListener(popupContent.YesButton);
                    Popup_Close();
                }

                if (popupContent.NoButton != null)
                {
                    Instance._yesButton.onClick.RemoveAllListeners();
                    Instance._yesButton.onClick.AddListener(popupContent.NoButton);
                    Popup_Close();
                }
            }
            catch (Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }
        public static void Popup_Close()
        {
            Instance._popUp.SetActive(false);
        }
    }
}