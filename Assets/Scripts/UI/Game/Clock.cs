using TMPro;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class Clock : MonoBehaviour
    {
        [SerializeField] private TMP_Text clock;

        private void OnValidate()
        {
            Debug.Assert(clock != null, "[Clock] Clock is missing!");
        }

        public void SetTime(uint minutes, uint seconds)
        {
            clock.text = $"{minutes:d2}:{seconds:d2}";
        }

        public void SetTime(uint seconds)
        {
            var minutes = seconds / 100;
            seconds %= 100;
            clock.text = $"{minutes:d2}:{seconds:d2}";
        }

        public void Obfuscate()
        {
            clock.text = "--:--";
        }
    }
}
