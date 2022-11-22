using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.Tests
{
    public class AnnouncerTest : MonoBehaviour
    {
        [SerializeField] private Announcer announcer;
        public bool Send1Message;
        public bool Send10Messages;

        private void OnValidate()
        {
            Debug.Assert(announcer != null, "[AnnouncerTest] Announcer is null");
        }

        private void Update()
        {
            if (Send1Message)
            {
                Send1Message = false;
                announcer.Announce("MESSAGE");
            }

            if (Send10Messages)
            {
                Send10Messages = false;
                for (int i = 0; i < 10; ++i)
                {
                    announcer.Announce($"Msg {i}");
                }
            }
        }
    }
}
