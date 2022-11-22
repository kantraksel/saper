using TMPro;
using UnityEngine;

namespace TheGame.UI.Classic
{
    public class CreditsWindow : Window
    {
        [SerializeField] private TMP_InputField inputField;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(inputField != null, "[CreditsWindow] InputField is null");
        }

        protected override void Start()
        {
            base.Start();

            var asset = Resources.Load<TextAsset>("credits");
            inputField.text = asset.text;
        }
    }
}
