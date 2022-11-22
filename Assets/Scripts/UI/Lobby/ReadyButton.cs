using TheGame.Networking.Framework;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Lobby
{
    public class ReadyButton : MonoBehaviour
    {
        [SerializeField] private DefaultLobby lobby;
        [SerializeField] private Button button;
        private bool isReady;
        public Color NotReadyColor;
        public Color ReadyColor;
        public Color NotReadyHighlightedColor;
        public Color ReadyHighlightedColor;

        private void OnValidate()
        {
            Debug.Assert(button != null, "[ReadyButton] Button is null");
            Debug.Assert(lobby != null, "[ReadyButton] Lobby is null");
        }

        private void Reset()
        {
            NotReadyColor = Color.white;
            ReadyColor = Color.green;
            NotReadyHighlightedColor = new Color(245.0f / 255.0f, 245.0f / 255.0f, 245.0f / 255.0f);
            ReadyHighlightedColor = new Color(0.0f, 245.0f / 255.0f, 0.0f);
        }

        private void Start()
        {
            isReady = false;
            UpdateColor();
        }

        private void OnDisable()
        {
            isReady = false;
            UpdateColor();
        }

        public void Ready()
        {
            isReady = !isReady;

            lobby.CmdReady();
            UpdateColor();
        }

        private void UpdateColor()
        {
            var block = button.colors;
            block.normalColor = isReady ? ReadyColor : NotReadyColor;
            block.highlightedColor = isReady ? ReadyHighlightedColor : NotReadyHighlightedColor;
            button.colors = block;
        }
    }
}
