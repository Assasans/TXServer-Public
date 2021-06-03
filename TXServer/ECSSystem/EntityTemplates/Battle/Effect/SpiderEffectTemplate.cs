using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.Mine;
using TXServer.ECSSystem.Components.Battle.Effect.SpiderMine;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1485337553359L)]
    public class SpiderEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/spidermine";

        public static Entity CreateEntity(MatchPlayer matchPlayer) {
            Entity effect = CreateEntity(new SpiderEffectTemplate(), _configPath, matchPlayer, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());

            effect.AddComponent(new EffectActiveComponent());
            effect.AddComponent(new MineConfigComponent());
            effect.AddComponent(new SpiderMineConfigComponent());

            effect.AddComponent(matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>());
            effect.AddComponent(new SplashWeaponComponent(40f, 0f, 15f));

            effect.AddComponent(new UnitComponent());
            effect.AddComponent(new UnitMoveComponent(new Movement(matchPlayer.TankPosition, Vector3.Zero, Vector3.Zero, new Quaternion())));

            return effect;
        }
    }
}
