using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(583528765588657091L)]
    public class TwinsBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattleTankPlayer battlePlayer)
        {
            Entity entity = CreateEntity(new TwinsBattleItemTemplate(), "garage/weapon/twins", tank, battlePlayer);
            entity.Components.Add(new TwinsComponent());

            entity.Components.Add(battlePlayer.BulletSpeed == null
                ? new WeaponBulletShotComponent(0.5f, 150f)
                : new WeaponBulletShotComponent(0.5f, (float)battlePlayer.BulletSpeed));

            return entity;
        }
    }
}
