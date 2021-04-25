using System.Collections.Generic;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public class BattleView
    {
        public TeamColor AllyTeamColor { get; init; }
        public TeamColor EnemyTeamColor { get; init; }

        public Entity AllyTeamEntity { get; init; }
        public Entity EnemyTeamEntity { get; init; }

        public List<BattlePlayer> AllyTeamPlayers { get; init; }
        public List<BattlePlayer> EnemyTeamPlayers { get; init; }

        public IEnumerable<UserResult> AllyTeamResults { get; init; }
        public IEnumerable<UserResult> EnemyTeamResults { get; init; }

        public Flag AllyTeamFlag { get; set; }
        public Flag EnemyTeamFlag { get; set; }

        public IList<SpawnPoint> SpawnPoints { get; init; }
    }
}
