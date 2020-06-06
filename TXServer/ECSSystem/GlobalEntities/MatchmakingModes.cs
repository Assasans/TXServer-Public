using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class MatchmakingModes : ItemList
    {
        public static MatchmakingModes GlobalItems { get; } = new MatchmakingModes();

        public Entity Rating { get; } = MatchMakingModeTemplate.CreateEntity(-2076021809, "rating");
    }
}
