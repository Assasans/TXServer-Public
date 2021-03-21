using System.Collections.Generic;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public interface IBattleModeHandler
        {
            Battle Battle { get; init; }

            void SetupBattle();
            void Tick();

            IEnumerable<BattlePlayer> Players { get; }
            bool IsEnoughPlayers { get; }
            TeamColor LosingTeam { get; }
            int EnemyCountFor(BattlePlayer battlePlayer);
            TeamBattleResult TeamBattleResultFor(BattlePlayer battlePlayer);

            void SetupBattle(IBattleModeHandler prevHandler);
            void CompleteWarmUp();

            BattlePlayer AddPlayer(Player player);
            void RemovePlayer(BattlePlayer battlePlayer);
            void OnMatchJoin(BattlePlayer battlePlayer);
            void OnMatchLeave(BattlePlayer battlePlayer);
            void SortRoundUsers();
        }
    }
}
