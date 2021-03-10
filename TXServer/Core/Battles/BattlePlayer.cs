using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Types;
using System.Text.Json;
using System.IO;
using TXServer.Core.ServerMapInformation;

namespace TXServer.Core.Battles
{
    public class BattlePlayer
    {
        public BattlePlayer(BattleLobbyPlayer battlePlayer, Entity battleEntity)
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
        }

        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(BattlePlayer).GetProperties()
                   where property.PropertyType == typeof(Entity)
                   select (Entity)property.GetValue(this);
        }

        public static readonly Dictionary<TankState, Type> StateComponents = new Dictionary<TankState, Type>
        {
            //{ TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

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
                        
                        Battle battle = ServerConnection.BattlePool.Single(b => b.BlueTeamPlayers.Concat(b.RedTeamPlayers).Concat(b.DMTeamPlayers).Contains(Player.BattleLobbyPlayer));

                        IList<SpawnPoint> coordinates;
                        TeamColor teamColor;
                        if (Player.BattleLobbyPlayer.Team == null) { teamColor = TeamColor.NONE; }
                        else
                        {
                            teamColor = Player.BattleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor;
                        }

                        if (battle.DeathmatchSpawnPoints == null)
                        {
                            if (teamColor == TeamColor.BLUE) { coordinates = battle.TeamSpawnPoints.BlueTeam; } 
                            else { coordinates = battle.TeamSpawnPoints.RedTeam; }
                        } 
                        else
                        {
                            coordinates = battle.DeathmatchSpawnPoints;
                        } 

                        int index = new Random().Next(coordinates.Count);
                        SpawnPoint coordinate = coordinates[index];

                        /* in case you want to set another json for testing a SINGLE spawn coordinate  
                        string CoordinatesJson = File.ReadAllText("YourPath\\test.json");
                        coordinate = JsonSerializer.Deserialize<Coordinates.spawnCoordinate>(CoordinatesJson);
                        */

                        CommandManager.SendCommandsSafe(Player,
                            new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])),
                            new ComponentAddCommand(Tank, new TankMovementComponent(new Movement(coordinate.Position, Vector3.Zero, Vector3.Zero, coordinate.Rotation), new MoveControl(), 0, 0)));
                    
                    }
                    else
                    {
                        CommandManager.SendCommandsSafe(Player,
                            new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])),
                            new ComponentRemoveCommand(Tank, StateComponents[_TankState]));
                    }
                }
                _TankState = value;

                switch (value)
                {
                    case TankState.Spawn:
                        TankStateChangeCountdown = 2;
                        break;
                    case TankState.SemiActive:
                        TankStateChangeCountdown = .5;
                        break;
                    case TankState.Dead:
                        TankStateChangeCountdown = 3;
                        break;
                    default:
                        TankStateChangeCountdown = 0;
                        break;
                }
            }
        }
        private TankState _TankState;
        public double TankStateChangeCountdown { get; set; }
        public bool WaitingForTankActivation { get; set; }
        public ConcurrentDictionary<Type, TranslatedEvent> TranslatedEvents { get; } = new ConcurrentDictionary<Type, TranslatedEvent>();
        public Vector3 TankPosition { get; set; }
        public int FlagBlocks;
        public Dictionary<BonusType, double> SupplyEffects = new();
    }
}
