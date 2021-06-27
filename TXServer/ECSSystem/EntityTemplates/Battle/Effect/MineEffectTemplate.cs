using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486709084156L)]
    public class MineEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, long activationTime, float beginHideDistance,
            float damageMaxRadius, float damageMinRadius, float damageMinPercent, float hideRange, float impact)
        {
            Entity effect = CreateEntity(new MineEffectTemplate(), "/battle/effect/mine", matchPlayer, addTeam:true);
            effect.Components.UnionWith(new Component[]
            {
                new EffectActiveComponent(),

                new MineConfigComponent(activationTime: activationTime,
                    beginHideDistance: beginHideDistance, hideRange: hideRange, impact: impact),
                new MinePositionComponent(matchPlayer.TankPosition),
                new MineEffectTriggeringAreaComponent(),

                new SplashWeaponComponent(damageMinPercent,  damageMaxRadius, damageMinRadius),
                new SplashEffectComponent(matchPlayer.Battle.Params.FriendlyFire),
                new SplashImpactComponent(impact),

                new DamageWeakeningByDistanceComponent(damageMinPercent, damageMaxRadius, damageMinRadius),
                new DiscreteWeaponComponent(),

                matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>(),
                matchPlayer.Player.User.GetComponent<UserGroupComponent>()
            });

            return effect;
        }
    }
}
