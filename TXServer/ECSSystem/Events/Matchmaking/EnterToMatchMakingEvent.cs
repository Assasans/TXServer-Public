using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.MatchMaking
{
    [SerialVersionUID(1494937115182)]
    public class EnterToMatchMakingEvent : ECSEvent
    {
        public void Execute(Player player, Entity mode)
        {
            // Validate given mode entity
            foreach (Entity existingMode in MatchmakingModes.GlobalItems.GetAllItems())
            {
                if (mode.EntityId == existingMode.EntityId)
                {
                    player.SendEvent(new EnteredToMatchMakingEvent(), mode);
                    player.SendEvent(new EnteredToMatchMakingEvent(), mode);
                    Core.Battles.Matchmaking.MatchMaking.EnterMatchMaking(player);
                    return;
                }
            }
        }
    }
}
