using Mirror;
using TheGame.Networking.Client;
using TheGame.Networking.Server;
using TheGame.Options;
using UnityEngine;

namespace TheGame.Networking.Shared
{
    internal class SessionAuthenticator : NetworkAuthenticator
    {
        [Header("Custom")]
        [SerializeField] private ServerCoordinator serverCoordinator;
        [SerializeField] private ClientCoordinator clientCoordinator;
        [SerializeField] private SessionStorage sessionStorage;
        [SerializeField] private PlayerProfile profile;
        [SerializeField] private SimpleClient client;

        private void OnValidate()
        {
            Debug.Assert(serverCoordinator != null, "[SessionAuthenticator] ServerCoordinator is null");
            Debug.Assert(clientCoordinator != null, "[SessionAuthenticator] ClientCoordinator is null");
            Debug.Assert(sessionStorage != null, "[SessionAuthenticator] SessionStorage is null");
            Debug.Assert(profile != null, "[SessionAuthenticator] Profile is null");
            Debug.Assert(client != null, "[SessionAuthenticator] Client is null");
        }

        #region messages
        public struct InitAuthMessage : NetworkMessage
        {
            public string Version;
        }

        public struct AuthRequestMessage : NetworkMessage
        {
            public string Name;
            public ulong Id;
        }

        public struct AuthResponseMessage : NetworkMessage
        {
            public byte Code;
            public GameState GameState;
            public int GameStateId;
        }
        #endregion

        #region client
        public override void OnStartClient()
        {
            NetworkClient.RegisterHandler<InitAuthMessage>(OnInitAuthMessage, false);
            NetworkClient.RegisterHandler<AuthResponseMessage>(OnAuthResponseMessage, false);
        }

        public override void OnStopClient()
        {
            NetworkClient.UnregisterHandler<AuthResponseMessage>();
            NetworkClient.UnregisterHandler<InitAuthMessage>();
        }

        public override void OnClientAuthenticate()
        {
            //NOTE: empty, waiting for InitAuthMessage
        }

        private void OnInitAuthMessage(InitAuthMessage msg)
        {
            //TODO: check game version

            var request = new AuthRequestMessage
            {
                Name = profile.Name,
                Id = profile.Id,
            };

            client.OnStartAuth();
            NetworkClient.Send(request);
        }

        private void OnAuthResponseMessage(AuthResponseMessage msg)
        {
            bool isAuthenticated = msg.Code == 200;

            client.OnFinishAuth();
            if (isAuthenticated)
            {
                clientCoordinator.LoadState(msg.GameState, msg.GameStateId);
                ClientAccept();
            }
            else
            {
                //TODO: parse code and show why auth failed
                //NOTE: due to Mirror design it is never executed
                ClientReject();
            }
        }
        #endregion

        #region server
        public override void OnStartServer()
        {
            NetworkServer.RegisterHandler<AuthRequestMessage>(OnAuthRequestMessage, false);
        }

        public override void OnStopServer()
        {
            NetworkServer.UnregisterHandler<AuthRequestMessage>();
        }

        public override void OnServerAuthenticate(NetworkConnectionToClient conn)
        {
            //NOTE: the Authenticator should be owned by Mirror.TimeoutAuthenticator

            var msg = new InitAuthMessage
            {
                Version = Application.version,
            };
            conn.Send(msg);
        }

        private void OnAuthRequestMessage(NetworkConnectionToClient conn, AuthRequestMessage msg)
        {
            if (conn.isAuthenticated)
                return;

            if (!CheckClient(msg.Id, ref msg.Name))
            {
                ServerReject(conn);
                return;
            }

            var data = sessionStorage.ResolveSession(msg.Id, conn.connectionId);
            conn.authenticationData = data;
            data.Name = msg.Name;

            if (!serverCoordinator.OnPlayerAuth(conn))
            {
                //NOTE: OnPlayerAuth returns false if server disconnected player
                //      so all we need to do is cleanup session
                sessionStorage.DestroySession(data.Id);
                return;
            }

            var responseMsg = new AuthResponseMessage
            {
                Code = 200,
                GameState = serverCoordinator.State,
                GameStateId = serverCoordinator.StateId,
            };
            conn.Send(responseMsg);
            ServerAccept(conn);
        }

        public void OnServerDisconnect(NetworkConnection conn)
        {
            var data = conn.authenticationData as SessionData;
            sessionStorage.UnlinkConnection(data.Id);
            conn.authenticationData = null;

            if (data.AllowDestroy)
            {
                sessionStorage.DestroySession(data.Id);
            }
        }

        private bool CheckClient(ulong id, ref string name)
        {
            name = name?.Trim();
            if (id == 0 || string.IsNullOrEmpty(name))
                return false;

            return !sessionStorage.ConnectionExists(id);
        }
        #endregion
    }
}
