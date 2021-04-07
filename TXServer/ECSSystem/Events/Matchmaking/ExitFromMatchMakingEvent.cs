using TXServer.Core;
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
            if (!player.IsInBattle)
                return;

            player.BattlePlayer.WaitingForExit = true;

            if (lobby.TemplateAccessor.Template is MatchMakingLobbyTemplate)
                player.SendEvent(new ExitedFromMatchMakingEvent(true), lobby);
        }
    }
}