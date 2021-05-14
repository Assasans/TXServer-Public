using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents
{
    public class KickbackPropertyComponent : RangedComponent, IConvertibleComponent<KickbackComponent>
    {
        public void Convert(KickbackComponent component) => component.KickbackForce = FinalValue;
    }
}
