using kcp2k;
using Mirror;
using System;
using System.Collections;
using TheGame.Integration;
using TheGame.Networking.Client;
using TheGame.Networking.CustomTransport;
using TheGame.Networking.Server;
using TheGame.Networking.Shared;
using UnityEngine;

namespace TheGame.Networking
{
    public class NetMgr : NetworkManager
    {
        [Header("Custom NetMgr")]

        [SerializeField, Tooltip("Timeout for game loading stage (just after auth, but before lobby/match)")]
        private int loadingTimeout;

        [SerializeField] private SessionAuthenticator sessionAuthenticator;
        [SerializeField] private ServerCoordinator serverCoordinator;
        [SerializeField] private ClientCoordinator clientCoordinator;
        [SerializeField] private IntegrationSystem integrations;
        [SerializeField] private SimpleClient simpleClient;
        [SerializeField] private RichPresenceManager richPresenceManager;
        [SerializeField] private ServerInfo serverInfo;
        private Uri connectUri;
        private bool clientInitSynced;

        public bool IsP2PActive => integrations.IsP2PActive;

        public override void OnValidate()
        {
            base.OnValidate();

            Debug.Assert(sessionAuthenticator != null, "[NetMgr] SessionAuthenticator is null");
            Debug.Assert(serverCoordinator != null, "[NetMgr] ServerCoordinator is null");
            Debug.Assert(clientCoordinator != null, "[NetMgr] ClientCoordinator is null");
            Debug.Assert(integrations != null, "[NetMgr] Integrations is null");
            Debug.Assert(simpleClient != null, "[NetMgr] SimpleClient is null");
            Debug.Assert(richPresenceManager != null, "[NetMgr] RichPresenceManager is null");
            Debug.Assert(serverInfo != null, "[NetMgr] ServerInfo is null");

            if (loadingTimeout <= 0)
            {
                Debug.Log("[NetMgr] Setting default LoadingTimeout to 30s");
                loadingTimeout = 30;
            }
        }

        public override void Start()
        {
            //NOTE: empty, disable some things :)
        }

        public override void OnStartHost()
        {
            connectUri = transport.ServerUri();
        }

        #region server
        public override void OnStartServer()
        {
            serverInfo.SetupServer(maxConnections);
            serverCoordinator.StartServer();
        }

        public override void OnStopServer()
        {
            serverCoordinator.StopServer();
        }

        public override void OnServerConnect(NetworkConnectionToClient conn)
        {
            var data = conn.authenticationData as SessionData;
            data.LoadingTimeoutCoroutine = StartCoroutine(LoadingTimeout(conn));
        }

        public override void OnServerDisconnect(NetworkConnectionToClient conn)
        {
            if (conn.isAuthenticated)
            {
                if (conn.isReady)
                    serverCoordinator.OnPlayerDisconnect(conn);
                sessionAuthenticator.OnServerDisconnect(conn);

                var data = conn.authenticationData as SessionData;
                if (data?.LoadingTimeoutCoroutine != null)
                    StopCoroutine(data.LoadingTimeoutCoroutine);
            }

            base.OnServerDisconnect(conn);
        }

        public override void OnServerReady(NetworkConnectionToClient conn)
        {
            if (conn.isReady) return;
            base.OnServerReady(conn);

            var data = conn.authenticationData as SessionData;
            StopCoroutine(data.LoadingTimeoutCoroutine);

            serverCoordinator.OnPlayerConnect(conn);
        }
        #endregion

        #region client
        public override void StartClient()
        {
            throw new NotSupportedException("Use StartClient(address, scheme) instead");
        }

        public override void StartClient(Uri uri)
        {
            connectUri = uri;
            base.StartClient(uri);
        }

        public override void OnStartClient()
        {
            clientCoordinator.OnStartClient();
        }

        public override void OnStopClient()
        {
            clientCoordinator.OnStopClient();
        }

        public override void OnClientError(Exception exception)
        {

        }

        public override void OnClientConnect()
        {
            //NOTE: empty, disable some things :)
        }

        public override void OnClientDisconnect(byte code)
        {
            clientInitSynced = false;
            richPresenceManager.OnLeave();

            simpleClient.OnDisconnected(code);
            base.OnClientDisconnect(code);

            StopServer();
        }

        public override void OnClientSceneChanged()
        {
            //NOTE: empty, disable some things :)
        }

        public override void OnClientReady()
        {
            clientCoordinator.OnClientReady();

            //NOTE: initial connect sync
            if (clientInitSynced) return;
            clientInitSynced = true;

            richPresenceManager.OnJoin();
        }

        public override void OnClientNotReady()
        {
            clientCoordinator.OnClientUnready();
        }
        #endregion

        public bool Available()
        {
            return transport.Available();
        }

        public void StopAllModes()
        {
            switch (mode)
            {
                case NetworkManagerMode.Host:
                    {
                        StopHost();
                        break;
                    }

                case NetworkManagerMode.ClientOnly:
                    {
                        StopClient();
                        break;
                    }

                //NOTE: future compat?
                case NetworkManagerMode.ServerOnly:
                    {
                        StopServer();
                        break;
                    }
            }
        }

        private IEnumerator LoadingTimeout(NetworkConnection conn)
        {
            yield return new WaitForSecondsRealtime(loadingTimeout);

            var data = conn.authenticationData as SessionData;
            if (data != null)
                data.LoadingTimeoutCoroutine = null;
            conn.Disconnect(GDisconnectCode.LoadTimeout);
        }

        public float GetTimeout()
        {
            float timeout = 0;

            switch (transport)
            {
                case KcpTransport:
                    {
                        var kcp = (KcpTransport)transport;
                        timeout = kcp.Timeout / 1000.0f;
                        break;
                    }

                case MultiTransport:
                    {
                        var multi = (MultiTransport)transport;
                        timeout = multi.Timeout / 1000.0f;
                        break;
                    }

                default:
                    {
                        Debug.LogWarning("[NetMgr] Could not detect transport");
                        break;
                    }
            }

            return timeout;
        }

        public Uri GetClientConnectUri()
        {
            return connectUri;
        }
    }
}
