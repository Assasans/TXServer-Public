using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles.BattleTanks;
using TXServer.Core.Battles.BattleWeapons;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Configuration;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.Tank;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Pause;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Events.Battle.Weapon.Hammer;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.ServerComponents.Tank;
using TXServer.ECSSystem.Types;
using TXServer.Library;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class MatchPlayer : IPlayerPart
    {
        public MatchPlayer(BattleTankPlayer battlePlayer)
        {
            OriginalSpeedComponent = Config.GetComponent<SpeedComponent>(
                battlePlayer.Player.CurrentPreset.HullItem.TemplateAccessor.ConfigPath);

            Battle = battlePlayer.Battle;
            Player = battlePlayer.Player;

            BattleUser = BattleUserTemplate.CreateEntity(battlePlayer.Player, battlePlayer.Battle.BattleEntity,
                battlePlayer.Team);

            TankEntity = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, BattleUser, battlePlayer);
            WeaponEntity = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, TankEntity, battlePlayer);
            HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], TankEntity);
            WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], TankEntity);
            WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, TankEntity);
            TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, TankEntity);
            Graffiti = GraffitiBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.Graffiti, TankEntity);
            Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], TankEntity);
            Modules = new List<BattleModule>();
            Incarnation = TankIncarnationTemplate.CreateEntity(TankEntity);
            RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, battlePlayer.Battle.BattleEntity, TankEntity);

            Tank = new BattleTank(this);
            Weapon = (BattleWeapon) Activator.CreateInstance(Weapons.WeaponToType[Player.CurrentPreset.Weapon], this);
            if (Weapon?.CustomComponents is not null)
            {
                foreach (Component component in Weapon.CustomComponents)
                {
                    WeaponEntity.TryRemoveComponent(component.GetType());
                    WeaponEntity.AddComponent(component);
                }
            }

            PersonalBattleResult = new PersonalBattleResultForClient(Player);
            UserResult = new UserResult(battlePlayer);

            TemperatureConfigComponent =
                Config.GetComponent<TemperatureConfigComponent>(TankEntity.TemplateAccessor.ConfigPath);

            if (Battle.ModeHandler is TeamBattleHandler handler)
                SpawnCoordinates = handler.BattleViewFor(Player.BattlePlayer).SpawnPoints;
            else
                SpawnCoordinates = ((SoloBattleHandler)Battle.ModeHandler).SpawnPoints;
        }

        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(MatchPlayer).GetProperties()
                   where property.PropertyType == typeof(Entity) && property.GetValue(this) != null
                   select (Entity)property.GetValue(this);
        }

        private static readonly Dictionary<TankState, (Type, double)> TankStates = new()
        {
            { TankState.New, (typeof(TankNewStateComponent), 0) },
            { TankState.Spawn, (typeof(TankSpawnStateComponent), 1.75) },
            { TankState.SemiActive, (typeof(TankSemiActiveStateComponent), 0.5) },
            { TankState.Active, (typeof(TankActiveStateComponent), 0) },
            { TankState.Dead, (typeof(TankDeadStateComponent), 3) }
        };

        public void UpdateStatistics(int additiveScore, int additiveKills, int additiveKillAssists, int additiveDeath,
            MatchPlayer killer = null)
        {
            if (!Player.IsActive) return;

            Player.BattlePlayer.MatchPlayer.RoundUser.ChangeComponent<RoundUserStatisticsComponent>(component =>
            {
                component.ScoreWithoutBonuses = Math.Clamp(component.ScoreWithoutBonuses + additiveScore, 0, int.MaxValue);
                component.Kills = Math.Clamp(component.Kills + additiveKills, 0, int.MaxValue);
                component.KillAssists = Math.Clamp(component.KillAssists + additiveKillAssists, 0, int.MaxValue);
                component.Deaths = Math.Clamp(component.Deaths + additiveDeath, 0, int.MaxValue);
            });

            // KillStrike = Kills, Kills = KillStrike
            UserResult.KillStrike += additiveKills;
            UserResult.KillAssists += additiveKillAssists;
            UserResult.Deaths += additiveDeath;

            Damage.ProcessKillStreak(additiveKills, additiveDeath > 0, this, killer);
            Battle.PlayersInMap.SendEvent(new RoundUserStatisticsUpdatedEvent(), RoundUser);
            Battle.SortRoundUsers();
            if (Battle.IsMatchMaking) Player.CheckRankUp();

            if (Battle.ModeHandler is DMHandler dmHandler) dmHandler.UpdateScore();
        }

        public int GetScoreWithBonus(int score) => (int) (PersonalBattleResult.ScoreBattleSeriesMultiplier * score + (Player.Data.IsPremium ? score * 0.5 : 0));

        private void PrepareForRespawning()
        {
            TankEntity.TryRemoveComponent<TankVisibleStateComponent>();

            if (TankEntity.TryRemoveComponent<TankMovementComponent>())
            {
                Entity prevIncarnation = Incarnation;
                Incarnation = TankIncarnationTemplate.CreateEntity(TankEntity);

                foreach (Player player in prevIncarnation.PlayerReferences.ToArray())
                {
                    player.UnshareEntities(prevIncarnation);
                    player.ShareEntities(Incarnation);
                }
            }

            LastSpawnPoint = null;
            LastTeleportPoint = null;

            if (NextTeleportPoint == null)
            {
                LastSpawnPoint = SpawnCoordinates.Count == 1
                    ? SpawnCoordinates[0]
                    : SpawnCoordinates.Where(spawnCoordinate => spawnCoordinate.Number != LastSpawnPoint?.Number)
                        .ElementAt(new Random().Next(SpawnCoordinates.Count - 1));
            }
            else
            {
                LastTeleportPoint = NextTeleportPoint;
                NextTeleportPoint = null;
            }

            // ReSharper disable once PossibleNullReferenceException
            Vector3 position = LastSpawnPoint?.Position ?? LastTeleportPoint.Position;
            Quaternion rotation = LastSpawnPoint?.Rotation ?? LastTeleportPoint.Rotation;

            TankEntity.AddComponent(new TankMovementComponent(new Movement(position, Vector3.Zero, Vector3.Zero, rotation), new MoveControl(), 0, 0));
        }

        public void RankUp()
        {
            Battle.PlayersInMap.SendEvent(new UpdateRankEvent(), Player.User);
            AlreadyAddedExperience += PersonalBattleResult.ScoreWithBonus - AlreadyAddedExperience;
            Player.Data.SetExperience(Player.Data.Experience + AlreadyAddedExperience, false);
        }

        public void SpeedByTemperature()
        {
            bool hasSpeedCheat = TryGetModule(typeof(TurboSpeedModule),
                out BattleModule turboSpeedModule) && turboSpeedModule.IsCheat;
            float originalSpeed = hasSpeedCheat ? float.MaxValue : OriginalSpeedComponent.Speed;

            float speed = MathUtils.Map(Temperature, 0, TemperatureConfigComponent.MinTemperature,
                originalSpeed, 0);
            speed = Math.Clamp(speed, originalSpeed / 100 * 20, originalSpeed);

            float turnSpeed = MathUtils.Map(Temperature, 0, TemperatureConfigComponent.MinTemperature,
                OriginalSpeedComponent.TurnSpeed, OriginalSpeedComponent.TurnSpeed / 100 * 50);

            float turretRotationMultiplier =
                MathUtils.Map(Temperature, 0, TemperatureConfigComponent.MinTemperature, 1, 0.5f);

            TankEntity.ChangeComponent<SpeedComponent>(component =>
            {
                component.Speed = speed;
                component.TurnSpeed = turnSpeed;
            });
            WeaponEntity.ChangeComponent<WeaponRotationComponent>(component =>
                component.ChangeByTemperature(Weapon, turretRotationMultiplier));
        }

        public bool TryGetModule(Type moduleType, out BattleModule module)
        {
            module = Modules.SingleOrDefault(m => m.GetType() == moduleType);
            return module != null;
        }

        public bool TryGetModule<T>(out T module) where T : BattleModule
        {
            module = Modules.SingleOrDefault(m => m.GetType() == typeof(T)) as T;
            return module != null;
        }

        public void TryDeactivateInvisibility() => Modules.SingleOrDefault(m => m.GetType() == typeof(InvisibilityModule))
            ?.Deactivate();


        public bool IsEnemyOf(MatchPlayer suspect) => (Battle.Params.BattleMode == BattleMode.DM ||
                                                       Player.BattlePlayer.Team != suspect.Player.BattlePlayer.Team) &&
                                                      suspect != this;

        private void EnableTank()
        {
            if (KeepDisabled) return;

            Weapon.OnSpawn();

            TankEntity.AddComponent(new TankMovableComponent());

            foreach (BattleModule module in Modules.Where(m => m.ActivateOnTankSpawn && !m.IsOnCooldown))
                module.Activate();
        }
        public void DisableTank(bool resetModuleCooldown = false)
        {
            TemperatureHits.Clear();
            Temperature = TemperatureFromAllHits();
            SpeedByTemperature();

            if (KeepDisabled)
                TankEntity.TryRemoveComponent(TankStates[_tankState].Item1);
            TankEntity.TryRemoveComponent<SelfDestructionComponent>();

            List<BattleModule> battleModules =
                resetModuleCooldown ? Modules : Modules.Where(m => m.DeactivateOnTankDisable).ToList();
            foreach (BattleModule module in battleModules)
            {
                module.TickHandlers.Clear();
                if (module.IsCheat)
                {
                    module.DeactivateCheat = true;
                    module.Deactivate();
                    module.DeactivateCheat = false;
                    module.IsWaitingForTank = true;
                    continue;
                }
                module.Deactivate();

                if (resetModuleCooldown && module.IsOnCooldown)
                {
                    module.DeactivateCooldown();
                    module.CurrentAmmunition = module.MaxAmmunition;
                }
            }

            TankEntity.ChangeComponent((Component) OriginalSpeedComponent.Clone());

            if (Weapon.GetType() == typeof(Vulcan)) ((Vulcan) Weapon).ResetOverheat();

            // trigger: Drone Module (stay after tank disable)
            if (TryGetModule(out TurretDroneModule turretDroneModule))
                foreach (var droneTuple in turretDroneModule.Drones)
                    TurretDroneModule.Stay(droneTuple.Item1);

            TankEntity.TryRemoveComponent<TankMovableComponent>();
            Weapon.OnDespawn();
        }

        public void Tick()
        {
            if (SelfDestructionTime != null && DateTime.UtcNow > SelfDestructionTime && Battle.BattleState != BattleState.Ended)
            {
                if (TankState != TankState.Dead)
                {
                    TankState = TankState.Dead;
                    Battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), BattleUser);

                    UpdateStatistics(-10, 0, 0, 1);
                    Player.User.ChangeComponent<UserStatisticsComponent>(
                        component => component.Statistics["SUICIDES"]++);

                    TankPosition = new();
                    PrevTankPosition = new();

                    if (Battle.ModeHandler is TDMHandler handler)
                        Battle.UpdateScore(handler.BattleViewFor(Player.BattlePlayer).AllyTeamEntity, -1);
                }

                SelfDestructionTime = null;
            }

            // switch state after it's ended
            if (DateTime.UtcNow > TankStateChangeTime)
            {
                switch (TankState)
                {
                    case TankState.Spawn:
                        TankState = TankState.SemiActive;
                        break;
                    case TankState.SemiActive:
                        if (!WaitingForTankActivation)
                        {
                            TankEntity.AddComponent(new TankStateTimeOutComponent());
                            WaitingForTankActivation = true;
                        }
                        break;
                    case TankState.Dead:
                        TankState = TankState.Spawn;
                        break;
                }
            }

            if (CollisionsPhase == Battle.CollisionsComponent.SemiActiveCollisionsPhase &&
                !Battle.DisqualifiedPlayers.Contains(this))
            {
                Battle.CollisionsComponent.SemiActiveCollisionsPhase++;

                TankEntity.TryRemoveComponent<TankStateTimeOutComponent>();
                Battle.BattleEntity.ChangeComponent(Battle.CollisionsComponent);

                TankState = TankState.Active;
                WaitingForTankActivation = false;

                Tank.CurrentHealth = Tank.MaxHealth;
            }

            foreach (KeyValuePair<Type, TranslatedEvent> pair in TranslatedEvents)
            {
                (from matchPlayer in Battle.PlayersInMap
                 where (matchPlayer as BattleTankPlayer)?.MatchPlayer != this
                 select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                TranslatedEvents.TryRemove(pair);
            }

            // battle modules
            foreach (BattleModule module in Modules.ToArray())
            {
                module.ModuleTick();
            }

            if (Paused && DateTime.UtcNow >= IdleKickTime)
            {
                if (Battle.SuppressInactivityKick)
                    new UnpauseEvent().Execute(Player, Player.User);
                else
                {
                    Paused = false;
                    IdleKickTime = null;
                    Player.SendEvent(new KickFromBattleEvent(), BattleUser);
                    Player.BattlePlayer.WaitingForExit = true;
                }
            }

            Weapon.Tick();
            Damage.DealAutoTemperature(this);
        }


        public readonly Battle Battle;
        public Player Player { get; }
        public Entity BattleUser { get; }
        public Entity RoundUser { get; }

        public BattleTank Tank { get; }
        public BattleWeapon Weapon { get; }

        public Entity Incarnation { get; private set; }
        public Entity TankEntity { get; }
        public Entity WeaponEntity { get; }
        public Entity HullSkin { get; }
        public Entity WeaponSkin { get; }
        public Entity WeaponPaint { get; }
        public Entity TankPaint { get; }
        public Entity Graffiti { get; }
        public Entity Shell { get; }
        public List<BattleModule> Modules { get; }

        public PersonalBattleResultForClient PersonalBattleResult { get; }
        public UserResult UserResult { get; }

        public long CollisionsPhase { get; set; } = -1;
        public bool KeepDisabled { get; set; }
        public DateTime? SelfDestructionTime { get; set; }
        public TankState TankState
        {
            get => _tankState;
            set
            {
                switch (value)
                {
                    case TankState.Spawn:
                        DisableTank();
                        PrepareForRespawning();
                        break;
                    case TankState.SemiActive:
                        EnableTank();
                        TankEntity.ChangeComponent(new TemperatureComponent(0));
                        TankEntity.AddComponent(new TankVisibleStateComponent());
                        break;
                    case TankState.Dead:
                        Player.SendEvent(new SelfTankExplosionEvent(), TankEntity);
                        DisableTank();
                        if (Battle.ModeHandler is CTFHandler handler)
                        {
                            foreach (Flag flag in handler.Flags.Values.Where(flag =>
                                flag is {State: FlagState.Captured} &&
                                flag.Carrier == Player.BattlePlayer))
                            {
                                flag.KillDrop(Player.BattlePlayer);
                            }
                        }
                        break;
                }

                TankEntity.TryRemoveComponent(TankStates[_tankState].Item1);
                TankEntity.AddComponent((Component)Activator.CreateInstance(TankStates[value].Item1));
                _tankState = value;

                if (value == TankState.Active &&
                    WeaponEntity.TemplateAccessor.Template.GetType() == typeof(HammerBattleItemTemplate))
                    Player.SendEvent(new SetMagazineReadyEvent(), WeaponEntity);

                TankStateChangeTime = DateTime.UtcNow.AddSeconds(TankStates[value].Item2);
            }
        }
        private TankState _tankState;
        private DateTime TankStateChangeTime { get; set; }
        private bool WaitingForTankActivation { get; set; }

        public Vector3 TankPosition { get; set; }
        public Vector3 PrevTankPosition { get; set; }
        public Quaternion TankQuaternion { get; set; }

        public float TemperatureFromAllHits() =>
            TemperatureHits.Sum(temperatureHit => temperatureHit.CurrentTemperature);
        public float Temperature
        {
            get => TankEntity.GetComponent<TemperatureComponent>().Temperature;
            set => TankEntity.ChangeComponent<TemperatureComponent>(component => component.Temperature = value);
        }
        public TemperatureConfigComponent TemperatureConfigComponent { get; set; }

        public ConcurrentDictionary<Type, TranslatedEvent> TranslatedEvents { get; } = new();

        public float ModuleCooldownSpeedCoeff
        {
            get => _moduleCooldownSpeedCoeff;
            set
            {
                BattleUser.TryRemoveComponent<BattleUserInventoryCooldownSpeedComponent>();
                BattleUser.AddComponent(new BattleUserInventoryCooldownSpeedComponent(value));
                this.SendEvent(new BattleUserInventoryCooldownSpeedChangedEvent(), BattleUser);

                foreach (BattleModule module in Modules.Where(m => m.ModuleEntity is not null))
                    module.UpdateCooldownSpeedCoeff(value is 1 ? _moduleCooldownSpeedCoeff : value, value is 1);

                _moduleCooldownSpeedCoeff = value;
            }
        }
        private float _moduleCooldownSpeedCoeff = 1;

        public Dictionary<MatchPlayer, DateTimeOffset> HitCooldownTimers { get; } = new();
        public Dictionary<MatchPlayer, (double, DateTimeOffset)> StreamHitLengths { get; } = new();
        public List<TemperatureHit> TemperatureHits { get; } = new();

        public bool Paused { get; set; }
        public DateTime? IdleKickTime { get; set; }

        public int AlreadyAddedExperience { get; private set; }

        public Dictionary<MatchPlayer, float> DamageAssistants { get; } = new();

        private SpeedComponent OriginalSpeedComponent { get; }

        public SpawnPoint LastSpawnPoint { get; private set; }
        public TeleportPoint LastTeleportPoint { get; private set; }
        public TeleportPoint NextTeleportPoint { get; set; }
        public readonly IList<SpawnPoint> SpawnCoordinates;
    }
}
