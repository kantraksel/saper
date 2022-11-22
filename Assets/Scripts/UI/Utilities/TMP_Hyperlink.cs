using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

namespace TheGame.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class TMP_Hyperlink : MonoBehaviour, IPointerClickHandler
    {
        private TMP_Text text;

        private void Start()
        {
            text = GetComponent<TMP_Text>();
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            var index = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);

            if (index != -1)
            {
                var info = text.textInfo.linkInfo[index];
                Application.OpenURL(info.GetLinkID());
            }
        }
    }
}
