using Mirror;
using TheGame.Networking.Client;
using UnityEngine;

namespace TheGame.Networking.Shared
{
    internal class StateSyncProvider : MonoBehaviour
    {
        [SerializeField] private NetMgr netMgr;
        [SerializeField] private ClientCoordinator client;
        public int ServerStateId { get; private set; }

        private void OnValidate()
        {
            Debug.Assert(netMgr != null, "[StateSyncProvider] NetMgr is null");
            Debug.Assert(client != null, "[StateSyncProvider] Client is null");
        }

        #region server
        public void OnStartServer()
        {
            NetworkServer.RegisterHandler<ReadyStateMessage>(OnReadyMessage);

            ServerStateId = 0;
        }

        public void OnStopServer()
        {
            NetworkServer.UnregisterHandler<ReadyStateMessage>();
        }

        private void OnReadyMessage(NetworkConnectionToClient conn, ReadyStateMessage msg)
        {
            if (msg.StateId != ServerStateId) return;
            netMgr.OnServerReady(conn);
        }

        public void ServerStateUpdate(GameState state)
        {
            ++ServerStateId;
            NetworkServer.SendToAll(new StateChangeMessage { State = state, StateId = ServerStateId });
        }
        #endregion

        #region client
        public void OnStartClient()
        {
            NetworkClient.RegisterHandler<StateChangeMessage>(OnStateChanged);
        }

        public void OnStopClient()
        {
            NetworkClient.UnregisterHandler<StateChangeMessage>();
        }

        private void OnStateChanged(StateChangeMessage msg)
        {
            client.LoadState(msg.State, msg.StateId);
        }

        public void ClientReadyState(int stateId)
        {
            NetworkClient.Ready(new ReadyStateMessage { StateId = stateId });
        }
        #endregion

        struct ReadyStateMessage : NetworkMessage
        {
            public int StateId;
        }

        struct StateChangeMessage : NetworkMessage
        {
            public GameState State;
            public int StateId;
        }
    }
}
