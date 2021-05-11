namespace TXServer.ECSSystem.ServerComponents
{
    public class WeightComponent : RangedComponent, IConvertibleComponent<Components.Battle.Chassis.WeightComponent>
    {
        public void Convert(Components.Battle.Chassis.WeightComponent component)
        {
            component.Weight = FinalValue;
        }
    }
}
