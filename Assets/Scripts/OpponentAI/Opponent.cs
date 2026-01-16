using System.Collections;

public abstract class Opponent
{
    protected float mistakeChance;

    protected Bank Bank;
    protected TransferManager TransferManager;
    protected DecisionService DecisionService;

    private bool initialized;
    
    public void Init()
    {
        if (initialized)
            return;

        Bank = UnityEngine.Object.FindFirstObjectByType<Bank>();
        TransferManager = UnityEngine.Object.FindFirstObjectByType<TransferManager>();

        DecisionService = new DecisionService(Bank);

        initialized = true;
    }

    protected bool RollMistake()
    {
        return UnityEngine.Random.value < mistakeChance;
    }

    public abstract bool DecideBuyProperty(Game game, Player self, Property property);
    public abstract bool TryResolveMoney(Player self, int requiredMoney);
    public abstract IEnumerator HandleTransfer(Player self);
}