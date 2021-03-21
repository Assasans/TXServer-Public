using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle.Time;
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

            CommandManager.SendCommands(player,
                new SendEventCommand(new EnteredToMatchMakingEvent(), mode));

            Core.Battles.Battle battle = ServerConnection.BattlePool.OrderBy(b => b.AllBattlePlayers).LastOrDefault(b => isValidToEnter(b));
            if (battle == null)
            {
                battle = new Core.Battles.Battle(battleParams: null, isMatchMaking: true, owner: null);
                ServerConnection.BattlePool.Add(battle);
            }
            battle.AddPlayer(player);
        }

        private static bool isValidToEnter(Core.Battles.Battle battle)
        {
            if (!battle.IsMatchMaking || battle.AllBattlePlayers.Count() == battle.Params.MaxPlayers || battle.BattleState == BattleState.Ended)
                return false;
            else if (battle.BattleState == BattleState.Running)
            {
                if (battle.CountdownTimer >= battle.BattleEntity.GetComponent<TimeLimitComponent>().TimeLimitSec - 180)
                    return true;
                else
                    return false;
            }
            return true;
        }
    }
}