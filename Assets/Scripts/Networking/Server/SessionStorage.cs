using Mirror;
using System.Collections.Generic;
using UnityEngine;

namespace TheGame.Networking.Server
{
    public class SessionStorage : MonoBehaviour
    {
        private readonly Dictionary<ulong, SessionData> sessionList = new();
        private readonly Dictionary<ulong, int> connectionList = new();

        #region Session
        private SessionData CreateSession(ulong ID)
        {
            var data = new SessionData { Id = ID };
            sessionList.Add(ID, data);
            return data;
        }

        public SessionData ResolveSession(ulong ID, int connId)
        {
            var data = GetSession(ID) ?? CreateSession(ID);
            connectionList.Add(ID, connId);
            return data;
        }

        public void DestroySession(ulong ID)
        {
            sessionList.Remove(ID);
            connectionList.Remove(ID);
        }

        public SessionData GetSession(ulong ID)
        {
            sessionList.TryGetValue(ID, out var data);
            return data;
        }
        #endregion

        #region Connection-Session Link
        public void UnlinkConnection(ulong ID)
        {
            connectionList.Remove(ID);
        }

        public bool ConnectionExists(ulong ID)
        {
            return connectionList.ContainsKey(ID);
        }
        #endregion

        public void ClearData()
        {
            sessionList.Clear();
            connectionList.Clear();
        }

        public void RemoveOldSessions()
        {
            var list = new Queue<ulong>();
            foreach (var id in sessionList.Keys)
            {
                if (!connectionList.ContainsKey(id))
                    list.Enqueue(id);
            }

            foreach (var id in list)
            {
                sessionList.Remove(id);
            }
        }

        public void MarkAllSessionsAsNew()
        {
            foreach (var data in sessionList.Values)
            {
                data.IsPlayer = false;
            }
        }

        public NetworkConnectionToClient GetConnection(ulong ID)
        {
            if (connectionList.TryGetValue(ID, out var connId))
                if (NetworkServer.connections.TryGetValue(connId, out var connection))
                    return connection;

            return null;
        }
    }
}
