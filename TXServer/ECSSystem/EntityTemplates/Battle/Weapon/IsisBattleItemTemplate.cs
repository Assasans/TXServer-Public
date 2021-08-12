using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(3413384256910001471L)]
    public class IsisBattleItemTemplate : StreamWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new IsisBattleItemTemplate(), "garage/weapon/isis", tank, battlePlayer);
            entity.Components.Add(new IsisComponent());
            entity.Components.Add(Config.GetComponent<StreamHitConfigComponent>("battle/weapon/isis"));

            return entity;
        }
    }
}
