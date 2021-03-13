using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Types
{
	public class PersonalBattleResultForClient
	{
        public PersonalBattleResultForClient(Player player, TeamBattleResult teamBattleResult)
        {
			UserTeamColor = player.User.GetComponent<TeamColorComponent>().TeamColor;
			TeamBattleResult = teamBattleResult;
			League = Leagues.GlobalItems.Silver;
			PrevLeague = Leagues.GlobalItems.Silver;
			Reward = BattleRewards.GlobalItems.XCrystalBonus;
		}

        public TeamColor UserTeamColor { get; set; }
		public TeamBattleResult TeamBattleResult { get; set; }
		public int Energy { get; set; } = 0;
		public int EnergyDelta { get; set; } = 0;
		public int CrystalsForExtraEnergy { get; set; } = 0;
		[OptionalMapped]
		public EnergySource MaxEnergySource { get; set; } = EnergySource.BONUS;
		public int CurrentBattleSeries { get; set; } = 0;
		public int MaxBattleSeries { get; set; } = 0;
		public float ScoreBattleSeriesMultiplier { get; set; } = 0;
		public int RankExp { get; set; } = 0;
		public int RankExpDelta { get; set; } = 0;
		public int WeaponExp { get; set; } = 0;
		public int TankLevel { get; set; } = 0;
		public int WeaponLevel { get; set; } = 0;
		public int WeaponInitExp { get; set; } = 0;
		public int WeaponFinalExp { get; set; } = 0;
		public int TankExp { get; set; } = 0;
		public int TankInitExp { get; set; } = 0;
		public int TankFinalExp { get; set; } = 0;
		public int ItemsExpDelta { get; set; } = 0;
		public int ContainerScore { get; set; } = 0;
		public int ContainerScoreDelta { get; set; } = 0;
		public int ContainerScoreLimit { get; set; } = 0;
		public Entity Container { get; set; } = Containers.GlobalItems.Xt_zeus;
		public float ContainerScoreMultiplier { get; set; } = 0;
		public double Reputation { get; set; } = 0;
		public double ReputationDelta { get; set; } = 0;
		public Entity League { get; set; }
		public Entity PrevLeague { get; set; }
		public int LeaguePlace { get; set; } = 1;
		[OptionalMapped]
		public Entity Reward { get; set; }
	}
}