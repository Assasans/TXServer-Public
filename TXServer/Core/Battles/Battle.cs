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
using TXServer.ECSSystem.Components.Battle.Team;

namespace TXServer.Core.Battles
{
    public class Battle
    {
        public Battle(ClientBattleParams battleParams, bool isMatchMaking, Player owner)
        {

            BattleParams = battleParams;
            IsMatchMaking = isMatchMaking;

            if (isMatchMaking)
            {
                int index = new Random().Next(MatchMakingMaps.Count);
                if (battleParams == null) {
                    battleParams = new ClientBattleParams(BattleMode: BattleMode.CTF, MapId: MatchMakingMaps[index].EntityId, MaxPlayers: 20, TimeLimit: 10, 
                        ScoreLimit: 100, FriendlyFire: false, Gravity: GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }
                var tuple = ConvertMapParams(battleParams, isMatchMaking);
                MapEntity = tuple.Item1;
                MapEntity = Maps.GlobalItems.Rio; // our test map
                battleParams.MaxPlayers = tuple.Item2;
                BattleParams = battleParams;
                BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[(GravityType)battleParams.Gravity]);
            }
            else
            {
                var tuple = ConvertMapParams(battleParams, isMatchMaking);
                MapEntity = tuple.Item1;
                Owner = owner;
                BattleLobbyEntity = CustomBattleLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[(GravityType)battleParams.Gravity], owner);
            }

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, 5, 600, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                // Rio pedestals/flags positions
                // TODO: read specific positions from json
                PedestalsPositions[0] = new Vector3(-6, 0, -65);
                PedestalsPositions[1] = new Vector3(13, 0, 65);
                FlagsPositions[0] = new Vector3(-6, 0, -65);
                FlagsPositions[1] = new Vector3(13, 0, 65);

