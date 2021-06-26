using System;
using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1485337553359L)]
    public class SpiderEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer, float acceleration, long activationTime,
            float beginHideDistance, float hideRange, float impact, float damageMaxRadius, float damageMinRadius,
            float damageMinPercent, float speed, float targetingDistance, float targetingPeriod)
        {
            Entity effect = CreateEntity(new SpiderEffectTemplate(), "/battle/effect/spidermine", matchPlayer, addTeam:true);
            effect.Components.UnionWith(new Component[]
            {
                new EffectActiveComponent(),

                new MineConfigComponent(activationTime: activationTime,
                    beginHideDistance: beginHideDistance, hideRange: hideRange, impact: impact),
                new SpiderMineConfigComponent(acceleration, speed),

                new SplashWeaponComponent(damageMinPercent, damageMaxRadius, damageMinRadius),

                new UnitComponent(),
                new UnitMoveComponent(new Movement(matchPlayer.TankPosition, Vector3.Zero, Vector3.Zero,
                    matchPlayer.TankQuaternion)),
                new UnitTargetingConfigComponent(targetingPeriod, targetingDistance),

                matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>(),
                matchPlayer.Player.User.GetComponent<UserGroupComponent>()
            });

            Console.WriteLine(targetingDistance);

            return effect;
        }
    }
}
