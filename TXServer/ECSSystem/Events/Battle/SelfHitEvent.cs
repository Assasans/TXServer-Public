using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
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

                TemperatureComponent temperatureComponent = hitPlayer.MatchPlayer.Tank.GetComponent<TemperatureComponent>();
                switch (weapon.TemplateAccessor.Template) {
                    case FlamethrowerBattleItemTemplate:
                        temperatureComponent.Temperature += 2;
                        hitPlayer.MatchPlayer.Tank.ChangeComponent(temperatureComponent);
                        break;
                    case FreezeBattleItemTemplate:
                        temperatureComponent.Temperature -= 2;
                        hitPlayer.MatchPlayer.Tank.ChangeComponent(temperatureComponent);
                        break;
                    case IsisBattleItemTemplate:
                        break;
                }

                HealthComponent healthComponent = hitPlayer.MatchPlayer.Tank.GetComponent<HealthComponent>();
                if (healthComponent.CurrentHealth > 0)
                    healthComponent.CurrentHealth -= 400;
                hitPlayer.MatchPlayer.Tank.ChangeComponent(healthComponent);
                battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), hitPlayer.MatchPlayer.Tank);
            }
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}