public static class MinimaxSolver
{
    public static float Evaluate(GameState state, Property property, int depth)
    {
        state.Money -= property.Price;

        if (property is Club)
            state.ClubCount++;
        else
            state.TelecompanyCount++;

        return Score(state);
    }

    private static float Score(GameState state)
    {
        return
            state.Money * 0.000001f +
            state.ClubCount * 3f +
            state.TelecompanyCount * 2f;
    }
}