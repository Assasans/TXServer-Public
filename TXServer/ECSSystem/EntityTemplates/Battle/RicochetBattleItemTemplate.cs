using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-8939173357737272930L)]
    public class RicochetBattleItemTemplate : DiscreteWeaponTemplate
    {
        public static Entity CreateEntity(Entity tank)
        {
            Entity entity = CreateEntity(new RicochetBattleItemTemplate(), "battle/weapon/ricochet", tank);
            entity.Components.Add(new WeaponBulletShotComponent(0.8f, 110f));
            entity.Components.Add(new RicochetComponent());

            return entity;
        }
    }
}