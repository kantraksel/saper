using Mirror;
using System.Collections;
using UnityEngine;

namespace TheGame.Networking.Server.States
{
    internal class Match : MonoBehaviour, IBaseManager
    {
        [SerializeField] private SessionStorage sessionStorage;
        [SerializeField] private LoadingPhase loadingPhase;
        private ServerCoordinator parent;
        private BaseLobby gameLobby;
        private BaseGame gameMode;
        private bool disableLobby;

        private bool isLoading;
        private Coroutine collapse;

        private void OnValidate()
        {
            Debug.Assert(sessionStorage != null, "[MatchServer] SessionStorage is null");
            Debug.Assert(loadingPhase != null, "[MatchServer] LoadingPhase is null");
        }

        public void Setup(ServerCoordinator parent)
        {
            this.parent = parent;
            gameLobby = parent.GameLobby;
            gameMode = parent.GameMode;
            disableLobby = parent.DisableLobby;
        }

        public void Enable()
        {
            NetworkServer.SetAllClientsNotReady();

            isLoading = true;
            StartCoroutine(StartMatchCoroutine());
        }

        public void Disable()
        {
            if (!isLoading)
                gameMode.OnMatchEnd();

            if (collapse != null)
            {
                StopCoroutine(collapse);
                collapse = null;
            }

            sessionStorage.RemoveOldSessions();
        }

        public bool OnPlayerAuth(NetworkConnectionToClient conn)
        {
            if (disableLobby) return true;

            var data = conn.authenticationData as SessionData;
            if (!data.IsPlayer)
            {
                conn.Disconnect(GDisconnectCode.NotInLobby);
                return false;
            }
            return true;
        }

        public void OnPlayerConnect(NetworkConnectionToClient conn)
        {
            if (isLoading)
            {
                loadingPhase.OnPlayerConnect();
                return;
            }

            var data = conn.authenticationData as SessionData;

            gameMode.OnPlayerHotJoined(conn, data);
            parent.TargetStartMatch(conn);
        }

        public void OnPlayerDisconnect(NetworkConnectionToClient conn)
        {
            if (isLoading)
            {
                loadingPhase.OnPlayerDisconnect();
                return;
            }

            var data = conn.authenticationData as SessionData;

            gameMode.OnPlayerLeft(conn, data);
        }

        private IEnumerator StartMatchCoroutine()
        {
            var expectedPlayers = disableLobby ? 0 : gameLobby.PlayersInLobby;
            yield return loadingPhase.LoadMatch(gameMode, expectedPlayers);

            BeginMatch();
        }

        private void BeginMatch()
        {
            isLoading = false;

            gameMode.OnMatchBegin();
            parent.RpcStartMatch();
        }

        public void CollapseMatch(object data)
        {
            if (collapse != null)
            {
                Debug.LogError("[MatchManager] Collapse is already running!");
                return;
            }

            collapse = StartCoroutine(DoFinalCollapse(data));
        }

        private IEnumerator DoFinalCollapse(object data)
        {
            yield return gameMode.OnMatchCollapse(data);

            parent.EndMatch();
            collapse = null;
        }
    }
}
