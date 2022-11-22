using System.Collections;
using TheGame.Networking.Client.States;
using TheGame.Networking.Shared;
using UnityEngine;

namespace TheGame.Networking.Client
{
    public class ClientCoordinator : MonoBehaviour
    {
        public BaseGameClient GameMode;
        public BaseLobbyClient GameLobby;
        public bool DisableLobby;

        [Header("Internal")]
        [SerializeField] private Menu menu;
        [SerializeField] private Lobby lobby;
        [SerializeField] private Match match;
        [SerializeField] private LoadScreen loadScreen;
        [SerializeField] private StateSyncProvider stateSyncProvider;

        private GameState state;
        private int stateId;
        private IBaseManager manager;
        private Coroutine loadingTask;
        private bool isLoadingTaskWaiting;
        private (GameState, IBaseManager, int) awaitingState;

        private void OnValidate()
        {
            Debug.Assert(GameMode != null, "[ClientCoordinator] GameMode is null");
            if (!DisableLobby)
                Debug.Assert(GameLobby != null, "[ClientCoordinator] GameLobby is null");

            Debug.Assert(menu != null, "[ClientCoordinator] Menu is null");
            Debug.Assert(lobby != null, "[ClientCoordinator] Lobby is null");
            Debug.Assert(match != null, "[ClientCoordinator] Match is null");
            Debug.Assert(loadScreen != null, "[ClientCoordinator] LoadScreen is null");
            Debug.Assert(stateSyncProvider != null, "[ClientCoordinator] StateSyncProvider is null");
        }

        private void Awake()
        {
            state = GameState.Unknown;
            manager = menu;
        }

        private void Start()
        {
            menu.Setup(this);
            lobby.Setup(this);
            match.Setup(this);
        }

        #region LoadState
        internal void LoadState(GameState state, int stateId)
        {
            var gameState = GetState(state);
            if (gameState == null)
            {
                Debug.LogWarning($"[ClientCoordinator] Unknown state {state}");
                return;
            }

            LoadState(gameState, state, stateId);
        }

        private void LoadState(IBaseManager newManager, GameState newState, int newStateId)
        {
            if (loadingTask != null)
            {
                if (!isLoadingTaskWaiting)
                {
                    awaitingState = (newState, newManager, newStateId);
                    return;
                }

                StopCoroutine(loadingTask);
                isLoadingTaskWaiting = false;
            }

            loadScreen.Enable();
            manager?.Disable();

            state = newState;
            manager = newManager;
            stateId = newStateId;

            loadingTask = StartCoroutine(DefferedLoadState());
        }

        private IEnumerator DefferedLoadState()
        {
            yield return manager.Load();
            loadingTask = null;
            isLoadingTaskWaiting = false;

            //NOTE: awaitingState guard is not needed here
            //guards in Ready() and LoadState() are sufficient

            loadScreen.Disable();
            manager.Enable();
        }
        #endregion

        #region misc
        private void ChangeActiveState(IBaseManager newManager, GameState newState)
        {
            manager?.Disable();

            state = newState;
            manager = newManager;

            manager.Enable();
        }

        private IBaseManager GetState(GameState state)
        {
            return state switch
            {
                GameState.Unknown => menu,
                GameState.InLobby => lobby,
                GameState.InMatch => match,
                _ => null,
            };
        }
        #endregion

        internal void StartMatch()
        {
            match.SetMatchStarted();
        }

        internal void OnClientReady()
        {
            manager.OnClientReady();
        }

        internal void OnClientUnready()
        {
            loadScreen.Enable();
        }

        internal void OnStartClient()
        {
            stateSyncProvider.OnStartClient();
        }

        internal void OnStopClient()
        {
            stateSyncProvider.OnStopClient();
            ChangeActiveState(menu, GameState.Unknown);
        }

        internal void Ready()
        {
            isLoadingTaskWaiting = true;
            if (awaitingState.Item1 != GameState.Unknown)
            {
                var state = awaitingState;
                awaitingState = (GameState.Unknown, null, 0);

                LoadState(state.Item2, state.Item1, state.Item3);
                return;
            }

            stateSyncProvider.ClientReadyState(stateId);
        }
    }
}
