public static class MinimaxSolver
{
    public static float Evaluate(GameState state, SimulatedAction action, int depth)
    {
        GameState newState = state.Clone();

        newState.Money -= action.Property.Price;

        if (action.Property is Club)
            newState.ClubCount++;
        else
            newState.TelecompanyCount++;

        return Score(newState);
    }

    private static float Score(GameState state)
    {
        return
            state.Money * 0.000001f +
            state.ClubCount * 3f +
            state.TelecompanyCount * 2f;
    }
}