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
        private static readonly string _configPath = "/battle/effect/mine";

        public static Entity CreateEntity(MatchPlayer matchPlayer, long activationTime, float beginHideDistance, float hideRange, float impact) {
            Entity effect = CreateEntity(new MineEffectTemplate(), _configPath, matchPlayer, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());

            effect.AddComponent(new EffectActiveComponent());
            effect.AddComponent(new MineConfigComponent(activationTime: activationTime,
                beginHideDistance: beginHideDistance, hideRange: hideRange, impact: impact));
            effect.AddComponent(new MinePositionComponent(matchPlayer.TankPosition));
            effect.AddComponent(new MineEffectTriggeringAreaComponent());

            effect.AddComponent(matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>());
            effect.AddComponent(new SplashWeaponComponent(40f, 0f, 15f));
            return effect;
        }
    }
}
