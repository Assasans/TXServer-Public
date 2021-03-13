using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading;
using TXServer.Core.Commands;
using TXServer.Core.Logging;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Events.Matchmaking;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

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
                if (battleParams == null) {
                    List<MapInfo> matchMakingMaps = new List<MapInfo>();
                    matchMakingMaps.AddRange(ServerConnection.ServerMapInfo.Where(p => p.Value.MatchMaking is true).Select(m => m.Value));
                    int index = new Random().Next(matchMakingMaps.Count);
                    battleParams = new ClientBattleParams(BattleMode: BattleMode.TDM, MapId: matchMakingMaps[index].MapId, MaxPlayers: 20, TimeLimit: 10, 
                        ScoreLimit: 100, FriendlyFire: false, Gravity: GravityType.EARTH, KillZoneEnabled: true, DisabledModules: false);
                }

                (MapEntity, battleParams.MaxPlayers) = ConvertMapParams(battleParams, isMatchMaking);
                BattleParams = battleParams;
                WarmUpSeconds = 60; // TODO: 1min in Bronze league, 1,5min in Silver, Gold & Master leagues

                BattleLobbyEntity = MatchMakingLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[battleParams.Gravity]);
            }
            else
            {
                (MapEntity, _) = ConvertMapParams(battleParams, isMatchMaking);
                Owner = owner;
                BattleLobbyEntity = CustomBattleLobbyTemplate.CreateEntity(battleParams, MapEntity, GravityTypes[battleParams.Gravity], owner);
                FlagsPlaced = true;

                BattleState = BattleState.CustomNotStarted;
                CountdownTimer = 0;
            }

            CreateBattle();
            BattleLobbyChatEntity = BattleLobbyChatTemplate.CreateEntity();
            GeneralBattleChatEntity = GeneralBattleChatTemplate.CreateEntity();

            if (BattleParams.BattleMode != BattleMode.DM)
                TeamBattleChatEntity = TeamBattleChatTemplate.CreateEntity();
        }
        
        public void CreateBattle()
        {
            BattleEntity = (Entity)BattleEntityCreators[BattleParams.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, BattleParams.ScoreLimit, BattleParams.TimeLimit * 60, 120 });
            RedTeamEntity = TeamTemplate.CreateEntity(TeamColor.RED, BattleEntity);
            BlueTeamEntity = TeamTemplate.CreateEntity(TeamColor.BLUE, BattleEntity);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            if (BattleParams.BattleMode == BattleMode.CTF)
            {
                RedPedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                BluePedestalEntity = PedestalTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                RedFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                BlueFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                FlagStates = new Dictionary<Entity, FlagState> { { RedFlagEntity, FlagState.Home }, { BlueFlagEntity, FlagState.Home } };
                DroppedFlags.Clear();
            }
        }

        public (Entity, int) ConvertMapParams(ClientBattleParams battleParams, bool isMatchMaking)
        {

            Entity mapEntity = Maps.GlobalItems.Rio;
            int maxPlayers = battleParams.MaxPlayers;
            foreach (PropertyInfo property in typeof(Maps.Items).GetProperties())
            {
                Entity entity = (Entity)property.GetValue(Maps.GlobalItems);
                if (entity.EntityId == battleParams.MapId)
                {
                    mapEntity = entity;
                    CurrentMapInfo = ServerConnection.ServerMapInfo[property.Name];
                    break;
                }
            }

            if (isMatchMaking)
                maxPlayers = CurrentMapInfo.MaxPlayers;

            if (battleParams.BattleMode == BattleMode.DM)
            {
                DeathmatchSpawnPoints = CurrentMapInfo.SpawnPoints.Deathmatch;
            }
            else
            {
                var teamModesSpawnPoints = new Dictionary<BattleMode, TeamSpawnPointList>
                {
                    { BattleMode.CTF, CurrentMapInfo.SpawnPoints.CaptureTheFlag },
                    { BattleMode.TDM, CurrentMapInfo.SpawnPoints.TeamDeathmatch }
                };

                TeamSpawnPoints = teamModesSpawnPoints[battleParams.BattleMode];
                // selects the spawnPoints from another team mode if there are no spawn points for the selected one
                if (TeamSpawnPoints == null)
                {
                    TeamSpawnPoints = (TeamSpawnPointList)teamModesSpawnPoints.Where(b => b.Key != battleParams.BattleMode);
                }
            }

            return (mapEntity, maxPlayers);
        }

        public void UpdateBattleParams(Player player, ClientBattleParams battleParams)
        {
            BattleMode oldBattleMode = BattleParams.BattleMode;
            BattleParams = battleParams;
            (MapEntity, _) = ConvertMapParams(battleParams, IsMatchMaking);

            foreach (Component component in new Component[]
            {
                new MapGroupComponent(MapEntity),
                new BattleModeComponent(battleParams.BattleMode),
                new UserLimitComponent(userLimit: battleParams.MaxPlayers, teamLimit: battleParams.MaxPlayers / 2),
                new GravityComponent(gravity: GravityTypes[battleParams.Gravity], gravityType: battleParams.Gravity),
                new ClientBattleParamsComponent(battleParams),
            })
            {
                BattleLobbyEntity.RemoveComponent(component.GetType());
                BattleLobbyEntity.AddComponent(component);
            }
            CreateBattle();
            foreach (BattleLobbyPlayer battleLobbyPlayer in AllBattlePlayers.ToList())
            {
                if (oldBattleMode != BattleMode.DM && BattleParams.BattleMode != BattleMode.DM)
                    if (battleLobbyPlayer.Team.GetComponent<TeamColorComponent>().TeamColor == TeamColor.RED)
                        battleLobbyPlayer.Team = RedTeamEntity;
                    else
                        battleLobbyPlayer.Team = BlueTeamEntity;
                else if (oldBattleMode == BattleMode.DM && BattleParams.BattleMode != BattleMode.DM)
                {
                    DMTeamPlayers.Clear();
                    Entity teamEntity;
                    List<BattleLobbyPlayer> teamPlayerList;
                    if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
                    {
                        teamEntity = RedTeamEntity;
                        teamPlayerList = RedTeamPlayers;
                    }
                    else
                    {
                        teamEntity = BlueTeamEntity;
                        teamPlayerList = BlueTeamPlayers;
                    }
                    battleLobbyPlayer.Team = teamEntity;
                    teamPlayerList.Add(battleLobbyPlayer);
                    battleLobbyPlayer.Player.User.ChangeComponent(teamEntity.GetComponent<TeamColorComponent>());
                }
                else
                {
                    battleLobbyPlayer.Team = null;
                    battleLobbyPlayer.Player.User.ChangeComponent(new TeamColorComponent(TeamColor.NONE));
                    BlueTeamPlayers.Clear();
                    RedTeamPlayers.Clear();
                    DMTeamPlayers.Add(battleLobbyPlayer);
                }
            }
        }
        
        public void AddPlayer(Player player)
        {
            Logger.Log($"{player}: Joined battle {BattleEntity.EntityId}");

            // prepare client
            player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
            player.ShareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
            player.User.AddComponent(new BattleLobbyGroupComponent(BattleLobbyEntity));

            if (IsMatchMaking)
                player.User.AddComponent(new MatchMakingUserComponent());

            player.ShareEntities(AllBattlePlayers.Select(x => x.User));

            Entity teamEntity = null;
            List<BattleLobbyPlayer> teamPlayerList;
            List<UserResult> teamResultList;
            if (BattleParams.BattleMode == BattleMode.DM)
            {
                teamPlayerList = DMTeamPlayers;
                teamResultList = DMTeamResults;
            }
            else if (RedTeamPlayers.Count < BlueTeamPlayers.Count)
            {
                teamEntity = RedTeamEntity;
                teamPlayerList = RedTeamPlayers;
                teamResultList = RedTeamResults;
            }
            else
            {
                teamEntity = BlueTeamEntity;
                teamPlayerList = BlueTeamPlayers;
                teamResultList = BlueTeamResults;
            }

            TeamColorComponent teamColorComponent = new TeamColorComponent(TeamColor.NONE);
            if (teamEntity != null)
                teamColorComponent = teamEntity.GetComponent<TeamColorComponent>();
            player.User.AddComponent(teamColorComponent);

            BattleLobbyPlayer battlePlayer = new BattleLobbyPlayer(player, teamEntity);
            player.BattleLobbyPlayer = battlePlayer;

            // broadcast client to other players
            AllBattlePlayers.Select(x => x.Player).ShareEntity(battlePlayer.User);

            lock (this)
                teamPlayerList.Add(battlePlayer);
            teamResultList.Add(new UserResult(player, teamResultList));

            if (IsMatchMaking && BattleState == BattleState.WarmUp || IsMatchMaking && BattleState == BattleState.Running)
            {
                player.SendEvent(new MatchMakingLobbyStartTimeEvent(new TimeSpan(0, 0, 10)), player.User);
                WaitingToJoinPlayers.Add(battlePlayer);
            }
        }

        private void RemovePlayer(BattleLobbyPlayer battlePlayer)
        {
            UserResult userResult = BlueTeamResults.Concat(RedTeamResults).Concat(DMTeamResults).Single(r => r.UserId == battlePlayer.Player.User.EntityId);
            if (DMTeamPlayers.Remove(battlePlayer))
                DMTeamResults.Remove(userResult);
            else if (RedTeamPlayers.Remove(battlePlayer))
                RedTeamResults.Remove(userResult);
            else
            {
                BlueTeamPlayers.Remove(battlePlayer);
                BlueTeamResults.Remove(userResult);
            }
            WaitingToJoinPlayers.Remove(battlePlayer);
            
            if (battlePlayer.Player == Owner)
            {
                if (AllBattlePlayers.Any())
                {
                    var allBattlePlayers = AllBattlePlayers.ToList();
                    Owner = allBattlePlayers[new Random().Next(allBattlePlayers.Count)].Player;
                    BattleLobbyEntity.RemoveComponent<UserGroupComponent>();
                    BattleLobbyEntity.AddComponent(new UserGroupComponent(Owner.User));
                }
            }

            battlePlayer.Player.BattleLobbyPlayer = null;

            battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
            battlePlayer.Player.UnshareEntity(BattleLobbyEntity);
            battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();
            battlePlayer.User.RemoveComponent<TeamColorComponent>();
            battlePlayer.Player.UnshareEntity(BattleLobbyChatEntity);

            if (IsMatchMaking)
                battlePlayer.User.RemoveComponent<MatchMakingUserComponent>();

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

            battlePlayer.Player.UnshareEntities(AllBattlePlayers.Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).UnshareEntity(battlePlayer.User);

            Logger.Log($"{battlePlayer.Player}: Left battle {BattleEntity.EntityId}");

            ServerConnection.BattlePool.RemoveAll(p => !p.AllBattlePlayers.Any() && !p.IsMatchMaking);
        }

        private void StartBattle()
        {
            if (!BattleParams.DisabledModules)
            {
                var battleModesBonusRegionsSpawnPoints = new Dictionary<BattleMode, BonusList> {
                    { BattleMode.DM, CurrentMapInfo.BonusRegions.Deathmatch },
                    { BattleMode.CTF, CurrentMapInfo.BonusRegions.CaptureTheFlag },
                    { BattleMode.TDM, CurrentMapInfo.BonusRegions.TeamDeathmatch }};
                var bonusTypeSpawnPoints = new Dictionary<BonusType, IList<Bonus>> {
                    { BonusType.ARMOR,  battleModesBonusRegionsSpawnPoints[BattleParams.BattleMode].Armor },
                    { BonusType.DAMAGE,  battleModesBonusRegionsSpawnPoints[BattleParams.BattleMode].Damage },
                    { BonusType.GOLD,  battleModesBonusRegionsSpawnPoints[BattleParams.BattleMode].Gold },
                    { BonusType.REPAIR,  battleModesBonusRegionsSpawnPoints[BattleParams.BattleMode].Repair },
                    { BonusType.SPEED,  battleModesBonusRegionsSpawnPoints[BattleParams.BattleMode].Speed }};

                BattleBonuses.Clear();
                foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType)))
                {
                    foreach (Bonus bonus in bonusTypeSpawnPoints[bonusType])
                    {
                        BattleBonus battleBonus = new(bonusType, bonus);
                        BattleBonuses.Add(battleBonus);
                    }
                }

                Random random = new();
                List<BattleBonus> supplyBonuses = new(BattleBonuses.Where(b => b.BattleBonusType != BonusType.GOLD).OrderBy(b => random.Next()));
                foreach (BattleBonus battleBonus in supplyBonuses.ToList())
                {
                    battleBonus.BonusState = BonusState.New;
                    battleBonus.BonusStateChangeCountdown = random.Next(10, 120);
                }
            }

            foreach (BattleLobbyPlayer battleLobbyPlayer in AllBattlePlayers)
                InitBattlePlayer(battleLobbyPlayer);
        }

        public void FinishBattle()
        {
            BattleState = BattleState.Ended;
            foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
            {
                battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankActiveStateComponent>();
                TeamBattleResult teamBattleResult = TeamBattleResult.DRAW;
                if (BattleParams.BattleMode != BattleMode.DM)
                {
                    Entity[] teams = { BlueTeamEntity, RedTeamEntity };
                    int allieScore = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor == battleLobbyPlayer.Player.User.GetComponent<TeamColorComponent>().TeamColor).GetComponent<TeamScoreComponent>().Score;
                    int enemyScore = teams.Single(t => t.GetComponent<TeamColorComponent>().TeamColor != battleLobbyPlayer.Player.User.GetComponent<TeamColorComponent>().TeamColor).GetComponent<TeamScoreComponent>().Score;
                    if (allieScore > enemyScore)
                        teamBattleResult = TeamBattleResult.WIN;
                    else if (enemyScore > allieScore)
                        teamBattleResult = TeamBattleResult.DEFEAT;
                }
                PersonalBattleResultForClient personalResult = new(battleLobbyPlayer.Player, teamBattleResult);
                BattleResultForClient battleResultForClient = new(this, personalResult);
                battleLobbyPlayer.Player.SendEvent(new BattleResultForClientEvent(battleResultForClient), battleLobbyPlayer.Player.User);

                BattleLeaveCounterComponent battleLeaveCounterComponent = battleLobbyPlayer.Player.User.GetComponent<BattleLeaveCounterComponent>();
                if (battleLeaveCounterComponent.Value > 0)
                    battleLeaveCounterComponent.Value -= 1;
                if (battleLeaveCounterComponent.NeedGoodBattles > 0)
                    battleLeaveCounterComponent.NeedGoodBattles -= 1;
                battleLobbyPlayer.Player.User.ChangeComponent(battleLeaveCounterComponent);
            }

            if (RoundEntity.GetComponent<RoundRestartingStateComponent>() == null)
                RoundEntity.AddComponent(new RoundRestartingStateComponent());
            if (BattleLobbyEntity.GetComponent<BattleGroupComponent>() != null)
                BattleLobbyEntity.RemoveComponent<BattleGroupComponent>();
        }

        public void InitBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.BattlePlayer = new BattlePlayer(battlePlayer, BattleEntity);

            // todo: update on equipment change
            UserResult userResult = BlueTeamResults.Concat(RedTeamResults).Concat(DMTeamResults).Single(r => r.UserId == battlePlayer.Player.User.EntityId);
            userResult.BattleUserId = battlePlayer.BattlePlayer.BattleUser.EntityId;
            userResult.WeaponId = battlePlayer.Player.CurrentPreset.Weapon.GetComponent<MarketItemGroupComponent>().Key;
            userResult.HullId = battlePlayer.Player.CurrentPreset.Hull.GetComponent<MarketItemGroupComponent>().Key;
            userResult.PaintId = battlePlayer.Player.CurrentPreset.TankPaint.GetComponent<MarketItemGroupComponent>().Key;
            userResult.CoatingId = battlePlayer.Player.CurrentPreset.WeaponPaint.GetComponent<MarketItemGroupComponent>().Key;
            userResult.HullSkinId = battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem].GetComponent<MarketItemGroupComponent>().Key;
            userResult.WeaponSkinId = battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem].GetComponent<MarketItemGroupComponent>().Key;

            battlePlayer.Player.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!BattleParams.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses)
                {
                    battlePlayer.Player.ShareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned)
                        battlePlayer.Player.ShareEntity(battleBonus.Bonus);
                }
            }

            if (BattleParams.BattleMode != BattleMode.DM)
            {
                battlePlayer.Player.ShareEntities(RedTeamEntity, BlueTeamEntity, TeamBattleChatEntity);

                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    battlePlayer.Player.ShareEntities(RedPedestalEntity, BluePedestalEntity);
                    if (!IsMatchMaking || FlagsPlaced)
                    {
                        battlePlayer.Player.ShareEntities(RedFlagEntity, BlueFlagEntity);
                    }
                }
            }

            foreach (BattleLobbyPlayer inBattlePlayer in MatchPlayers)
                battlePlayer.Player.ShareEntities(inBattlePlayer.BattlePlayer.GetEntities());

            MatchPlayers.Add(battlePlayer);

            MatchPlayers.Select(x => x.Player).ShareEntities(battlePlayer.BattlePlayer.GetEntities());
        }

        private void RemoveBattlePlayer(BattleLobbyPlayer battlePlayer)
        {
            battlePlayer.Player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!BattleParams.DisabledModules)
                foreach (BattleBonus battleBonus in BattleBonuses)
                {
                    battlePlayer.Player.UnshareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned)
                        battlePlayer.Player.UnshareEntity(battleBonus.Bonus);
                }
            if (BattleParams.BattleMode != BattleMode.DM)
            {
                battlePlayer.Player.UnshareEntities(BlueTeamEntity, RedTeamEntity, TeamBattleChatEntity);

                if (BattleParams.BattleMode == BattleMode.CTF)
                {
                    battlePlayer.Player.UnshareEntities(RedPedestalEntity, BluePedestalEntity);

                    if (!IsMatchMaking || FlagsPlaced)
                    {
                        Entity[] flags = { BlueFlagEntity, RedFlagEntity };
                        foreach (Entity flag in flags)
                        {
                            if (FlagStates[flag] == FlagState.Captured)
                            {
                                if (flag.GetComponent<TankGroupComponent>().Key == battlePlayer.BattlePlayer.Tank.GetComponent<TankGroupComponent>().Key)
                                {
                                    FlagStates[flag] = FlagState.Dropped;
                                    flag.PlayerReferences.Remove(battlePlayer.Player);
                                    DroppedFlags.Add(flag, DateTime.Now.AddMinutes(1));
                                    Vector3 flagPosition = new(battlePlayer.BattlePlayer.TankPosition.X, battlePlayer.BattlePlayer.TankPosition.Y - 1,
                                        battlePlayer.BattlePlayer.TankPosition.Z);
                                    flag.AddComponent(new FlagGroundedStateComponent());
                                    flag.ChangeComponent(new FlagPositionComponent(flagPosition));
                                    MatchPlayers.Select(x => x.Player).SendEvent(new FlagDropEvent(IsUserAction: false), flag);
                                }
                            }
                        }
                        battlePlayer.Player.UnshareEntities(RedFlagEntity, BlueFlagEntity);
                    }
                }
            }

            MatchPlayers.Select(x => x.Player).UnshareEntities(battlePlayer.BattlePlayer.GetEntities());

            MatchPlayers.Remove(battlePlayer);

            foreach (BattleLobbyPlayer inBattlePlayer in MatchPlayers)
                battlePlayer.Player.UnshareEntities(inBattlePlayer.BattlePlayer.GetEntities());

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
                    {
                        List<BattleLobbyPlayer> enemyTeamPlayers = new() { battleLobbyPlayer };
                        if (BattleParams.BattleMode != BattleMode.DM)
                        {
                            List<List<BattleLobbyPlayer>> teamPlayers = new() { BlueTeamPlayers, RedTeamPlayers };
                            enemyTeamPlayers = teamPlayers.Single(t => !(t.Contains(battleLobbyPlayer)));
                        }
                        if (IsMatchMaking && (MatchPlayers.Count != 1 || enemyTeamPlayers.Count != 0))
                        {
                            // TODO: add deserter status only when player leaves 2 out of 4 last battles prematurely & conditions above
                            BattleLeaveCounterComponent battleLeaveCounterComponent = battleLobbyPlayer.Player.User.GetComponent<BattleLeaveCounterComponent>();
                            battleLeaveCounterComponent.Value += 1;
                            battleLeaveCounterComponent.NeedGoodBattles += 2;
                            battleLobbyPlayer.Player.User.ChangeComponent(battleLeaveCounterComponent);
                        }
                        RemovePlayer(battleLobbyPlayer);
                    }
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
                    // Matchmaking only
                    if (AllBattlePlayers.Any() && (RedTeamPlayers.Count == BlueTeamPlayers.Count || DMTeamPlayers.Count >= 2))
                        BattleState = BattleState.StartCountdown;
                    break;
                case BattleState.StartCountdown:
                    // Matchmaking only

                    if (RedTeamPlayers.Count != BlueTeamPlayers.Count || (DMTeamPlayers.Any() && DMTeamPlayers.Count < 2))
                    {
                        Thread.Sleep(1000); // TODO: find a better solution for this (client crash when no delay)
                        BattleState = BattleState.NotEnoughPlayers;
                    }

                    if (CountdownTimer < 0)
                        BattleState = BattleState.Starting;

                    break;
                case BattleState.Starting:
                    if (IsMatchMaking)
                    {
                        if (RedTeamPlayers.Count != BlueTeamPlayers.Count || (DMTeamPlayers.Any() && DMTeamPlayers.Count < 2))
                        {
                            BattleState = BattleState.NotEnoughPlayers;
                            break;
                        }

                        if (CountdownTimer < 0)
                        {
                            StartBattle();
                            BattleState = BattleState.WarmUp;
                            WarmUpState = WarmUpState.WarmingUp;
                        }
                    }
                    else
                    {
                        // todo replace matchmaking component with custom battle one
                        if (!AllBattlePlayers.Any())
                        {
                            BattleState = BattleState.CustomNotStarted;
                            break;
                        }

                        if (CountdownTimer < 0)
                        {
                            StartBattle();
                            BattleLobbyEntity.AddComponent(BattleEntity.GetComponent<BattleGroupComponent>());
                            BattleState = BattleState.Running;
                        }
                    }
                    break;
                case BattleState.WarmUp:
                    // Matchmaking only
                    switch (WarmUpState)
                    {
                        case WarmUpState.WarmingUp:
                            if (CountdownTimer < 4)
                            {
                                foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                {
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankMovableComponent>();
                                    battleLobbyPlayer.BattlePlayer.Weapon.RemoveComponent<ShootableComponent>();
                                }
                                if (BattleParams.BattleMode == BattleMode.CTF)
                                {
                                    AllBattlePlayers.Select(x => x.Player).ShareEntity(RedFlagEntity);
                                    AllBattlePlayers.Select(x => x.Player).ShareEntity(BlueFlagEntity);
                                    FlagsPlaced = true;
                                }
                                WarmUpState = WarmUpState.MatchBegins;
                            }
                            break;
                        case WarmUpState.MatchBegins:
                            if (CountdownTimer < 0)
                            {
                                foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                {
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankActiveStateComponent>();
                                    battleLobbyPlayer.BattlePlayer.TankState = TankState.New;
                                    battleLobbyPlayer.BattlePlayer.Tank.RemoveComponent<TankMovementComponent>();
                                    battleLobbyPlayer.BattlePlayer.Incarnation.RemoveComponent<TankIncarnationComponent>();
                                    battleLobbyPlayer.BattlePlayer.TankState = TankState.Spawn;
                                }
                                WarmUpState = WarmUpState.Respawning;
                                CountdownTimer = 1;
                            }
                            break;
                        case WarmUpState.Respawning:
                            if (CountdownTimer < 0)
                            {
                                foreach (BattleLobbyPlayer battleLobbyPlayer in MatchPlayers)
                                    battleLobbyPlayer.BattlePlayer.Weapon.AddComponent(new ShootableComponent());

                                BattleState = BattleState.Running;
                            }
                            break;
                    }
                    break;
                case BattleState.Running:
                    if (IsMatchMaking)
                    {
                        if (!AllBattlePlayers.Any())
                        {
                            CreateBattle();
                            BattleState = BattleState.NotEnoughPlayers;
                        }
                    }
                    else if (!MatchPlayers.Any())
                    {
                        BattleState = BattleState.CustomNotStarted;
                    }
                    if (CountdownTimer < 0)
                    {
                        FinishBattle();
                    }
                    break;
                case BattleState.Ended:
                    // todo: delete battle when all players left & don't reuse it
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

        private void ProcessMatchPlayers(double deltaTime)
        {
            foreach (BattlePlayer battlePlayer in MatchPlayers.Select(x => x.BattlePlayer))
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
                            battlePlayer.Tank.AddComponent(new TankVisibleStateComponent());
                            battlePlayer.Tank.AddComponent(new TankMovableComponent());
                            break;
                        case TankState.SemiActive:
                            if (!battlePlayer.WaitingForTankActivation)
                            {
                                battlePlayer.Tank.AddComponent(new TankStateTimeOutComponent());
                                battlePlayer.WaitingForTankActivation = true;
                            }
                            break;
                        case TankState.Dead:
                            battlePlayer.TankState = TankState.Spawn;
                            battlePlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                            battlePlayer.Tank.RemoveComponent<TankMovableComponent>();
                            break;
                    }
                }

                if (battlePlayer.CollisionsPhase == CollisionsComponent.SemiActiveCollisionsPhase)
                {
                    CollisionsComponent.SemiActiveCollisionsPhase++;

                    battlePlayer.Tank.RemoveComponent<TankStateTimeOutComponent>();
                    BattleEntity.ChangeComponent(CollisionsComponent);

                    battlePlayer.TankState = TankState.Active;
                    battlePlayer.WaitingForTankActivation = false;
                }

                foreach (KeyValuePair<Type, TranslatedEvent> pair in battlePlayer.TranslatedEvents)
                {
                    (from matchPlayer in MatchPlayers
                     where matchPlayer.BattlePlayer != battlePlayer
                     select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                    battlePlayer.TranslatedEvents.TryRemove(pair.Key, out _);
                }
            
            
                // supply effects
                foreach (KeyValuePair<BonusType, double> entry in battlePlayer.SupplyEffects.ToArray())
                {
                    battlePlayer.SupplyEffects[entry.Key] -= deltaTime;

                    if (entry.Value < 0)
                    {
                        switch (entry.Key)
                        {
                            case BonusType.ARMOR:
                                battlePlayer.Tank.RemoveComponent<ArmorEffectComponent>();
                                break;
                            case BonusType.DAMAGE:
                                battlePlayer.Tank.RemoveComponent<DamageEffectComponent>();
                                break;
                            case BonusType.SPEED:
                                battlePlayer.Tank.RemoveComponent<TurboSpeedEffectComponent>();
                                battlePlayer.Tank.ChangeComponent(new SpeedComponent(9.967f, 98f, 13.205f));
                                break;
                        }
                        battlePlayer.SupplyEffects.Remove(entry.Key);
                    }
                }
            }
        }
        
        private void ProcessBonuses(double deltaTime)
        {
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.BonusState != BonusState.Unused || battleBonus.BonusState == BonusState.Spawned)
                {
                    battleBonus.BonusStateChangeCountdown -= deltaTime;
                }

                if (battleBonus.BonusStateChangeCountdown < 0)
                {
                    if (battleBonus.BonusState == BonusState.Redrop || battleBonus.BonusState == BonusState.New)
                    {
                        battleBonus.BonusState = BonusState.Spawned;
                        battleBonus.CreateBonus(BattleEntity);
                        MatchPlayers.Select(x => x.Player).ShareEntity(battleBonus.Bonus);
                        break;
                    }
                    
                }
            }
        }
        
        private void ProcessDroppedFlags()
        {
            DateTime currentTime = DateTime.Now;
            foreach (KeyValuePair<Entity, DateTime> droppedFlag in DroppedFlags.ToList())
            {
                if (DateTime.Compare(droppedFlag.Value, currentTime) <= 0 || BattleState != BattleState.Running)
                {
                    DroppedFlags.Remove(droppedFlag.Key);
                    droppedFlag.Key.RemoveComponent<TankGroupComponent>();
                    droppedFlag.Key.RemoveComponent<FlagGroundedStateComponent>();

                    Entity newFlag;
                    if (RedFlagEntity.GetComponent<TeamGroupComponent>().Key == droppedFlag.Key.GetComponent<TeamGroupComponent>().Key)
                    {
                        RedFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Red.Position, team: RedTeamEntity, battle: BattleEntity);
                        newFlag = RedFlagEntity;
                    }
                    else
                    {
                        BlueFlagEntity = FlagTemplate.CreateEntity(CurrentMapInfo.Flags.Blue.Position, team: BlueTeamEntity, battle: BattleEntity);
                        newFlag = BlueFlagEntity;
                    }
                    
                    FlagStates.Remove(droppedFlag.Key);
                    FlagStates.Add(newFlag, FlagState.Home);
                    MatchPlayers.Select(x => x.Player).SendEvent(new FlagReturnEvent(), droppedFlag.Key);
                    MatchPlayers.Select(x => x.Player).UnshareEntity(droppedFlag.Key);
                    MatchPlayers.Select(x => x.Player).ShareEntity(newFlag);

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
                ProcessMatchPlayers(deltaTime);
                ProcessBonuses(deltaTime);
                ProcessDroppedFlags();
            }
        }

        public void UpdateScore(Player player, Entity team, int additiveScore)
        {
            int oldScore = team.GetComponent<TeamScoreComponent>().Score;
            team.ChangeComponent(new TeamScoreComponent(oldScore + additiveScore));
            MatchPlayers.Select(x => x.Player).SendEvent(new RoundScoreUpdatedEvent(), RoundEntity);

            int? neededDifference = null;
            if (BattleParams.BattleMode == BattleMode.CTF)
                neededDifference = 6;
            else if (BattleParams.BattleMode == BattleMode.TDM)
                neededDifference = 30;

            if (neededDifference != null)
            {
                if (Math.Abs(BlueTeamEntity.GetComponent<TeamScoreComponent>().Score - RedTeamEntity.GetComponent<TeamScoreComponent>().Score) >= neededDifference)
                {

                    if (BattleEntity.GetComponent<RoundDisbalancedComponent>() == null)
                    {
                        Entity[] teams = { BlueTeamEntity, RedTeamEntity };
                        Entity loserTeam = teams.OrderBy(t => t.GetComponent<TeamScoreComponent>().Score).First();
                        TeamColor loserColor = loserTeam.GetComponent<TeamColorComponent>().TeamColor;
                        Component roundDisbalancedComponent = new RoundDisbalancedComponent(Loser: loserColor, InitialDominationTimerSec: 30, FinishTime: new TXDate(new TimeSpan(0, 0, 30)));

                        BattleEntity.AddComponent(roundDisbalancedComponent);
                        BattleEntity.AddComponent(new RoundComponent());
                    }
                } 
                else if (BattleEntity.GetComponent<RoundDisbalancedComponent>() != null)
                {
                    MatchPlayers.Select(x => x.Player).SendEvent(new RoundBalanceRestoredEvent(), BattleEntity);
                    BattleEntity.RemoveComponent<RoundComponent>();
                    BattleEntity.RemoveComponent<RoundDisbalancedComponent>();
                }
            }
        }
        
        public void UpdateUserStatistics(Player player, int additiveScore, int additiveKills, int additiveKillAssists, int additiveDeath)
        {
            // TODO: rank up effect/system
            RoundUserStatisticsComponent roundUserStatisticsComponent = player.BattleLobbyPlayer.BattlePlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>();
            UserExperienceComponent userExperienceComponent = player.User.GetComponent<UserExperienceComponent>();

            roundUserStatisticsComponent.ScoreWithoutBonuses += additiveScore;
            roundUserStatisticsComponent.Kills += additiveKills;
            roundUserStatisticsComponent.KillAssists += additiveKillAssists;
            roundUserStatisticsComponent.Deaths += additiveDeath;

            player.BattleLobbyPlayer.BattlePlayer.RoundUser.ChangeComponent(roundUserStatisticsComponent);

            MatchPlayers.Select(x => x.Player).SendEvent(new RoundUserStatisticsUpdatedEvent(), player.BattleLobbyPlayer.BattlePlayer.RoundUser);
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

        public ClientBattleParams BattleParams { get; set; }
        public int WarmUpSeconds { get; set; }
        public Entity MapEntity { get; private set; }
        public bool IsMatchMaking { get; }
        public bool IsOpen { get; set; }
        private bool FlagsPlaced { get; set; }

        public MapInfo CurrentMapInfo { get; set; }
        public IList<SpawnPoint> DeathmatchSpawnPoints { get; set; }
        public TeamSpawnPointList TeamSpawnPoints { get; set; }
        public List<BattleBonus> BattleBonuses { get; set; } = new List<BattleBonus>();

        public BattleState BattleState
        {
            get => _BattleState;
            set
            {
                switch (_BattleState)
                {
                    case BattleState.StartCountdown:
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartTimeComponent>();
                        break;
                    case BattleState.Starting:
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                        break;
                    case BattleState.WarmUp:
                        RoundEntity.RemoveComponent<RoundWarmingUpStateComponent>();
                        break;
                }

                switch (value)
                {
                    case BattleState.StartCountdown:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartTimeComponent(new TimeSpan(0, 0, 10)));
                        CountdownTimer = 10;
                        break;
                    case BattleState.Starting:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartingComponent());
                        CountdownTimer = 3;
                        break;
                    case BattleState.WarmUp:
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(new DateTimeOffset(DateTime.Now.AddSeconds(WarmUpSeconds))));
                        RoundEntity.AddComponent(new RoundWarmingUpStateComponent());
                        CountdownTimer = WarmUpSeconds;
                        break;
                    case BattleState.Running:
                        CountdownTimer = 60 * BattleParams.TimeLimit;
                        break;
                }

                _BattleState = value;
            }
        }
        private BattleState _BattleState;

        public WarmUpState WarmUpState { get; set; }
        public double CountdownTimer { get; set; }

        // All players (not only in match)
        public List<BattleLobbyPlayer> RedTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> BlueTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public List<BattleLobbyPlayer> DMTeamPlayers { get; } = new List<BattleLobbyPlayer>();
        public IEnumerable<BattleLobbyPlayer> AllBattlePlayers => RedTeamPlayers.Concat(BlueTeamPlayers).Concat(DMTeamPlayers);
        public List<UserResult> RedTeamResults = new();
        public List<UserResult> BlueTeamResults = new();
        public List<UserResult> DMTeamResults = new();

        public List<BattleLobbyPlayer> MatchPlayers { get; } = new List<BattleLobbyPlayer>();
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
        public Dictionary<Entity, FlagState> FlagStates { get; set; }
        public Dictionary<Entity, DateTime> DroppedFlags { get; } = new Dictionary<Entity, DateTime> { };
        public Player Owner { get; set; }
    }
}
