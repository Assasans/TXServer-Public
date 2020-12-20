using System;
using System.Collections.Generic;
using System.Linq;
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

            foreach (BattleLobbyPlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                commands.Add(new EntityShareCommand(existingBattlePlayer.User));
            }

            BattleLobbyPlayer battlePlayer;
            List<BattleLobbyPlayer> selectedTeam;
            if (RedTeamPlayers.Count <= BlueTeamPlayers.Count)
            {
                battlePlayer = new BattleLobbyPlayer(player, RedTeamEntity);
                selectedTeam = RedTeamPlayers;
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.RED)));
            }
            else
            {
                battlePlayer = new BattleLobbyPlayer(player, BlueTeamEntity);
                selectedTeam = BlueTeamPlayers;
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.BLUE)));
            }

            player.BattleLobbyPlayer = battlePlayer;
            CommandManager.SendCommands(player, commands);

            // broadcast client to other players
            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityShareCommand(battlePlayer.User));

            lock (this)
                selectedTeam.Add(battlePlayer);

            if (IsMatchMaking && BattleState == BattleState.WarmingUp || BattleState == BattleState.Running)
            {
                CommandManager.SendCommands(player, new SendEventCommand(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), player.User));
                WaitingToJoinPlayers.Add(battlePlayer);
            }
        }

        private void RemovePlayer(BattleLobbyPlayer battlePlayer)
        {
            if (!RedTeamPlayers.Remove(battlePlayer))
                BlueTeamPlayers.Remove(battlePlayer);
            WaitingToJoinPlayers.Remove(battlePlayer);
            battlePlayer.Player.BattleLobbyPlayer = null;

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

            foreach (BattleLobbyPlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                commands.Add(new EntityUnshareCommand(existingBattlePlayer.User));
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new EntityUnshareCommand(battlePlayer.User));
        }

        private void StartBattle()
        {
            foreach (BattleLobbyPlayer battleLobbyPlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
            {
                InitBattlePlayer(battleLobbyPlayer);
            }
        }

        private void InitBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.BattlePlayer = new BattlePlayer(battlePlayer, BattleEntity);

            List<Command> commands = new List<Command>
            {
                new EntityShareCommand(BattleEntity),
                new EntityShareCommand(RoundEntity)
            };

            foreach (BattleLobbyPlayer inBattlePlayer in BattlePlayers)
            {
                commands.AddRange(inBattlePlayer.BattlePlayer.GetEntities().Select(x => new EntityShareCommand(x)));
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            BattlePlayers.Add(battlePlayer);

            CommandManager.BroadcastCommands(BattlePlayers.Select(x => x.Player), battlePlayer.BattlePlayer.GetEntities().Select(x => new EntityShareCommand(x)));
        }

        private void RemoveBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            CommandManager.BroadcastCommands(BattlePlayers.Select(x => x.Player), battlePlayer.BattlePlayer.GetEntities().Select(x => new EntityUnshareCommand(x)));

            BattlePlayers.Remove(battlePlayer);

            List<Command> commands = new List<Command>
            {
                new EntityUnshareCommand(BattleEntity),
                new EntityUnshareCommand(RoundEntity)
            };

            foreach (BattleLobbyPlayer inBattlePlayer in BattlePlayers)
            {
                commands.AddRange(inBattlePlayer.BattlePlayer.GetEntities().Select(x => new EntityUnshareCommand(x)));
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);

            if (IsMatchMaking) RemovePlayer(battlePlayer);
            battlePlayer.Reset();
        }

        private void ProcessExitedPlayers()
        {
            for (int i = 0; i < RedTeamPlayers.Count + BlueTeamPlayers.Count; i++)
            {
                BattleLobbyPlayer battleLobbyPlayer;
                if (i < RedTeamPlayers.Count)
                    battleLobbyPlayer = RedTeamPlayers[i];
                else
                    battleLobbyPlayer = BlueTeamPlayers[i - RedTeamPlayers.Count];

                if (!battleLobbyPlayer.Player.IsActive || battleLobbyPlayer.WaitingForExit)
                {
                    if (battleLobbyPlayer.BattlePlayer != null)
                        RemoveBattlePlayer(battleLobbyPlayer);
                    else
                        RemovePlayer(battleLobbyPlayer);
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
                    if (BattlePlayers.Count + WaitingToJoinPlayers.Count == 0)
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
                BattleLobbyPlayer battleLobbyPlayer = WaitingToJoinPlayers[i];

                battleLobbyPlayer.MatchMakingJoinCountdown -= deltaTime;
                if (battleLobbyPlayer.MatchMakingJoinCountdown < 0)
                {
                    InitBattlePlayer(battleLobbyPlayer);

                    // Prevent joining and immediate exiting
                    battleLobbyPlayer.WaitingForExit = false;

                    WaitingToJoinPlayers.RemoveAt(i);
                    i--;
                }
            }
        }

        private void ProcessInBattlePlayers(double deltaTime)
        {
            for (int i = 0; i < BattlePlayers.Count; i++)
            {
                BattlePlayer battlePlayer = BattlePlayers[i].BattlePlayer;

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
                        new ComponentRemoveCommand(battlePlayer.Tank, typeof(TankStateTimeOutComponent)),
                        new ComponentChangeCommand(BattleEntity, CollisionsComponent));
                    battlePlayer.TankState = TankState.Active;
                    battlePlayer.WaitingForTankActivation = false;
                }

                if (battlePlayer.LastMoveCommand.ClientTime != 0)
                {
                    CommandManager.BroadcastCommands(BattlePlayers.Where(x => x.BattlePlayer != battlePlayer).Select(x => x.Player),
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

        private List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        private List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();

        private List<BattleLobbyPlayer> BattlePlayers { get; } = new List<BattleLobbyPlayer>();
        private List<BattleLobbyPlayer> WaitingToJoinPlayers { get; } = new List<BattleLobbyPlayer>();

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
