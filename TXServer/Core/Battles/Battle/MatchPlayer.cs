using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles.Effect;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Module;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class MatchPlayer : IPlayerPart
    {
        public MatchPlayer(BattleTankPlayer battlePlayer, Entity battleEntity, IEnumerable<UserResult> userResults)
        {
            Battle = battlePlayer.Battle;
            Player = battlePlayer.Player;

            BattleUser = BattleUserTemplate.CreateEntity(battlePlayer.Player, battleEntity, battlePlayer.Team);

            Tank = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, BattleUser, battlePlayer);
            Weapon = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, Tank, battlePlayer);
            HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], Tank);
            WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, Tank);
            TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, Tank);
            Graffiti = GraffitiBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.Graffiti, Tank);
            Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            Modules = new List<BattleModule>();
            Incarnation = TankIncarnationTemplate.CreateEntity(Tank);
            RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, battleEntity, Tank);

            PersonalBattleResult = new PersonalBattleResultForClient(Player);
            UserResult = new UserResult(battlePlayer, userResults);

            if (Battle.ModeHandler is TeamBattleHandler handler)
                _spawnCoordinates = handler.BattleViewFor(Player.BattlePlayer).SpawnPoints;
            else
                _spawnCoordinates = ((DMHandler)Battle.ModeHandler).SpawnPoints;
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
            { TankState.Spawn, (typeof(TankSpawnStateComponent), 1.5) },
            { TankState.SemiActive, (typeof(TankSemiActiveStateComponent), 1.75) },
            { TankState.Active, (typeof(TankActiveStateComponent), 0) },
            { TankState.Dead, (typeof(TankDeadStateComponent), 3) },
        };

        public void UpdateStatistics(int additiveScore, int additiveKills, int additiveKillAssists, int additiveDeath, MatchPlayer killer)
        {
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
        }

        public int GetScoreWithPremium(int score) => Player.IsPremium ? score * 2 : score;

        public void HealthChanged()
        {
            Battle.PlayersInMap.SendEvent(new HealthChangedEvent(), Tank);

            if (TryGetModule(out AdrenalineModule adrenalineModule))
                adrenalineModule.CheckActivationNecessity();
        }

        private void PrepareForRespawning()
        {
            Tank.TryRemoveComponent<TankVisibleStateComponent>();

            if (Tank.TryRemoveComponent<TankMovementComponent>())
            {
                Entity prevIncarnation = Incarnation;
                Incarnation = TankIncarnationTemplate.CreateEntity(Tank);

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
                LastSpawnPoint = _spawnCoordinates.Count == 1
                    ? _spawnCoordinates[0]
                    : _spawnCoordinates.Where(spawnCoordinate => spawnCoordinate.Number != LastSpawnPoint?.Number)
                        .ElementAt(new Random().Next(_spawnCoordinates.Count - 1));
            }
            else
            {
                LastTeleportPoint = NextTeleportPoint;
                NextTeleportPoint = null;
            }

            // ReSharper disable once PossibleNullReferenceException
            Vector3 position = LastSpawnPoint?.Position ?? LastTeleportPoint.Position;
            Quaternion rotation = LastSpawnPoint?.Rotation ?? LastTeleportPoint.Rotation;

            /* in case you want to set another json for testing a SINGLE spawn coordinate
            string CoordinatesJson = File.ReadAllText("YourPath\\test.json");
            coordinate = JsonSerializer.Deserialize<Coordinates.spawnCoordinate>(CoordinatesJson); */

            Tank.AddComponent(new TankMovementComponent(new Movement(position, Vector3.Zero, Vector3.Zero, rotation), new MoveControl(), 0, 0));
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
            Tank.AddComponent(new TankMovableComponent());
            Weapon.AddComponent(new ShootableComponent());

            foreach (BattleModule module in Modules.Where(m => m.ActivateOnTankSpawn && !m.IsOnCooldown))
                module.Activate();
        }

        public void DisableTank(bool deactivateModuleCooldown = false)
        {
            if (KeepDisabled)
                Tank.TryRemoveComponent(TankStates[_tankState].Item1);
            Tank.TryRemoveComponent<SelfDestructionComponent>();

            foreach (BattleModule module in Modules.ToArray().Where(m => m.DeactivateOnTankDisable))
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

                if (deactivateModuleCooldown && module.IsOnCooldown) module.DeactivateCooldown();
            }

            if (!Tank.TryRemoveComponent<TankMovableComponent>()) return;
            Weapon.TryRemoveComponent<ShootableComponent>();

            // trigger Drone to stay
            if (TryGetModule(out TurretDroneModule turretDroneModule)) turretDroneModule.Stay();
        }

        public void Tick()
        {
            if (SelfDestructionTime != null && DateTime.UtcNow > SelfDestructionTime && Battle.BattleState != BattleState.Ended)
            {
                if (TankState != TankState.Dead)
                {
                    TankState = TankState.Dead;
                    Battle.PlayersInMap.SendEvent(new SelfDestructionBattleUserEvent(), BattleUser);
                    UpdateStatistics(-10, -1, 0, 1, null);

                    TankPosition = new();
                    PrevTankPosition = new();
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
                            Tank.AddComponent(new TankStateTimeOutComponent());
                            WaitingForTankActivation = true;
                        }
                        break;
                    case TankState.Dead:
                        TankState = TankState.Spawn;
                        break;
                }
            }

            if (CollisionsPhase == Battle.CollisionsComponent.SemiActiveCollisionsPhase)
            {
                Battle.CollisionsComponent.SemiActiveCollisionsPhase++;

                Tank.RemoveComponent<TankStateTimeOutComponent>();
                Battle.BattleEntity.ChangeComponent(Battle.CollisionsComponent);

                TankState = TankState.Active;
                WaitingForTankActivation = false;

                var component = Tank.GetComponent<HealthComponent>();
                Tank.RemoveComponent<HealthComponent>();
                component.CurrentHealth = component.MaxHealth;
                Tank.AddComponent(component);
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

            if (Paused && DateTime.UtcNow > IdleKickTime)
            {
                Paused = false;
                IdleKickTime = null;
                Player.SendEvent(new KickFromBattleEvent(), BattleUser);
                Player.BattlePlayer.WaitingForExit = true;
            }
        }


        public readonly Battle Battle;
        public Player Player { get; }
        public Entity BattleUser { get; }
        public Entity RoundUser { get; }

        public Entity Incarnation { get; private set; }
        public Entity Tank { get; }
        public Entity Weapon { get; }
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
                        Tank.ChangeComponent(new TemperatureComponent(0));
                        Tank.AddComponent(new TankVisibleStateComponent());
                        break;
                    case TankState.Dead:

                        Player.SendEvent(new SelfTankExplosionEvent(), Tank);
                        DisableTank();
                        if (Battle.ModeHandler is CTFHandler handler)
                        {
                            foreach (Flag flag in handler.Flags.Values.Where(flag =>
                                flag is {State: FlagState.Captured} &&
                                flag.FlagEntity.GetComponent<TankGroupComponent>().Key == Tank.EntityId))
                            {
                                flag.Drop(false);
                            }
                        }
                        break;
                }

                Tank.TryRemoveComponent(TankStates[_tankState].Item1);
                Tank.AddComponent((Component)Activator.CreateInstance(TankStates[value].Item1));
                _tankState = value;

                TankStateChangeTime = DateTime.UtcNow.AddSeconds(TankStates[value].Item2);
            }
        }
        private TankState _tankState;
        private DateTime TankStateChangeTime { get; set; }
        private bool WaitingForTankActivation { get; set; }

        public Vector3 TankPosition { get; set; }
        public Vector3 PrevTankPosition { get; set; }
        public Quaternion TankQuaternion { get; set; }

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

        public Dictionary<BattleTankPlayer, TankDamageCooldown> DamageCooldowns { get; } = new();

        public bool Paused { get; set; }
        public DateTime? IdleKickTime { get; set; }

        public int AlreadyAddedExperience { get; set; }

        public Dictionary<MatchPlayer, float> DamageAssistants { get; } = new();

        public SpawnPoint LastSpawnPoint { get; private set; }
        public TeleportPoint LastTeleportPoint { get; private set; }
        public TeleportPoint NextTeleportPoint { get; set; }
        private readonly IList<SpawnPoint> _spawnCoordinates;
    }

    public class TankDamageCooldown
    {
        public DateTimeOffset LastDamageTime { get; set; }
        public TimeSpan DamageTime { get; set; }

        public TimeSpan DifferenceToLastShot => LastDamageTime == default ? default : DateTimeOffset.UtcNow - LastDamageTime;
    }
}
