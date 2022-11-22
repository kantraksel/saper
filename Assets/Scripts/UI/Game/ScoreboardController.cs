using System.Collections;
using System.Collections.Generic;
using TheGame.GameModes.Saper;
using TheGame.Networking.Framework;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class ScoreboardController : MonoBehaviour
    {
        [SerializeField] private List<ScoreboardToast> toasts;
        [SerializeField] private CanvasGroup group;

        private void OnValidate()
        {
            Debug.Assert(group != null, "[ScoreboardController] Group is missing!");
        }

        private void OnDisable()
        {
            StopAllCoroutines();
            group.alpha = 1.0f;
        }

        public void Clear()
        {
            gameObject.SetActive(false);

            foreach (var toast in toasts)
            {
                toast.Clear();
            }
        }

        public void ShowSwitch() => gameObject.SetActive(!gameObject.activeSelf);

        public void AddPlayer(ClassicPlayerList.Player player)
        {
            foreach (var toast in toasts)
            {
                if (!toast.IsActive)
                {
                    toast.Setup(player.ID, player.Name);
                    return;
                }
            }

            Debug.LogWarning("[ScoreboardController] There are no free toasts available!");
        }

        public void UpdateScore(PlayerScore score)
        {
            foreach (var toast in toasts)
            {
                if (toast.Id == score.id)
                {
                    toast.UpdateScore(score);
                    return;
                }
            }

            Debug.LogWarning("[ScoreboardController] No matching toast found!");
        }

        public void ShowAnimated()
        {
            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
                StartCoroutine(AnimateShow());
            }
        }

        private IEnumerator AnimateShow()
        {
            float duration = 0f;

            while (true)
            {
                duration += 4f * Time.deltaTime;
                float alpha = Mathf.Clamp01(duration);
                group.alpha = alpha;

                if (alpha == 1.0f)
                    break;

                yield return null;
            }
        }
    }
}
