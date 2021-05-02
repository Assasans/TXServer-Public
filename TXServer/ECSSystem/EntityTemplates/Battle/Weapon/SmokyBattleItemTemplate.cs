using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-2434344547754767853L)]
    public class SmokyBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new SmokyBattleItemTemplate(), "battle/weapon/smoky", tank, battlePlayer);
            entity.Components.Add(new SmokyComponent());

            return entity;
        }
    }
}