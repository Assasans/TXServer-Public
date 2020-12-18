using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public enum BattleState
    {
        NotEnoughPlayers,
        StartCountdown,
        CustomNotStarted,
        Starting,
        WarmingUp,
        Running,
        Ended
    }

    public class Battle
    {
        public Battle() : this(20, GravityType.EARTH, null) { }

        public Battle(int userLimit, GravityType gravity, BattleMode? battleMode)
        {
            if (battleMode == null)
            {
                IsMatchMaking = true;

                /*
                BattleMode[] modes = (BattleMode[])Enum.GetValues(typeof(BattleMode));
                lock (Random)
                    BattleMode = modes[Random.Next(modes.Length)];
                */
                BattleMode = BattleMode.CTF; // todo implement all battle modes
            }
            else
            {
                BattleMode = battleMode.Value;
            }

            MapEntity = Maps.GlobalItems.Rio;
            BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(MapEntity, BattleMode, userLimit, GravityTypes[gravity], gravity);
            BattleEntity = (Entity)BattleEntityCreators[BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, 5, 600, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);

            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();
        }

        public void AddPlayer(Player player)
        {
            // prepare client
            List<Command> commands = new List<Command>
            {
                new EntityShareCommand(BattleLobbyEntity),
                new EntityShareCommand(RedTeamEntity),
                new EntityShareCommand(BlueTeamEntity),
                new ComponentAddCommand(player.User, new BattleLobbyGroupComponent(BattleLobbyEntity)),
            };
            if (IsMatchMaking)
                commands.Add(new ComponentAddCommand(player.User, new MatchMakingUserComponent()));

            foreach (BattlePlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                commands.Add(new EntityShareCommand(existingBattlePlayer.User));
            }

            BattlePlayer battlePlayer;
            List<BattlePlayer> selectedTeam;
            if (RedTeamPlayers.Count <= BlueTeamPlayers.Count)
            {
                battlePlayer = new BattlePlayer(player, BattleEntity, RedTeamEntity);
                selectedTeam = RedTeamPlayers;
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.RED)));
            }
            else
            {
                battlePlayer = new BattlePlayer(player, BattleEntity, BlueTeamEntity);
                selectedTeam = BlueTeamPlayers;
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.BLUE)));
            }

            player.BattlePlayer = battlePlayer;
            CommandManager.SendCommands(player, commands);

            lock (this)
                selectedTeam.Add(battlePlayer);

            // broadcast client to other players
            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityShareCommand(battlePlayer.User));

            if (IsMatchMaking && BattleState == BattleState.WarmingUp || BattleState == BattleState.Running)
            {
                WaitingToJoinPlayers.Add(battlePlayer);
            }
        }

        private void RemovePlayer(BattlePlayer battlePlayer)
        {
            if (!RedTeamPlayers.Remove(battlePlayer))
                BlueTeamPlayers.Remove(battlePlayer);
            WaitingToJoinPlayers.Remove(battlePlayer);
            battlePlayer.Player.BattlePlayer = null;

            List<Command> commands = new List<Command>
            {
                new EntityUnshareCommand(BattleLobbyEntity),
                new EntityUnshareCommand(RedTeamEntity),
                new EntityUnshareCommand(BlueTeamEntity),
                new ComponentRemoveCommand(battlePlayer.User, typeof(TeamColorComponent)),
                new ComponentRemoveCommand(battlePlayer.User, typeof(BattleLobbyGroupComponent))
            };
            if (IsMatchMaking)
                commands.Add(new ComponentRemoveCommand(battlePlayer.User, typeof(MatchMakingUserComponent)));

            foreach (BattlePlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                commands.Add(new EntityUnshareCommand(existingBattlePlayer.User));
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityUnshareCommand(battlePlayer.User));
        }

        private void StartBattle()
        {
            foreach (BattlePlayer battlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                InitBattlePlayer(battlePlayer);
            }
        }

        private void InitBattlePlayer(BattlePlayer battlePlayer)
        {
            battlePlayer.BattleUser = BattleUserTemplate.CreateEntity(battlePlayer.Player, BattleEntity, battlePlayer.Team);
            battlePlayer.Tank = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, battlePlayer.BattleUser);
            battlePlayer.Weapon = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, battlePlayer.Tank);
            battlePlayer.HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], battlePlayer.Tank);
            battlePlayer.WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], battlePlayer.Tank);
            battlePlayer.WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, battlePlayer.Tank);
            battlePlayer.TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, battlePlayer.Tank);
            battlePlayer.Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], battlePlayer.Tank);
            battlePlayer.RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, BattleEntity);
            battlePlayer.Incarnation = TankIncarnationTemplate.CreateEntity(battlePlayer.Tank);

            List<Command> commands = new List<Command>
            {
                new EntityShareCommand(BattleEntity),
                new EntityShareCommand(RoundEntity)
            };

            foreach (BattlePlayer inBattlePlayer in InBattlePlayers)
            {
                commands.AddRange(new[] {
                    new EntityShareCommand(inBattlePlayer.BattleUser),
                    new EntityShareCommand(inBattlePlayer.Tank),
                    new EntityShareCommand(inBattlePlayer.Weapon),
                    new EntityShareCommand(inBattlePlayer.HullSkin),
                    new EntityShareCommand(inBattlePlayer.WeaponSkin),
                    new EntityShareCommand(inBattlePlayer.TankPaint),
                    new EntityShareCommand(inBattlePlayer.WeaponPaint),
                    new EntityShareCommand(inBattlePlayer.Shell),
                    new EntityShareCommand(inBattlePlayer.RoundUser),
                    new EntityShareCommand(inBattlePlayer.Incarnation)});
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            InBattlePlayers.Add(battlePlayer);

            CommandManager.BroadcastCommands(InBattlePlayers.Select(x => x.Player),
                new EntityShareCommand(battlePlayer.BattleUser),
                new EntityShareCommand(battlePlayer.Tank),
                new EntityShareCommand(battlePlayer.Weapon),
                new EntityShareCommand(battlePlayer.HullSkin),
                new EntityShareCommand(battlePlayer.WeaponSkin),
                new EntityShareCommand(battlePlayer.TankPaint),
                new EntityShareCommand(battlePlayer.WeaponPaint),
                new EntityShareCommand(battlePlayer.Shell),
                new EntityShareCommand(battlePlayer.RoundUser),
                new EntityShareCommand(battlePlayer.Incarnation));
        }

        private void RemoveBattlePlayer(BattlePlayer battlePlayer)
        {
            CommandManager.BroadcastCommands(InBattlePlayers.Select(x => x.Player),
                new EntityUnshareCommand(battlePlayer.BattleUser),
                new EntityUnshareCommand(battlePlayer.Tank),
                new EntityUnshareCommand(battlePlayer.Weapon),
                new EntityUnshareCommand(battlePlayer.HullSkin),
                new EntityUnshareCommand(battlePlayer.WeaponSkin),
                new EntityUnshareCommand(battlePlayer.TankPaint),
                new EntityUnshareCommand(battlePlayer.WeaponPaint),
                new EntityUnshareCommand(battlePlayer.Shell),
                new EntityUnshareCommand(battlePlayer.RoundUser));

            InBattlePlayers.Remove(battlePlayer);

            List<Command> commands = new List<Command>
            {
                new EntityUnshareCommand(BattleEntity),
                new EntityUnshareCommand(RoundEntity)
            };

            foreach (BattlePlayer inBattlePlayer in InBattlePlayers)
            {
                commands.AddRange(new[] {
                    new EntityUnshareCommand(inBattlePlayer.BattleUser),
                    new EntityUnshareCommand(inBattlePlayer.Tank),
                    new EntityUnshareCommand(inBattlePlayer.Weapon),
                    new EntityUnshareCommand(inBattlePlayer.HullSkin),
                    new EntityUnshareCommand(inBattlePlayer.WeaponSkin),
                    new EntityUnshareCommand(inBattlePlayer.TankPaint),
                    new EntityUnshareCommand(inBattlePlayer.WeaponPaint),
                    new EntityUnshareCommand(inBattlePlayer.Shell),
                    new EntityUnshareCommand(inBattlePlayer.RoundUser)});
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            battlePlayer.BattleUser = null;
            battlePlayer.Tank = null;
            battlePlayer.Weapon = null;
            battlePlayer.HullSkin = null;
            battlePlayer.WeaponSkin = null;
            battlePlayer.TankPaint = null;
            battlePlayer.WeaponPaint = null;
            battlePlayer.Shell = null;
            battlePlayer.RoundUser = null;

            if (IsMatchMaking) RemovePlayer(battlePlayer);
            battlePlayer.Reset();
        }

        private void ProcessExitedPlayers()
        {
            for (int i = 0; i < RedTeamPlayers.Count + BlueTeamPlayers.Count; i++)
            {
                BattlePlayer battlePlayer = null;
                if (i < RedTeamPlayers.Count)
                    battlePlayer = RedTeamPlayers[i];
                else
                    battlePlayer = BlueTeamPlayers[i - RedTeamPlayers.Count];

                if (!battlePlayer.Player.IsActive || battlePlayer.WaitingForExit)
                {
                    if (battlePlayer.BattleUser != null)
                        RemoveBattlePlayer(battlePlayer);
                    else
                        RemovePlayer(battlePlayer);
                    i--;
                }
            }
        }

        private void ProcessBattleState(double deltaTime)
        {
            CountdownTimer -= deltaTime;
            switch (BattleState)
            {
                case BattleState.NotEnoughPlayers:
                    if (RedTeamPlayers.Count + BlueTeamPlayers.Count > 0 && RedTeamPlayers.Count == BlueTeamPlayers.Count)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers[0].Player,
                            new ComponentAddCommand(BattleLobbyEntity, new MatchMakingLobbyStartTimeComponent(new TimeSpan(0, 0, 10))));
                        CountdownTimer = 10;
                        BattleState = BattleState.StartCountdown;
                    }
                    break;
                case BattleState.StartCountdown:
                    if (RedTeamPlayers.Count + BlueTeamPlayers.Count == 0)
                    {
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartTimeComponent>();
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (RedTeamPlayers.Count != BlueTeamPlayers.Count)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers.Concat(BlueTeamPlayers).First().Player,
                            new ComponentRemoveCommand(BattleLobbyEntity, typeof(MatchMakingLobbyStartTimeComponent)));
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (CountdownTimer < 0)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers[0].Player,
                            new ComponentRemoveCommand(BattleLobbyEntity, typeof(MatchMakingLobbyStartTimeComponent)),
                            new ComponentAddCommand(BattleLobbyEntity, new MatchMakingLobbyStartingComponent()));
                        CountdownTimer = 3;
                        BattleState = BattleState.Starting;
                    }
                    break;
                case BattleState.Starting:
                    if (RedTeamPlayers.Count + BlueTeamPlayers.Count == 0)
                    {
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (IsMatchMaking && RedTeamPlayers.Count != BlueTeamPlayers.Count)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers.Concat(BlueTeamPlayers).First().Player,
                            new ComponentRemoveCommand(BattleLobbyEntity, typeof(MatchMakingLobbyStartingComponent)));
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (CountdownTimer < 0)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers[0].Player,
                            new ComponentRemoveCommand(BattleLobbyEntity, typeof(MatchMakingLobbyStartingComponent)));
                        StartBattle();
                        BattleState = BattleState.Running;
                    }

                    break;
                case BattleState.WarmingUp:
                    break;
                case BattleState.Running:
                    if (InBattlePlayers.Count + WaitingToJoinPlayers.Count == 0)
                    {
                        BattleState = BattleState.NotEnoughPlayers;
                    }
                    break;
            }
        }

        private void ProcessWaitingPlayers(double deltaTime)
        {
            for (int i = 0; i < WaitingToJoinPlayers.Count; i++)
            {
                BattlePlayer battlePlayer = WaitingToJoinPlayers[i];

                battlePlayer.MatchMakingJoinCountdown -= deltaTime;
                if (battlePlayer.MatchMakingJoinCountdown < 0)
                {
                    InitBattlePlayer(battlePlayer);

                    // Prevent joining and immediate exiting
                    battlePlayer.WaitingForExit = false;

                    WaitingToJoinPlayers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ProcessInBattlePlayers(double deltaTime)
        {
            for (int i = 0; i < InBattlePlayers.Count; i++)
            {
                BattlePlayer battlePlayer = InBattlePlayers[i];

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
                            CommandManager.SendCommandsSafe(battlePlayer.Player,
                                new ComponentAddCommand(battlePlayer.Tank, new TankVisibleStateComponent()),
                                new ComponentAddCommand(battlePlayer.Tank, new TankMovableComponent()));
                            break;
                        case TankState.SemiActive:
                            if (!battlePlayer.WaitingForTankActivation)
                            {
                                CommandManager.SendCommandsSafe(battlePlayer.Player,
                                    new ComponentAddCommand(battlePlayer.Tank, new TankStateTimeOutComponent()));
                                battlePlayer.WaitingForTankActivation = true;
                            }
                            break;
                        case TankState.Dead:
                            battlePlayer.TankState = TankState.Spawn;
                            CommandManager.SendCommandsSafe(battlePlayer.Player,
                                new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankVisibleStateComponent)),
                                new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankMovableComponent)));
                            break;
                    }
                }

                if (battlePlayer.CollisionsPhase == CollisionsComponent.SemiActiveCollisionsPhase)
                {
                    CollisionsComponent.SemiActiveCollisionsPhase++;
                    CommandManager.SendCommandsSafe(battlePlayer.Player,
                        new ComponentChangeCommand(BattleEntity, CollisionsComponent),
                        new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankStateTimeOutComponent)));
                    battlePlayer.TankState = TankState.Active;
                    battlePlayer.WaitingForTankActivation = false;
                }

                if (battlePlayer.LastMoveCommand.ClientTime != 0)
                {
                    CommandManager.BroadcastCommands(InBattlePlayers.Where(x => x != battlePlayer).Select(x => x.Player),
                        new SendEventCommand(new MoveCommandServerEvent(battlePlayer.LastMoveCommand), battlePlayer.Tank));

                    MoveCommand moveCommand = battlePlayer.LastMoveCommand;
                    moveCommand.ClientTime = 0;
                    battlePlayer.LastMoveCommand = moveCommand;
                }
            }
        }

        public void Tick(double deltaTime)
        {
            lock (this)
            {
                ProcessExitedPlayers();
                ProcessBattleState(deltaTime);
                ProcessWaitingPlayers(deltaTime);
                ProcessInBattlePlayers(deltaTime);
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
        public BattleState BattleState { get; private set; }
        public double CountdownTimer { get; private set; }

        private List<BattlePlayer> RedTeamPlayers { get; } = new List<BattlePlayer>();
        private List<BattlePlayer> BlueTeamPlayers { get; } = new List<BattlePlayer>();

        private List<BattlePlayer> InBattlePlayers { get; } = new List<BattlePlayer>();
        private List<BattlePlayer> WaitingToJoinPlayers { get; } = new List<BattlePlayer>();

        public Entity BattleEntity { get; }
        public Entity BattleLobbyEntity { get; }
        public Entity MapEntity { get; }
        public Entity RoundEntity { get; }
        public BattleTankCollisionsComponent CollisionsComponent { get; }

        public Entity RedTeamEntity { get; }
        public Entity BlueTeamEntity { get; }

        public Player Owner { get; set; }
    }
}
