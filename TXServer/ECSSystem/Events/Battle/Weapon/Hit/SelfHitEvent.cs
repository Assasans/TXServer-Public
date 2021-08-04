using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hit
{
    [SerialVersionUID(8814758840778124785L)]
    public class SelfHitEvent : HitEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon)
        {
            SelfEvent.Execute(this, player, weapon);

            BattleTankPlayer battlePlayer = player.BattlePlayer;

            if (battlePlayer.MatchPlayer.TankState == TankState.Dead)
                return;

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            foreach (HitTarget hitTarget in Targets)
            {
                MatchPlayer victim = battle.MatchTankPlayers
                    .Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity).MatchPlayer;

                Damage.HandleHit(weapon, victim, player.BattlePlayer.MatchPlayer, hitTarget);
            }

            player.User.ChangeComponent<UserStatisticsComponent>(component => component.Statistics["SHOTS"]++);
            player.User.ChangeComponent<UserStatisticsComponent>(component =>
                component.Statistics["HITS"] += Targets.Count);

            battlePlayer.MatchPlayer.ShaftAimingBeginTime = null;
            battlePlayer.MatchPlayer.ShaftLastAimingDurationMs = null;
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}
