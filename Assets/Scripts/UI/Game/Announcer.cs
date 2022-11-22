using System.Collections.Generic;
using UnityEngine;

namespace TheGame.UI.Game
{
    public class Announcer : MonoBehaviour
    {
        [SerializeField] private List<AnnouncerToast> toasts;
        [SerializeField] private int toastDuration;
        private readonly Queue<string> messageQueue = new();
        public int ToastDuration => toastDuration;

        private void Start()
        {
            for (int i = 0; i < toasts.Count; ++i)
            {
                toasts[i].Id = i;
            }
        }

        private void OnDisable()
        {
            foreach (var toast in toasts)
            {
                toast.Clear();
            }
            messageQueue.Clear();
        }

        public void Announce(string message)
        {
            foreach (var toast in toasts)
            {
                if (toast.IsActive)
                    continue;

                SetupToast(toast, message);
                return;
            }

            messageQueue.Enqueue(message);
        }

        public void OnToastDecayed(AnnouncerToast toast)
        {
            MoveToasts(ref toast);

            if (messageQueue.Count == 0)
                return;

            SetupToast(toast, messageQueue.Dequeue());
        }

        private void SetupToast(AnnouncerToast toast, string message)
        {
            toast.SetText(message);
        }

        private void MoveToasts(ref AnnouncerToast deadToast)
        {
            var nextId = deadToast.Id + 1;

            if (nextId == toasts.Count)
                return;

            for (; nextId < toasts.Count; ++nextId)
            {
                var toast = toasts[nextId];
                if (!toast.IsActive)
                    return;

                deadToast.MoveFrom(toast);
                deadToast = toast;
            }
        }
    }
}
