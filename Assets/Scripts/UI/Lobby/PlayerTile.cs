using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Lobby
{
    public class PlayerTile : MonoBehaviour
    {
        [SerializeField] private Image border;
        [SerializeField] private Image icon;
        [SerializeField] private TooltipController tooltip;
        [SerializeField] private MouseArea mouseArea;
        private string playerName;
        private bool isVisible;
        private bool isOccupied;
        public ulong ID { get; set; }

        public Color DefaultColor;
        public Color ReadyColor;

        private void OnValidate()
        {
            Debug.Assert(border != null, "[PlayerTile] Border is null");
            Debug.Assert(icon != null, "[PlayerTile] Icon is null");
            Debug.Assert(tooltip != null, "[PlayerTile] Tooltip is null");
            Debug.Assert(mouseArea != null, "[PlayerTile] MouseArea is null");
        }

        private void Reset()
        {
            DefaultColor = Color.white;
            ReadyColor = Color.green;
        }

        private void Start()
        {
            border.color = DefaultColor;
            icon.color = DefaultColor;

            mouseArea.OnChangeMouseHoverEvent = OnMouseAreaHoverChange;

            if (!isVisible)
                gameObject.SetActive(false);
        }

        private void Update()
        {
            if (mouseArea.IsHovering)
                tooltip.UpdatePosition();
        }

        public void EnableTile(ulong ID, string name)
        {
            isVisible = true;
            this.ID = ID;
            isOccupied = true;
            gameObject.SetActive(true);

            border.color = DefaultColor;
            icon.color = DefaultColor;
            playerName = name;
        }

        public void DisableTile()
        {
            isVisible = false;
            isOccupied = false;
            gameObject.SetActive(false);
        }

        public void ChangeReadyStatus(bool isReady)
        {
            border.color = isReady ? ReadyColor : DefaultColor;
        }

        public void MoveTo(PlayerTile tile)
        {
            tile.icon.color = icon.color;
            tile.border.color = border.color;
            tile.ID = ID;
            tile.playerName = playerName;

            if (tile.mouseArea.IsHovering)
                tile.tooltip.Show(playerName);
        }

        public bool IsOccupied() => isOccupied;

        private void OnMouseAreaHoverChange(bool isHovering)
        {
            if (isHovering)
                tooltip.Show(playerName);
            else
                tooltip.Hide();
        }
    }
}
