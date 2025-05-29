using Multiplayer.PlayerSystem;
using Multiplayer.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Multiplayer
{
    public class ReadyUpButton : BaseMonoBehaviour
    {
        [SerializeField] private Image _readyImage;
        [SerializeField] private Color _readyColor;
        [SerializeField] private Color _notReadyColor;
        [SerializeField] private TMP_Text _readyText;

        protected override void RegisterEvents()
        {
            PlayerClient.OnIsReady += ToggleReadyButton;
        }
        protected override void UnregisterEvents()
        {
            PlayerClient.OnIsReady -= ToggleReadyButton;
        }
        
        private void ToggleReadyButton(bool value)
        {
            _readyImage.color = value ? _readyColor : _notReadyColor;
            _readyText.text = !value ? "Ready" : "Not Ready";
        }
    }
}