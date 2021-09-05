using System.Collections.Generic;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.ServerMapInformation
{
    public class MapSpawnPointInfo
    {
        public IList<SpawnPoint> Deathmatch { get; set; }
        public TeamSpawnPointList TeamDeathmatch { get; set; }
        public TeamSpawnPointList CaptureTheFlag { get; set; }


        public TeamSpawnPointList GetAnyTeamSpawnPoints()
        {
            if (TeamDeathmatch is not null) return TeamDeathmatch;
            if (CaptureTheFlag is not null) return CaptureTheFlag;

            TeamSpawnPointList teamSpawnPointList = new TeamSpawnPointList();
            teamSpawnPointList.BlueTeam.Add(new SpawnPoint());
            teamSpawnPointList.RedTeam.Add(new SpawnPoint());

            return teamSpawnPointList;
        }

        public BattleMode GetAvailableBattleMode()
        {
            if (CaptureTheFlag != null)
                return BattleMode.CTF;
            return TeamDeathmatch != null ? BattleMode.TDM : BattleMode.DM;
        }

        public TeamSpawnPointList GetTeamSpawnPoints(BattleMode battleMode) =>
            battleMode switch
            {
                BattleMode.CTF => CaptureTheFlag,
                BattleMode.TDM or _ => TeamDeathmatch
            };

        public bool IsBattleModeAvailable(BattleMode battleMode) =>
            battleMode switch
            {
                BattleMode.CTF when CaptureTheFlag == null => false,
                BattleMode.DM when Deathmatch == null => false,
                BattleMode.TDM when TeamDeathmatch == null => false,
                _ => true
            };
    }
}
