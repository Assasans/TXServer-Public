using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Events.Battle.Weapon.Shot;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hammer
{
    [SerialVersionUID(-1937089974629265090L)]
	public class SelfHammerShotEvent : SelfShotEvent
	{
        public static void Execute(Player player, Entity tank)
        {
            new SelfShotEvent().Execute(player, tank);
            ((Core.Battles.BattleWeapons.Hammer) player.BattlePlayer.MatchPlayer.BattleWeapon).ProcessShot();
        }

        public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHammerShotEvent>();
		public int RandomSeed { get; set; }
	}
}
