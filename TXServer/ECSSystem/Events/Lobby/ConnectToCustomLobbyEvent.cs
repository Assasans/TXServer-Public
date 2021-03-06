using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Lobby
{
    [SerialVersionUID(1547616531111L)]
    public class ConnectToCustomLobbyEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            Core.Battles.Battle battle = Server.Instance.FindBattleById(LobbyId, 0);

            // admins can enter with the "last" code the newest custom lobby
            if (player.Data.Admin || player.Data.Beta)
            {
                switch (LobbyId)
                {
                    // soft join latest open (custom) lobby
                    case 1211920:
                        battle = ServerConnection.BattlePool.LastOrDefault(b => ((b.TypeHandler as CustomBattleHandler)?.IsOpen).GetValueOrDefault());
                        break;
                    // brute join latest (custom) lobby
                    case 21211920:
                        battle = ServerConnection.BattlePool.LastOrDefault(b => !b.IsMatchMaking);
                        break;
                    // brute join specific lobby at index
                    default:
                        if (LobbyId < ServerConnection.BattlePool.Count && LobbyId >= 0)
                            battle = ServerConnection.BattlePool[(int)LobbyId];
                        break;
                }
            }

            if (battle != null)
            {
                if (battle.JoinedTankPlayers.Count() >= battle.Params.MaxPlayers)
                    player.SendEvent(new EnterBattleLobbyFailedEvent(false, true), player.User);
                else if (player.IsInBattle && player.BattlePlayer.Battle == battle)
                    player.SendEvent(new EnterBattleLobbyFailedEvent(true, false), player.User);
                else
                    battle.AddPlayer(player, false);
            }
            else
                player.SendEvent(new CustomLobbyNotExistsEvent(), player.User);
        }
        public long LobbyId { get; set; }
    }
}
