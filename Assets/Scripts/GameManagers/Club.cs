[System.Serializable]
public class Club : Property
{
    public int IncomeWithPlayer;
    public int IncomeWithTrainer;
    public int IncomeWithManager;
    public Footballer Footballer;
    public Trainer Trainer;
    public Manager Manager;
    public bool IsPlayable;
}
