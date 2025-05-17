using Assets.Scripts.Interfaces.Multiplayer;
using Multiplayer.PlayerSystem;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class ReadyUpButton : MonoBehaviour, IRegisterEvents
    {
        [SerializeField] private Image _readyImage;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _notReadyColor;
        [SerializeField] private TMP_Text _readyText;

        public void RegisterEvents()
        {
            PlayerClient.OnIsReady += ToggleReadyButton;
        }
        public void UnregisterEvents()
        {
            PlayerClient.OnIsReady -= ToggleReadyButton;
        }
        
        private void ToggleReadyButton(bool value)
        {
            _readyImage.color = value ? _readyColor : _notReadyColor;
            _readyText.text = value ? "Ready" : "Not Ready";
        }
    }
}