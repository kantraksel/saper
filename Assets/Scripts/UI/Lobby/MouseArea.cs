using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheGame.UI.Lobby
{
    public class MouseArea : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public bool IsHovering { get; private set; }
        public Action<bool> OnChangeMouseHoverEvent { get; set; }

        public void OnPointerEnter(PointerEventData eventData)
        {
            IsHovering = true;
            OnChangeMouseHoverEvent?.Invoke(true);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnChangeMouseHoverEvent?.Invoke(false);
            IsHovering = false;
        }
    }
}
