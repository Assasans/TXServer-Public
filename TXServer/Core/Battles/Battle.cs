using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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

            ModeHandler = Params.BattleMode switch
            {
                BattleMode.DM => new DMHandler { Battle = this },
                BattleMode.TDM => new TDMHandler { Battle = this },
                BattleMode.CTF => new CTFHandler { Battle = this },
                _ => throw new NotImplementedException()
            };
            ModeHandler.SetupBattle();
        }
        
        public void CreateBattle()
        {
            BattleEntity = (Entity)BattleEntityCreators[Params.BattleMode].GetMethod("CreateEntity").Invoke(null, new object[] { BattleLobbyEntity, Params.ScoreLimit, Params.TimeLimit * 60, 120 });
            RoundEntity = RoundTemplate.CreateEntity(BattleEntity);
            CollisionsComponent = BattleEntity.GetComponent<BattleTankCollisionsComponent>();
        }

        public (Entity, int) ConvertMapParams(ClientBattleParams Params, bool isMatchMaking)
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

            IBattleModeHandler prevHandler = ModeHandler;
            ModeHandler = @params.BattleMode switch
            {
                BattleMode.DM => new DMHandler { Battle = this },
                BattleMode.TDM => new TDMHandler { Battle = this },
                BattleMode.CTF => new CTFHandler { Battle = this },
                _ => throw new NotImplementedException()
            };
            ModeHandler.SetupBattle(prevHandler);
        }

        public void AddPlayer(Player player)
        {
            Logger.Log($"{player}: Joined battle {BattleEntity.EntityId}");

            // prepare client
            player.User.AddComponent(new UserEquipmentComponent(player.CurrentPreset.Weapon.EntityId, player.CurrentPreset.Hull.EntityId));
            player.ShareEntities(BattleLobbyEntity, BattleLobbyChatEntity);
            player.User.AddComponent(new BattleLobbyGroupComponent(BattleLobbyEntity));

            player.ShareEntities(AllBattlePlayers.Where(y => !player.IsInSquadWith(y.Player)).Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).Where(x => !x.EntityList.Contains(player.User)).ShareEntity(player.User);

            BattlePlayer battlePlayer = ModeHandler.AddPlayer(player);
            TypeHandler.OnPlayerAdded(battlePlayer);

            if (!IsMatchMaking && player.IsSquadLeader)
                foreach (SquadPlayer participant in player.SquadPlayer.Squad.ParticipantsWithoutLeader)
                    AddPlayer(participant.Player);
        }

        public void RemovePlayer(BattlePlayer battlePlayer)
        {
            TypeHandler.OnPlayerRemoved(battlePlayer);
            ModeHandler.RemovePlayer(battlePlayer);

            battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
            battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();
            battlePlayer.Player.UnshareEntities(BattleLobbyEntity, BattleLobbyChatEntity);

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

            battlePlayer.Player.UnshareEntities(AllBattlePlayers
                .Where(x => !battlePlayer.Player.IsInSquadWith(x.Player)).Select(x => x.User));
            AllBattlePlayers.Where(y => !y.Player.IsInSquadWith(battlePlayer.Player)).Select(x => x.Player)
                .UnshareEntity(battlePlayer.User);

            Logger.Log($"{battlePlayer.Player}: Left battle {BattleEntity.EntityId}");

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
                List<BattleBonus> supplyBonuses = new(BattleBonuses.Where(b => b.BattleBonusType != BonusType.GOLD).OrderBy(b => random.Next()));
                foreach (BattleBonus battleBonus in supplyBonuses.ToList())
                {
                    battleBonus.BonusStateChangeCountdown = random.Next(10, 120);
                    battleBonus.BonusState = BonusState.New;
                }
            }

            foreach (BattlePlayer battlePlayer in AllBattlePlayers.ToArray())
                InitMatchPlayer(battlePlayer);
        }

        public void FinishBattle()
        {
            BattleState = BattleState.Ended;

            ModeHandler.OnFinish();

            foreach (BattlePlayer battlePlayer in MatchPlayers)
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
            battlePlayer.Player.User.AddComponent(BattleEntity.GetComponent<BattleGroupComponent>());
            battlePlayer.MatchPlayer = new MatchPlayer(battlePlayer, BattleEntity, (ModeHandler as TeamBattleHandler)?.BattleViewFor(battlePlayer).AllyTeamResults ?? ((DMHandler)ModeHandler).Results);

            battlePlayer.Player.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.BonusState != BonusState.Unused && b.BonusState != BonusState.New))
                {
                    battlePlayer.Player.ShareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned)
                        battlePlayer.Player.ShareEntity(battleBonus.Bonus);
                }
                foreach (BattlePlayer battlePlayer1 in MatchPlayers)
                    battlePlayer.Player.ShareEntities(battlePlayer1.MatchPlayer.SupplyEffects.Select(supplyEffect => supplyEffect.SupplyEffectEntity));
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
            battlePlayer.Player.User.RemoveComponent<BattleGroupComponent>();
            Player player = battlePlayer.Player;

            player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses.Where(b => b.BonusState != BonusState.Unused && b.BonusState != BonusState.New))
                {
                    player.UnshareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned) 
                        player.UnshareEntity(battleBonus.Bonus);
                }
                foreach (BattlePlayer battlePlayer1 in MatchPlayers.Where(x => x != battlePlayer))
                    battlePlayer.Player.UnshareEntities(battlePlayer1.MatchPlayer.SupplyEffects.Select(supplyEffect => supplyEffect.SupplyEffectEntity));
                foreach (SupplyEffect supplyEffect in battlePlayer.MatchPlayer.SupplyEffects.ToArray())
                    supplyEffect.Remove();
            }

            ModeHandler.OnMatchLeave(battlePlayer);

            MatchPlayers.Select(x => x.Player).UnshareEntities(battlePlayer.MatchPlayer.GetEntities());

            MatchPlayers.Remove(battlePlayer);

            foreach (BattlePlayer matchPlayer in MatchPlayers)
                player.UnshareEntities(matchPlayer.MatchPlayer.GetEntities());

            if (IsMatchMaking) RemovePlayer(battlePlayer);
            battlePlayer.Reset();

            SortRoundUsers();
        }

        private void ProcessExitedPlayers()
        {
            foreach (BattlePlayer battlePlayer in AllBattlePlayers.ToArray())
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
            {
                matchPlayer.Tick();
            }
        }

        private void ProcessBonuses(double deltaTime)
        {
            foreach (BattleBonus battleBonus in BattleBonuses)
            {
                if (battleBonus.BonusState != BonusState.Unused)
                    battleBonus.BonusStateChangeCountdown -= deltaTime;

                if (battleBonus.BonusStateChangeCountdown < 0 || battleBonus.BonusState == BonusState.New)
                {
                    if (battleBonus.BonusState == BonusState.New)
                    {
                        battleBonus.CreateRegion();

                        continue;
                    }
                    if (battleBonus.BonusState == BonusState.Redrop || battleBonus.BonusState == BonusState.RegionShared)
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
            }
        }

        public void UpdateScore(Entity team, int additiveScore)
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
                if (battleBonus.BattleBonusType == bonusType)
                {
                    if ((bonusType != BonusType.GOLD && battleBonus.BonusState != BonusState.Spawned) || (bonusType == BonusType.GOLD && battleBonus.BonusState == BonusState.Unused))
                        suppliesIndex.Add(index);
                }
                ++index;
            }

            if (!suppliesIndex.Any()) return;

            int supplyIndex = suppliesIndex[new Random().Next(suppliesIndex.Count)];
            if (bonusType != BonusType.GOLD)
                BattleBonuses[supplyIndex].BonusStateChangeCountdown = 0;
            else
            {
                BattleBonuses[supplyIndex].BonusState = BonusState.New;
                if (String.IsNullOrWhiteSpace(sender)) sender = "";
                AllBattlePlayers.Select(x => x.Player).SendEvent(new GoldScheduleNotificationEvent(sender), RoundEntity);
            }
        }

        public void SortRoundUsers() => ModeHandler.SortRoundUsers();
        private void CompleteWarmUp() => ModeHandler.CompleteWarmUp();

        private int EnemyCountFor(BattlePlayer battlePlayer) => ModeHandler.EnemyCountFor(battlePlayer);

        private static readonly Dictionary<BattleMode, Type> BattleEntityCreators = new()
        {
            { BattleMode.DM, typeof(DMTemplate) },
            { BattleMode.TDM, typeof(TDMTemplate) },
            { BattleMode.CTF, typeof(CTFTemplate) },
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
        public bool ForceOpen { get; set; }
        public int WarmUpSeconds { get; set; }
        private bool IsWarmUpCompleted { get; set; }

        public MapInfo CurrentMapInfo { get; set; }
        public IList<SpawnPoint> DeathmatchSpawnPoints { get; set; }
        
        public List<BattleBonus> BattleBonuses { get; set; } = new();

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
                        CountdownTimer = 60 * Params.TimeLimit;
                        BattleEntity.ChangeComponent(new BattleStartTimeComponent(DateTime.Now));
                        break;
                }

                _BattleState = value;
            }
        }
        private BattleState _BattleState;

        public IBattleTypeHandler TypeHandler { get; }
        public IBattleModeHandler ModeHandler { get; private set; }

        public double CountdownTimer { get; set; }
        public double DominationTimer { get; set; }

        public IEnumerable<BattlePlayer> AllBattlePlayers => ModeHandler.Players;
        public List<BattlePlayer> MatchPlayers { get; } = new();

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
