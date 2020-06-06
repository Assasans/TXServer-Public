using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1496982580733L)]
    public class MatchMakingModeTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new MatchMakingModeTemplate(), "battleselect/matchmaking/mode/" + configPathEntry),
                new MatchMakingRatingModeComponent());
        }
    }
}
