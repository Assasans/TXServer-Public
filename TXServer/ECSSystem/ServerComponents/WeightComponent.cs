using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public class WeightComponent : RangedComponent, IConvertibleComponent
    {
        public Component Convert() => new Components.Battle.Chassis.WeightComponent(FinalValue);
    }
}
