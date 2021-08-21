
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Logger.Debug($"{player}: Entering screen {Screen}");

            switch (Screen)
            {
                case "MainScreen":
                    if (player.BattlePlayer?.Rejoin == true)
                    {
                        player.BattlePlayer.Battle.InitMatchPlayer(player.BattlePlayer);
                        player.BattlePlayer.Rejoin = false;
                        player.BattlePlayer.Battle.KeepRunning = false;
                        return;
                    }

                    player.CheckNotifications();

                    break;
            }
        }

        public string Screen { get; set; }
    }
}
