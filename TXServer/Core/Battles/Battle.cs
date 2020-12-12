using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public enum TankState
    {
        New,
        Spawn,
        SemiActive,
        Active,
        Dead
    }

    public class BattlePlayer
    {
        public static readonly Dictionary<TankState, Type> StateComponents = new Dictionary<TankState, Type>
        {
            //{ TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public BattlePlayer(Player player, Entity battleEntity, Entity team)
        {
            Player = player;
            User = player.User;

            BattleUser = BattleUserTemplate.CreateEntity(player, battleEntity, team);
            Team = team;
        }

        public Player Player { get; }
        public Entity User { get; }
        public Entity Team { get; set; }

        public Entity BattleUser { get; set; }
        public Entity RoundUser { get; set; }

        public Entity Incarnation { get; }
        public Entity Tank { get; set; }
        public Entity Weapon { get; set; }
        public Entity HullSkin { get; set; }
        public Entity WeaponSkin { get; set; }
        public Entity WeaponPaint { get; set; }
        public Entity TankPaint { get; set; }
        public Entity Shell { get; set; }

        public bool IsTankVisible { get; set; }
        public TankState TankState
        {
            get => _TankState;
            set
            {
                // New state only when tank is deleted or not ready yet
                if (_TankState == TankState.New)
                {
                    CommandManager.SendCommands(Player,
                        new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])),
                        new ComponentAddCommand(Tank, new TankMovementComponent(new Movement(new Vector3(0, 2, 0), Vector3.Zero, Vector3.Zero, Quaternion.Identity), new MoveControl(), 0, 0)));
                }
                else if (value != TankState.New)
                {
                    CommandManager.SendCommands(Player,
                        new ComponentRemoveCommand(Tank, StateComponents[_TankState]),
                        new ComponentAddCommand(Tank, (Component)Activator.CreateInstance(StateComponents[value])));
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
    }

    public class Battle
    {
        public Battle() : this(20, GravityType.EARTH, BattleMode.CTF) { } // todo replace battlemode with null

        public Battle(int userLimit, GravityType gravity, BattleMode? battleMode)
        {
            if (battleMode == null)
            {
                IsMatchMaking = true;

                BattleMode[] modes = (BattleMode[])Enum.GetValues(typeof(BattleMode));
                lock (Random)
                    BattleMode = modes[Random.Next(modes.Length)];
            }
            else
            {
                BattleMode = battleMode.Value;
            }

            MapEntity = Maps.GlobalItems.Rio;
            BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(MapEntity, BattleMode, 2, GravityTypes[gravity], gravity);
            BattleEntity = (Entity)BattleEntityCreators[BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, 5, 600, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
        }

        public void AddPlayer(Player player)
        {
            // prepare client and send all players to it
            List<Command> commands = new List<Command>
            {
                new EntityShareCommand(BattleLobbyEntity),
                new EntityShareCommand(RedTeamEntity),
                new EntityShareCommand(BlueTeamEntity),
                new ComponentAddCommand(player.User, new MatchMakingUserComponent()), // todo remove it
                new ComponentAddCommand(player.User, new BattleLobbyGroupComponent(BattleLobbyEntity)),
                
            };
            foreach (BattlePlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                commands.Add(new EntityShareCommand(existingBattlePlayer.BattleUser));
            }

            BattlePlayer battlePlayer;
            if (RedTeamPlayers.Count <= BlueTeamPlayers.Count)
            {
                battlePlayer = new BattlePlayer(player, BattleEntity, RedTeamEntity);
                RedTeamPlayers.Add(battlePlayer);
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.RED)));
            }
            else
            {
                battlePlayer = new BattlePlayer(player, BattleEntity, BlueTeamEntity);
                BlueTeamPlayers.Add(battlePlayer);
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.BLUE)));
            }

            player.BattlePlayer = battlePlayer;
            CommandManager.SendCommands(player, commands);

            // broadcast client to other players
            CommandManager.BroadcastCommands(from x in RedTeamPlayers.Concat(BlueTeamPlayers)
                                             where x.Player != player
                                             select x.Player,
                new EntityShareCommand(battlePlayer.User),
                new EntityShareCommand(battlePlayer.BattleUser));
        }

        public void RemovePlayer(BattlePlayer battlePlayer)
        {
            if (!RedTeamPlayers.Remove(battlePlayer) && !BlueTeamPlayers.Remove(battlePlayer))
            {
                throw new KeyNotFoundException($"Battle player {battlePlayer.User.EntityId} not found.");
            }

            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityUnshareCommand(battlePlayer.User),
                new EntityUnshareCommand(battlePlayer.BattleUser));
        }

        public void StartBattle()
        {
            foreach (BattlePlayer battlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                InitBattlePlayer(battlePlayer);
            }
        }

        public void InitBattlePlayer(BattlePlayer battlePlayer)
        {
            CommandManager.SendCommands(battlePlayer.Player,
                new EntityShareCommand(BattleEntity),
                new EntityShareCommand(RoundEntity));

            battlePlayer.Tank = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, battlePlayer.BattleUser);
            battlePlayer.Weapon = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, battlePlayer.Tank);
            battlePlayer.HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], battlePlayer.Tank);
            battlePlayer.WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], battlePlayer.Tank);
            battlePlayer.WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, battlePlayer.Tank);
            battlePlayer.TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, battlePlayer.Tank);
            battlePlayer.Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], battlePlayer.Tank);

            battlePlayer.RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, BattleEntity);

            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityShareCommand(battlePlayer.BattleUser),
                new EntityShareCommand(battlePlayer.Tank),
                new EntityShareCommand(battlePlayer.Weapon),
                new EntityShareCommand(battlePlayer.HullSkin),
                new EntityShareCommand(battlePlayer.WeaponSkin),
                new EntityShareCommand(battlePlayer.TankPaint),
                new EntityShareCommand(battlePlayer.WeaponPaint),
                new EntityShareCommand(battlePlayer.Shell),
                new EntityShareCommand(battlePlayer.RoundUser));
        }

        public void RemoveBattlePlayer()
        {

        }

        public void Tick(double deltaTime)
        {
            if (!IsRunning)
            {
                if (RedTeamPlayers.Count + BlueTeamPlayers.Count > 0)// && RedTeamPlayers.Count == BlueTeamPlayers.Count)
                {
                    CountdownTimer -= deltaTime;
                    if (CountdownTimer > 0) return;

                    StartBattle();
                    IsRunning = true;
                }
                else
                {
                    CountdownTimer = 5.0f;
                }
                return;
            }

            foreach (BattlePlayer battlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                if (battlePlayer.TankState != TankState.Active && battlePlayer.TankState != TankState.New)
                {
                    battlePlayer.TankStateChangeCountdown -= deltaTime;
                }

                // switch state after it's ended
                if (battlePlayer.TankStateChangeCountdown < 0)
                {
                    switch (battlePlayer.TankState)
                    {
                        case TankState.Spawn:
                            battlePlayer.TankState = TankState.SemiActive;
                            CommandManager.SendCommands(battlePlayer.Player,
                                new ComponentAddCommand(battlePlayer.Tank, new TankVisibleStateComponent()),
                                new ComponentAddCommand(battlePlayer.Tank, new TankMovableComponent()));
                            break;
                        case TankState.SemiActive:
                            battlePlayer.TankState = TankState.Active;
                            break;
                        case TankState.Dead:
                            battlePlayer.TankState = TankState.Spawn;
                            CommandManager.SendCommands(battlePlayer.Player,
                                new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankVisibleStateComponent)),
                                new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankMovableComponent)));
                            break;
                    }
                }
            }
        }
        
        private static readonly Dictionary<BattleMode, Type> BattleEntityCreators = new Dictionary<BattleMode, Type>
        {
            { BattleMode.DM, typeof(DMTemplate) },
            //{ BattleMode.TDM, typeof(TDMTemplate) },
            { BattleMode.CTF, typeof(CTFTemplate) },
        };
        private static readonly Dictionary<GravityType, float> GravityTypes = new Dictionary<GravityType, float>
        {
            { GravityType.EARTH, 9.81f },
        };

        private static readonly Random Random = new Random();

        public BattleMode BattleMode { get; private set; }

        public bool IsMatchMaking { get; }
        public bool IsEnoughPlayers { get; private set; }

        public double CountdownTimer { get; private set; }
        public bool IsRunning { get; private set; }

        public List<BattlePlayer> RedTeamPlayers { get; } = new List<BattlePlayer>();
        public List<BattlePlayer> BlueTeamPlayers { get; } = new List<BattlePlayer>();
        private bool PrevTeamsEqual;

        public Entity BattleEntity { get; }
        public Entity BattleLobbyEntity { get; }
        public Entity MapEntity { get; }
        public Entity RoundEntity { get; }

        public Entity RedTeamEntity { get; }
        public Entity BlueTeamEntity { get; }

        public Player Owner { get; set; }
    }
}
