using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public class MatchPlayer
    {
        public MatchPlayer(BattlePlayer battlePlayer, Entity battleEntity, IEnumerable<UserResult> userResults)
        {
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
        }

        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(MatchPlayer).GetProperties()
                   where property.PropertyType == typeof(Entity)
                   select (Entity)property.GetValue(this);
        }

        private static readonly Dictionary<TankState, Type> StateComponents = new Dictionary<TankState, Type>
        {
            //{ TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public void IsisHeal()
        {
            TemperatureComponent temperatureComponent = Tank.GetComponent<TemperatureComponent>();
            if (temperatureComponent.Temperature.CompareTo(0) < 0)
                temperatureComponent.Temperature = 0;
            else if (temperatureComponent.Temperature > 0)
                temperatureComponent.Temperature -= 2;
            else if (temperatureComponent.Temperature < 0)
                temperatureComponent.Temperature += 2;
            Tank.ChangeComponent(temperatureComponent);

            HealthComponent healthComponent = Tank.GetComponent<HealthComponent>();
            int healingPerSecod = 415;
            if (healthComponent.CurrentHealth != healthComponent.MaxHealth)
            {
                if (healthComponent.MaxHealth - healthComponent.CurrentHealth > healingPerSecod)
                    healthComponent.CurrentHealth -= healingPerSecod;
                else
                    healthComponent.CurrentHealth = healthComponent.MaxHealth;
            }
            Tank.ChangeComponent(healthComponent);
            Player.BattlePlayer.Battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), Tank);
        }

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
                // New state only when tank is deleted or not ready yet
                if (value != TankState.New)
                {
                    if (_TankState == TankState.New)
                    {
                        
                        Battle battle = Player.BattlePlayer.Battle;

                        IList<SpawnPoint> coordinates;
                        TeamColor teamColor = Player.BattlePlayer.Team?.GetComponent<TeamColorComponent>().TeamColor ?? TeamColor.NONE;

                        if (battle.ModeHandler is Battle.TeamBattleHandler handler)
                            coordinates = handler.BattleViewFor(Player.BattlePlayer).SpawnPoints;
                        else
                            coordinates = ((Battle.DMHandler)battle.ModeHandler).SpawnPoints;

                        int index = new Random().Next(coordinates.Count);
                        SpawnPoint coordinate = coordinates[index];

                        /* in case you want to set another json for testing a SINGLE spawn coordinate  
                        string CoordinatesJson = File.ReadAllText("YourPath\\test.json");
                        coordinate = JsonSerializer.Deserialize<Coordinates.spawnCoordinate>(CoordinatesJson);
                        */

                        Tank.AddComponent((Component)Activator.CreateInstance(StateComponents[value]));
                        Tank.AddComponent(new TankMovementComponent(new Movement(coordinate.Position, Vector3.Zero, Vector3.Zero, coordinate.Rotation), new MoveControl(), 0, 0));
                    
                    }
                    else
                    {
                        Tank.AddComponent((Component)Activator.CreateInstance(StateComponents[value]));
                        Tank.RemoveComponent(StateComponents[_TankState]);
                    }
                }
                _TankState = value;

                TankStateChangeCountdown = value switch
                {
                    TankState.Spawn => 2,
                    TankState.SemiActive => .5,
                    TankState.Dead => 3,
                    _ => 0,
                };
            }
        }
        private TankState _TankState;

        public double TankStateChangeCountdown { get; set; }
        public bool WaitingForTankActivation { get; set; }

        public ConcurrentDictionary<Type, TranslatedEvent> TranslatedEvents { get; } = new ConcurrentDictionary<Type, TranslatedEvent>();
        public Vector3 TankPosition { get; set; }
        public bool Paused { get; set; } = false;
        public int FlagBlocks;
        public Dictionary<BonusType, double> SupplyEffects = new();
    }
}
