using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public class TDMHandler : TeamBattleHandler
        {
            public override TeamColor LosingTeam => LastLosingTeam = (BlueTeamEntity.GetComponent<TeamScoreComponent>().Score - RedTeamEntity.GetComponent<TeamScoreComponent>().Score) switch
            {
                > 30 => TeamColor.RED,
                < -30 => TeamColor.BLUE,
                < DifferenceToRestoreBalance and > -DifferenceToRestoreBalance => TeamColor.NONE,
                _ => LastLosingTeam
            };

            private const int DifferenceToRestoreBalance = 26;

            private TeamColor LastLosingTeam;
        }
    }
}
