using System.Collections;
using UnityEngine;

namespace TheGame.Networking.Server.States
{
    internal class LoadingPhase : MonoBehaviour
    {
        private int playersReady;
        private int playersTotal;

        public IEnumerator LoadMatch(BaseGame gameMode, int expectedPlayers)
        {
            playersReady = 0;
            playersTotal = expectedPlayers;

            var waiter = PlayerWait(gameMode.LoadTimeout);
            yield return gameMode.OnMatchLoad();
            yield return waiter;
        }

        public void OnPlayerConnect()
        {
            ++playersReady;
        }

        public void OnPlayerDisconnect()
        {
            --playersReady;
        }

        private IEnumerator PlayerWait(int timeout)
        {
            yield return null;
            int counter = 0;
            while (playersReady < playersTotal && counter < timeout)
            {
                yield return new WaitForSecondsRealtime(1.0f);
                ++counter;
            }
        }
    }
}
