using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;

namespace TXServer.ECSSystem.EntityTemplates.Battle
{
    [SerialVersionUID(-1911920453295891173)]
    public class CTFTemplate : TeamBattleTemplate
    {
        public static Entity CreateEntity(Entity battleLobby, int scoreLimit, int timeLimit, int warmingUpTimeLimit)
        {
            Entity entity = CreateEntity(battleLobby, new CTFTemplate(), "ctf", scoreLimit, timeLimit, warmingUpTimeLimit);
            entity.Components.Add(new CTFComponent());

            return entity;
        }
    }
}