public class SimulatedAction
{
    public Property Property;

    private SimulatedAction(Property property)
    {
        Property = property;
    }

    public static SimulatedAction Buy(Property property)
    {
        return new SimulatedAction(property);
    }
}