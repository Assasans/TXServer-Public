using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636222333880646188L)]
    public class SonarEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/sonar";

        public static Entity CreateEntity(MatchPlayer matchPlayer, float duration)
        {
            Entity effect = CreateEntity(new SonarEffectTemplate(), _configPath, matchPlayer, (long)duration, addTeam:true);
            effect.AddComponent(matchPlayer.Player.User.GetComponent<UserGroupComponent>());

            return effect;
        }
    }
}
