using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Battles.Module;
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
using TXServer.ECSSystem.EntityTemplates.Chat;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Bonus;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.Battles
{
    public partial class Battle
    {
        public Battle(ClientBattleParams battleParams, bool isMatchMaking, Player owner)
        {
            Params = battleParams;
            IsMatchMaking = isMatchMaking;

            if (isMatchMaking)
                TypeHandler = new MatchMakingBattleHandler { Battle = this };
            else
                TypeHandler = new CustomBattleHandler { Battle = this, Owner = owner };
            TypeHandler.SetupBattle();

            CreateBattle();
            BattleLobbyChatEntity = BattleLobbyChatTemplate.CreateEntity();
            GeneralBattleChatEntity = GeneralBattleChatTemplate.CreateEntity();

            tickHandlers = new List<TickHandler>();
            nextTickHandlers = new List<Action>();
        }
        
        public void CreateBattle()
        {
            BattleEntity = BattleEntityCreators[Params.BattleMode](BattleLobbyEntity, Params.ScoreLimit, Params.TimeLimit * 60, 120);
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();

            IBattleModeHandler prevHandler = ModeHandler;
            ModeHandler = Params.BattleMode switch
            {
                BattleMode.DM => new DMHandler { Battle = this },
                BattleMode.TDM => new TDMHandler { Battle = this },
                BattleMode.CTF => new CTFHandler { Battle = this },
                _ => throw new NotImplementedException()
            };

            if (prevHandler != null)
                ModeHandler.SetupBattle(prevHandler);
            else
                ModeHandler.SetupBattle();
        }

        private (Entity, int) ConvertMapParams(ClientBattleParams Params, bool isMatchMaking)
        {
            Entity mapEntity = Maps.GlobalItems.Rio;
            int maxPlayers = Params.MaxPlayers;
            foreach (PropertyInfo property in typeof(Maps.Items).GetProperties())
            {
                Entity entity = (Entity)property.GetValue(Maps.GlobalItems);
                if (entity.EntityId == Params.MapId)
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

        public void UpdateParams(Player player, ClientBattleParams @params)
        {
            Params = @params;
            (MapEntity, _) = ConvertMapParams(@params, IsMatchMaking);

            foreach (Component component in new Component[]
            {
                new MapGroupComponent(MapEntity),
                new BattleModeComponent(@params.BattleMode),
                new UserLimitComponent(userLimit: @params.MaxPlayers, teamLimit: @params.MaxPlayers / 2),
                new GravityComponent(gravity: GravityTypes[@params.Gravity], gravityType: @params.Gravity),
                new ClientBattleParamsComponent(@params),
            })
            {
                BattleLobbyEntity.RemoveComponent(component.GetType());
                BattleLobbyEntity.AddComponent(component);
            }

            CreateBattle();
        }

        public void AddPlayer(Player player, bool isSpectator)
        {
            Logger.Log($"{player}: Joined battle {BattleEntity.EntityId} " + 
                       (isSpectator ? "as spectator" : null));

            // prepare client
            player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
            player.ShareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
            player.User.AddComponent(new BattleLobbyGroupComponent(BattleLobbyEntity));

            player.ShareEntities(AllBattlePlayers.Where(y => !player.IsInSquadWith(y.Player)).Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).Where(x => !x.EntityList.Contains(player.User)).ShareEntity(player.User);

            BattlePlayer battlePlayer = ModeHandler.AddPlayer(player, isSpectator);
            TypeHandler.OnPlayerAdded(battlePlayer);

            if (!IsMatchMaking && player.IsSquadLeader)
                foreach (SquadPlayer participant in player.SquadPlayer.Squad.ParticipantsWithoutLeader)
                    AddPlayer(participant.Player, isSpectator);

            if (isSpectator)
            {
                Spectators.Add(player.BattlePlayer);
                InitMatchPlayer(player.BattlePlayer);
            }
        }

        public void RemovePlayer(BattlePlayer battlePlayer)
        {
            Spectators.Remove(battlePlayer);
            bool wasSpectator = battlePlayer.IsSpectator;
            TypeHandler.OnPlayerRemoved(battlePlayer);
            ModeHandler.RemovePlayer(battlePlayer);

            battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
            battlePlayer.Player.UnshareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
            battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

            battlePlayer.Player.UnshareEntities(AllBattlePlayers
                .Where(x => !battlePlayer.Player.IsInSquadWith(x.Player)).Select(x => x.User));
            AllBattlePlayers.Where(y => !y.Player.IsInSquadWith(battlePlayer.Player)).Select(x => x.Player)
                .UnshareEntity(battlePlayer.User);

            Logger.Log($"{battlePlayer.Player}: Left battle {BattleEntity.EntityId}" + 
                       (wasSpectator ? " as spectator" : null));

            ServerConnection.BattlePool.RemoveAll(p => !p.AllBattlePlayers.Any());
            
            if (battlePlayer.Player.IsInSquad)
                battlePlayer.Player.SquadPlayer.Squad.ProcessBattleLeave(battlePlayer.Player, this);
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
                var bonusTypeSpawnPoints = new Dictionary<BonusType, IList<Bonus>> {
                    { BonusType.ARMOR,  modeBonusRegions[Params.BattleMode].Armor },
                    { BonusType.DAMAGE,  modeBonusRegions[Params.BattleMode].Damage },
                    { BonusType.GOLD,  modeBonusRegions[Params.BattleMode].Gold },
                    { BonusType.REPAIR,  modeBonusRegions[Params.BattleMode].Repair },
                    { BonusType.SPEED,  modeBonusRegions[Params.BattleMode].Speed }};

                BattleBonuses.Clear();
                foreach (BonusType bonusType in Enum.GetValues(typeof(BonusType)))
                {
                    foreach (Bonus bonus in bonusTypeSpawnPoints[bonusType])
                    {
                        BattleBonus battleBonus = new(bonusType, bonus, this);
                        BattleBonuses.Add(battleBonus);
                    }
                }

                Random random = new();
                List<BattleBonus> supplyBonuses = new(BattleBonuses.Where(b => b.BonusType != BonusType.GOLD).OrderBy(b => random.Next()));
                foreach (BattleBonus battleBonus in supplyBonuses.ToList())
                {
                    battleBonus.StateChangeCountdown = random.Next(10, 120);
                    battleBonus.State = BonusState.New;
                }
            }

            foreach (BattlePlayer battlePlayer in AllBattlePlayers.ToArray())
                InitMatchPlayer(battlePlayer);
        }

        public void FinishBattle()
        {
            BattleState = BattleState.Ended;

            ModeHandler.OnFinish();

            foreach (BattlePlayer battlePlayer in MatchPlayers.Where(x => !x.IsSpectator))
            {
                battlePlayer.MatchPlayer.KeepDisabled = true;
                battlePlayer.MatchPlayer.DisableTank();

                PersonalBattleResultForClient personalResult = new(battlePlayer.Player, ModeHandler.TeamBattleResultFor(battlePlayer));
                BattleResultForClient battleResultForClient = new(this, ModeHandler, personalResult);
                battlePlayer.Player.SendEvent(new BattleResultForClientEvent(battleResultForClient), battlePlayer.Player.User);

                battlePlayer.Player.User.ChangeComponent<BattleLeaveCounterComponent>(component =>
                {
                    if (component.Value > 0)
                        component.Value -= 1;
                    if (component.NeedGoodBattles > 0)
                        component.NeedGoodBattles -= 1;
                });

                battlePlayer.MatchPlayer.UserResult.ScoreWithoutPremium = battlePlayer.MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
                battlePlayer.Player.User.ChangeComponent<UserExperienceComponent>(component =>
                    component.Experience += battlePlayer.MatchPlayer.UserResult.ScoreWithoutPremium - battlePlayer.MatchPlayer.AlreadyAddedExperience);

                if (AllBattlePlayers.Count() <= 3 ||
                    (ModeHandler is TeamBattleHandler tbHandler && Math.Abs(tbHandler.RedTeamPlayers.Count - tbHandler.BlueTeamPlayers.Count) >= 2))
                    battlePlayer.MatchPlayer.UserResult.UnfairMatching = true;
            }

            IsWarmUpCompleted = false;

            if (RoundEntity.GetComponent<RoundRestartingStateComponent>() == null)
                RoundEntity.AddComponent(new RoundRestartingStateComponent());
            if (BattleLobbyEntity.GetComponent<BattleGroupComponent>() != null)
                BattleLobbyEntity.RemoveComponent<BattleGroupComponent>();
        }

        public void InitMatchPlayer(BattlePlayer battlePlayer)
        {
            if (!battlePlayer.IsSpectator)
                battlePlayer.Player.User.AddComponent(BattleEntity.GetComponent<BattleGroupComponent>());
            battlePlayer.MatchPlayer = new MatchPlayer(battlePlayer, BattleEntity, (ModeHandler as TeamBattleHandler)?.BattleViewFor(battlePlayer).AllyTeamResults ?? ((DMHandler)ModeHandler).Results);

            battlePlayer.Player.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.State != BonusState.Unused && b.State != BonusState.New))
                {
                    battlePlayer.Player.ShareEntity(battleBonus.BonusRegion);
                    if (battleBonus.State == BonusState.Spawned)
                        battlePlayer.Player.ShareEntity(battleBonus.BonusEntity);
                }
                foreach (BattlePlayer battlePlayer1 in MatchPlayers)
                    battlePlayer.Player.ShareEntities(battlePlayer1.MatchPlayer.SupplyEffects.Select(supplyEffect => supplyEffect.SupplyEffectEntity));

                if (!battlePlayer.IsSpectator)
                {
                    foreach ((Entity garageSlot, Entity garageModule) in battlePlayer.Player.CurrentPreset.Modules.Where(
                        (entry) => entry.Value?.GetComponent<MountedItemComponent>() != null
                    ))
                    {
                        try {
                            BattleModule module = Server.Instance.ModuleRegistry.CreateModule(
                                battlePlayer.MatchPlayer,
                                garageModule
                            );
                            if (module == null)
                                throw new InvalidOperationException(
                                    $"Failed to create module '{garageModule.EntityId}'"
                                );

                            battlePlayer.MatchPlayer.Modules.Add(module);
                            battlePlayer.Player.ShareEntities(module.SlotEntity, module.ModuleEntity);
                        }
                        catch(Exception exception) {
                        }
                    }

                    if (IsMatchMaking || battlePlayer.Player.Data.Admin)
                    {
                        BattleModule module = Server.Instance.ModuleRegistry.CreateModule(
                            battlePlayer.MatchPlayer,
                            Modules.GlobalItems.Gold
                        );
                        if (module == null) throw new InvalidOperationException($"Failed to create module '{Modules.GlobalItems.Gold.EntityId}'");

                        battlePlayer.MatchPlayer.Modules.Add(module);
                        battlePlayer.Player.ShareEntities(module.SlotEntity, module.ModuleEntity);
                    }
                }
            }

            ModeHandler.OnMatchJoin(battlePlayer);

            foreach (BattlePlayer battlePlayer2 in MatchPlayers)
                battlePlayer.Player.ShareEntities(battlePlayer2.MatchPlayer.GetEntities());

            MatchPlayers.Add(battlePlayer);

            MatchPlayers.Select(x => x.Player).ShareEntities(battlePlayer.MatchPlayer.GetEntities());

            SortRoundUsers();
        }

        private void RemoveMatchPlayer(BattlePlayer battlePlayer)
        {
            if (!battlePlayer.IsSpectator)
                battlePlayer.Player.User.RemoveComponent<BattleGroupComponent>();
            Player player = battlePlayer.Player;

            player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.State != BonusState.Unused && b.State != BonusState.New))
                {
                    player.UnshareEntity(battleBonus.BonusRegion);
                    if (battleBonus.State == BonusState.Spawned) 
                        player.UnshareEntity(battleBonus.BonusEntity);
                }
                foreach (BattlePlayer battlePlayer1 in MatchPlayers.Where(x => x != battlePlayer))
                    battlePlayer.Player.UnshareEntities(battlePlayer1.MatchPlayer.SupplyEffects.Select(supplyEffect => supplyEffect.SupplyEffectEntity));
                foreach (SupplyEffect supplyEffect in battlePlayer.MatchPlayer.SupplyEffects.ToArray())
                    supplyEffect.Remove();

                if (!battlePlayer.IsSpectator)
                    foreach (BattleModule module in battlePlayer.MatchPlayer.Modules.ToArray())
                        battlePlayer.Player.UnshareEntities(module.SlotEntity, module.ModuleEntity);
            }

            ModeHandler.OnMatchLeave(battlePlayer);

            MatchPlayers.Select(x => x.Player).UnshareEntities(battlePlayer.MatchPlayer.GetEntities());

            MatchPlayers.Remove(battlePlayer);

            foreach (BattlePlayer matchPlayer in MatchPlayers)
                player.UnshareEntities(matchPlayer.MatchPlayer.GetEntities());

            battlePlayer.Reset();

            if (battlePlayer.Rejoin) return;

            if (IsMatchMaking || battlePlayer.IsSpectator)
                RemovePlayer(battlePlayer);

            SortRoundUsers();
        }

        private void ProcessExitedPlayers()
        {
            foreach (BattlePlayer battlePlayer in AllBattlePlayers.Concat(Spectators).ToArray())
            {
                if (!battlePlayer.Player.IsActive || battlePlayer.WaitingForExit)
                {
                    if (battlePlayer.MatchPlayer != null)
                    {
                        RemoveMatchPlayer(battlePlayer);
                        if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                            battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();
                    }
                    else
                        RemovePlayer(battlePlayer);
                }
            }
        }

        private void ProcessMatchPlayers()
        {
            foreach (MatchPlayer matchPlayer in MatchPlayers.Select(x => x.MatchPlayer))
                matchPlayer.Tick();
        }

        private void ProcessBonuses(double deltaTime)
        {
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.State != BonusState.Unused)
                    battleBonus.StateChangeCountdown -= deltaTime;

                if (battleBonus.StateChangeCountdown < 0 || battleBonus.State == BonusState.New)
                {
                    if (battleBonus.State == BonusState.New)
                    {
                        battleBonus.CreateRegion();

                        continue;
                    }
                    if (battleBonus.State == BonusState.Redrop || battleBonus.State == BonusState.RegionShared)
                        battleBonus.CreateBonus(BattleEntity);
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
                
                foreach(TickHandler handler in tickHandlers.Where(handler => DateTimeOffset.Now >= handler.Time).ToArray()) {
                    tickHandlers.Remove(handler);

                    handler.Action();
                }
			
                foreach(Action handler in nextTickHandlers.ToArray()) {
                    nextTickHandlers.Remove(handler);

                    handler();
                }
            }
        }

        public void UpdateScore(Entity team, int additiveScore = 1)
        {
            if (BattleState != BattleState.Running) return;

            var scoreComponent = team.GetComponent<TeamScoreComponent>();
            scoreComponent.Score = Math.Clamp(scoreComponent.Score + additiveScore, 0, int.MaxValue);
            team.ChangeComponent(scoreComponent);
            MatchPlayers.Select(x => x.Player).SendEvent(new RoundScoreUpdatedEvent(), RoundEntity);
        }

        public void DropSpecificBonusType(BonusType bonusType, string sender)
        {
            List<int> suppliesIndex = new();
            int index = 0;
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.BonusType == bonusType)
                {
                    if (bonusType != BonusType.GOLD && battleBonus.State != BonusState.Spawned || bonusType == BonusType.GOLD && battleBonus.State == BonusState.Unused)
                        suppliesIndex.Add(index);
                }
                ++index;
            }

            if (!suppliesIndex.Any()) return;

            int supplyIndex = suppliesIndex[new Random().Next(suppliesIndex.Count)];
            if (bonusType != BonusType.GOLD)
                BattleBonuses[supplyIndex].StateChangeCountdown = 0;
            else
            {
                BattleBonuses[supplyIndex].State = BonusState.New;
                if (String.IsNullOrWhiteSpace(sender)) sender = "";
                MatchPlayers.Select(x => x.Player).SendEvent(new GoldScheduleNotificationEvent(sender), RoundEntity);
            }
        }

        public void SortRoundUsers() => ModeHandler.SortRoundUsers();
        private void CompleteWarmUp() => ModeHandler.CompleteWarmUp();

        public Player FindPlayerByUid(string uid)
        {
            Player searchedPlayer = AllBattlePlayers.FirstOrDefault(controlledPlayer =>
                controlledPlayer.Player.Data.Username == uid)
                ?.Player;
            return searchedPlayer;
        } 

        private int EnemyCountFor(BattlePlayer battlePlayer) => ModeHandler.EnemyCountFor(battlePlayer);

        private static readonly Dictionary<BattleMode, Func<Entity, int, int, int, Entity>> BattleEntityCreators = new()
        {
            { BattleMode.DM, DMTemplate.CreateEntity },
            { BattleMode.TDM, TDMTemplate.CreateEntity },
            { BattleMode.CTF, CTFTemplate.CreateEntity },
        };

        public readonly Dictionary<GravityType, float> GravityTypes = new()
        {
            { GravityType.EARTH, 9.81f },
            { GravityType.SUPER_EARTH, 30 },
            { GravityType.MOON, 1.62f },
            { GravityType.MARS, 3.71f }
        };

        public ClientBattleParams Params { get; set; }
        public bool IsMatchMaking { get; }
        public Entity MapEntity { get; private set; }

        public bool ForceStart { get; set; }
        public bool ForcePause { get; set; }
        public bool ForceOpen { get; set; }
        public int WarmUpSeconds { get; set; }
        private bool IsWarmUpCompleted { get; set; }

        public MapInfo CurrentMapInfo { get; set; }

        public List<BattleBonus> BattleBonuses { get; set; } = new();
        public IEnumerable<BattleBonus> GoldBonuses => BattleBonuses.Where(b => b.BonusType == BonusType.GOLD);

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
                        if (value != BattleState.Ended)
                            RoundEntity.RemoveComponent<RoundWarmingUpStateComponent>();
                        break;
                }

                switch (value)
                {
                    case BattleState.StartCountdown:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartTimeComponent { StartTime = DateTime.Now.AddSeconds(10) });
                        CountdownTimer = 10;
                        break;
                    case BattleState.Starting:
                        BattleLobbyEntity.AddComponent(new MatchMakingLobbyStartingComponent());
                        CountdownTimer = 3;
                        break;
                    case BattleState.WarmUp:
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(DateTimeOffset.Now.AddSeconds(WarmUpSeconds)));
                        RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.Now.AddSeconds(60 * Params.TimeLimit)));
                        RoundEntity.AddComponent(new RoundWarmingUpStateComponent());
                        CountdownTimer = WarmUpSeconds;
                        break;
                    case BattleState.Running:
                        CountdownTimer = 60 * Params.TimeLimit;
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(DateTime.Now));
                        RoundEntity.ChangeComponent(new RoundStopTimeComponent(DateTimeOffset.Now.AddSeconds(CountdownTimer)));
                        break;
                }

                _BattleState = value;
            }
        }
        
        /// <summary>
        /// Schedules an action to run at next battle tick
        /// </summary>
        /// <param name="handler">Action to run at next battle tick</param>
        public void Schedule(Action handler) {
            nextTickHandlers.Add(handler);
        }

        /// <summary>
        /// Schedules an action to run at specified time
        /// </summary>
        /// <param name="time">Time at which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        public void Schedule(DateTimeOffset time, Action handler) {
            tickHandlers.Add(new TickHandler(time, handler));
        }

        /// <summary>
        /// Schedules an action to run after specified time
        /// </summary>
        /// <param name="timeSpan">TimeSpan after which action should run</param>
        /// <param name="handler">Action to run at specified time</param>
        public void Schedule(TimeSpan timeSpan, Action handler) {
            Schedule(DateTimeOffset.Now + timeSpan, handler);
        }
        
        private readonly List<TickHandler> tickHandlers;
        private readonly List<Action> nextTickHandlers;
        
        private BattleState _BattleState;
        public bool KeepRunning { get; set; }

        public IBattleTypeHandler TypeHandler { get; }
        public IBattleModeHandler ModeHandler { get; private set; }

        public double CountdownTimer { get; set; }

        public IEnumerable<BattlePlayer> AllBattlePlayers => ModeHandler.Players;
        public List<BattlePlayer> MatchPlayers { get; } = new();
        public List<BattlePlayer> Spectators { get; } = new();

        private bool IsEnoughPlayers => ModeHandler.IsEnoughPlayers;
        private TeamColor LosingTeam => ModeHandler.LosingTeam;

        public Entity BattleEntity { get; set; }
        public Entity BattleLobbyEntity { get; set; }
        public Entity RoundEntity { get; set; }
        public BattleTankCollisionsComponent CollisionsComponent { get; set; }

        public Entity GeneralBattleChatEntity { get; }
        public Entity BattleLobbyChatEntity { get; }
    }
}
