using System;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Battles;
using TXServer.Core.Protocol;
using TXServer.Core.Configuration;
using TXServer.Library;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(8814758840778124785L)]
    public class SelfHitEvent : HitEvent, ISelfEvent
    {
        public void Execute(Player player, Entity weapon) {
            SelfEvent.Execute(this, player, weapon);
            var battlePlayer = player.BattlePlayer;

            if (battlePlayer.MatchPlayer.TankState == TankState.Dead)
                return;

            Core.Battles.Battle battle = player.BattlePlayer.Battle;
            foreach (HitTarget hitTarget in Targets)
            {
                BattleTankPlayer victim = battle.MatchTankPlayers.Single(p => p.MatchPlayer.Incarnation == hitTarget.IncarnationEntity);

                if (battle.Params.BattleMode != BattleMode.DM)
                {
                    if (victim.Player.User.GetComponent<TeamColorComponent>().TeamColor == player.User.GetComponent<TeamColorComponent>().TeamColor)
                    {
                        if (weapon.TemplateAccessor.Template.GetType() == typeof(IsisBattleItemTemplate))
                        {
                            Damage.IsisHeal(victim.MatchPlayer, battlePlayer.MatchPlayer, hitTarget);
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

                long weaponItemId = battlePlayer.MatchPlayer.Weapon.GetComponent<MarketItemGroupComponent>().Key;
                string path = GlobalEntities.Weapons.GlobalItems.GetAllItems()
                    .First((item) => item.EntityId == weaponItemId).TemplateAccessor.ConfigPath;

                var minDamage = Config.GetComponent<ServerComponents.Damage.MinDamagePropertyComponent>(path);
                var maxDamage = Config.GetComponent<ServerComponents.Damage.MaxDamagePropertyComponent>(path);

                int damage = (int)Math.Round(new Random().NextGaussianRange(minDamage.FinalValue, maxDamage.FinalValue));

                // ChatMessageReceivedEvent.SystemMessageTarget(
                //     $"[Damage] Min: {minDamage.FinalValue} | Max: {maxDamage.FinalValue} | Random: {damage}",
                //     battle.GeneralBattleChatEntity, player
                // );

                Damage.DealDamage(victim.MatchPlayer, battlePlayer.MatchPlayer, hitTarget, damage);
            }
        }

        public virtual IRemoteEvent ToRemoteEvent() => this.ToRemoteEvent<RemoteHitEvent>();
    }
}