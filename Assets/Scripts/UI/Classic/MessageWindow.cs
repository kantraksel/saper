using System;
using TMPro;
using UnityEngine;

using Type = TheGame.UI.MenuMessage.Type;

namespace TheGame.UI.Classic
{
    public class MessageWindow : Window
    {
        [SerializeField] private TMP_Text titleField;
        [SerializeField] private TMP_Text contentField;

        [SerializeField] private GameObject okLayout;
        [SerializeField] private GameObject askLayout;

        private Action<bool> callback;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(titleField != null, "[MenuController] TitleField is null");
            Debug.Assert(contentField != null, "[MenuController] ContentField is null");
            Debug.Assert(okLayout != null, "[MenuController] OkLayout is null");
            Debug.Assert(askLayout != null, "[MenuController] AskLayout is null");
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return))
                Close();
        }

        public void Show(MessageInfo info)
        {
            titleField.text = info.Title;
            contentField.text = info.Message;
            callback = info.Callback;

            switch (info.Type)
            {
                case Type.Ok:
                    {
                        okLayout.SetActive(true);
                        askLayout.SetActive(false);
                        break;
                    }

                case Type.OkCancel:
                    {
                        okLayout.SetActive(false);
                        askLayout.SetActive(true);
                        break;
                    }
            }

            base.Open();
        }

        public override void Open() => throw new InvalidOperationException();

        public override void Close() => Close(true);

        public void Close(bool okInvoked)
        {
            base.Close();

            var callback = this.callback;
            this.callback = null;
            callback?.Invoke(okInvoked);
        }

        public struct MessageInfo
        {
            public string Title;
            public string Message;
            public Type Type;
            public Action<bool> Callback;
        }
    }
}
