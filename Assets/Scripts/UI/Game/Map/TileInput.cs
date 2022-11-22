using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheGame.UI.Game
{
    public class TileInput : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private VisualTile tile;
        public VisualTile Tile => tile;
        public Action<VisualTile, bool> OnInteract { get; set; }
        private bool isHovering;

        private void OnValidate()
        {
            Debug.Assert(tile != null, "[TileInput] Tile is null");
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            isHovering = true;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            isHovering = false;
        }

        private void Update()
        {
            if (!isHovering) return;

            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                OnInteract(tile, true);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                OnInteract(tile, false);
            }
        }
    }
}
