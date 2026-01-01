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

    // Термін перебування команди у запасі - 1 круг поля (31 клітинка)
    private byte spareTerm;
    public byte SpareTerm
    {
        get
        {
            if (IsPlayable)
            {
                spareTerm = 31;
            }

            return spareTerm;
        }
        private set => spareTerm = value;
    }

    public void DecreaseSpareTerm(int cells)
    {
        if (!IsPlayable)
            spareTerm -= (byte)cells;
        
        if (spareTerm <= 0)
        {
            spareTerm = 0;
            IsPlayable = true;
        }
    }
}
