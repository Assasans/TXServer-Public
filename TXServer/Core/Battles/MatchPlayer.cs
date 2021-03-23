using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class MatchPlayer
    {
        public MatchPlayer(BattlePlayer battlePlayer, Entity battleEntity, IEnumerable<UserResult> userResults)
        {
            Battle = battlePlayer.Battle;
            Player = battlePlayer.Player;
            BattleUser = BattleUserTemplate.CreateEntity(battlePlayer.Player, battleEntity, battlePlayer.Team);
            Tank = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, BattleUser);
            Weapon = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, Tank);
            HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], Tank);
            WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, Tank);
            TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, Tank);
            Graffiti = GraffitiBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.Graffiti, Tank);
            Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, battleEntity, Tank);
            Incarnation = TankIncarnationTemplate.CreateEntity(Tank);
            UserResult = new(battlePlayer, userResults);

            if (Battle.ModeHandler is Battle.TeamBattleHandler handler)
                SpawnCoordinates = handler.BattleViewFor(Player.BattlePlayer).SpawnPoints;
            else
                SpawnCoordinates = ((Battle.DMHandler)Battle.ModeHandler).SpawnPoints;
        }

        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(MatchPlayer).GetProperties()
                   where property.PropertyType == typeof(Entity)
                   select (Entity)property.GetValue(this);
        }

        private static readonly Dictionary<TankState, Type> StateComponents = new()
        {
            { TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public void IsisHeal()
        {
            Tank.ChangeComponent<TemperatureComponent>(component =>
            {
                if (component.Temperature.CompareTo(0) < 0)
                    component.Temperature = 0;
                else if (component.Temperature > 0)
                    component.Temperature -= 2;
                else if (component.Temperature < 0)
                    component.Temperature += 2;
            });

            Tank.ChangeComponent<HealthComponent>(component =>
            {
                int healingPerSecond = 415;
                if (component.CurrentHealth != component.MaxHealth)
                {
                    if (component.MaxHealth - component.CurrentHealth > healingPerSecond)
                        component.CurrentHealth -= healingPerSecond;
                    else
                        component.CurrentHealth = component.MaxHealth;
                }
            });
            Player.BattlePlayer.Battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), Tank);
        }

        private void PrepareForRespawning()
        {
            Tank.TryRemoveComponent<TankVisibleStateComponent>();

            if (Tank.TryRemoveComponent<TankMovementComponent>())
            {
                Entity prevIncarnation = Incarnation;
                Incarnation = TankIncarnationTemplate.CreateEntity(Tank);

                foreach (Player player in prevIncarnation.PlayerReferences.ToArray())
                {
                    player.UnshareEntity(prevIncarnation);
                    player.ShareEntity(Incarnation);
                }
            }

            int index = new Random().Next(SpawnCoordinates.Count);
            LastSpawnPoint = SpawnCoordinates[index];

            /* in case you want to set another json for testing a SINGLE spawn coordinate  
            string CoordinatesJson = File.ReadAllText("YourPath\\test.json");
            coordinate = JsonSerializer.Deserialize<Coordinates.spawnCoordinate>(CoordinatesJson);
            */

            Tank.AddComponent(new TankMovementComponent(new Movement(LastSpawnPoint.Position, Vector3.Zero, Vector3.Zero, LastSpawnPoint.Rotation), new MoveControl(), 0, 0));
        }

        public void EnableTank()
        {
            if (KeepDisabled) return;
            Tank.AddComponent(new TankMovableComponent());
            Weapon.AddComponent(new ShootableComponent());
        }

        public void DisableTank()
        {
            if (KeepDisabled)
                Tank.TryRemoveComponent(StateComponents[_TankState]);
            Tank.TryRemoveComponent<SelfDestructionComponent>();

            if (!Tank.TryRemoveComponent<TankMovableComponent>()) return;
            Weapon.TryRemoveComponent<ShootableComponent>();
        }

        public void Tick()
        {
            if (SelfDestructionTime != null && DateTime.Now > SelfDestructionTime)
            {
                TankState = TankState.Dead;
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new SelfDestructionBattleUserEvent(), BattleUser);
                SelfDestructionTime = null;
                Battle.UpdateUserStatistics(Player, 0, 0, 0, 1);
            }

            // switch state after it's ended
            if (DateTime.Now > TankStateChangeTime)
            {
                switch (TankState)
                {
                    case TankState.Spawn:
                        TankState = TankState.SemiActive;
                        break;
                    case TankState.SemiActive:
                        if (!WaitingForTankActivation)
                        {
                            Tank.AddComponent(new TankStateTimeOutComponent());
                            WaitingForTankActivation = true;
                        }
                        break;
                    case TankState.Dead:
                        TankState = TankState.Spawn;
                        break;
                }
            }

            if (CollisionsPhase == Battle.CollisionsComponent.SemiActiveCollisionsPhase)
            {
                Battle.CollisionsComponent.SemiActiveCollisionsPhase++;

                Tank.RemoveComponent<TankStateTimeOutComponent>();
                Battle.BattleEntity.ChangeComponent(Battle.CollisionsComponent);

                TankState = TankState.Active;
                WaitingForTankActivation = false;

                var component = Tank.GetComponent<HealthComponent>();
                Tank.RemoveComponent<HealthComponent>();
                component.CurrentHealth = component.MaxHealth;
                Tank.AddComponent(component);
            }

            foreach (KeyValuePair<Type, TranslatedEvent> pair in TranslatedEvents)
            {
                (from matchPlayer in Battle.MatchPlayers
                 where matchPlayer.MatchPlayer != this
                 select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                TranslatedEvents.TryRemove(pair.Key, out _);
            }

            // supply effects
            foreach (KeyValuePair<BonusType, DateTime> entry in SupplyEffects.ToArray())
            {
                if (DateTime.Now > entry.Value)
                {
                    switch (entry.Key)
                    {
                        case BonusType.ARMOR:
                            Tank.RemoveComponent<ArmorEffectComponent>();
                            break;
                        case BonusType.DAMAGE:
                            Tank.RemoveComponent<DamageEffectComponent>();
                            break;
                        case BonusType.SPEED:
                            Tank.RemoveComponent<TurboSpeedEffectComponent>();
                            Tank.ChangeComponent(new SpeedComponent(9.967f, 98f, 13.205f));
                            break;
                    }
                    SupplyEffects.Remove(entry.Key);
                }
            }
        }

        private readonly Battle Battle;
        public Player Player { get; }
        public Entity BattleUser { get; }
        public Entity RoundUser { get; }

        public Entity Incarnation { get; set; }
        public Entity Tank { get; }
        public Entity Weapon { get; }
        public Entity HullSkin { get; }
        public Entity WeaponSkin { get; }
        public Entity WeaponPaint { get; }
        public Entity TankPaint { get; }
        public Entity Graffiti { get; }
        public Entity Shell { get; }

        public UserResult UserResult { get; }

        public long CollisionsPhase { get; set; } = -1;
        public TankState TankState
        {
            get => _TankState;
            set
            {
                switch (value)
                {
                    case TankState.Spawn:
                        DisableTank();
                        PrepareForRespawning();
                        break;
                    case TankState.SemiActive:
                        EnableTank();
                        Tank.ChangeComponent(new TemperatureComponent(0));
                        Tank.AddComponent(new TankVisibleStateComponent());
                        break;
                    case TankState.Dead:
                        DisableTank();
                        Player.SendEvent(new SelfTankExplosionEvent(), Tank);
                        if (Battle.ModeHandler is CTFHandler handler)
                        {
                            foreach (Flag flag in handler.Flags.Values)
                            {
                                if (flag != null && flag.State == FlagState.Captured && flag.FlagEntity.GetComponent<TankGroupComponent>().Key == Tank.EntityId)
                                    flag.Drop(false);
                            }
                        }
                        foreach (BonusType bonusType in SupplyEffects.Keys.ToArray())
                            SupplyEffects[bonusType] = DateTime.Now;
                        break;
                }

                if (Tank.GetComponent(StateComponents[_TankState]) != null)
                    Tank.RemoveComponent(StateComponents[_TankState]);
                Tank.AddComponent((Component)Activator.CreateInstance(StateComponents[value]));
                _TankState = value;

                TankStateChangeTime = DateTime.Now.AddSeconds(value switch
                {
                    TankState.Spawn => 2,
                    TankState.SemiActive => .5,
                    TankState.Dead => 3,
                    _ => 0,
                });
            }
        }
        private TankState _TankState;
        public bool KeepDisabled { get; set; }

        public DateTime TankStateChangeTime { get; set; }
        public DateTime? SelfDestructionTime { get; set; }
        public bool WaitingForTankActivation { get; set; }

        public ConcurrentDictionary<Type, TranslatedEvent> TranslatedEvents { get; } = new ConcurrentDictionary<Type, TranslatedEvent>();
        public Vector3 TankPosition { get; set; }
        public bool Paused { get; set; } = false;
        public Dictionary<BonusType, DateTime> SupplyEffects { get; } = new();
        private IList<SpawnPoint> SpawnCoordinates;
        public SpawnPoint LastSpawnPoint { get; set; }
    }
}
