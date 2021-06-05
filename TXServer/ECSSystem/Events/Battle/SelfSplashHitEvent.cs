using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent, ISelfEvent
	{
		public void Execute(Player player, Entity weapon)
		{
            if (player.BattlePlayer.MatchPlayer.TankState == TankState.Dead)
                return;

            BattleTankPlayer battlePlayer = player.BattlePlayer;
            Entity weaponMarketItem = player.CurrentPreset.Weapon;

            Core.Battles.Battle battle = player.BattlePlayer.Battle;

			foreach (HitTarget hitTarget in Targets)
			{
				BattleTankPlayer victim = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

				if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
				    victim.Team == player.BattlePlayer.Team && !battle.Params.FriendlyFire)
					return;

				Damage.DealDamage(weaponMarketItem, victim.MatchPlayer, battlePlayer.MatchPlayer, hitTarget, 900);
			}

            foreach (HitTarget splashTarget in SplashTargets)
			{
                BattleTankPlayer hitPlayer = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == splashTarget.IncarnationEntity);

                if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
                    hitPlayer.Team.EntityId == player.BattlePlayer.Team.EntityId && hitPlayer != player.BattlePlayer &&
                    !battle.Params.FriendlyFire)
					continue;

                // TODO: proper damage
                float damage = 500;

                if (weapon.TemplateAccessor.Template.GetType() == typeof(SpiderEffectTemplate))
                {
                    SpiderMineModule spiderMineModule =
                        player.BattlePlayer.MatchPlayer.Modules.Single(m => m.GetType() == typeof(SpiderMineModule)) as
                            SpiderMineModule;
                    spiderMineModule?.Explode();
                    damage = SpiderMineModule.Damage;
                    weaponMarketItem = Modules.GlobalItems.Spidermine;
                }

                Damage.DealDamage(weaponMarketItem, hitPlayer.MatchPlayer, battlePlayer.MatchPlayer, splashTarget, damage);
			}
		}

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}
