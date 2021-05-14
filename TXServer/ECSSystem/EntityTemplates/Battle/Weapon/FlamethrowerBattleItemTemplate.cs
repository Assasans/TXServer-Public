using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(4652768934679402653L)]
    public class FlamethrowerBattleItemTemplate : StreamWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new FlamethrowerBattleItemTemplate(), "garage/weapon/flamethrower", tank, battlePlayer);
            entity.Components.Add(new FlamethrowerComponent());

            return entity;
        }
    }
}
