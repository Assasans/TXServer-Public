using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1502712502830L)]
    public class LeagueTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry, Entity chest, int index)
        {
            return new Entity(id, new TemplateAccessor(new LeagueTemplate(), "leagues/leagues/" + configPathEntry),
                new ChestBattleRewardComponent(chest),
                new LeagueGroupComponent(id),
                new CurrentSeasonRewardForClientComponent(),
                new LeagueConfigComponent(LeagueIndex: index));
        }
    }
}
