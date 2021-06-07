using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates.Battle.Effect
{
    [SerialVersionUID(1486978694968L)]
    public class AcceleratedGearsEffectTemplate : EffectBaseTemplate
    {
        public static Entity CreateEntity(MatchPlayer matchPlayer) =>
            CreateEntity(new AcceleratedGearsEffectTemplate(), "battle/effect/acceleratedgears", matchPlayer);
    }
}
