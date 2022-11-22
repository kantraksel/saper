using Mirror;
using System.Collections;
using TheGame.Networking;
using TheGame.UI.Game;
using UnityEngine;

namespace TheGame.GameModes.Saper
{
    public class SaperGamemodeClient : BaseGameClient
    {
        [SerializeField] private GameOverlay overlay;
        [SerializeField] private VisualMapController mapController;
        [SerializeField] private SpectatorController spectatorController;
        [SerializeField] private MapSync mapSync;
        [SerializeField] private Scoreboard scoreboard;

        private void OnValidate()
        {
            Debug.Assert(overlay != null, "[SaperGamemodeClient] Overlay is null");
            Debug.Assert(mapController != null, "[SaperGamemodeClient] MapController is null");
            Debug.Assert(spectatorController != null, "[SaperGamemodeClient] SpectatorController is null");
            Debug.Assert(mapSync != null, "[SaperGamemodeClient] MapSync is null");
            Debug.Assert(scoreboard != null, "[SaperGamemodeClient] Scoreboard is null");
        }

        [ClientRpc]
        public void RpcAnnounce(string message) =>
            overlay.GetAnnouncer().Announce(message);

        [TargetRpc]
        public void TargetResyncEnvironment(NetworkConnection target, Vector2Int mapSize, int bombs)
        {
            mapSync.GenerateMapChain(mapSize, bombs);
            scoreboard.SetupClient();
        }

        public override void OnMatchLoaded()
        {
            overlay.gameObject.SetActive(true);
            scoreboard.SetupClient();
        }

        public override void OnMatchUnload()
        {
            scoreboard.ClearClient();
            overlay.gameObject.SetActive(false);
            mapController.ReleaseMaps();
        }

        [ClientRpc]
        public void RpcPlayerFinished(ulong id, bool hasDied)
        {
            if (mapController.IsLocalMap(id))
                spectatorController.EnterSpectatorMode();
        }

        [ClientRpc]
        public void RpcMatchEnd()
        {
            StartCoroutine(AnimateEnd());
        }

        private IEnumerator AnimateEnd()
        {
            yield return new WaitForSeconds(1.0f);
            scoreboard.ShowAnimated();
        }
    }
}
