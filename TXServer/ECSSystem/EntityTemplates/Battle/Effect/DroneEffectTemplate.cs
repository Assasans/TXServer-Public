using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Drone;
using TXServer.ECSSystem.Components.Battle.Effect.Unit;
using TXServer.ECSSystem.Components.Battle.Tank;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1485335642293L)]
    public class DroneEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer)
        {
            Vector3 spawnPosition = new (matchPlayer.TankPosition.X, matchPlayer.TankPosition.Y + 4,
                matchPlayer.TankPosition.Z);

            Entity effect = CreateEntity(new DroneEffectTemplate(), "/battle/effect/drone", matchPlayer, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());

            effect.AddComponent(new EffectActiveComponent());

            effect.AddComponent(new DroneEffectComponent());
            effect.AddComponent(new DroneMoveConfigComponent(matchPlayer));

            effect.AddComponent(new UnitComponent());
            effect.AddComponent(new UnitMoveComponent(new Movement(spawnPosition, Vector3.Zero,
                Vector3.Zero, matchPlayer.TankQuaternion)));
            effect.AddComponent(new UnitGroupComponent(effect));

            return effect;
        }
    }
}
