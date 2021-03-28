using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Battle.Score;
using TXServer.ECSSystem.Events.Battle.VisualScore;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Battles
{
    public class MatchPlayer
    {
        public MatchPlayer(BattlePlayer battlePlayer, Entity battleEntity, IEnumerable<UserResult> userResults)
        {
            Battle = battlePlayer.Battle;
            Player = battlePlayer.Player;
            BattleUser = BattleUserTemplate.CreateEntity(battlePlayer.Player, battleEntity, battlePlayer.Team);
            Tank = TankTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullItem, BattleUser);
            Weapon = WeaponTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponItem, Tank);
            HullSkin = HullSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.HullSkins[battlePlayer.Player.CurrentPreset.HullItem], Tank);
            WeaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponSkins[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            WeaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponPaint, Tank);
            TankPaint = TankPaintBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.TankPaint, Tank);
            Graffiti = GraffitiBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.Graffiti, Tank);
            Shell = ShellBattleItemTemplate.CreateEntity(battlePlayer.Player.CurrentPreset.WeaponShells[battlePlayer.Player.CurrentPreset.WeaponItem], Tank);
            RoundUser = RoundUserTemplate.CreateEntity(battlePlayer, battleEntity, Tank);
            Incarnation = TankIncarnationTemplate.CreateEntity(Tank);
            UserResult = new(battlePlayer, userResults);

            if (Battle.ModeHandler is Battle.TeamBattleHandler handler)
                SpawnCoordinates = handler.BattleViewFor(Player.BattlePlayer).SpawnPoints;
            else
                SpawnCoordinates = ((Battle.DMHandler)Battle.ModeHandler).SpawnPoints;
        }

        public IEnumerable<Entity> GetEntities()
        {
            return from property in typeof(MatchPlayer).GetProperties()
                   where property.PropertyType == typeof(Entity)
                   select (Entity)property.GetValue(this);
        }

        private static readonly Dictionary<TankState, Type> StateComponents = new()
        {
            { TankState.New, typeof(TankNewStateComponent) },
            { TankState.Spawn, typeof(TankSpawnStateComponent) },
            { TankState.SemiActive, typeof(TankSemiActiveStateComponent) },
            { TankState.Active, typeof(TankActiveStateComponent) },
            { TankState.Dead, typeof(TankDeadStateComponent) },
        };

        public void DealDamage(Player damager, HitTarget hitTarget, int damage)
        {
            if (damager.BattlePlayer.MatchPlayer.SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.DAMAGE))
                damage = (int)(damage * 1.5);
            if (SupplyEffects.Any(supplyEffect => supplyEffect.BonusType == BonusType.ARMOR))
                damage = (int)(damage * 0.5);

            Tank.ChangeComponent<HealthComponent>(component =>
            {
                if (component.CurrentHealth >= 0)
                    component.CurrentHealth -= damage;

                if (component.CurrentHealth <= 0)
                {
                    TankState = TankState.Dead;

                    if (damager != Player)
                    {
                        int killScore = 10;
                        Battle.MatchPlayers.Select(x => x.Player).SendEvent(new KillEvent(damager.CurrentPreset.Weapon, hitTarget.Entity), damager.BattlePlayer.MatchPlayer.BattleUser);
                        damager.SendEvent(new VisualScoreKillEvent(Player.User.GetComponent<UserUidComponent>().Uid, Player.User.GetComponent<UserRankComponent>().Rank, damager.BattlePlayer.MatchPlayer.GetScoreWithPremium(killScore)), damager.BattlePlayer.MatchPlayer.BattleUser);
                        damager.BattlePlayer.MatchPlayer.UpdateStatistics(killScore, additiveKills:1, 0, 0, null);
                    }
                    else
                        Battle.MatchPlayers.Select(x => x.Player).SendEvent(new SelfDestructionBattleUserEvent(), BattleUser);
                    UpdateStatistics(0, 0, 0, 1, damager.BattlePlayer.MatchPlayer);

                    if (Battle.ModeHandler is TDMHandler)
                        Battle.UpdateScore(damager.BattlePlayer.Team, 1);

                    damager.BattlePlayer.MatchPlayer.UserResult.Damage += damage;

                    foreach (KeyValuePair<MatchPlayer, int> assist in damageAssisters.Where(assist => assist.Key != damager.BattlePlayer.MatchPlayer && assist.Key != this))
                    {
                        int assistScore = 5;
                        assist.Key.UpdateStatistics(additiveScore:assistScore, 0, additiveKillAssists:1, 0, null);
                        int percent = (int)(assist.Value / component.MaxHealth * 100);
                        assist.Key.Player.SendEvent(new VisualScoreAssistEvent(Player.User.GetComponent<UserUidComponent>().Uid, percent, assist.Key.GetScoreWithPremium(assistScore)), assist.Key.BattleUser);
                    }
                    damageAssisters.Clear();
                }
                else
                {
                    if (damageAssisters.ContainsKey(damager.BattlePlayer.MatchPlayer))
                        damageAssisters[damager.BattlePlayer.MatchPlayer] += damage;
                    else
                        damageAssisters.Add(damager.BattlePlayer.MatchPlayer, damage);
                }
                
                damager.SendEvent(new DamageInfoEvent(damage, hitTarget.LocalHitPoint, false, false), Tank);
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), Tank);
            });
        }

        public void IsisHeal(Player healer, HitTarget hitTarget)
        {
            bool healed = false;
            Tank.ChangeComponent<TemperatureComponent>(component =>
            {
                if (component.Temperature.CompareTo(0) < 0)
                    component.Temperature = 0;
                else if (component.Temperature > 0)
                    component.Temperature -= 2;
                else if (component.Temperature < 0)
                    component.Temperature += 2;
            });

            Tank.ChangeComponent<HealthComponent>(component =>
            {
                int healingPerSecond = 415;
                if (component.CurrentHealth != component.MaxHealth)
                {
                    healed = true;
                    if (component.MaxHealth - component.CurrentHealth > healingPerSecond)
                        component.CurrentHealth += healingPerSecond;
                    else
                        component.CurrentHealth = component.MaxHealth;
                }
            });
            
            if (healed)
            {
                Player.BattlePlayer.Battle.MatchPlayers.Select(x => x.Player).SendEvent(new HealthChangedEvent(), Tank);
                healer.SendEvent(new DamageInfoEvent(900, hitTarget.LocalHitPoint, false, true), Tank);
                healer.SendEvent(new VisualScoreHealEvent(healer.BattlePlayer.MatchPlayer.GetScoreWithPremium(4)), healer.BattlePlayer.MatchPlayer.BattleUser);
                healer.BattlePlayer.MatchPlayer.UpdateStatistics(additiveScore: 4, 0, 0, 0, null);
            }
        }

        public void ProcessKillStreak(int additiveKills, bool died, MatchPlayer killer)
        {
            if (additiveKills >= 1)
            {
                Incarnation.ChangeComponent<TankIncarnationKillStatisticsComponent>(component =>
                {
                    component.Kills += additiveKills;
                    if (component.Kills >= 2)
                    {
                        KillStreakScores.TryGetValue(component.Kills, out int streakScore);
                        if (component.Kills > 40) streakScore = 70;
                        Player.BattlePlayer.MatchPlayer.RoundUser.ChangeComponent<RoundUserStatisticsComponent>(statistics => statistics.ScoreWithoutBonuses += streakScore);
                        Player.SendEvent(new KillStreakEvent(streakScore), Incarnation);
                        if (component.Kills > 2)
                            Player.SendEvent(new VisualScoreStreakEvent(GetScoreWithPremium(streakScore)), BattleUser);
                    }
                });
            }

            if (died)
            {
                if (killer != null)
                {
                    Incarnation.ChangeComponent<TankIncarnationKillStatisticsComponent>(component =>
                    {
                        if (component.Kills >= 2)
                            killer.Player.SendEvent(new StreakTerminationEvent(Player.User.GetComponent<UserUidComponent>().Uid), killer.BattleUser);
                        component.Kills = 0;
                    });
                }
            }

        }

        public void UpdateStatistics(int additiveScore, int additiveKills, int additiveKillAssists, int additiveDeath, MatchPlayer killer)
        {
            // TODO: rank up effect/system
            RoundUser.ChangeComponent<RoundUserStatisticsComponent>(component =>
            {
                component.ScoreWithoutBonuses += additiveScore;
                component.Kills += additiveKills;
                component.KillAssists += additiveKillAssists;
                component.Deaths += additiveDeath;
            });
            UserResult.Kills += additiveKills;
            UserResult.KillAssists += additiveKillAssists;
            UserResult.Deaths += additiveDeath;

            ProcessKillStreak(additiveKills, additiveDeath > 0, killer);
            Battle.MatchPlayers.Select(x => x.Player).SendEvent(new RoundUserStatisticsUpdatedEvent(), RoundUser);
            Battle.SortRoundUsers();
            Player.CheckRankUp();
        }

        public int GetScoreWithPremium(int score)
        {
            if (Player.User.GetComponent<PremiumAccountBoostComponent>() == null) return score;
            else return score * 2;
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
                    player.UnshareEntity(prevIncarnation);
                    player.ShareEntity(Incarnation);
                }
            }

            LastSpawnPoint = SpawnCoordinates.Where(spawnCoordinate => spawnCoordinate.Number != LastSpawnPoint.Number).ElementAt(new Random().Next(1, SpawnCoordinates.Count - 1));

            /* in case you want to set another json for testing a SINGLE spawn coordinate  
            string CoordinatesJson = File.ReadAllText("YourPath\\test.json");
            coordinate = JsonSerializer.Deserialize<Coordinates.spawnCoordinate>(CoordinatesJson); */

            Tank.AddComponent(new TankMovementComponent(new Movement(LastSpawnPoint.Position, Vector3.Zero, Vector3.Zero, LastSpawnPoint.Rotation), new MoveControl(), 0, 0));
        }

        public void EnableTank()
        {
            if (KeepDisabled) return;
            Tank.AddComponent(new TankMovableComponent());
            Weapon.AddComponent(new ShootableComponent());
        }

        public void DisableTank()
        {
            if (KeepDisabled)
                Tank.TryRemoveComponent(StateComponents[_TankState]);
            Tank.TryRemoveComponent<SelfDestructionComponent>();

            if (!Tank.TryRemoveComponent<TankMovableComponent>()) return;
            Weapon.TryRemoveComponent<ShootableComponent>();
        }

        public void Tick()
        {
            if (SelfDestructionTime != null && DateTime.Now > SelfDestructionTime)
            {
                TankState = TankState.Dead;
                Battle.MatchPlayers.Select(x => x.Player).SendEvent(new SelfDestructionBattleUserEvent(), BattleUser);
                SelfDestructionTime = null;
                UpdateStatistics(0, 0, 0, additiveDeath: 1, null);
            }

            // switch state after it's ended
            if (DateTime.Now > TankStateChangeTime)
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
                (from matchPlayer in Battle.MatchPlayers
                 where matchPlayer.MatchPlayer != this
                 select matchPlayer.Player).SendEvent(pair.Value.Event, pair.Value.TankPart);
                TranslatedEvents.TryRemove(pair.Key, out _);
            }

            // supply effects
            foreach (SupplyEffect supplyEffect in SupplyEffects.ToArray())
            {
                if (DateTime.Now > supplyEffect.StopTime)
                    supplyEffect.Remove();
            }
        }

        public readonly Battle Battle;
        public Player Player { get; }
        public Entity BattleUser { get; }
        public Entity RoundUser { get; }

        public Entity Incarnation { get; set; }
        public Entity Tank { get; }
        public Entity Weapon { get; }
        public Entity HullSkin { get; }
        public Entity WeaponSkin { get; }
        public Entity WeaponPaint { get; }
        public Entity TankPaint { get; }
        public Entity Graffiti { get; }
        public Entity Shell { get; }

        public UserResult UserResult { get; }

        public long CollisionsPhase { get; set; } = -1;
        public TankState TankState
        {
            get => _TankState;
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
                        DisableTank();
                        Player.SendEvent(new SelfTankExplosionEvent(), Tank);
                        if (Battle.ModeHandler is CTFHandler handler)
                        {
                            foreach (Flag flag in handler.Flags.Values)
                            {
                                if (flag != null && flag.State == FlagState.Captured && flag.FlagEntity.GetComponent<TankGroupComponent>().Key == Tank.EntityId)
                                    flag.Drop(false);
                            }
                        }
                        foreach (SupplyEffect supplyEffect in SupplyEffects.ToArray())
                            supplyEffect.Remove();
                        break;
                }

                if (Tank.GetComponent(StateComponents[_TankState]) != null)
                    Tank.RemoveComponent(StateComponents[_TankState]);
                Tank.AddComponent((Component)Activator.CreateInstance(StateComponents[value]));
                _TankState = value;

                TankStateChangeTime = DateTime.Now.AddSeconds(value switch
                {
                    TankState.Spawn => 2,
                    TankState.SemiActive => .5,
                    TankState.Dead => 3,
                    _ => 0,
                });
            }
        }
        private TankState _TankState;
        public bool KeepDisabled { get; set; }

        public DateTime TankStateChangeTime { get; set; }
        public DateTime? SelfDestructionTime { get; set; }
        public bool WaitingForTankActivation { get; set; }

        public ConcurrentDictionary<Type, TranslatedEvent> TranslatedEvents { get; } = new ConcurrentDictionary<Type, TranslatedEvent>();
        public Vector3 TankPosition { get; set; }
        public bool Paused { get; set; } = false;
        public List<SupplyEffect> SupplyEffects { get; } = new();
        private Dictionary<MatchPlayer, int> damageAssisters { get; set; } = new();
        public int AlreadyAddedExperience { get; set; } = 0;

        private IList<SpawnPoint> SpawnCoordinates;
        public SpawnPoint LastSpawnPoint { get; set; } = new SpawnPoint();

        private readonly Dictionary<int, int> KillStreakScores = new() {{2,0},{3,5},{4,7},{5,10},{10,10},{15,10},{20,20},{25,30},{30,40},{35,50},{40,60}};
    }
}
