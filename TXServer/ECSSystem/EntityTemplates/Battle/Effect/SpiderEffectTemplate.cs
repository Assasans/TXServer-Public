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
            float beginHideDistance, float hideRange, float impact, float speed)
        {
            Entity effect = CreateEntity(new SpiderEffectTemplate(), "/battle/effect/spidermine", matchPlayer, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());

            effect.AddComponent(new EffectActiveComponent());
            effect.AddComponent(new MineConfigComponent(activationTime: activationTime,
                beginHideDistance: beginHideDistance, hideRange: hideRange, impact: impact));
            effect.AddComponent(new SpiderMineConfigComponent(acceleration:acceleration, speed:speed));

            effect.AddComponent(matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>());
            effect.AddComponent(new SplashWeaponComponent(40f, 0f, 15f));

            effect.AddComponent(new UnitComponent());
            effect.AddComponent(new UnitMoveComponent(new Movement(matchPlayer.TankPosition, Vector3.Zero, Vector3.Zero,
                matchPlayer.TankQuaternion)));

            return effect;
        }
    }
}
