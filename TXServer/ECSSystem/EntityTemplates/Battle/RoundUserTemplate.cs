using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(140335313420508312)]
    public class RoundUserTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(BattleLobbyPlayer battlePlayer, Entity battle, Entity tank)
        {
            return new Entity(new TemplateAccessor(new RoundUserTemplate(), "battle/round/rounduser"),
                new RoundUserStatisticsComponent(place:1, scoreWithoutBonuses:0, kills:0, killAssists:0, deaths:0),
                new RoundUserComponent(),
                new UserGroupComponent(battlePlayer.User),
                new TeamGroupComponent(battlePlayer.Team),
                new BattleGroupComponent(battle),
                new TankGroupComponent(tank));
        }
    }
}