using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    public abstract class TeamBattleTemplate : BattleTemplate
    {
        protected static new Entity CreateEntity(Entity battleLobby, BattleTemplate template, string modeName, int scoreLimit, int timeLimit, int warmingUpTimeLimit)
        {
            Entity entity = BattleTemplate.CreateEntity(battleLobby, template, modeName, scoreLimit, timeLimit, warmingUpTimeLimit);
            entity.Components.Add(new TeamBattleComponent());

            return entity;
        }
    }
}