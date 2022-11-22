using TheGame.Options;
using UnityEngine;

namespace TheGame.UI.Classic
{
    public class OptionsWindow : Window
    {
        [SerializeField] private Settings settings;
        private bool lockClose;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(settings != null, "[OptionsWindow] Settings is null");
        }

        public override void Open()
        {
            if (IsOpened)
                return;

            settings.Refresh();
            base.Open();
        }

        public override void Close()
        {
            if (lockClose)
                return;

            base.Close();
        }

        public void CloseSave()
        {
            if (lockClose)
                return;

            lockClose = true;

            settings.Save();
        }

        public void CloseCancel()
        {
            if (lockClose)
                return;

            Close();
        }

        public void CloseForce()
        {
            lockClose = false;
            Close();
        }
    }
}
