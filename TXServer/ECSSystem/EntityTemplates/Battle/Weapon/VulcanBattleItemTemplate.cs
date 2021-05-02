using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-3936735916503799349L)]
    public class VulcanBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new VulcanBattleItemTemplate(), "battle/weapon/vulcan", tank, battlePlayer);
            entity.Components.Add(new VulcanWeaponComponent(1, 1, 1, 1, 1, 1, 1));
            entity.Components.Add(new KickbackComponent(1));
            entity.Components.Add(new ImpactComponent(1));
            entity.Components.Add(new VulcanComponent());
            return entity;
        }
    }
}