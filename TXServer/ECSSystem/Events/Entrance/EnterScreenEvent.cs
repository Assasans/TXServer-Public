using Serilog;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        private static readonly ILogger Logger = Log.Logger.ForType<EnterScreenEvent>();

        public void Execute(Player player, Entity entity)
        {
            Logger.WithPlayer(player).Debug("Entering screen {Screen}", Screen);

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
