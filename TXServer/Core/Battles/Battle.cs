using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Commands;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using System.Threading;

namespace TXServer.Core.Battles
{
    public class Battle
    {
        public Battle(long? mapID, int? maxPlayers, BattleMode? battleMode, int? timeLimit, int? scoreLimit, bool? friendlyFire, GravityType? gravity, bool? killZoneEnabled, 
            bool? disabledModules, bool isMatchMaking, Player owner)
        {

            mapID = mapID ?? -1664220274;  // default: Rio
            MaxPlayers = maxPlayers ?? 20;
            timeLimit = timeLimit ?? 10;
            scoreLimit = scoreLimit ?? 100;
            friendlyFire = friendlyFire ?? false;
            gravity = gravity ?? GravityType.EARTH;
            killZoneEnabled = killZoneEnabled ?? true;
            disabledModules = disabledModules ?? false;
            IsMatchMaking = isMatchMaking;

            if (battleMode == null)
            {
                BattleMode = BattleMode.CTF; // todo implement all battle modes + random choice with correct probabilities
                /*
                BattleMode[] modes = (BattleMode[])Enum.GetValues(typeof(BattleMode));
                lock (Random)
                    BattleMode = modes[Random.Next(modes.Length)];
                */
            }
            else
            {
                BattleMode = battleMode.Value;
            }

            if (IsMatchMaking)
            {
                if (mapID == null)
                {
                    var random = new Random();
                    var matchMakingMaps = new List<Entity> { Maps.GlobalItems.Silence, Maps.GlobalItems.Nightiran, Maps.GlobalItems.Acidlake, Maps.GlobalItems.Sandbox,
                        Maps.GlobalItems.Iran, Maps.GlobalItems.Area159, Maps.GlobalItems.Repin, Maps.GlobalItems.Westprime, Maps.GlobalItems.Boombox, Maps.GlobalItems.Silencemoon,
                        Maps.GlobalItems.Rio, Maps.GlobalItems.MassacremarsBG, Maps.GlobalItems.Massacre, Maps.GlobalItems.Kungur };
                    int index = random.Next(matchMakingMaps.Count);
                    mapID = matchMakingMaps[index].EntityId;
                }
                ConvertMapParams((long)mapID, isMatchMaking);
                BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(map: MapEntity, maxPlayers:MaxPlayers, BattleMode, timeLimit: (int)timeLimit,
                    scoreLimit: (int)scoreLimit, friendlyFire: (bool)friendlyFire, gravity: GravityTypes[(GravityType)gravity], gravityType: GravityType.EARTH,
                    killZoneEnabled: (bool)killZoneEnabled, disabledModules: (bool)disabledModules);
            }
            else
            {
                ConvertMapParams((long)mapID, isMatchMaking);
                this.BattleMode = battleMode.Value;
                Owner = owner;
                BattleLobbyEntity = CustomBattleLobbyTemplate.CreateEntity(map:MapEntity, mapId:(long)mapID, maxPlayers:MaxPlayers, BattleMode, timeLimit:(int)timeLimit, 
                    scoreLimit:(int)scoreLimit, friendlyFire:(bool)friendlyFire, gravity:GravityTypes[(GravityType)gravity], gravityType:GravityType.EARTH, 
                    killZoneEnabled:(bool)killZoneEnabled, disabledModules:(bool)disabledModules);
            }

            BattleEntity = (Entity)BattleEntityCreators[BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, 5, 600, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleMode == BattleMode.CTF)
            {
                // Rio pedestals/flags positions
                // TODO: read specific positions from json
                PedestalsPositions[0] = new Vector3(-6, 0, -65);
                PedestalsPositions[1] = new Vector3(13, 0, 65);
                FlagsPositions[0] = new Vector3(-6, 0, -65);
                FlagsPositions[1] = new Vector3(13, 0, 65);

                RedPedestalEntity = PedestalTemplate.CreateEntity(TeamColor.RED, PedestalsPositions[0], RedTeamEntity);
                BluePedestalEntity = PedestalTemplate.CreateEntity(TeamColor.BLUE, PedestalsPositions[1], BlueTeamEntity);
                RedFlagEntity = FlagTemplate.CreateEntity(TeamColor.RED, FlagsPositions[0], RedTeamEntity, new FlagHomeStateComponent());
                BlueFlagEntity = FlagTemplate.CreateEntity(TeamColor.BLUE, FlagsPositions[1], BlueTeamEntity, new FlagHomeStateComponent());
            }      
        }

