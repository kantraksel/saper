using TMPro;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class GameOverlay : MonoBehaviour
    {
        [SerializeField] private Announcer announcer;
        [SerializeField] private Clock clock;
        [SerializeField] private TMP_Text flagCounter;

        private void OnValidate()
        {
            Debug.Assert(announcer != null, "[GameOverlay] Announcer is null");
            Debug.Assert(clock != null, "[GameOverlay] Clock is null");
            Debug.Assert(flagCounter != null, "[GameOverlay] FlagCounter is null");
        }

        public Clock GetClock() => clock;
        public Announcer GetAnnouncer() => announcer;

        public void UpdateFlagCount(int count)
        {
            flagCounter.text = count.ToString();
        }
    }
}
