using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(8814758840778124785L)]
    public class SelfHitEvent : HitEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon) {
            SelfEvent.Execute(this, player, weapon);

            if (player.BattlePlayer.MatchPlayer.TankState == TankState.Dead)
                return;

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            foreach (HitTarget hitTarget in Targets)
            {
                BattlePlayer victim = battle.MatchPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

                if (battle.Params.BattleMode != BattleMode.DM)
                {
                    if (victim.Player.User.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor)
                    {
                        if (weapon.TemplateAccessor.Template.GetType() == typeof(IsisBattleItemTemplate))
                        {
                            Damage.IsisHeal(victim.MatchPlayer, player.BattlePlayer.MatchPlayer, hitTarget);
                            return;
                        }
                        if (!battle.Params.FriendlyFire)
                            return;
                    } 
                }

                victim.MatchPlayer.Tank.ChangeComponent<TemperatureComponent>(component =>
                {
                    component.Temperature += weapon.TemplateAccessor.Template switch
                    {
                        FlamethrowerBattleItemTemplate => 2,
                        FreezeBattleItemTemplate => -2,
                        _ => 0
                    };
                });

                Damage.DealDamage(victim.MatchPlayer, player.BattlePlayer.MatchPlayer, hitTarget, 900);
            }
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}