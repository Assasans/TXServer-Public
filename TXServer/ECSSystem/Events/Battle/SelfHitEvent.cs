﻿using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(8814758840778124785L)]
    public class SelfHitEvent : HitEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon) {
            SelfEvent.Execute(this, player, weapon);

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            foreach (HitTarget hitTarget in Targets)
            {
                BattlePlayer hitPlayer = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

                if (battle.Params.BattleMode != BattleMode.DM)
                {
                    if (hitPlayer.Player.User.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor)
                    {
                        if (weapon.TemplateAccessor.Template.GetType() == typeof(IsisBattleItemTemplate))
                        {
                            hitPlayer.MatchPlayer.IsisHeal();
                            return;
                        }
                        if (!battle.Params.FriendlyFire)
                            return;
                    } 
                }

                hitPlayer.MatchPlayer.Tank.ChangeComponent<TemperatureComponent>(component =>
                {
                    component.Temperature += weapon.TemplateAccessor.Template switch
                    {
                        FlamethrowerBattleItemTemplate => 2,
                        FreezeBattleItemTemplate => -2,
                        _ => 0
                    };
                });

                hitPlayer.MatchPlayer.Tank.ChangeComponent<HealthComponent>(component =>
                {
                    int damage = 900;
                    if (component.CurrentHealth >= 0)
                        component.CurrentHealth -= damage;

                    if (component.CurrentHealth <= 0)
                    {
                        hitPlayer.MatchPlayer.TankState = TankState.Dead;

                        battle.MatchPlayers.Select(x => x.Player).SendEvent(new KillEvent(player.CurrentPreset.Weapon, hitTarget.Entity), player.BattlePlayer.MatchPlayer.BattleUser);
                        battle.UpdateUserStatistics(player, 10, 1, 0, 0);
                        battle.UpdateUserStatistics(hitPlayer.Player, 0, 0, 0, 1);

                        if (battle.ModeHandler is TDMHandler)
                            battle.UpdateScore(player.BattlePlayer.Team, 1);
                        hitPlayer.MatchPlayer.UserResult.Deaths += 1;
                        // todo: why the hell is KillStrike = Kills
                        player.BattlePlayer.MatchPlayer.UserResult.KillStrike += 1;
                        player.BattlePlayer.MatchPlayer.UserResult.Damage += damage;
                    }
                });
                battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), hitPlayer.MatchPlayer.Tank);
            }
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}