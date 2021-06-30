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

            BattleTankPlayer battlePlayer = player.BattlePlayer;
            Core.Battles.Battle battle = player.BattlePlayer.Battle;

            //if (!Damage.IsModule(weaponMarketItem) && player.BattlePlayer.MatchPlayer.TankState == TankState.Dead)
                //return;

			foreach (HitTarget hitTarget in Targets)
			{
				MatchPlayer victim = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity).MatchPlayer;

				//if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
				    //victim.Team == player.BattlePlayer.Team && !battle.Params.FriendlyFire)
					//return;

				Damage.HandleHit(weapon, victim, player.BattlePlayer.MatchPlayer, hitTarget);
			}

            foreach (HitTarget splashTarget in SplashTargets)
			{
                MatchPlayer target = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == splashTarget.IncarnationEntity).MatchPlayer;

                //if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.DM &&
                    //hitPlayer.Team.EntityId == player.BattlePlayer.Team.EntityId && hitPlayer != player.BattlePlayer &&
                    //!battle.Params.FriendlyFire)
					//continue;

                if (weapon.TemplateAccessor.Template.GetType() == typeof(SpiderEffectTemplate))
                {
                    SpiderMineModule spiderMineModule =
                        player.BattlePlayer.MatchPlayer.Modules.Single(m => m.GetType() == typeof(SpiderMineModule)) as
                            SpiderMineModule;
                    spiderMineModule?.Explode();
                }

                Damage.HandleHit(weapon, target, player.BattlePlayer.MatchPlayer, splashTarget);
			}
		}

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped]
		public List<HitTarget> SplashTargets { get; set; }
	}
}
