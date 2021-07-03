using System;
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
            // (I don't think chis check is really necessary)
            bool modeValid = false;
            foreach (Entity existingMode in MatchmakingModes.GlobalItems.GetAllItems())
            {
                if (mode.EntityId == existingMode.EntityId)
                {
                    modeValid = true;
                    break;
                }
            }
            if (!modeValid) throw new ArgumentException($"Invalid mode entity: {mode.TemplateAccessor.Template.GetType().Name} ({mode.TemplateAccessor.ConfigPath})");

            player.SendEvent(new EnteredToMatchMakingEvent(), mode);

            Core.Battles.Matchmaking.MatchMaking.FindBattle(player);
        }
    }
}