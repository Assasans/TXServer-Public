using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public class DampingComponent : RangedComponent, IConvertibleComponent
    {
        public Component Convert() => new Components.Battle.Chassis.DampingComponent(FinalValue);
    }
}
