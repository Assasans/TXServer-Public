using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1554279538858L)]
    public class KamikadzeEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(float damageMinPercent, bool friendlyFire, float impact, float splashRadius,
            MatchPlayer matchPlayer)
        {
            Entity effect = CreateEntity(new KamikadzeEffectTemplate(), "battle/effect/kamikadze", matchPlayer, addTeam:true);
            effect.Components.UnionWith(new Component[]
            {
                new SplashEffectComponent(friendlyFire),
                new SplashWeaponComponent(damageMinPercent, 0, splashRadius),
                new SplashImpactComponent(impact),

                new DamageWeakeningByDistanceComponent(damageMinPercent, 0, splashRadius),

                new DiscreteWeaponComponent(),

                matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>()
            });

            return effect;
        }
    }
}
