using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(4939169559170921259L)]
    public class HammerBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new HammerBattleItemTemplate(), "battle/weapon/hammer", tank);
            entity.Components.Add(new HammerPelletConeComponent(1f, 1f, 1));
            entity.Components.Add(new HammerComponent());
            return entity;
        }
    }
}