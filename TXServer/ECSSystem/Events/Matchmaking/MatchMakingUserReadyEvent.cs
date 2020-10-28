using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1496829083447)]
    public class MatchMakingUserReadyEvent : ECSEvent
    {
        public void Execute(Player player, Entity lobby)
        {
            //todo handle it in the lobby
            CommandManager.SendCommands(player, new ComponentAddCommand(player.User, new MatchMakingUserReadyComponent()));
        }
    }
}