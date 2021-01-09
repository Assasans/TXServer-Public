namespace TXServer.ECSSystem.Types
{
	public class ClientBattleParams
	{
        public ClientBattleParams(BattleMode BattleMode, long MapId, int MaxPlayers, int TimeLimit, int ScoreLimit, bool FriendlyFire, GravityType Gravity, bool KillZoneEnabled, bool DisabledModules)
        {
            this.BattleMode = BattleMode;
            this.MapId = MapId;
            this.MaxPlayers = MaxPlayers;
            this.TimeLimit = TimeLimit;
            this.ScoreLimit = ScoreLimit;
            this.FriendlyFire = FriendlyFire;
            this.Gravity = Gravity;
            this.KillZoneEnabled = KillZoneEnabled;
            this.DisabledModules = DisabledModules;
        }

        public BattleMode BattleMode { get; set; }
		public long MapId { get; set; }
		public int MaxPlayers { get; set; }
		public int TimeLimit { get; set; }
		public int ScoreLimit { get; set; }
		public bool FriendlyFire { get; set; }
		public GravityType Gravity { get; set; }
		public bool KillZoneEnabled { get; set; }
		public bool DisabledModules { get; set; }
	}
}
