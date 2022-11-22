using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI
{
    public class LoadingAnimator : MonoBehaviour
    {
        [SerializeField] private Image background;
        [SerializeField] private float animationSpeed;
        [SerializeField] private Color32 minColor = new(110, 110, 110, 255);
        [SerializeField] private Color32 maxColor = new(90, 90, 90, 255);

        [SerializeField] private Image icon;
        [SerializeField] private float iconAnimationSpeed;

        [SerializeField] private TMP_Text loadingText;
        [SerializeField] private float textSpeed;

        private void OnValidate()
        {
            Debug.Assert(background != null, "[LoadingAnimator] Background is null");
            Debug.Assert(icon != null, "[LoadingAnimator] Icon is null");
            Debug.Assert(loadingText != null, "[LoadingAnimator] LoadingText is null");
        }

        private void Update()
        {
            background.color = Color.Lerp(minColor, maxColor, Mathf.Sin(Time.time * animationSpeed));
            icon.transform.Rotate(Vector3.forward, iconAnimationSpeed * Time.deltaTime);
            loadingText.text = Mathf.Sin(Time.time * textSpeed) <= 0 ? "Game is loading..." : "Please stand by...";
        }
    }
}
