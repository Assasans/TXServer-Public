using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.Core;

namespace TXServer.ECSSystem.Events.Battle
{
	[SerialVersionUID(1496750075382L)]
	public class CreateCustomBattleLobbyEvent : ECSEvent
	{
		public void Execute(Player player, Entity mode)
		{
			ServerConnection.BattlePool.Add(new TXServer.Core.Battles.Battle(battleMode:Params.BattleMode, mapID:Params.MapId, maxPlayers:Params.MaxPlayers, 
				timeLimit: Params.TimeLimit, scoreLimit: Params.ScoreLimit, friendlyFire:Params.FriendlyFire, gravity:Params.Gravity, killZoneEnabled:Params.KillZoneEnabled, 
				disabledModules:Params.DisabledModules, isMatchMaking:false, owner:player));
			ServerConnection.BattlePool[ServerConnection.BattlePool.Count - 1].AddPlayer(player);
		}

		public ClientBattleParams Params { get; set; }
	}
}