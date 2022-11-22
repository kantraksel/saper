using TheGame.Networking.Server;

namespace TheGame.GameModes.Saper
{
    public interface IMode
    {
        public void OnMatchStart(int playerCount);
        public void OnMatchCollapse(object rawData);
        public void CheckMatchEndConditions(Map map, SessionData data);
        public void OnPlayerFinished(bool isBombed);
    }
}
