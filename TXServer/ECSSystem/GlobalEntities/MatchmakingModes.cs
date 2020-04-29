using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class MatchmakingModes
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Rating { get; } = new Entity(-2076021809, new TemplateAccessor(new MatchMakingModeTemplate(), "battleselect/matchmaking/mode/rating"),
                new MatchMakingRatingModeComponent());
        }
    }
}
