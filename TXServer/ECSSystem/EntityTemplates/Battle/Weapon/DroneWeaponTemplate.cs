using TXServer.Core.Battles;
using TXServer.Core.Configuration;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Weapon
{
    [SerialVersionUID(1485335125183L)]
    public class DroneWeaponTemplate : IEntityTemplate
    {
        private const string ConfigPath = "battle/effect/droneweapon";

        public static Entity CreateEntity(MatchPlayer matchPlayer)
        {
            Entity effect = new(new TemplateAccessor(new DroneWeaponTemplate(), ConfigPath),
                new DroneWeaponComponent(),

                matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>(),
                matchPlayer.Tank.GetComponent<TeamGroupComponent>(),
                matchPlayer.Tank.GetComponent<UserGroupComponent>(),

                new ShootableComponent(),
                Config.GetComponent<StreamHitConfigComponent>(ConfigPath),

                new WeaponComponent(),
                Config.GetComponent<WeaponCooldownComponent>(ConfigPath));

            effect.AddComponent(new UnitGroupComponent(effect));

            return effect;
        }
    }
}
