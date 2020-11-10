using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(525358843506658817L)]
    public class FreezeBattleItemTemplate : StreamWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new FreezeBattleItemTemplate(), "battle/weapon/freeze", tank);
            entity.Components.Add(new FreezeComponent());

            return entity;
        }
    }
}