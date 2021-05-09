using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public class HealthComponent : RangedComponent, IConvertibleComponent
    {
        public Component Convert() => new Components.Battle.Health.HealthComponent(FinalValue, FinalValue);
    }
}
