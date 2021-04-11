using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-2043703779834243389)]
    public class BattleUserTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(Player player, Entity battle, Entity team)
        {
            Entity entity = new Entity(new TemplateAccessor(new BattleUserTemplate(), "battle/battleuser"),
                    new UserGroupComponent(player.User),
                    new BattleGroupComponent(battle),
                    new BattleUserComponent(),
                    new IdleCounterComponent(10000L)); //todo this is the kick time after becoming idle
            
            if (player.BattlePlayer.IsSpectator)
                entity.Components.Add(new UserInBattleAsSpectatorComponent(battle.EntityId));
            else
                entity.Components.Add(new UserInBattleAsTankComponent());
            
            if (team != null)
                entity.Components.Add(team.GetComponent<TeamGroupComponent>());
            
            return entity;
        }
    }
}