using System;
using System.Collections.Generic;
using TheGame.UI.Classic;
using UnityEngine;

using MessageInfo = TheGame.UI.Classic.MessageWindow.MessageInfo;

namespace TheGame.UI
{
    public class MenuMessage : MonoBehaviour
    {
        [SerializeField] private MessageWindow messageWindow;
        private readonly Queue<MessageInfo> queue = new();

        private void OnValidate()
        {
            Debug.Assert(messageWindow != null, "[MenuMessage] MessageWindow is null");
        }

        public void Show(string title, string message, Type type = Type.Ok, Action<bool> callback = null)
        {
            var info = new MessageInfo { Title = title, Message = message, Type = type, Callback = (value) => OnWindowClosed(value, callback) };

            if (messageWindow.IsOpened)
            {
                queue.Enqueue(info);
                return;
            }

            messageWindow.Show(info);
        }

        private void OnWindowClosed(bool okInvoked, Action<bool> callback)
        {
            callback?.Invoke(okInvoked);
            if (!queue.TryDequeue(out var info))
                return;

            messageWindow.Show(info);
        }

        public enum Type
        {
            Ok,
            OkCancel,
        }
    }
}
