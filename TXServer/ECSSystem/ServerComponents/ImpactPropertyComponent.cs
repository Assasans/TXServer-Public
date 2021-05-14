using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents
{
    public class ImpactPropertyComponent : RangedComponent, IConvertibleComponent<ImpactComponent>
    {
        public void Convert(ImpactComponent component) => component.ImpactForce = FinalValue;
    }
}
