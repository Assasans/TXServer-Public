using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1542694831168L)]
    public class FireRingEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/firering";

        public static Entity CreateEntity(MatchPlayer matchPlayer)
        {
            Entity effect = CreateEntity(new FireRingEffectTemplate(), _configPath, matchPlayer, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());
            effect.AddComponent(new FireRingEffectComponent());

            effect.AddComponent(matchPlayer.Battle.BattleEntity.GetComponent<BattleGroupComponent>());

            effect.AddComponent(new SplashEffectComponent(true));
            effect.AddComponent(new SplashWeaponComponent(40f, 0f, 15f));

            return effect;
        }
    }
}
