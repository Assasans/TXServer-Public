using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-2434344547754767853L)]
    public class SmokyBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new SmokyBattleItemTemplate(), "battle/weapon/smoky", tank);
            entity.Components.Add(new SmokyComponent());

            return entity;
        }
    }
}