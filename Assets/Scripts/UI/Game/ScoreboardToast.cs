using TheGame.GameModes.Saper;
using TMPro;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class ScoreboardToast : IconToast
    {
        [Header("ScoreboardToast")]
        [SerializeField] private TMP_Text displayName;
        [SerializeField] private TMP_Text completion;
        [SerializeField] private TMP_Text time;
        [SerializeField] private TMP_Text score;
        public bool IsActive => gameObject.activeSelf;
        public ulong Id { get; private set; }

        protected override void OnValidate()
        {
            base.OnValidate();
            Debug.Assert(displayName != null, "[ScoreboardToast] DisplayName is null");
            Debug.Assert(completion != null, "[ScoreboardToast] Completion is null");
            Debug.Assert(time != null, "[ScoreboardToast] Time is null");
            Debug.Assert(score != null, "[ScoreboardToast] Score is null");
        }

        public void Setup(ulong id, string name)
        {
            gameObject.SetActive(true);

            Id = id;
            displayName.text = name;
        }

        public void Clear()
        {
            displayName.text = string.Empty;
            icon.sprite = GetIcon(PlayerState.Active);
            completion.text = string.Empty;
            time.text = string.Empty;
            score.text = string.Empty;

            gameObject.SetActive(false);
        }

        public void UpdateScore(PlayerScore player)
        {
            icon.sprite = GetIcon(player.state);
            completion.text = $"{player.completionPercent}%";
            time.text = GetTime((int)player.timeEnd);
            score.text = $"{player.score}";
        }

        private string GetTime(int seconds)
        {
            int minutes = seconds / 60;
            seconds %= 60;
            return $"{minutes:d2}:{seconds:d2}";
        }
    }
}
