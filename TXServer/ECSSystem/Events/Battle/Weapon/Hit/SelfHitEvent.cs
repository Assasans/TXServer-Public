using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle.Weapon.Hit
{
    [SerialVersionUID(8814758840778124785L)]
    public class SelfHitEvent : HitEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon)
        {
            SelfEvent.Execute(this, player, weapon);

            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;

            if (matchPlayer.TankState == TankState.Dead)
                return;

            if (weapon.TemplateAccessor.Template.GetType() == typeof(HammerBattleItemTemplate))
            {
                ((Core.Battles.BattleWeapons.Hammer) matchPlayer.BattleWeapon).ProcessHits(Targets);
                return;
            }

            foreach (HitTarget hitTarget in Targets)
            {
                if (matchPlayer.IsEnemyOf(Damage.GetTargetByHit(matchPlayer, hitTarget)) ||
                    matchPlayer.Battle.Params.FriendlyFire && !matchPlayer.BattleWeapon.NotFriendlyFireUsable)
                    Damage.HandleHit(weapon, matchPlayer, hitTarget);
                else
                    Damage.HandleMateHit(weapon, matchPlayer, hitTarget);
            }

            player.Data.Statistics.Shots++;
            player.Data.Statistics.Hits += Targets.Count;
            player.User.ChangeComponent<UserStatisticsComponent>();

            (matchPlayer.BattleWeapon as Core.Battles.BattleWeapons.Shaft)?.ResetAiming();
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}
