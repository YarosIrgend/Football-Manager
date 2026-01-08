public class GameState
{
    public int Money;
    public int ClubCount;
    public int TelecompanyCount;

    public static GameState FromGame(Game game, Player self)
    {
        return new GameState
        {
            Money = self.MoneySum,
            ClubCount = self.Clubs.Count,
            TelecompanyCount = self.Telecompanies.Count
        };
    }

    public GameState Clone()
    {
        return (GameState)MemberwiseClone();
    }
}