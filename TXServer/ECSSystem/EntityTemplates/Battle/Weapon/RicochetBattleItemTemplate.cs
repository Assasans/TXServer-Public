using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-8939173357737272930L)]
    public class RicochetBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank, BattlePlayer battlePlayer)
        {
            Entity entity = CreateEntity(new RicochetBattleItemTemplate(), "battle/weapon/ricochet", tank, battlePlayer);
            entity.Components.Add(new RicochetComponent());
            
            entity.Components.Add(battlePlayer.BulletSpeed == null
                ? new WeaponBulletShotComponent(0.5f, 110f)
                : new WeaponBulletShotComponent(0.5f, (float) battlePlayer.BulletSpeed));

            return entity;
        }
    }
}