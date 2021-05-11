namespace TXServer.ECSSystem.ServerComponents
{
    public class HealthComponent : RangedComponent, IConvertibleComponent<Components.Battle.Health.HealthComponent>
    {
        public void Convert(Components.Battle.Health.HealthComponent component)
        {
            (component.CurrentHealth, component.MaxHealth) = (FinalValue, FinalValue);
        }
    }
}
