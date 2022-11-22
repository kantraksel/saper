using TMPro;
using UnityEngine;

namespace TheGame.UI.Lobby
{
    public class TooltipController : MonoBehaviour
    {
        [SerializeField] private TMP_Text label;
        [SerializeField] private RectTransform rootUITrasform;

        private void OnValidate()
        {
            Debug.Assert(label != null, "[TooltipController] Label is null");
            Debug.Assert(rootUITrasform != null, "[TooltipController] RootUITrasform is null");
        }

        public void Show(string title)
        {
            gameObject.SetActive(true);
            label.text = title;
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        public void UpdatePosition()
        {
            var transform = GetComponent<RectTransform>();
            var size = (transform.sizeDelta / 2) * rootUITrasform.localScale;
            size.x = -size.x;
            size.y = -size.y;
            var position = (Vector2)Input.mousePosition - size;
            gameObject.transform.position = position;
        }
    }
}
