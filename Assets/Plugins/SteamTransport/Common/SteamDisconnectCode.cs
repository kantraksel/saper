namespace SteamSockets
{
    public struct SteamDisconnectCode
    {
        public const byte Generic = 0;
        public const byte Timeout = 1;
        public const byte HostUnreachable = 2;
        public const byte ServiceNotAvailable = 9;
    }
}
