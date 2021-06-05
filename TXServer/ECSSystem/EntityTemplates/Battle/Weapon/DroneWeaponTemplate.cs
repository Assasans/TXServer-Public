using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(1485335125183L)]
    public class DroneWeaponTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, Entity drone)
        {
            return new(new TemplateAccessor(new DroneWeaponTemplate(), "battle/effect/droneweapon"),
                matchPlayer.Player.User.GetComponent<UserGroupComponent>(),
                matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>(),

                new WeaponComponent(),

                new DroneWeaponComponent(),
                new WeaponCooldownComponent(1),

                drone.GetComponent<UnitGroupComponent>());
        }
    }
}
