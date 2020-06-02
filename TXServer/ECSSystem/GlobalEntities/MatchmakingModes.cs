using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class MatchmakingModes : ItemList
    {
        public static MatchmakingModes GlobalItems { get; } = new MatchmakingModes();

        public Entity Rating { get; } = new Entity(-2076021809, new TemplateAccessor(new MatchMakingModeTemplate(), "battleselect/matchmaking/mode/rating"),
            new MatchMakingRatingModeComponent());
    }
}