                RedPedestalEntity = PedestalTemplate.CreateEntity(PedestalsPositions[0], RedTeamEntity);
                BluePedestalEntity = PedestalTemplate.CreateEntity(PedestalsPositions[1], BlueTeamEntity);
                RedFlagEntity = FlagTemplate.CreateEntity(FlagsPositions[0], RedTeamEntity, new FlagHomeStateComponent());
                BlueFlagEntity = FlagTemplate.CreateEntity(FlagsPositions[1], BlueTeamEntity, new FlagHomeStateComponent());
            }
        }

        public Tuple<Entity, int> ConvertMapParams(ClientBattleParams battleParams, bool isMatchMaking)
        {
            Entity mapEntity = Maps.GlobalItems.Rio;
            int maxPlayers = battleParams.MaxPlayers;
            switch (battleParams.MapId)
            {
                case -321842153:
                    mapEntity = Maps.GlobalItems.Silence;
                    break;
                case 343745828:
                    mapEntity = Maps.GlobalItems.Nightiran;
                    break;
                case 485053206:
                    mapEntity = Maps.GlobalItems.Acidlake;
                    break;
                case -820833801:
                    mapEntity = Maps.GlobalItems.Acidlakehalloween;
                    break;
                case 458045295:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Testbox;
                    break;
                case -549069251:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Sandbox;
                    break;
                case -51480736:
                    mapEntity = Maps.GlobalItems.Iran;
                    break;
                case 1133979230:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Area159;
                    break;
                case -1587964040:
                    mapEntity = Maps.GlobalItems.Repin;
                    break;
                case 980475942:
                    mapEntity = Maps.GlobalItems.Westprime;
                    break;
                case 1945237110:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Boombox;
                    break;
                case 933129112:
                    mapEntity = Maps.GlobalItems.Silencemoon;
                    break;
                case -1664220274:
                    mapEntity = Maps.GlobalItems.Rio;
                    break;
                case 989096365:
                    mapEntity = Maps.GlobalItems.MassacremarsBG;
                    break;
                case -1551247853:
                    mapEntity = Maps.GlobalItems.Massacre;
                    break;
                case 2127033418:
                    mapEntity = Maps.GlobalItems.Kungur;
                    break;
            }
            return new Tuple<Entity, int>(mapEntity, maxPlayers);
        }

        public void UpdateBattleParams(Player player, ClientBattleParams battleParams)
        {
            BattleParams = battleParams;
            var tuple = ConvertMapParams(battleParams, IsMatchMaking);
            MapEntity = tuple.Item1;

            CommandManager.SendCommands(player,
                new ComponentRemoveCommand(BattleLobbyEntity, typeof(MapGroupComponent)),
                new ComponentAddCommand(BattleLobbyEntity, new MapGroupComponent(MapEntity)),
                new ComponentRemoveCommand(BattleLobbyEntity, typeof(BattleModeComponent)),
                new ComponentAddCommand(BattleLobbyEntity, new BattleModeComponent(battleParams.BattleMode)),
                new ComponentRemoveCommand(BattleLobbyEntity, typeof(UserLimitComponent)),
                new ComponentAddCommand(BattleLobbyEntity, new UserLimitComponent(userLimit: battleParams.MaxPlayers, teamLimit: battleParams.MaxPlayers / 2)),
                new ComponentRemoveCommand(BattleLobbyEntity, typeof(GravityComponent)),
                new ComponentAddCommand(BattleLobbyEntity, new GravityComponent(gravity: GravityTypes[battleParams.Gravity], gravityType: battleParams.Gravity)),
                new ComponentRemoveCommand(BattleLobbyEntity, typeof(ClientBattleParamsComponent)),
                new ComponentAddCommand(BattleLobbyEntity, new ClientBattleParamsComponent(battleParams)));

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, 5, 600, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleParams.BattleMode != BattleMode.DM)
            {    
                // Rio pedestals/flags positions
                // TODO: read specific positions from json
                PedestalsPositions[0] = new Vector3(-6, 0, -65);
                PedestalsPositions[1] = new Vector3(13, 0, 65);
                FlagsPositions[0] = new Vector3(-6, 0, -65);
                FlagsPositions[1] = new Vector3(13, 0, 65);

                RedPedestalEntity = PedestalTemplate.CreateEntity(PedestalsPositions[0], RedTeamEntity);
                BluePedestalEntity = PedestalTemplate.CreateEntity(PedestalsPositions[1], BlueTeamEntity);
                RedFlagEntity = FlagTemplate.CreateEntity(FlagsPositions[0], RedTeamEntity, new FlagHomeStateComponent());
                BlueFlagEntity = FlagTemplate.CreateEntity(FlagsPositions[1], BlueTeamEntity, new FlagHomeStateComponent());

                foreach (BattleLobbyPlayer battleLobbyPlayer in RedTeamPlayers.Concat(BlueTeamPlayers))
                {
                    if (battleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor == TeamColor.RED)
                    {
                        battleLobbyPlayer.Team = RedTeamEntity;
                    }
                    else
                    {
                        battleLobbyPlayer.Team = BlueTeamEntity;
                    }
                }
            }
        }
        
        public void AddPlayer(Player player)
        {
            // prepare client
            List<ICommand> commands = new List<ICommand>
            {
                new EntityShareCommand(BattleLobbyEntity),
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
            
            // transfers Owner ship to a random player in the lobby
            if (battlePlayer.Player == Owner)
            {
                if (RedTeamPlayers.Concat(BlueTeamPlayers).Any())
                {
                    var allBattlePlayers = RedTeamPlayers.Concat(BlueTeamPlayers).ToList();
                    Owner = allBattlePlayers[new Random().Next(allBattlePlayers.Count)].Player;
                    CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                        new ComponentRemoveCommand(BattleLobbyEntity, typeof(UserGroupComponent)),
                        new ComponentAddCommand(BattleLobbyEntity, new UserGroupComponent(Owner.User)));
                }
            }

            battlePlayer.Player.BattleLobbyPlayer = null;

            List<ICommand> commands = new List<ICommand>
            {
                new EntityUnshareCommand(BattleLobbyEntity),
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

        public void InitBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.BattlePlayer = new BattlePlayer(battlePlayer, BattleEntity);
            List<ICommand> commands = new List<ICommand>
            {
                new EntityShareCommand(BattleEntity),
                new EntityShareCommand(RoundEntity),
                new EntityShareCommand(RedTeamEntity),
                new EntityShareCommand(BlueTeamEntity)
            };

            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                commands.AddRange(new List<ICommand>
                {
                    new EntityShareCommand(RedPedestalEntity),
                    new EntityShareCommand(BluePedestalEntity),
                    new EntityShareCommand(RedFlagEntity),
                    new EntityShareCommand(BlueFlagEntity)
                });
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
                new EntityUnshareCommand(RoundEntity),
                new EntityUnshareCommand(RedTeamEntity),
                new EntityUnshareCommand(BlueTeamEntity)
            };

            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                commands.AddRange(new List<ICommand>
                {
                    new EntityUnshareCommand(RedPedestalEntity),
                    new EntityUnshareCommand(BluePedestalEntity),
                    new EntityUnshareCommand(RedFlagEntity),
                    new EntityUnshareCommand(BlueFlagEntity)
                });
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
                    if (IsMatchMaking && RedTeamPlayers.Count + BlueTeamPlayers.Count > 0 && RedTeamPlayers.Count == BlueTeamPlayers.Count)
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
                    if (!IsMatchMaking && RedTeamPlayers.Count + BlueTeamPlayers.Count == 0)
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

                    if (IsMatchMaking && CountdownTimer < 0)
                    {
                        CommandManager.SendCommandsSafe(RedTeamPlayers[0].Player,
                            new ComponentRemoveCommand(BattleLobbyEntity, typeof(MatchMakingLobbyStartingComponent)));
                        StartBattle();
                        BattleState = BattleState.Running;
                    }
                    
                    if (!IsMatchMaking)
                    {
                        StartBattle();
                        BattleState = BattleState.Running;
                    }
                    break;
                case BattleState.WarmingUp:
                    break;
                case BattleState.Running:
                    if (IsMatchMaking && BattlePlayers.Count + WaitingToJoinPlayers.Count == 0)
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
        private static readonly List<Entity> MatchMakingMaps = new List<Entity> { Maps.GlobalItems.Silence, Maps.GlobalItems.Nightiran, Maps.GlobalItems.Acidlake, 
            Maps.GlobalItems.Sandbox, Maps.GlobalItems.Iran, Maps.GlobalItems.Area159, Maps.GlobalItems.Repin, Maps.GlobalItems.Westprime, Maps.GlobalItems.Boombox, 
            Maps.GlobalItems.Silencemoon, Maps.GlobalItems.Rio, Maps.GlobalItems.MassacremarsBG, Maps.GlobalItems.Massacre, Maps.GlobalItems.Kungur
    };
        public ClientBattleParams BattleParams { get; set; }
        public Entity MapEntity { get; private set; }
        public bool IsMatchMaking { get; }
        public bool IsOpen { get; set; }
        public BattleState BattleState { get; set; }
        public double CountdownTimer { get; private set; }

        public List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();

        private List<BattleLobbyPlayer> BattlePlayers { get; } = new List<BattleLobbyPlayer>();
        private List<BattleLobbyPlayer> WaitingToJoinPlayers { get; } = new List<BattleLobbyPlayer>();

        public Entity BattleEntity { get; set; }
        public Entity BattleLobbyEntity { get; set; }
        public Entity RoundEntity { get; set; }
        public BattleTankCollisionsComponent CollisionsComponent { get; set; }

        public Entity RedTeamEntity { get; set; }
        public Entity BlueTeamEntity { get; set; }
        public Entity RedPedestalEntity { get; set; }
        public Entity BluePedestalEntity { get; set; }
        public Entity RedFlagEntity { get; set; }
        public Entity BlueFlagEntity { get; set; }
        private static readonly Vector3[] PedestalsPositions = new Vector3[2];
        private static readonly Vector3[] FlagsPositions = new Vector3[2];
        public Player Owner { get; set; }
    }
}
