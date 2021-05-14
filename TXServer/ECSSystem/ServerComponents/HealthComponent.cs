using TXServer.ECSSystem.Components.Battle.Health;

namespace TXServer.ECSSystem.ServerComponents
{
    public class HealthComponent : RangedComponent, IConvertibleComponent<Components.Battle.Health.HealthComponent>, IConvertibleComponent<Components.Battle.Health.HealthConfigComponent>
    {
        public void Convert(Components.Battle.Health.HealthComponent component)
        {
            (component.CurrentHealth, component.MaxHealth) = (FinalValue, FinalValue);
        }

        public void Convert(HealthConfigComponent component) => component.BaseHealth = FinalValue;
    }
}
