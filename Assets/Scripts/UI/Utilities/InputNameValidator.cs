using TMPro;
using UnityEngine;

namespace TheGame.UI
{
    public class InputNameValidator : MonoBehaviour
    {
        [SerializeField] private TMP_Text input;
        [SerializeField] private Color normalColor;
        [SerializeField] private Color warningColor;

        private void OnValidate()
        {
            Debug.Assert(input != null, "[InputNameValidator] Input is null");
        }

        public void OnValueChanged(string value)
        {
            var color = normalColor;

            value = value.Trim();
            if (value.Length < 3)
                color = warningColor;

            input.color = color;
        }
    }
}
