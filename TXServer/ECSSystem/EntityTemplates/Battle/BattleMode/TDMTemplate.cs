using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(8215935014037697786L)]
    public class TDMTemplate : TeamBattleTemplate
    {
        public static Entity CreateEntity(Entity battleLobby, int scoreLimit, int timeLimit, int warmingUpTimeLimit)
        {
            Entity entity = CreateEntity(battleLobby, new TDMTemplate(), "tdm", scoreLimit, timeLimit, warmingUpTimeLimit);
            entity.Components.Add(new TDMComponent());
            entity.Components.Add(new BattleScoreComponent(0, 0, 0));

            return entity;
        }
    }
}
