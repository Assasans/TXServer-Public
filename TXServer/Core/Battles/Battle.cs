using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TXServer.Core.Logging;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Bonus;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
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

            player.ShareEntities(AllBattlePlayers.Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).ShareEntity(player.User);

            BattlePlayer battlePlayer = ModeHandler.AddPlayer(player);
            TypeHandler.OnPlayerAdded(battlePlayer);
        }

        private void RemovePlayer(BattlePlayer battlePlayer)
        {
            ModeHandler.RemovePlayer(battlePlayer);
            TypeHandler.OnPlayerRemoved(battlePlayer);

            battlePlayer.User.RemoveComponent<UserEquipmentComponent>();
            battlePlayer.User.RemoveComponent<BattleLobbyGroupComponent>();
            battlePlayer.Player.UnshareEntities(BattleLobbyEntity, BattleLobbyChatEntity);

            if (battlePlayer.User.GetComponent<MatchMakingUserReadyComponent>() != null)
                battlePlayer.User.RemoveComponent<MatchMakingUserReadyComponent>();

            battlePlayer.Player.UnshareEntities(AllBattlePlayers.Select(x => x.User));
            AllBattlePlayers.Select(x => x.Player).UnshareEntity(battlePlayer.User);

            Logger.Log($"{battlePlayer.Player}: Left battle {BattleEntity.EntityId}");

            ServerConnection.BattlePool.RemoveAll(p => !p.AllBattlePlayers.Any() && !p.IsMatchMaking);
        }

        private void StartBattle()
        {
            if (!Params.DisabledModules)
            {
                var battleModesBonusRegionsSpawnPoints = new Dictionary<BattleMode, BonusList> {
                    { BattleMode.DM, CurrentMapInfo.BonusRegions.Deathmatch },
                    { BattleMode.CTF, CurrentMapInfo.BonusRegions.CaptureTheFlag },
                    { BattleMode.TDM, CurrentMapInfo.BonusRegions.TeamDeathmatch }};
                var bonusTypeSpawnPoints = new Dictionary<BonusType, IList<Bonus>> {
                    { BonusType.ARMOR,  battleModesBonusRegionsSpawnPoints[Params.BattleMode].Armor },
                    { BonusType.DAMAGE,  battleModesBonusRegionsSpawnPoints[Params.BattleMode].Damage },
                    { BonusType.GOLD,  battleModesBonusRegionsSpawnPoints[Params.BattleMode].Gold },
                    { BonusType.REPAIR,  battleModesBonusRegionsSpawnPoints[Params.BattleMode].Repair },
                    { BonusType.SPEED,  battleModesBonusRegionsSpawnPoints[Params.BattleMode].Speed }};

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

            foreach (BattlePlayer battlePlayer in AllBattlePlayers)
                InitMatchPlayer(battlePlayer);
        }

        public void FinishBattle()
        {
            BattleState = BattleState.Ended;
            foreach (BattlePlayer battlePlayer in MatchPlayers)
            {
                battlePlayer.MatchPlayer.Tank.RemoveComponent<TankActiveStateComponent>();
                battlePlayer.MatchPlayer.Tank.RemoveComponent<TankMovableComponent>();

                PersonalBattleResultForClient personalResult = new(battlePlayer.Player, ModeHandler.TeamBattleResultFor(battlePlayer));
                BattleResultForClient battleResultForClient = new(this, ModeHandler, personalResult);
                battlePlayer.Player.SendEvent(new BattleResultForClientEvent(battleResultForClient), battlePlayer.Player.User);

                BattleLeaveCounterComponent battleLeaveCounterComponent = battlePlayer.Player.User.GetComponent<BattleLeaveCounterComponent>();
                if (battleLeaveCounterComponent.Value > 0)
                    battleLeaveCounterComponent.Value -= 1;
                if (battleLeaveCounterComponent.NeedGoodBattles > 0)
                    battleLeaveCounterComponent.NeedGoodBattles -= 1;
                battlePlayer.Player.User.ChangeComponent(battleLeaveCounterComponent);
            }

            foreach (BattlePlayer battlePlayer1 in AllBattlePlayers)
            {
                battlePlayer1.MatchPlayer.UserResult.ScoreWithoutPremium = battlePlayer1.MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>().ScoreWithoutBonuses;
                
                if (AllBattlePlayers.Count() <= 3 ||
                    (ModeHandler is TeamBattleHandler tbHandler && Math.Abs(tbHandler.RedTeamPlayers.Count - tbHandler.BlueTeamPlayers.Count) >= 2))
                    battlePlayer1.MatchPlayer.UserResult.UnfairMatching = true;
            }

            if (RoundEntity.GetComponent<RoundRestartingStateComponent>() == null)
                RoundEntity.AddComponent(new RoundRestartingStateComponent());
            if (BattleLobbyEntity.GetComponent<BattleGroupComponent>() != null)
                BattleLobbyEntity.RemoveComponent<BattleGroupComponent>();
        }

        public void InitMatchPlayer(BattlePlayer battlePlayer)
        {
            battlePlayer.MatchPlayer = new MatchPlayer(battlePlayer, BattleEntity, (ModeHandler as TeamBattleHandler)?.BattleViewFor(battlePlayer).AllyTeamResults ?? ((DMHandler)ModeHandler).Results);

            battlePlayer.Player.ShareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses)
                {
                    battlePlayer.Player.ShareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned)
                        battlePlayer.Player.ShareEntity(battleBonus.Bonus);
                }
            }

            ModeHandler.OnMatchJoin(battlePlayer);

            foreach (BattlePlayer battlePlayer1 in MatchPlayers)
                battlePlayer.Player.ShareEntities(battlePlayer1.MatchPlayer.GetEntities());

            MatchPlayers.Add(battlePlayer);

            MatchPlayers.Select(x => x.Player).ShareEntities(battlePlayer.MatchPlayer.GetEntities());
        }

        private void RemoveMatchPlayer(BattlePlayer battlePlayer)
        {
            Player player = battlePlayer.Player;

            player.UnshareEntities(BattleEntity, RoundEntity, GeneralBattleChatEntity);

            if (!Params.DisabledModules)
            {
                foreach (BattleBonus battleBonus in BattleBonuses)
                {
                    player.UnshareEntity(battleBonus.BonusRegion);
                    if (battleBonus.BonusState == BonusState.Spawned)
                        player.UnshareEntity(battleBonus.Bonus);
                }
            }

            ModeHandler.OnMatchLeave(battlePlayer);

            MatchPlayers.Select(x => x.Player).UnshareEntities(battlePlayer.MatchPlayer.GetEntities());

            MatchPlayers.Remove(battlePlayer);

            foreach (BattlePlayer matchPlayer in MatchPlayers)
                player.UnshareEntities(matchPlayer.MatchPlayer.GetEntities());

            if (IsMatchMaking) RemovePlayer(battlePlayer);
            battlePlayer.Reset();
        }

        private void ProcessExitedPlayers()
        {
            foreach (BattlePlayer battlePlayer in AllBattlePlayers.ToArray())
            {
                if (!battlePlayer.Player.IsActive || battlePlayer.WaitingForExit)
                {
                    if (battlePlayer.MatchPlayer != null)
                        RemoveMatchPlayer(battlePlayer);
                    else
                        RemovePlayer(battlePlayer);
                }
            }
        }

        private void ProcessMatchPlayers(double deltaTime)
        {
            foreach (MatchPlayer MatchPlayer in MatchPlayers.Select(x => x.MatchPlayer))
            {
                if (MatchPlayer.TankState != TankState.Active && MatchPlayer.TankState != TankState.New)
                {
                    MatchPlayer.TankStateChangeCountdown -= deltaTime;
                }

                // switch state after it's ended
                if (MatchPlayer.TankStateChangeCountdown < 0)
                {
                    switch (MatchPlayer.TankState)
                    {
                        case TankState.Spawn:
                            MatchPlayer.TankState = TankState.SemiActive;
                            MatchPlayer.Tank.AddComponent(new TankVisibleStateComponent());
                            MatchPlayer.Tank.AddComponent(new TankMovableComponent());
                            break;
                        case TankState.SemiActive:
                            if (!MatchPlayer.WaitingForTankActivation)
                            {
                                MatchPlayer.Tank.AddComponent(new TankStateTimeOutComponent());
                                MatchPlayer.WaitingForTankActivation = true;
                            }
                            break;
                        case TankState.Dead:
                            MatchPlayer.TankState = TankState.Spawn;
                            MatchPlayer.Tank.RemoveComponent<TankVisibleStateComponent>();
                            MatchPlayer.Tank.RemoveComponent<TankMovableComponent>();
                            break;
                    }
                }

                if (MatchPlayer.CollisionsPhase == CollisionsComponent.SemiActiveCollisionsPhase)
                {
                    CollisionsComponent.SemiActiveCollisionsPhase++;

                    MatchPlayer.Tank.RemoveComponent<TankStateTimeOutComponent>();
                    BattleEntity.ChangeComponent(CollisionsComponent);

                    MatchPlayer.TankState = TankState.Active;
                    MatchPlayer.WaitingForTankActivation = false;
                }

                foreach (KeyValuePair<Type, TranslatedEvent> pair in MatchPlayer.TranslatedEvents)
                {
                    (from matchPlayer in MatchPlayers
                     where matchPlayer.MatchPlayer != MatchPlayer
                     select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                    MatchPlayer.TranslatedEvents.TryRemove(pair.Key, out _);
                }

                // supply effects
                foreach (KeyValuePair<BonusType, double> entry in MatchPlayer.SupplyEffects.ToArray())
                {
                    MatchPlayer.SupplyEffects[entry.Key] -= deltaTime;

                    if (entry.Value < 0)
                    {
                        switch (entry.Key)
                        {
                            case BonusType.ARMOR:
                                MatchPlayer.Tank.RemoveComponent<ArmorEffectComponent>();
                                break;
                            case BonusType.DAMAGE:
                                MatchPlayer.Tank.RemoveComponent<DamageEffectComponent>();
                                break;
                            case BonusType.SPEED:
                                MatchPlayer.Tank.RemoveComponent<TurboSpeedEffectComponent>();
                                MatchPlayer.Tank.ChangeComponent(new SpeedComponent(9.967f, 98f, 13.205f));
                                break;
                        }
                        MatchPlayer.SupplyEffects.Remove(entry.Key);
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

        public void Tick(double deltaTime)
        {
            lock (this)
            {
                CountdownTimer -= deltaTime;

                ProcessExitedPlayers();

                ModeHandler.Tick();
                TypeHandler.Tick();

                ProcessMatchPlayers(deltaTime);
                ProcessBonuses(deltaTime);
            }
        }

        public void UpdateScore(Player player, Entity team, int additiveScore)
        {
            var scoreComponent = team.GetComponent<TeamScoreComponent>();
            scoreComponent.Score += additiveScore;
            team.ChangeComponent(scoreComponent);
            MatchPlayers.Select(x => x.Player).SendEvent(new RoundScoreUpdatedEvent(), RoundEntity);
        }

        public void UpdateUserStatistics(Player player, int additiveScore, int additiveKills, int additiveKillAssists, int additiveDeath)
        {
            // TODO: rank up effect/system
            RoundUserStatisticsComponent roundUserStatisticsComponent = player.BattlePlayer.MatchPlayer.RoundUser.GetComponent<RoundUserStatisticsComponent>();
            UserExperienceComponent userExperienceComponent = player.User.GetComponent<UserExperienceComponent>();

            roundUserStatisticsComponent.ScoreWithoutBonuses += additiveScore;
            roundUserStatisticsComponent.Kills += additiveKills;
            roundUserStatisticsComponent.KillAssists += additiveKillAssists;
            roundUserStatisticsComponent.Deaths += additiveDeath;

            player.BattlePlayer.MatchPlayer.RoundUser.ChangeComponent(roundUserStatisticsComponent);

            MatchPlayers.Select(x => x.Player).SendEvent(new RoundUserStatisticsUpdatedEvent(), player.BattlePlayer.MatchPlayer.RoundUser);
        }

        private void CompleteWarmUp() => ModeHandler.CompleteWarmUp();

        private int EnemyCountFor(BattlePlayer battlePlayer) => ModeHandler.EnemyCountFor(battlePlayer);

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

        public ClientBattleParams Params { get; set; }
        public int WarmUpSeconds { get; set; }
        public Entity MapEntity { get; private set; }
        public bool IsMatchMaking { get; }
        private bool FlagsPlaced { get; set; }

        public MapInfo CurrentMapInfo { get; set; }
        public IList<SpawnPoint> DeathmatchSpawnPoints { get; set; }
        
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
        public WarmUpState WarmUpState { get; set; }
        private int ScoreGap { get; set; }

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

        public Entity GeneralBattleChatEntity { get; set; }
        public Entity BattleLobbyChatEntity { get; set; }
    }
}
