using Mirror;

namespace TheGame.Networking
{
    public static class GDisconnectCode
    {
        public const byte LoadTimeout = DisconnectCode.User;
        public const byte NotInLobby = DisconnectCode.User + 1;
        public const byte LobbyNotAvailable = DisconnectCode.User + 2;

        //NOTE: local errors
        public const byte NoDevices = 255;
    }
}
