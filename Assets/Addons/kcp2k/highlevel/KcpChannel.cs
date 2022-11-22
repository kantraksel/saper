namespace kcp2k
{
    // channel type and header for raw messages
    public enum KcpChannel : byte
    {
        // don't react on 0x00. might help to filter out random noise.
        Reliable = 0x01,
        Unreliable = 0x02
    }

    // keep in sync with Mirror.DisconnectCode
    public struct KcpDisconnectCode
    {
        public const byte Default = 0;
        public const byte Timeout = 1;
        public const byte HostUnreachable = 2;
    }
}