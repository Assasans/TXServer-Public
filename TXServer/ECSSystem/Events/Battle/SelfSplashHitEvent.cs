using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent, ISelfEvent
	{
        public void Execute(Player player, Entity weapon)
		{
            SelfEvent.Execute(this, player, weapon);

            Entity weaponMarketItem = Damage.WeaponToModuleMarketItem(weapon, player) ?? player.CurrentPreset.Weapon;
            BattleTankPlayer battlePlayer = player.BattlePlayer;
            Core.Battles.Battle battle = player.BattlePlayer.Battle;

            if (!Damage.IsModule(weaponMarketItem) && player.BattlePlayer.MatchPlayer.TankState == TankState.Dead)
                return;

			foreach (HitTarget hitTarget in Targets)
			{
				BattleTankPlayer victim = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

				if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
				    victim.Team == player.BattlePlayer.Team && !battle.Params.FriendlyFire)
					return;

				Damage.DealNormalDamage(weapon, weaponMarketItem, victim.MatchPlayer, battlePlayer.MatchPlayer, hitTarget);
			}

            foreach (HitTarget splashTarget in SplashTargets)
			{
                BattleTankPlayer hitPlayer = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == splashTarget.IncarnationEntity);

                if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
                    hitPlayer.Team.EntityId == player.BattlePlayer.Team.EntityId && hitPlayer != player.BattlePlayer &&
                    !battle.Params.FriendlyFire)
					continue;

                if (weapon.TemplateAccessor.Template.GetType() == typeof(SpiderEffectTemplate))
                {
                    SpiderMineModule spiderMineModule =
                        player.BattlePlayer.MatchPlayer.Modules.Single(m => m.GetType() == typeof(SpiderMineModule)) as
                            SpiderMineModule;
                    spiderMineModule?.Explode();
                }

                Damage.DealSplashDamage(weapon, weaponMarketItem, hitPlayer.MatchPlayer, battlePlayer.MatchPlayer, splashTarget);
			}
		}

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}