        public void ConvertMapParams(long mapID, bool isMatchMaking)
        {
            switch (mapID)
            {
                case -321842153:
                    MapEntity = Maps.GlobalItems.Silence;
                    break;
                case 343745828:
                    MapEntity = Maps.GlobalItems.Nightiran;
                    break;
                case 485053206:
                    MapEntity = Maps.GlobalItems.Acidlake;
                    break;
                case -820833801:
                    MapEntity = Maps.GlobalItems.Acidlakehalloween;
                    break;
                case 458045295:
                    MapEntity = Maps.GlobalItems.Testbox;
                    if (isMatchMaking)
                    {
                        MaxPlayers = 8;
                    }
                    break;
                case -549069251:
                    MapEntity = Maps.GlobalItems.Sandbox;
                    if (isMatchMaking)
                    {
                        MaxPlayers = 8;
                    }
                    break;
                case -51480736:
                    MapEntity = Maps.GlobalItems.Iran;
                    break;
                case 1133979230:
                    MapEntity = Maps.GlobalItems.Area159;
                    if (isMatchMaking)
                    {
                        MaxPlayers = 8;
                    }
                    break;
                case -1587964040:
                    MapEntity = Maps.GlobalItems.Repin;
                    break;
                case 980475942:
                    MapEntity = Maps.GlobalItems.Westprime;
                    break;
                case 1945237110:
                    MapEntity = Maps.GlobalItems.Boombox;
                    if (isMatchMaking)
                    {
                        MaxPlayers = 8;
                    }
                    break;
                case 933129112:
                    MapEntity = Maps.GlobalItems.Silencemoon;
                    break;
                case -1664220274:
                    MapEntity = Maps.GlobalItems.Rio;
                    break;
                case 989096365:
                    MapEntity = Maps.GlobalItems.MassacremarsBG;
                    break;
                case -1551247853:
                    MapEntity = Maps.GlobalItems.Massacre;
                    break;
                case 2127033418:
                    MapEntity = Maps.GlobalItems.Kungur;
                    break;
                default:
                    MapEntity = Maps.GlobalItems.Rio;
                    break;
            }
        }

        public void AddPlayer(Player player)
        {
            // prepare client
            List<ICommand> commands = new List<ICommand>
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
            if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
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

            List<ICommand> commands = new List<ICommand>
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

            if (!IsMatchMaking && (RedTeamPlayers.Count + BlueTeamPlayers.Count) < 1)
            {
                ServerConnection.BattlePool.RemoveAll(p => (p.RedTeamPlayers.Count + p.BlueTeamPlayers.Count) < 1 && !p.IsMatchMaking);
            }
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

            List<ICommand> commands = new List<ICommand>
            {
                new EntityShareCommand(BattleEntity),
                new EntityShareCommand(RoundEntity)
            };

            if (BattleMode == BattleMode.CTF)
            {
                ICommand[] flagsPedestalsICommands =
                {
                    new EntityShareCommand(RedPedestalEntity),
                    new EntityShareCommand(BluePedestalEntity),
                    new EntityShareCommand(RedFlagEntity),
                    new EntityShareCommand(BlueFlagEntity)
                };
                commands.AddRange(flagsPedestalsICommands);
            }

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

            List<ICommand> commands = new List<ICommand>
            {
                new EntityUnshareCommand(BattleEntity),
                new EntityUnshareCommand(RoundEntity)
            };

            if (BattleMode == BattleMode.CTF)
            {
                ICommand[] flagsPedestalsICommands =
                {
                    new EntityUnshareCommand(RedPedestalEntity),
                    new EntityUnshareCommand(BluePedestalEntity),
                    new EntityUnshareCommand(RedFlagEntity),
                    new EntityUnshareCommand(BlueFlagEntity)
                };
                commands.AddRange(flagsPedestalsICommands);
            }

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
                        Thread.Sleep(1000); // TODO: find a better solution for this (client crash when no delay)
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

                foreach (KeyValuePair<Type, TranslatedEvent> pair in battlePlayer.TranslatedEvents)
                {
                    CommandManager.BroadcastCommands(BattlePlayers.Where(x => x.BattlePlayer != battlePlayer).Select(x => x.Player),
                        new SendEventCommand(pair.Value.Event, pair.Value.TankPart));
                    battlePlayer.TranslatedEvents.TryRemove(pair.Key, out _);
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
            { GravityType.SUPER_EARTH, 30 },
            { GravityType.MOON, 1.62f },
            { GravityType.MARS, 3.71f }
        };

        public Entity MapEntity { get; private set; }
        public BattleMode BattleMode { get; private set; }
        public int MaxPlayers { get; private set; }
        public bool IsMatchMaking { get; }
        public int TimeLimit { get; }
        public int ScoreLimit { get; }
        public bool FriendlyFire { get; }
        public BattleState BattleState { get; private set; }
        public double CountdownTimer { get; private set; }

        public List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();

        private List<BattleLobbyPlayer> BattlePlayers { get; } = new List<BattleLobbyPlayer>();
        private List<BattleLobbyPlayer> WaitingToJoinPlayers { get; } = new List<BattleLobbyPlayer>();

        public Entity BattleEntity { get; }
        public Entity BattleLobbyEntity { get; }
        public Entity RoundEntity { get; }
        public BattleTankCollisionsComponent CollisionsComponent { get; }

        public Entity RedTeamEntity { get; }
        public Entity BlueTeamEntity { get; }
        public Entity RedPedestalEntity { get; }
        public Entity BluePedestalEntity { get; }
        public Entity RedFlagEntity { get; }
        public Entity BlueFlagEntity { get; }
        private static readonly Vector3[] PedestalsPositions = new Vector3[2];
        private static readonly Vector3[] FlagsPositions = new Vector3[2];
        public Player Owner { get; set; }
    }
}
