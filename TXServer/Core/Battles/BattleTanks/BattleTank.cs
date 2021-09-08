using System.Linq;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.Events.Battle;

namespace TXServer.Core.Battles.BattleTanks
{
    public class BattleTank
    {
        public BattleTank(MatchPlayer matchPlayer)
        {
            MatchPlayer = matchPlayer;

            MaxHealth = Config.GetComponent<HealthComponent>(MatchPlayer.Player.CurrentPreset.
                HullItem.TemplateAccessor.ConfigPath).MaxHealth;
        }

        public float CurrentHealth
        {
            get => _currentHealth;
            set
            {
                _currentHealth = value;

                if (MatchPlayer.TryGetModule(out RepairKitModule repairKitModule) && repairKitModule.EffectIsActive &&
                    (repairKitModule.IsSupply || repairKitModule.IsCheat))
                {
                    // this way prevents the client from crashing when using the other way below while a repair kit
                    // supply is active
                    TankEntity.ChangeComponent<HealthComponent>(component => component.CurrentHealth = value);
                }
                else
                {
                    // this way fixes the sometimes not updating health bar
                    HealthComponent healthComponent = TankEntity.GetComponent<HealthComponent>();
                    TankEntity.RemoveComponent<HealthComponent>();
                    healthComponent.CurrentHealth = value;
                    TankEntity.AddComponent(healthComponent);
                }

                MatchPlayer.Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), TankEntity);

                if (MatchPlayer.TryGetModule(out AdrenalineModule adrenalineModule))
                    adrenalineModule.CheckActivationNecessity();
            }
        }
        public float MaxHealth
        {
            get => _maxHealth;
            private init
            {
                _maxHealth = value;

                if (TankEntity.HasComponent<HealthComponent>())
                    TankEntity.ChangeComponent<HealthComponent>(component => component.MaxHealth = value);
                else
                    TankEntity.AddComponent(new HealthComponent(value));
            }
        }

        private MatchPlayer MatchPlayer { get; }
        private Entity TankEntity => MatchPlayer.TankEntity;

        private float _currentHealth;
        private readonly float _maxHealth;
    }
}
