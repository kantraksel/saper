namespace TheGame.GameModes.Saper
{
    public enum TileState : int
    {
        QuestionMarked = -4,
        Hidden = -3,
        Flagged,
        Bombed,
        Shown,
        //NOTE: unused below/MapSolver purposes only
        One,
        Two,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
    }
}
