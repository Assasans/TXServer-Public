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
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Components.Battle.Round;

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
                battleParams.MapId = -1664220274; // our test map RIO
                var tuple = ConvertMapParams(battleParams, isMatchMaking);
                MapEntity = tuple.Item1;
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

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, battleParams.ScoreLimit, battleParams.TimeLimit*60, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            BattleLobbyChatEntity = BattleLobbyChatTemplate.CreateEntity();
            GeneralBattleChatEntity = GeneralBattleChatTemplate.CreateEntity();

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                TeamBattleChatEntity = TeamBattleChatTemplate.CreateEntity();
                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    RedPedestalEntity = PedestalTemplate.CreateEntity(MapCoordinates.flags.flagRed.position.V3, RedTeamEntity, battle: BattleEntity);
                    BluePedestalEntity = PedestalTemplate.CreateEntity(MapCoordinates.flags.flagBlue.position.V3, BlueTeamEntity, battle: BattleEntity);
                    RedFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagRed.position.V3, team: RedTeamEntity, battle: BattleEntity);
                    BlueFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagBlue.position.V3, team: BlueTeamEntity, battle: BattleEntity);
                }
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
                    MapCoordinates = ServerConnection.AllCoordinates.Silence;
                    break;
                case 343745828:
                    mapEntity = Maps.GlobalItems.Nightiran;
                    MapCoordinates = ServerConnection.AllCoordinates.Nightiran;
                    break;
                case 485053206:
                    mapEntity = Maps.GlobalItems.Acidlake;
                    MapCoordinates = ServerConnection.AllCoordinates.Acidlake;
                    break;
                case -820833801:
                    mapEntity = Maps.GlobalItems.Acidlakehalloween;
                    MapCoordinates = ServerConnection.AllCoordinates.Acidlakehalloween;
                    break;
                case 458045295:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Testbox;
                    MapCoordinates = ServerConnection.AllCoordinates.Testbox;
                    break;
                case -549069251:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Sandbox;
                    MapCoordinates = ServerConnection.AllCoordinates.Sandbox;
                    break;
                case -51480736:
                    mapEntity = Maps.GlobalItems.Iran;
                    MapCoordinates = ServerConnection.AllCoordinates.Iran;
                    break;
                case 1133979230:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Area159;
                    MapCoordinates = ServerConnection.AllCoordinates.Area159;
                    break;
                case -1587964040:
                    mapEntity = Maps.GlobalItems.Repin;
                    MapCoordinates = ServerConnection.AllCoordinates.Repin;
                    break;
                case 980475942:
                    mapEntity = Maps.GlobalItems.Westprime;
                    MapCoordinates = ServerConnection.AllCoordinates.Westprime;
                    break;
                case 1945237110:
                    if (isMatchMaking) { maxPlayers = 8; }
                    mapEntity = Maps.GlobalItems.Boombox;
                    MapCoordinates = ServerConnection.AllCoordinates.Boombox;
                    break;
                case 933129112:
                    mapEntity = Maps.GlobalItems.Silencemoon;
                    MapCoordinates = ServerConnection.AllCoordinates.Silencemoon;
                    break;
                case -1664220274:
                    mapEntity = Maps.GlobalItems.Rio;
                    MapCoordinates = ServerConnection.AllCoordinates.Rio;
                    break;
                case 989096365:
                    mapEntity = Maps.GlobalItems.MassacremarsBG;
                    MapCoordinates = ServerConnection.AllCoordinates.MassacremarsBG;
                    break;
                case -1551247853:
                    mapEntity = Maps.GlobalItems.Massacre;
                    MapCoordinates = ServerConnection.AllCoordinates.Massacre;
                    break;
                case 2127033418:
                    mapEntity = Maps.GlobalItems.Kungur;
                    MapCoordinates = ServerConnection.AllCoordinates.Kungur;
                    break;
            }

            if (battleParams.BattleMode == BattleMode.DM)
            {
                DeathmatchSpawnPoints = MapCoordinates.spawnPoints.deathmatch;
            }
            else
            {
                var teamModesSpawnPoints = new Dictionary<BattleMode, Coordinates.teamBattleSpawnPoints>
                {
                    { BattleMode.CTF, MapCoordinates.spawnPoints.captureTheFlag },
                    { BattleMode.TDM, MapCoordinates.spawnPoints.teamDeathmatch }
                };

                TeamsSpawnPoints = teamModesSpawnPoints[battleParams.BattleMode];
                // selects the spawnPoints from another team mode if there are no spawn points for the selected one
                if (TeamsSpawnPoints == null)
                {
                    TeamsSpawnPoints = (Coordinates.teamBattleSpawnPoints)teamModesSpawnPoints.Where(b => b.Key != battleParams.BattleMode);
                }
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

            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, battleParams.ScoreLimit, battleParams.TimeLimit*60, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    RedPedestalEntity = PedestalTemplate.CreateEntity(MapCoordinates.flags.flagRed.position.V3, team:RedTeamEntity, battle:BattleEntity);
                    BluePedestalEntity = PedestalTemplate.CreateEntity(MapCoordinates.flags.flagBlue.position.V3, team:BlueTeamEntity, battle:BattleEntity);
                    RedFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagRed.position.V3, team: RedTeamEntity, battle: BattleEntity);
                    BlueFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagBlue.position.V3, team: BlueTeamEntity, battle: BattleEntity);
                }
                
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
                new EntityShareCommand(BattleLobbyChatEntity),
                new ComponentAddCommand(player.User, new BattleLobbyGroupComponent(BattleLobbyEntity)),
            };
            if (IsMatchMaking)
                commands.Add(new ComponentAddCommand(player.User, new MatchMakingUserComponent()));

            foreach (BattleLobbyPlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers))
            {
                commands.Add(new EntityShareCommand(existingBattlePlayer.User));
            }

            BattleLobbyPlayer battlePlayer;
            List<BattleLobbyPlayer> selectedTeam;
            if (BattleParams.BattleMode == BattleMode.DM)
            {
                battlePlayer = new BattleLobbyPlayer(player, null);
                selectedTeam = DMTeamPlayers;
                commands.Add(new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.NONE)));
            }
            else
            {
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
            }

            player.BattleLobbyPlayer = battlePlayer;
            CommandManager.SendCommands(player, commands);

            // broadcast client to other players
            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers).Select(x => x.Player),
                new EntityShareCommand(battlePlayer.User));

            lock (this)
                selectedTeam.Add(battlePlayer);

            if (IsMatchMaking && BattleState == BattleState.WarmingUp || IsMatchMaking && BattleState == BattleState.Running)
            {
                CommandManager.SendCommands(player, new SendEventCommand(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), player.User));
                WaitingToJoinPlayers.Add(battlePlayer);
            }
        }

        private void RemovePlayer(BattleLobbyPlayer battlePlayer)
        {
            if (BattleParams.BattleMode == BattleMode.DM) { 
                DMTeamPlayers.Remove(battlePlayer); }
            else {
                if (!RedTeamPlayers.Remove(battlePlayer))
                    BlueTeamPlayers.Remove(battlePlayer);}  
            WaitingToJoinPlayers.Remove(battlePlayer);
            
            // transfers owner ship to a random player in the lobby
            if (battlePlayer.Player == Owner)
            {
                if (RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers).Any())
                {
                    var allBattlePlayers = RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers).ToList();
                    Owner = allBattlePlayers[new Random().Next(allBattlePlayers.Count)].Player;
                    CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers).Select(x => x.Player),
                        new ComponentRemoveCommand(BattleLobbyEntity, typeof(UserGroupComponent)),
                        new ComponentAddCommand(BattleLobbyEntity, new UserGroupComponent(Owner.User)));
                }
            }

            battlePlayer.Player.BattleLobbyPlayer = null;

            List<ICommand> commands = new List<ICommand>
            {
                new EntityUnshareCommand(BattleLobbyEntity),
                new ComponentRemoveCommand(battlePlayer.User, typeof(BattleLobbyGroupComponent)),
                new ComponentRemoveCommand(battlePlayer.User, typeof(TeamColorComponent)),
                new EntityUnshareCommand(BattleLobbyChatEntity)
            };

            if (IsMatchMaking)
                commands.Add(new ComponentRemoveCommand(battlePlayer.User, typeof(MatchMakingUserComponent)));

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
            {
                commands.Add(new ComponentRemoveCommand(battlePlayer.User, typeof(MatchMakingUserReadyComponent)));
            }

            foreach (BattleLobbyPlayer existingBattlePlayer in RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers))
            {
                commands.Add(new EntityUnshareCommand(existingBattlePlayer.User));
            }

            CommandManager.SendCommandsSafe(battlePlayer.Player, commands);
            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers).Select(x => x.Player),
                new EntityUnshareCommand(battlePlayer.User));

            ServerConnection.BattlePool.RemoveAll(p => (p.RedTeamPlayers.Count + p.BlueTeamPlayers.Count + p.DMTeamPlayers.Count) < 1 && !p.IsMatchMaking);
        }

        private void StartBattle()
        {
            if (!IsMatchMaking)
            {
                CommandManager.SendCommands(Owner,
                    new ComponentRemoveCommand(BattleLobbyEntity, typeof(ClientBattleParamsComponent)));
            }
            foreach (BattleLobbyPlayer battleLobbyPlayer in RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers))
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
                new EntityShareCommand(GeneralBattleChatEntity)
            };
            if (BattleParams.BattleMode != BattleMode.DM)
            {
                commands.AddRange(new List<ICommand>
                {
                    new EntityShareCommand(BlueTeamEntity),
                    new EntityShareCommand(RedTeamEntity),
                    new EntityShareCommand(TeamBattleChatEntity)
                });

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

            List<ICommand> commands = new List<ICommand>
            {
                new EntityUnshareCommand(BattleEntity),
                new EntityUnshareCommand(RoundEntity),
                new EntityUnshareCommand(GeneralBattleChatEntity)
            };

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                commands.AddRange(new List<ICommand>
                {
                    new EntityUnshareCommand(RedTeamEntity),
                    new EntityUnshareCommand(BlueTeamEntity),
                    new EntityUnshareCommand(TeamBattleChatEntity)
                });

                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    Entity[] flags = { BlueFlagEntity, RedFlagEntity };
                    foreach (Entity flag in flags)
                    {
                        if (flag.GetComponent<TankGroupComponent>() != null && flag.GetComponent<FlagGroundedStateComponent>() == null)
                        {
                            if (flag.GetComponent<TankGroupComponent>().Key == battlePlayer.BattlePlayer.Tank.GetComponent<TankGroupComponent>().Key)
                            {
                                commands.Add(new ComponentAddCommand(flag, new FlagGroundedStateComponent()));
                                // TODO: drop flag at latest tank position
                                commands.Add(new ComponentChangeCommand(flag, new FlagPositionComponent(new Vector3(x: 0, y: 3, z: 0))));

                                CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                                    new SendEventCommand(new FlagDropEvent(IsUserAction: false), flag));
                            }
                        }
                    }
                }
            }

            CommandManager.BroadcastCommands(BattlePlayers.Select(x => x.Player), battlePlayer.BattlePlayer.GetEntities().Select(x => new EntityUnshareCommand(x)));

            BattlePlayers.Remove(battlePlayer);

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
            for (int i = 0; i < RedTeamPlayers.Count + BlueTeamPlayers.Count + DMTeamPlayers.Count; i++)
            {
                BattleLobbyPlayer battleLobbyPlayer;
                if (BattleParams.BattleMode == BattleMode.DM)
                {
                    battleLobbyPlayer = DMTeamPlayers[i];
                }
                else
                {
                    if (i < RedTeamPlayers.Count)
                        battleLobbyPlayer = RedTeamPlayers[i];
                    else
                        battleLobbyPlayer = BlueTeamPlayers[i - RedTeamPlayers.Count];
                }

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
                    
                    if (!IsMatchMaking)
                    {
                        BattleState = BattleState.CustomNotStarted;
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
                    if (!IsMatchMaking && RedTeamPlayers.Count + BlueTeamPlayers.Count + DMTeamPlayers.Count == 0)
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

        private void ProcessDroppedFlags()
        {
            DateTime currentTime = DateTime.Now;
            foreach (KeyValuePair<Entity, DateTime> entry in DroppedFlags.ToList())
            {
                if (DateTime.Compare(entry.Value, currentTime) <= 0)
                {
                    DroppedFlags.Remove(entry.Key);
                    CommandManager.SendCommands((BlueTeamPlayers.Concat(RedTeamPlayers)).First().Player,
                        new ComponentRemoveCommand(entry.Key, typeof(TankGroupComponent)),
                        new ComponentRemoveCommand(entry.Key, typeof(FlagGroundedStateComponent)));
                    Entity newFlag;
                    if (RedFlagEntity.GetComponent<TeamGroupComponent>().Key == entry.Key.GetComponent<TeamGroupComponent>().Key)
                    {
                        RedFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagRed.position.V3, team: RedTeamEntity, battle: BattleEntity);
                        newFlag = RedFlagEntity;
                    }
                    else
                    {
                        BlueFlagEntity = FlagTemplate.CreateEntity(MapCoordinates.flags.flagBlue.position.V3, team: BlueTeamEntity, battle: BattleEntity);
                        newFlag = BlueFlagEntity;
                    }
                    CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                        new SendEventCommand(new FlagReturnEvent(), entry.Key),
                        new EntityUnshareCommand(entry.Key),
                        new EntityShareCommand(newFlag));
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
                ProcessDroppedFlags();
            }
        }

        public void UpdatedScore(Player player)
        {

            int? neededDifference = null;
            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                neededDifference = 6;
            } else
            {
                if (BattleParams.BattleMode == BattleMode.TDM)
                {
                    neededDifference = 30;
                }
            }

            if (neededDifference != null)
            {
                if (Math.Abs(BlueTeamEntity.GetComponent<TeamScoreComponent>().Score - RedTeamEntity.GetComponent<TeamScoreComponent>().Score) >= neededDifference)
                {

                    if (BattleEntity.GetComponent<RoundDisbalancedComponent>() == null)
                    {
                        Entity[] teams = { BlueTeamEntity, RedTeamEntity };
                        Entity loserTeam = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor != player.BattleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor);
                        TeamColor loserColor = loserTeam.GetComponent<TeamColorComponent>().TeamColor;

                        Component roundDisbalancedComponent = new RoundDisbalancedComponent(Loser: loserColor, InitialDominationTimerSec: 30, FinishTime: new TXDate(new TimeSpan(0, 0, 30)));

                        CommandManager.SendCommands(player,
                            new ComponentAddCommand(BattleEntity, roundDisbalancedComponent));
                    }
                } 

                else if (BattleEntity.GetComponent<RoundDisbalancedComponent>() != null)
                {
                    //TODO: restore round balance
                }
            }
        }
        
        public void UpdateUserStatistics(Player player, int xp, int kills, int killAssists, int death)
        {
            // TODO: rank up effect/system
            RoundUserStatisticsComponent roundUserStatisticsComponent = player.BattleLobbyPlayer.BattlePlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>();
            UserExperienceComponent userExperienceComponent = player.User.GetComponent<UserExperienceComponent>();

            roundUserStatisticsComponent.ScoreWithoutBonuses += xp;
            roundUserStatisticsComponent.Kills += kills;
            roundUserStatisticsComponent.KillAssists += killAssists;
            roundUserStatisticsComponent.Deaths += death;

            userExperienceComponent.Experience += xp;

            CommandManager.SendCommands(player,
                new ComponentChangeCommand(player.BattleLobbyPlayer.BattlePlayer.RoundUser, roundUserStatisticsComponent),
                new ComponentChangeCommand(player.User, userExperienceComponent));

            CommandManager.BroadcastCommands(RedTeamPlayers.Concat(BlueTeamPlayers).Select(x => x.Player),
                new SendEventCommand(new RoundUserStatisticsUpdatedEvent(), player.BattleLobbyPlayer.BattlePlayer.RoundUser));
        }
        
        private static readonly Dictionary<BattleMode, Type> BattleEntityCreators = new Dictionary<BattleMode, Type>
        {
            { BattleMode.DM, typeof(DMTemplate) },
            { BattleMode.TDM, typeof(TDMTemplate) },
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
        public Coordinates.map MapCoordinates { get; set; }
        public IList<Coordinates.spawnCoordinate> DeathmatchSpawnPoints { get; set; }
        public Coordinates.teamBattleSpawnPoints TeamsSpawnPoints { get; set; }
        public BattleState BattleState { get; set; }
        public double CountdownTimer { get; private set; }

        public List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> DMTeamPlayers { get; } = new List<BattleLobbyPlayer>();

        private List<BattleLobbyPlayer> BattlePlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> WaitingToJoinPlayers { get; } = new List<BattleLobbyPlayer>();

        public Entity BattleEntity { get; set; }
        public Entity BattleLobbyEntity { get; set; }
        public Entity RoundEntity { get; set; }
        public BattleTankCollisionsComponent CollisionsComponent { get; set; }

        public Entity GeneralBattleChatEntity { get; set; }
        public Entity TeamBattleChatEntity { get; set; }
        public Entity BattleLobbyChatEntity { get; set; }

        public Entity RedTeamEntity { get; set; }
        public Entity BlueTeamEntity { get; set; }
        public Entity RedPedestalEntity { get; set; }
        public Entity BluePedestalEntity { get; set; }
        public Entity RedFlagEntity { get; set; }
        public Entity BlueFlagEntity { get; set; }
        public Dictionary<Entity, DateTime> DroppedFlags = new Dictionary<Entity, DateTime> { };
        public long? FlagBlockedTankKey { get; set; }
        public Player Owner { get; set; }
    }
}
