using Mirror;
using TheGame.Networking.Client;
using TheGame.Networking.Server.States;
using TheGame.Networking.Shared;
using UnityEngine;

namespace TheGame.Networking.Server
{
    public class ServerCoordinator : NetworkBehaviour
    {
        public BaseGame GameMode;
        public BaseLobby GameLobby;
        public bool DisableLobby;

        [Header("Internal")]
        [SerializeField] private Lobby lobby;
        [SerializeField] private Match match;
        [SerializeField] private StopServer stopServer;
        [SerializeField] private ClientCoordinator clientCoordinator;
        [SerializeField] private StateSyncProvider stateSyncProvider;

        public GameState State { get; private set; }
        public int StateId => stateSyncProvider.ServerStateId;
        private IBaseManager manager;

        private void OnValidate()
        {
            Debug.Assert(GameMode != null, "[ServerCoordinator] GameMode is null");
            if (!DisableLobby)
                Debug.Assert(GameLobby != null, "[ServerCoordinator] GameLobby is null");

            Debug.Assert(lobby != null, "[ServerCoordinator] Lobby is null");
            Debug.Assert(match != null, "[ServerCoordinator] Match is null");
            Debug.Assert(stopServer != null, "[ServerCoordinator] StopServer is null");
            Debug.Assert(clientCoordinator != null, "[ServerCoordinator] ClientCoordinator is null");
            Debug.Assert(stateSyncProvider != null, "[ServerCoordinator] StateSyncProvider is null");
        }

        private void Setup()
        {
            lobby.Setup(this);
            match.Setup(this);
            stopServer.Setup(this);
        }

        internal bool OnPlayerAuth(NetworkConnectionToClient conn)
        {
            return manager.OnPlayerAuth(conn);
        }

        internal void OnPlayerConnect(NetworkConnectionToClient conn)
        {
            manager.OnPlayerConnect(conn);
        }

        internal void OnPlayerDisconnect(NetworkConnectionToClient conn)
        {
            manager.OnPlayerDisconnect(conn);
        }

        #region misc
        private void ChangeActiveManager(GameState newState)
        {
            var newManager = GetState(newState);
            Debug.Assert(newManager != null, "[MatchCoordinator] NewManager is null!");

            manager?.Disable();

            State = newState;
            manager = newManager;

            manager.Enable();

            if (State == newState)
                stateSyncProvider.ServerStateUpdate(newState);
        }

        private IBaseManager GetState(GameState state)
        {
            return state switch
            {
                GameState.Unknown => stopServer,
                GameState.InLobby => lobby,
                GameState.InMatch => match,
                _ => null,
            };
        }
        #endregion

        public void LoadMatch()
        {
            ChangeActiveManager(GameState.InMatch);
        }

        public void EndMatch()
        {
            var state = DisableLobby ? GameState.InMatch : GameState.InLobby;
            ChangeActiveManager(state);
        }

        public void StartServer()
        {
            Setup();
            stateSyncProvider.OnStartServer();

            var state = DisableLobby ? GameState.InMatch : GameState.InLobby;
            ChangeActiveManager(state);
        }

        public void StopServer()
        {
            ChangeActiveManager(GameState.Unknown);
            stateSyncProvider.OnStopServer();
        }

        [ClientRpc]
        internal void RpcStartMatch()
        {
            clientCoordinator.StartMatch();
        }

        [TargetRpc]
        internal void TargetStartMatch(NetworkConnection target)
        {
            clientCoordinator.StartMatch();
        }
    }
}
