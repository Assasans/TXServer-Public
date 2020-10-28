using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.Events.Matchmaking
{
    [SerialVersionUID(1495176527022)]
    public class ExitFromMatchMakingEvent : ECSEvent
    {
        public bool InBattle { get; set; }

        public void Execute(Player player, Entity lobby)
        {
            if (typeof(MatchMakingLobbyTemplate) == lobby.TemplateAccessor.Template.GetType())
            {
                //todo make lobby system and remove it from there,
                // and make it work
                CommandManager.SendCommands(player, new Command[]
                {
                    new SendEventCommand(new ExitedFromMatchMakingEvent(true), lobby),
                    new EntityUnshareCommand(lobby)
                });
            }
        }
    }
}