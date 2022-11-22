using System;
using TheGame.Networking;
using UnityEngine;
using UnityEngine.UI;

namespace TheGame.UI.Classic
{
    public class ConnectingWindow : Window
    {
        [SerializeField] private NetMgr netMgr;
        [SerializeField] private Slider progressSlider;
        [SerializeField] private GameObject menuButtons;
        private float Timeout;
        private float CurrentTicks;

        protected override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(netMgr != null, "[ConnectingWindow] NetMgr is null");
            Debug.Assert(progressSlider != null, "[ConnectingWindow] ProgressSlider is null");
            Debug.Assert(menuButtons != null, "[ConnectingWindow] MenuButtons is null");
        }

        private void OnEnable()
        {
            menuButtons.SetActive(false);
        }

        private void OnDisable()
        {
            menuButtons.SetActive(true);
            Close();
        }

        public override void Open() => throw new InvalidOperationException();

        public void Open(float timeout)
        {
            base.Open();
            Timeout = timeout;
            CurrentTicks = 0;
            progressSlider.value = 0;
        }

        private void Update()
        {
            CurrentTicks += Time.deltaTime;
            progressSlider.value = CurrentTicks / Timeout;
        }

        public void StopConnect()
        {
            netMgr.StopAllModes();
        }
    }
}
