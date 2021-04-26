using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-4141404049750078994)]
    public class DMTemplate : BattleTemplate
    {
        public static Entity CreateEntity(Entity battleLobby, int scoreLimit, int timeLimit, int warmingUpTimeLimit)
        {
            Entity entity = CreateEntity(battleLobby, new DMTemplate(), "dm", scoreLimit, timeLimit, warmingUpTimeLimit);
            entity.Components.Add(new DMComponent());

            return entity;
        }
    }
}