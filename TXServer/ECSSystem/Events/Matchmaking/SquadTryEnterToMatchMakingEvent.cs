using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.MatchMaking
{
    [SerialVersionUID(1510144894187L)]
    public class SquadTryEnterToMatchMakingEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            foreach (SquadPlayer squadPlayer in player.SquadPlayer.Squad.Participants)
                squadPlayer.SendEvent(new EnteredToMatchMakingEvent(), MatchmakingModes.GlobalItems.Rating);

            Core.Battles.MatchMaking.FindSquadBattle(player.SquadPlayer.Squad);
        }
        
        public long MatchMakingModeId { get; set; }
        public bool RatingMatchMakingMode { get; set; }
    }
}