using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle;

namespace TXServer.Core.Battles
{
    public class Spectator : BaseBattlePlayer
    {
        public Spectator(Battle battle, Player player)
        {
            Battle = battle;
            Player = player;
            User = player.User;

            BattleUser = BattleUserTemplate.CreateSpectatorEntity(Player, battle.BattleEntity);
        }

        public Entity BattleUser { get; }
    }
}
