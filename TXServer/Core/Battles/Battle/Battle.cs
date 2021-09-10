using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Battles.Matchmaking;
using TXServer.Core.HeightMaps;
using TXServer.Core.Logging;
using TXServer.Core.ServerMapInformation;
using TXServer.Core.Squads;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.BattleMode;
using TXServer.ECSSystem.EntityTemplates.Chat;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Types.Battle;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public Battle(ClientBattleParams battleParams, bool isMatchMaking, Player owner)
        {
            Params = battleParams;
            IsMatchMaking = isMatchMaking;

            if (Params is not null)
                ExtendedBattleMode = (ExtendedBattleMode) (int) Params.BattleMode;

            if (isMatchMaking)
                TypeHandler = new MatchMakingBattleHandler { Battle = this };
            else
                TypeHandler = new CustomBattleHandler { Battle = this, Owner = owner };
            TypeHandler.SetupBattle();

            CreateBattle();
            BattleLobbyChatEntity = BattleLobbyChatTemplate.CreateEntity();
            GeneralBattleChatEntity = GeneralBattleChatTemplate.CreateEntity();

            _tickHandlers = new List<TickHandler>();
            _nextTickHandlers = new List<Action>();
        }

        public void CreateBattle()
        {
            BattleEntity = BattleEntityCreators[Params.BattleMode](BattleLobbyEntity, Params.ScoreLimit, Params.TimeLimit * 60, 60);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            IBattleModeHandler prevHandler = ModeHandler;
            ModeHandler = ExtendedBattleMode switch
            {
                ExtendedBattleMode.CTF => new CTFHandler { Battle = this },
                ExtendedBattleMode.HPS => new HPSHandler { Battle = this },
                ExtendedBattleMode.TDM => new TDMHandler { Battle = this },
                ExtendedBattleMode.DM or _ => new DMHandler { Battle = this }
            };

            if (prevHandler != null)
                ModeHandler.SetupBattle(prevHandler);
            else
                ModeHandler.SetupBattle();

            if (!Server.Instance.Settings.DisableHeightMaps)
                HeightMap = ServerConnection.HeightMaps[
                    ServerConnection.ServerMapInfo.First(entry => entry.Value.MapId == Params.MapId).Key];

        }

        private (Entity, int) ConvertMapParams(ClientBattleParams newParams, bool isMatchMaking)
        {
            Entity mapEntity = Maps.GlobalItems.Rio;
            int maxPlayers = newParams.MaxPlayers;
            foreach (PropertyInfo property in typeof(Maps.Items).GetProperties())
            {
                Entity entity = (Entity)property.GetValue(Maps.GlobalItems);
                if (entity.EntityId == newParams.MapId)
                {
                    mapEntity = entity;
                    CurrentMapInfo = ServerConnection.ServerMapInfo[property.Name];
                    break;
                }
            }

            if (isMatchMaking)
                maxPlayers = CurrentMapInfo.MaxPlayers;

            return (mapEntity, maxPlayers);
        }

        public void UpdateParams(ClientBattleParams @params, bool suppressModeUpdate = false)
        {
            Params = @params;
            if (!suppressModeUpdate)
                ExtendedBattleMode = (ExtendedBattleMode) (int) Params.BattleMode;
            (MapEntity, _) = ConvertMapParams(@params, IsMatchMaking);

            List<Component> paramComponents = new(){
                new MapGroupComponent(MapEntity),
                new BattleModeComponent(@params.BattleMode),
                new UserLimitComponent(@params.MaxPlayers, @params.MaxPlayers / 2),
                new GravityComponent(GravityTypes[@params.Gravity], @params.Gravity)
            };
            if (!IsMatchMaking)
                paramComponents.Add(new ClientBattleParamsComponent(@params));

            foreach (Component component in paramComponents) {
                BattleLobbyEntity.RemoveComponent(component.GetType());
                BattleLobbyEntity.AddComponent(component);
            }

            CreateBattle();

            if (ModeHandler.GetType().BaseType == typeof(TeamBattleHandler))
            {
                var handler = (TeamBattleHandler)ModeHandler;
                foreach (BattleTankPlayer battlePlayer in JoinedTankPlayers.ToList())
                {
                    BattleView view = handler.BattleViewFor(battlePlayer);
                    battlePlayer.User.RemoveComponent<TeamColorComponent>();
                    battlePlayer.User.AddComponent(new TeamColorComponent(view.AllyTeamColor));
                }
            }
        }

        public void AddPlayer(Player player, bool isSpectator)
        {
            if (player.IsInBattle) return;

            Logger.Log($"{player}: Joined battle {BattleEntity.EntityId}{(isSpectator ? " as spectator" : null)}");

            player.SharePlayers(JoinedTankPlayers.Select(x => x.Player));

            if (isSpectator)
            {
                player.Spectator = new Spectator(this, player);
                Spectators.Add(player.Spectator);
            }
            else
            {
                player.ShareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
                player.User.AddComponent(new BattleLobbyGroupComponent(BattleLobbyEntity));
                player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));

                JoinedTankPlayers.SharePlayers(player);

                BattleTankPlayer battlePlayer = ModeHandler.AddPlayer(player);
                TypeHandler.OnPlayerAdded(battlePlayer);
            }

            if (!IsMatchMaking && player.IsSquadLeader)
                foreach (SquadPlayer participant in player.SquadPlayer.Squad.ParticipantsWithoutLeader)
                    AddPlayer(participant.Player, isSpectator);

            if (isSpectator)
                InitMatchPlayer(player.Spectator);
        }

        public void RemovePlayer(BaseBattlePlayer battlePlayer)
        {
            if (battlePlayer is Spectator spectator)
            {
                Spectators.Remove(spectator);

                Logger.Log($"{battlePlayer.Player}: Stopped spectating battle {BattleEntity.EntityId}");
            }
            else
            {
                var battlePlayer1 = (BattleTankPlayer)battlePlayer;

                TypeHandler.OnPlayerRemoved(battlePlayer1);
                ModeHandler.RemovePlayer(battlePlayer1);

                battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
                battlePlayer.UnshareEntities(BattleLobbyChatEntity, BattleLobbyEntity);
                battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();

                if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                    battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

                if (battlePlayer.Player.IsInSquad)
                    battlePlayer.Player.SquadPlayer.Squad.ProcessBattleLeave(battlePlayer.Player, this);

                JoinedTankPlayers
                    .Where(p => p != battlePlayer && battlePlayer.Player.EntityList.Contains(p.Player.User))
                    .UnsharePlayers(battlePlayer.Player);
                Spectators.UnsharePlayers(battlePlayer.Player);

                Logger.Log($"{battlePlayer.Player}: Left battle {BattleEntity.EntityId}");
            }

            battlePlayer.Player.UnsharePlayers(JoinedTankPlayers.Select(x => x.Player));

            ServerConnection.BattlePool.RemoveAll(p => !p.JoinedTankPlayers.Any());
        }

        private void StartBattle()
        {
            if (!Params.DisabledModules)
            {
                var modeBonusRegions = new Dictionary<BattleMode, BonusList> {
                    { BattleMode.DM, CurrentMapInfo.BonusRegions.Deathmatch },
                    { BattleMode.CTF, CurrentMapInfo.BonusRegions.CaptureTheFlag },
                    { BattleMode.TDM, CurrentMapInfo.BonusRegions.TeamDeathmatch }};
                if (modeBonusRegions[Params.BattleMode] == null)
                {
                    BattleMode newMode = modeBonusRegions.Keys.First(mode => modeBonusRegions[mode] != null && mode != BattleMode.DM);
                    modeBonusRegions[Params.BattleMode] = modeBonusRegions[newMode];
                }
                Dictionary<BonusType, IList<Bonus>> bonusTypeSpawnPoints = new() {
                    { BonusType.ARMOR,  modeBonusRegions[Params.BattleMode].Armor },
                    { BonusType.DAMAGE,  modeBonusRegions[Params.BattleMode].Damage },
                    { BonusType.GOLD,  modeBonusRegions[Params.BattleMode].Gold },
                    { BonusType.REPAIR,  modeBonusRegions[Params.BattleMode].Repair },
                    { BonusType.SPEED,  modeBonusRegions[Params.BattleMode].Speed }};

                BattleBonuses.Clear();
                foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType)))
                foreach (Bonus bonus in bonusTypeSpawnPoints[bonusType])
                {
                    BattleBonus battleBonus = new(bonusType, bonus, this);
                    BattleBonuses.Add(battleBonus);
                }

                Random random = new();
                List<BattleBonus> supplyBonuses =
                    new(BattleBonuses.Where(b => b.BonusType != BonusType.GOLD).OrderBy(_ => random.Next()));
                foreach (BattleBonus battleBonus in supplyBonuses.ToList())
                {
                    battleBonus.StateChangeCountdown = random.Next(10, 120);
                    battleBonus.State = BonusState.New;
                }
            }

            if (ModeHandler is TeamBattleHandler handler && !BattleEntity.HasComponent<TeamGroupComponent>())
                BattleEntity.AddComponent(handler.RedTeamEntity.GetComponent<TeamGroupComponent>());

            foreach (BattleTankPlayer battlePlayer in JoinedTankPlayers.ToArray())
                InitMatchPlayer(battlePlayer);
        }

        public void FinishBattle()
        {
            BattleState = BattleState.Ended;

            ModeHandler.OnFinish();

            (double antaeusScore, double frontierScore, double neutralScore) = (0, 0, 0);

            foreach (BattleTankPlayer battlePlayer in MatchTankPlayers)
            {
                battlePlayer.MatchPlayer.KeepDisabled = true;
                battlePlayer.MatchPlayer.DisableTank();

                // battle results
                battlePlayer.MatchPlayer.PersonalBattleResult.FinalizeResult();
                BattleResultForClient battleResultForClient = new(battlePlayer);

                // recruit reward check
                if (IsMatchMaking) battlePlayer.Player.CheckRecruitReward();

                // collect fraction score
                if (IsMatchMaking)
                {
                    double fractionScore = battlePlayer.MatchPlayer.UserResult.ScoreWithoutPremium * 0.03;
                    switch (battlePlayer.Player.Data.Fraction)
                    {
                        case { } e when e.EntityId == Fractions.GlobalItems.Antaeus.EntityId:
                            antaeusScore += fractionScore;
                            break;
                        case { } e when e.EntityId == Fractions.GlobalItems.Frontier.EntityId:
                            frontierScore += fractionScore;
                            break;
                        case null:
                            neutralScore += fractionScore;
                            break;
                    }
                    if (battlePlayer.Player.Data.Fraction is not null)
                        battlePlayer.Player.Data.FractionUserScore += Convert.ToInt64(fractionScore);
                }

                battlePlayer.SendEvent(new BattleResultForClientEvent(battleResultForClient), battlePlayer.Player.User);
            }

            foreach (Spectator spectator in Spectators)
            {
                BattleResultForClient spectatorResult = new(spectator);
                spectator.SendEvent(new BattleResultForClientEvent(spectatorResult), spectator.Player.User);
            }

            // process waiting goldboxes queue
            foreach (Player player in WaitingGoldBoxSenders.ToList())
            {
                WaitingGoldBoxSenders.Remove(player);
                player.Data.GoldBonus++;
            }

            // fractions competition
            if (IsMatchMaking && ServerData.FractionsCompetitionActive && !ServerData.FractionsCompetitionFinished)
            {
                ServerData.AntaeusScore +=
                    Convert.ToInt64(antaeusScore + (antaeusScore < frontierScore ? neutralScore : 0));
                ServerData.FrontierScore +=
                    Convert.ToInt64(frontierScore + (frontierScore < antaeusScore ? neutralScore : 0));
            }

            IsWarmUpCompleted = false;

            if (RoundEntity.GetComponent<RoundRestartingStateComponent>() == null)
                RoundEntity.AddComponent(new RoundRestartingStateComponent());
            BattleLobbyEntity.TryRemoveComponent<BattleGroupComponent>();
        }

        public void InitMatchPlayer(BaseBattlePlayer battlePlayer)
        {
            battlePlayer.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            // Supply boxes
            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.State != BonusState.Unused && b.State != BonusState.New))
                {
                    battlePlayer.ShareEntities(battleBonus.BonusRegion);
                    if (battleBonus.State == BonusState.Spawned)
                        battlePlayer.ShareEntities(battleBonus.BonusEntity);
                }
            }

            // Module/Supply effects
            foreach (BattleModule module in MatchTankPlayers.SelectMany(tankPlayer => tankPlayer.MatchPlayer.Modules))
                module.ShareEffect(battlePlayer.Player);

            // Enter battle and add critical entities
            battlePlayer.Player.User.TryAddComponent(BattleEntity.GetComponent<BattleGroupComponent>());
            ModeHandler.OnMatchJoin(battlePlayer);

            if (battlePlayer is Spectator spectator)
                battlePlayer.ShareEntities(spectator.BattleUser);
            else
            {
                BattleTankPlayer tankPlayer = (BattleTankPlayer)battlePlayer;

                MatchPlayer matchPlayer = new(tankPlayer);
                tankPlayer.MatchPlayer = matchPlayer;

                if (!Params.DisabledModules)
                {
                    // add all selected modules
                    foreach ((Entity _, Entity garageModule) in battlePlayer.Player.CurrentPreset.Modules.Where(
                        entry => entry.Value?.GetComponent<MountedItemComponent>() != null
                    ))
                    {
                        BattleModule module =
                            Server.Instance.ModuleRegistry.CreateModule(matchPlayer, garageModule);
                        matchPlayer.Modules.Add(module);
                        battlePlayer.ShareEntities(module.SlotEntity, module.ModuleEntity);
                    }

                    // add gold module
                    if (IsMatchMaking)
                    {
                        BattleModule module =
                            Server.Instance.ModuleRegistry.CreateModule(matchPlayer, Modules.GlobalItems.Gold);
                        matchPlayer.Modules.Add(module);
                        battlePlayer.ShareEntities(module.SlotEntity, module.ModuleEntity);
                    }
                }

                // Add and share self to players in list
                MatchTankPlayers.Add(tankPlayer);
                PlayersInMap.ShareEntities(tankPlayer.MatchPlayer.GetEntities());
                Spectators.SharePlayers(tankPlayer.Player);

                SortRoundUsers();
            }

            // Add other players' entities
            battlePlayer.ShareEntities(MatchTankPlayers.Where(x => x != battlePlayer).SelectMany(x => x.MatchPlayer.GetEntities()));


        }

        private void RemoveMatchPlayer(BaseBattlePlayer baseBattlePlayer)
        {
            Player player = baseBattlePlayer.Player;

            if (IsMatchMaking && baseBattlePlayer is not Spectator) MatchMaking.ProcessDeserterState(player, this);

            player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            // Supply boxes
            foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.State != BonusState.Unused && b.State != BonusState.New))
            {
                player.UnshareEntities(battleBonus.BonusRegion);
                if (battleBonus.State == BonusState.Spawned)
                    player.UnshareEntities(battleBonus.BonusEntity);
            }

            // Remove other players' entities and leave battle
            player.UnshareEntities(MatchTankPlayers.Where(x => x != baseBattlePlayer).SelectMany(x => x.MatchPlayer.GetEntities()));
            player.User.RemoveComponent<BattleGroupComponent>();
            ModeHandler.OnMatchLeave(baseBattlePlayer);

            // supply effects and modules
            foreach (BattleModule module in MatchTankPlayers.ToList().SelectMany(p => p.MatchPlayer.Modules))
                module.UnshareEffect(baseBattlePlayer.Player);

            if (baseBattlePlayer is Spectator spectator)
            {
                baseBattlePlayer.UnshareEntities(spectator.BattleUser);
                if (baseBattlePlayer.Rejoin) return;

                RemovePlayer(baseBattlePlayer);
                return;
            }

            //
            // Only for tank players
            //
            var battlePlayer = (BattleTankPlayer)baseBattlePlayer;

            // Unshare and remove self from list
            PlayersInMap.UnshareEntities(battlePlayer.MatchPlayer.GetEntities());
            MatchTankPlayers.Remove(battlePlayer);

            // Keep in match if need
            battlePlayer.Reset();
            if (battlePlayer.Rejoin) return;

            // Remove spectators if last
            if (MatchTankPlayers.Count == 0)
            {
                foreach (Spectator spec in Spectators.ToArray())
                {
                    spec.SendEvent(new KickFromBattleEvent(), spec.BattleUser);
                    RemoveMatchPlayer(spec);
                }
            }

            if (IsMatchMaking)
                RemovePlayer(battlePlayer);

            SortRoundUsers();
        }

        private void ProcessExitedPlayers()
        {
            foreach (BaseBattlePlayer battlePlayer in JoinedTankPlayers.ToArray().Cast<BaseBattlePlayer>().Concat(Spectators).ToArray())
            {
                if (!battlePlayer.Player.IsActive || battlePlayer.WaitingForExit)
                {
                    if ((battlePlayer as BattleTankPlayer)?.MatchPlayer != null || battlePlayer is Spectator)
                        RemoveMatchPlayer(battlePlayer);
                    else
                        RemovePlayer(battlePlayer);
                }
            }
        }

        private void ProcessMatchPlayers()
        {
            foreach (MatchPlayer matchPlayer in MatchTankPlayers.ToList().Select(x => x.MatchPlayer))
                matchPlayer?.Tick();
        }

        private void ProcessBonuses(double deltaTime)
        {
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.State != BonusState.Unused)
                    battleBonus.StateChangeCountdown -= deltaTime;

                if (battleBonus.StateChangeCountdown < 0 || battleBonus.State == BonusState.New)
                {
                    switch (battleBonus.State)
                    {
                        case BonusState.New:
                            battleBonus.CreateRegion();
                            continue;
                        case BonusState.ReDrop or BonusState.RegionShared:
                            battleBonus.CreateBonus(BattleEntity);
                            break;
                    }
                }
            }
        }

        public void Tick(double deltaTime)
        {
            lock (this)
            {
                CountdownTimer -= deltaTime;

                ProcessExitedPlayers();

                ModeHandler.Tick();
                TypeHandler.Tick();

                ProcessMatchPlayers();
                ProcessBonuses(deltaTime);

                foreach (TickHandler handler in _tickHandlers.Where(handler => DateTimeOffset.UtcNow >= handler.Time).ToArray())
                {
                    _tickHandlers.Remove(handler);

                    handler.Action();
                }

                foreach (Action handler in _nextTickHandlers.ToArray())
                {
                    _nextTickHandlers.Remove(handler);

                    handler();
                }
            }
        }

        public void TriggerRandomGoldbox()
        {
            if (!IsMatchMaking) return;
            if (new Random().Next(0, 300) <= 1)
                DropSpecificBonusType(BonusType.GOLD);
        }

        public void UpdateScore(Entity team, int additiveScore = 1)
        {
            if (BattleState != BattleState.Running) return;

            var scoreComponent = team.GetComponent<TeamScoreComponent>();
            scoreComponent.Score = Math.Clamp(scoreComponent.Score + additiveScore, 0, int.MaxValue);
            team.ChangeComponent(scoreComponent);
            PlayersInMap.SendEvent(new RoundScoreUpdatedEvent(), RoundEntity);
        }

        public void DropSpecificBonusType(BonusType bonusType, Player sender = null)
        {
            List<int> suppliesIndex = new();
            int index = 0;
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.BonusType == bonusType)
                {
                    if (bonusType != BonusType.GOLD && battleBonus.State != BonusState.Spawned ||
                        bonusType == BonusType.GOLD && battleBonus.State == BonusState.Unused)
                        suppliesIndex.Add(index);
                }
                ++index;
            }

            if (bonusType == BonusType.GOLD && DroppedGoldBoxes + 1 > MaxDroppedGoldBoxes) return;
            if (!suppliesIndex.Any())
            {
                if (bonusType == BonusType.GOLD && sender is not null && DroppedGoldBoxes + 1 <= MaxDroppedGoldBoxes)
                    WaitingGoldBoxSenders.Add(sender);
                return;
            }

            int supplyIndex = suppliesIndex[new Random().Next(suppliesIndex.Count)];

            if (bonusType != BonusType.GOLD)
                BattleBonuses[supplyIndex].StateChangeCountdown = 0;
            else
            {
                BattleBonuses[supplyIndex].State = BonusState.New;
                DroppedGoldBoxes++;

                PlayersInMap.SendEvent(new GoldScheduleNotificationEvent(sender is null ? "" : sender.Data.Username),
                    RoundEntity);
            }
        }

        public void SortRoundUsers() => ModeHandler.SortRoundUsers();
        private void CompleteWarmUp() => ModeHandler.CompleteWarmUp();

        public Player FindPlayerByUsername(string username)
        {
            Player searchedPlayer = JoinedTankPlayers.FirstOrDefault(controlledPlayer =>
                controlledPlayer.Player.Data.Username == username)
                ?.Player;
            return searchedPlayer;
        }

        public int EnemyCountFor(BattleTankPlayer battlePlayer) => ModeHandler.EnemyCountFor(battlePlayer);

        private static readonly Dictionary<BattleMode, Func<Entity, int, int, int, Entity>> BattleEntityCreators = new()
        {
            { BattleMode.CTF, CTFTemplate.CreateEntity },
            { BattleMode.DM, DMTemplate.CreateEntity },
            { BattleMode.TDM, TDMTemplate.CreateEntity }
        };

        public readonly Dictionary<GravityType, float> GravityTypes = new()
        {
            { GravityType.EARTH, 9.81f },
            { GravityType.SUPER_EARTH, 30 },
            { GravityType.MOON, 1.62f },
            { GravityType.MARS, 3.71f }
        };

        public ClientBattleParams Params { get; private set; }
        public ExtendedBattleMode ExtendedBattleMode { get; set; }
        public bool IsMatchMaking { get; }
        public Entity MapEntity { get; private set; }
        public bool SuppressInactivityKick { get; set; }

        public bool ForceStart { get; set; }
        public bool ForcePause { get; set; }
        public bool ForceOpen { get; set; }
        private int WarmUpSeconds { get; set; }
        private bool IsWarmUpCompleted { get; set; }

        public MapInfo CurrentMapInfo { get; private set; }

        public List<BattleBonus> BattleBonuses { get; } = new();
        public IEnumerable<BattleBonus> GoldBonuses => BattleBonuses.Where(b => b.BonusType == BonusType.GOLD);
        public int DroppedGoldBoxes { get; set; }
        public List<Player> WaitingGoldBoxSenders { get; } = new();
        public const int MaxDroppedGoldBoxes = 16;

        public BattleState BattleState
        {
            get => _battleState;
            set
            {
                switch (_battleState)
                {
                    case BattleState.StartCountdown:
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartTimeComponent>();
                        break;
                    case BattleState.Starting:
                        BattleLobbyEntity.RemoveComponent<MatchMakingLobbyStartingComponent>();
                        break;
                    case BattleState.WarmUp:
                        if (value != BattleState.Ended)
                            RoundEntity.RemoveComponent<RoundWarmingUpStateComponent>();
                        break;
                }

                switch (value)
                {
                    case BattleState.StartCountdown:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartTimeComponent { StartTime = DateTime.UtcNow.AddSeconds(10) });
                        CountdownTimer = 10;
                        break;
                    case BattleState.Starting:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartingComponent());
                        CountdownTimer = 3;
                        break;
                    case BattleState.WarmUp:
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(DateTimeOffset.UtcNow.AddSeconds(WarmUpSeconds)));
                        RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.UtcNow.AddSeconds(60 * Params.TimeLimit)));
                        RoundEntity.AddComponent(new RoundWarmingUpStateComponent());
                        CountdownTimer = WarmUpSeconds;
                        break;
                    case BattleState.Running:
                        CountdownTimer = 60 * Params.TimeLimit;
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(DateTime.UtcNow));
                        RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.UtcNow.AddSeconds(CountdownTimer)));
                        break;
                }

                _battleState = value;
            }
        }

        /// <summary>
        /// Schedules an action to run at next battle tick
        /// </summary>
        /// <param name="handler">Action to run at next battle tick</param>
        public void Schedule(Action handler)
        {
            _nextTickHandlers.Add(handler);
        }

        /// <summary>
        /// Schedules an action to run at specified time
        /// </summary>
        /// <param name="time">Time at which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        private void Schedule(DateTimeOffset time, Action handler)
        {
            _tickHandlers.Add(new TickHandler(time, handler));
        }

        /// <summary>
        /// Schedules an action to run after specified time
        /// </summary>
        /// <param name="timeSpan">TimeSpan after which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        public void Schedule(TimeSpan timeSpan, Action handler)
        {
            Schedule(DateTimeOffset.UtcNow + timeSpan, handler);
        }

        private readonly List<TickHandler> _tickHandlers;
        private readonly List<Action> _nextTickHandlers;

        private BattleState _battleState;
        public bool KeepRunning { get; set; }

        public IBattleTypeHandler TypeHandler { get; }
        public IBattleModeHandler ModeHandler { get; private set; }

        public double CountdownTimer { get; private set; }

        public IEnumerable<BattleTankPlayer> JoinedTankPlayers => ModeHandler.Players;
        public List<BattleTankPlayer> MatchTankPlayers { get; } = new();
        public List<Spectator> Spectators { get; } = new();
        public IEnumerable<BaseBattlePlayer> PlayersInMap => MatchTankPlayers.Concat(Spectators.Cast<BaseBattlePlayer>());
        public List<MatchPlayer> DisqualifiedPlayers { get; } = new();

        private bool IsEnoughPlayers => ModeHandler.IsEnoughPlayers;
        private TeamColor LosingTeam => ModeHandler.LosingTeam;

        public Entity BattleEntity { get; private set; }
        public Entity BattleLobbyEntity { get; private set; }
        public Entity RoundEntity { get; private set; }
        public BattleTankCollisionsComponent CollisionsComponent { get; private set; }

        public Entity GeneralBattleChatEntity { get; }
        public Entity BattleLobbyChatEntity { get; }

        public HeightMap HeightMap { get; private set; }

        private static ServerData ServerData => Server.Instance.ServerData;
    }
}
