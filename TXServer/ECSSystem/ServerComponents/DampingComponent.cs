namespace TXServer.ECSSystem.ServerComponents
{
    public class DampingComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.DampingComponent>
    {
        public void Convert(Components.Battle.Chassis.DampingComponent component)
        {
            component.Damping = FinalValue;
        }
    }
}
