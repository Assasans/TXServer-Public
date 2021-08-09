using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hit
{
    [SerialVersionUID(196833391289212110L)]
	public class SelfSplashHitEvent : SelfHitEvent, ISelfEvent
	{
        public void Execute(Player player, Entity weapon)
		{
            SelfEvent.Execute(this, player, weapon);

            Core.Battles.Battle battle = player.BattlePlayer.Battle;

            foreach (HitTarget hitTarget in Targets)
                Damage.HandleHit(weapon, player.BattlePlayer.MatchPlayer, hitTarget);

            foreach (HitTarget splashTarget in SplashTargets)
			{
                if (weapon.TemplateAccessor.Template.GetType() == typeof(SpiderEffectTemplate))
                {
                    SpiderMineModule spiderMineModule = player.BattlePlayer.MatchPlayer.Modules.Single(m
                        => m.GetType() == typeof(SpiderMineModule)) as SpiderMineModule;
                    spiderMineModule?.Explode();
                }

                Damage.HandleHit(weapon, player.BattlePlayer.MatchPlayer, splashTarget, true);
			}

            player.User.ChangeComponent<UserStatisticsComponent>(component => component.Statistics["SHOTS"]++);
            player.User.ChangeComponent<UserStatisticsComponent>(component => component.Statistics["HITS"] += Targets.Count + SplashTargets.Count);
        }

		public override IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteSplashHitEvent>();

        [OptionalMapped] public List<HitTarget> SplashTargets { get; set; }
	}
}
