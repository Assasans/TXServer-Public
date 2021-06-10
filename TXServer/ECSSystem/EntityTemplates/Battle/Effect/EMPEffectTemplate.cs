using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Effect;
using TXServer.ECSSystem.Components.Battle.Effect.EMP;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(636250001674528714L)]
    public class EMPEffectTemplate : EffectBaseTemplate
    {
        private static readonly string _configPath = "/battle/effect/emp";

        public static Entity CreateEntity(MatchPlayer matchPlayer, float radius)
        {
            Entity effect = CreateEntity(new EMPEffectTemplate(), _configPath, matchPlayer, addTeam: true);

            effect.AddComponent(new EMPEffectComponent(radius));

            return effect;
        }
    }
}
