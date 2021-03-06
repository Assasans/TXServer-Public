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

            if (matchPlayer.TankState == TankState.Dead && !matchPlayer.Weapon.IsBulletWeapon)
                return;

            if (weapon.TemplateAccessor.Template.GetType() == typeof(HammerBattleItemTemplate))
            {
                Targets.RemoveAll(h =>
                    !matchPlayer.IsEnemyOf(Damage.GetTargetByHit(matchPlayer, h)) &&
                    !matchPlayer.Battle.Params.FriendlyFire);

                ((Core.Battles.BattleWeapons.Hammer) matchPlayer.Weapon).ProcessHits(Targets);
                return;
            }

            foreach (HitTarget hitTarget in Targets)
            {
                if (matchPlayer.IsEnemyOf(Damage.GetTargetByHit(matchPlayer, hitTarget)) ||
                     matchPlayer.Battle.Params.FriendlyFire && !matchPlayer.Weapon.NotFriendlyFireUsable)
                    Damage.HandleHit(weapon, matchPlayer, hitTarget);
                else
                    Damage.HandleMateHit(weapon, matchPlayer, hitTarget);
            }

            player.User.ChangeComponent<UserStatisticsComponent>(component => component.Statistics["SHOTS"]++);
            player.User.ChangeComponent<UserStatisticsComponent>(component =>
                component.Statistics["HITS"] += Targets.Count);

            (matchPlayer.Weapon as Core.Battles.BattleWeapons.Shaft)?.ResetAiming();
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}
