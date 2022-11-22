using UnityEngine;

namespace TheGame.Networking.Framework
{
    public class DefaultGameClient : BaseGameClient
    {
        [SerializeField] protected GameObject gameUI;

        protected virtual void OnValidate()
        {
            Debug.Assert(gameUI != null, "[DefaultGameClient] GameUI is null");
        }

        public override void OnMatchLoaded()
        {
            gameUI.SetActive(true);
        }

        public override void OnMatchUnload()
        {
            gameUI.SetActive(false);
        }
    }
}
