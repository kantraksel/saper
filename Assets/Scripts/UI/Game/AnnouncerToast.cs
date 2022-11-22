using TMPro;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class AnnouncerToast : MonoBehaviour
    {
        [SerializeField] private TMP_Text text;
        [SerializeField] private CanvasGroup group;
        [SerializeField] private Announcer announcer;
        private float duration;
        private float maxDuration;
        public bool IsActive => gameObject.activeSelf;
        public int Id { get; set; }

        private void OnValidate()
        {
            Debug.Assert(text != null, "[AnnouncerToast] Text is missing!");
            Debug.Assert(group != null, "[AnnouncerToast] Group is missing!");
            Debug.Assert(announcer != null, "[AnnouncerToast] Announcer is missing!");
        }

        private void Awake()
        {
            Clear();
        }

        private void Update()
        {
            duration += Time.deltaTime;
            if (duration < maxDuration)
            {
                var alpha = 1 - (duration / maxDuration);
                group.alpha = Mathf.Clamp01(alpha * alpha * 3) * 0.9f;
            }
            else
            {
                Clear();
                announcer.OnToastDecayed(this);
            }
        }

        private void OnDisable()
        {
            text.text = null;
            duration = 0f;
        }

        public void SetText(string text)
        {
            this.text.text = text;
            maxDuration = announcer.ToastDuration;
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            gameObject.SetActive(false);
        }

        public void MoveFrom(AnnouncerToast victim)
        {
            text.text = victim.text.text;
            duration = victim.duration;
            maxDuration = victim.maxDuration;
            gameObject.SetActive(true);
            victim.Clear();
        }
    }
}
