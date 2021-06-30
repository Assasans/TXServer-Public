using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Drone;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1485335642293L)]
    public class DroneEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(float duration, float targetingDistance, float targetingPeriod, Entity weapon,
            MatchPlayer matchPlayer)
        {
            Vector3 spawnPosition = new (matchPlayer.TankPosition.X, matchPlayer.TankPosition.Y + 4,
                matchPlayer.TankPosition.Z);

            Entity effect = CreateEntity(new DroneEffectTemplate(), "battle/effect/drone", matchPlayer,
                (long) duration, addTeam: true);

            effect.Components.UnionWith(new Component[]
            {
                new DroneEffectComponent(),
                new DroneMoveConfigComponent(),
                new EffectActiveComponent(),
                new UnitComponent(),
                new UnitMoveComponent(new Movement(spawnPosition, Vector3.Zero,
                    Vector3.Zero, matchPlayer.TankQuaternion)),
                new UnitTargetingConfigComponent(targetingPeriod, targetingDistance),

                weapon.GetComponent<UnitGroupComponent>(),
                matchPlayer.Player.User.GetComponent<UserGroupComponent>(),
            });

            return effect;
        }
    }
}
