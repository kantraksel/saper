using Mirror;
using Mirror.Authenticators;
using System.Collections;
using UnityEngine;

namespace TheGame.Networking.Shared
{
    internal class CustomTimeoutAuthenticator : TimeoutAuthenticator
    {
        protected override IEnumerator BeginAuthentication(NetworkConnection conn)
        {
            if (conn == null)
                yield break;

            var timeout = this.timeout;
            if (conn is NetworkConnectionToServer)
                ++timeout;

            yield return new WaitForSecondsRealtime(timeout);

            if (NetworkServer.connections.ContainsKey(conn.connectionId) && !conn.isAuthenticated)
                conn.Disconnect(DisconnectCode.AuthTimeout);
        }
    }
}
