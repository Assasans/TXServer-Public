using System;
using System.Numerics;
using System.Threading;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Energy;
using TXServer.ECSSystem.Components.Battle.Health;
using TXServer.ECSSystem.Components.Battle.Hull;
using TXServer.ECSSystem.Components.Battle.Incarnation;
using TXServer.ECSSystem.Components.Battle.Location;
using TXServer.ECSSystem.Components.Battle.Round;
using TXServer.ECSSystem.Components.Battle.Tank;
using TXServer.ECSSystem.Components.Battle.Team;
using TXServer.ECSSystem.Components.Battle.Time;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.MatchMaking
{
    [SerialVersionUID(1494937115182)]
    public class EnterToMatchMakingEvent : ECSEvent
    {
        public void Execute(Player player, Entity chosenMode)
        {
            foreach (Entity mode in MatchmakingModes.GlobalItems.GetAllItems())
            {
                if (mode.EntityId.Equals(chosenMode.EntityId))
                {
                    Entity battleLobby = new Entity(new TemplateAccessor(new MatchMakingLobbyTemplate(), ""),
                        Maps.GlobalItems.Rio.GetComponent<MapGroupComponent>(),
                        new BattleModeComponent(BattleMode.CTF),
                        new UserLimitComponent(2, 1),
                        new GravityComponent(1.0F, GravityType.EARTH),
                        new MatchMakingLobbyStartTimeComponent(DateTimeOffset.Now.AddSeconds(5))
                        // new MatchMakingLobbyStartingComponent()
                        );
                    battleLobby.Components.Add(new BattleLobbyGroupComponent(battleLobby));

                    Entity fakePlayer = new Entity(new TemplateAccessor(new UserTemplate(), ""),
                        new UserComponent(),
                        new BattleLobbyGroupComponent(battleLobby),
                        new UserUidComponent("AReallyCoolPlayer"),
                        new TeamColorComponent(TeamColor.RED),
                        new UserAvatarComponent("6a770fe0-9e8c-4602-947f-4ed1032cee4a"), // crab
                        new BattleLeaveCounterComponent() //todo add data
                    );
                    fakePlayer.Components.Add(new UserGroupComponent(fakePlayer.EntityId));

                    Console.WriteLine("Yay, we found a match. " + mode.EntityId);
                    //todo make use of a lobby entity instead of using the player's entity id?
                    CommandManager.SendCommands(player,
                        new SendEventCommand(new EnteredToMatchMakingEvent(), player.User),
                        new EntityShareCommand(battleLobby),
                        new EntityShareCommand(fakePlayer),
                        new ComponentAddCommand(player.User, new MatchMakingUserComponent()),
                        new ComponentAddCommand(player.User, new BattleLobbyGroupComponent(battleLobby)),
                        new ComponentAddCommand(player.User, new TeamColorComponent(TeamColor.BLUE))
                    );
                    //todo MatchMakingLobbySystem & MatchMakingEntranceSystem.
                    // how does setStarting for example work (Lobby)

                    new Thread(() =>
                    {
                        Thread.Sleep(5000);
                        
                        Entity battle = new Entity(new TemplateAccessor(new CTFTemplate(), "battle/modes/ctf"),
                            new BattleModeComponent(BattleMode.CTF),
                        // Entity battle = new Entity(new TemplateAccessor(new DMTemplate(), "battle/modes/dm"),
                        //     new BattleModeComponent(BattleMode.DM),
                            new BattleComponent(),
                            new TeamBattleComponent(),
                            new CTFComponent(),
                            // new DMComponent(),
                            new TimeLimitComponent(120, 60), //optional, same as ScoreLimit (according to BattleInfoSystem)
                            Maps.GlobalItems.Rio.GetComponent<MapGroupComponent>(),
                            new GravityComponent(1.0F, GravityType.EARTH),
                            new UserLimitComponent(2, 1), // from TeamBattleScoreTableSystem#BattleNode
                            new BattleStartTimeComponent(DateTimeOffset.Now.AddSeconds(20)), // From WarmingUpTimerNotificationSystem#BattleNode
                            new BattleConfiguratedComponent(), // from BattleInfoSystem#BattleNode
                            new VisibleItemComponent(), // from BattleInfoSystem#VisibleBattleNode
                            new UserCountComponent(4) // from BattleInfoSystem#BattleNode
                        );
                        battle.Components.Add(new BattleGroupComponent(battle));
                        
                        Entity battleUser = new Entity(new TemplateAccessor(new BattleUserTemplate(), "battle/battleuser"),
                            new UserGroupComponent(player.User),
                            new BattleGroupComponent(battle),
                            
                            new UserInBattleAsTankComponent(),
                            // new UserInBattleAsSpectatorComponent(99L),//todo
                            
                            new IdleCounterComponent(10000L), //todo this is the kick time after becoming idle
                            new SelfBattleUserComponent()
                            );
                        
                        CommandManager.SendCommands(player, new Command[]
                        {
                            new EntityShareCommand(battle),
                            new EntityShareCommand(battleUser)
                        });

                        new Thread(() =>
                        {
                            Thread.Sleep(3000);

                            Entity redTeam = new Entity(new TemplateAccessor(new TeamTemplate(), ""),
                                new TeamComponent(),
                                new TeamColorComponent(TeamColor.RED),
                                new BattleGroupComponent(battle),
                                new TeamScoreComponent(100),
                                new PositionComponent(new Vector3(100, 200, 100)),
                                new RotationComponent(new Vector3(0, 0, 0))
                                );
                            redTeam.Components.Add(new TeamGroupComponent(redTeam));
                            
                            Entity blueTeam = new Entity(new TemplateAccessor(new TeamTemplate(), ""),
                                new TeamComponent(),
                                new TeamColorComponent(TeamColor.BLUE),
                                new BattleGroupComponent(battle),
                                new TeamScoreComponent(100),
                                new PositionComponent(new Vector3(-100, 200, -100)),
                                new RotationComponent(new Vector3(0, 0, 0))
                                );
                            blueTeam.Components.Add(new TeamGroupComponent(blueTeam));

                            Entity round = new Entity(new TemplateAccessor(new RoundTemplate(), ""),
                                new RoundComponent(),
                                new BattleGroupComponent(battle),
                                
                                // WarmingUpTimerSystem
                                new RoundStopTimeComponent(DateTimeOffset.Now.AddSeconds(40)),
                                new RoundActiveStateComponent()
                                // new RoundWarmingUpStateComponent() todo
                            );
                            
                            // Entity tank = new Entity(new TemplateAccessor(new TankTemplate(), ""),
                            //     new SelfTankComponent(),
                            //     // new TankComponent(),
                            //     new BattleGroupComponent(battle), //maybe autoadded with selftankcomponent?
                            //     new UserGroupComponent(player.User),
                            //     new TankSpawnStateComponent(),
                            //     new PositionComponent(new Vector3(0, 200, 0)),
                            //     new RotationComponent(new Vector3(0, 0, 0)),
                            //     
                            //     new HealthComponent(200, 200), //todo test
                            //     new HealthConfigComponent(200),
                            //     
                            //     new TankMovableComponent() // WeaponRotationSystem
                            //     );
                            // tank.Components.Add(new TankGroupComponent(tank));
                            Entity tank = new Entity(new TemplateAccessor(new TankTemplate(), "battle/tank/hornet"),
                                // new SelfTankComponent(),
                                new TankComponent(),
                                new BattleGroupComponent(battle), //maybe autoadded with selftankcomponent?
                                new UserGroupComponent(player.User),
                                // new MarketItemGroupComponent(Hulls.GlobalItems.Hornet),
                                new TankPartComponent(),
                                // new TankSpawnStateComponent(), todo this should be in incarnation, see TransitionCameraSystem
                                new PositionComponent(new Vector3(20, 18, 98)),
                                new RotationComponent(new Vector3(0, 0, 0))
                                
                                // new HealthComponent(200, 200), //todo test
                                // new HealthConfigComponent(200),
                                
                                // new TankMovableComponent() // WeaponRotationSystem todo should be in incarnation as well
                                );
                            tank.Components.Add(new TankGroupComponent(tank));

                            Entity roundUser = new Entity(new TemplateAccessor(new RoundUserTemplate(), "battle/round/rounduser"),
                                new RoundUserStatisticsComponent(1, 100, 1, 0, 1),
                                new RoundUserComponent(),
                                new UserGroupComponent(player.User),
                                new TeamGroupComponent(redTeam),
                                new BattleGroupComponent(battle),
                                new TankGroupComponent(tank));
                                
                            // new TankIncarnationComponent()); // WeaponRotationSystem, might need to create an incarnation entity
                            
                            Entity incarnation = new Entity(new TemplateAccessor(new TankIncarnationTemplate(), ""),
                                new TankIncarnationKillStatisticsComponent(0),

                                // TankIncarnationSystem
                                // new TankIncarnationComponent(), //todo this is already present?
                                new TankGroupComponent(tank),
                                
                                new SelfTankComponent(), //todo check in combination with Tank
                                new TankSpawnStateComponent(),
                                new TankMovableComponent()
                                
                                // new HealthComponent(600, 1800) // HealthSystem todo is this added by default?
                                );

                            // RoundUserEquipment#HullNode & TurretNode
                            
                            // Entity hull = new Entity(
                            //     new UserGroupComponent(player.User),
                            //     new TankComponent(),
                            //     new MarketItemGroupComponent(Hulls.GlobalItems.Hornet),
                            //     new TankPartComponent(),
                            //     new TankGroupComponent(tank));
                            
                            Entity weapon = new Entity(new TemplateAccessor(new WeaponTemplate(), "battle/weapon/railgun"),
                                new TankPartComponent(),
                                new WeaponComponent(),
                                new WeaponCooldownComponent(2f), //todo remove?
                                new UserGroupComponent(player.User),
                                new TankGroupComponent(tank),

                                new WeaponEnergyComponent(20) //todo test
                                //
                                // // WeaponRotationSystem
                                // new WeaponRotationComponent(1.0F, 1.0F, 1.0F)
                            );
                            
                            Entity weaponSkin = new Entity(new TemplateAccessor(new WeaponSkinBattleItemTemplate(), "garage/skin/weapon/railgun/may2017"),
                                new WeaponSkinBattleItemComponent(),
                                new TankGroupComponent(tank));

                            Entity hullSkin = new Entity(new TemplateAccessor(new HullSkinBattleItemTemplate(), "garage/skin/tank/hornet/may2017"),
                                new HullSkinBattleItemComponent(),
                                new TankGroupComponent(tank));
                            
                            Entity weaponPaint = new Entity(new TemplateAccessor(new WeaponPaintBattleItemTemplate(), "garage/paint/marine"),
                                new WeaponPaintBattleItemComponent(),
                                new TankGroupComponent(tank));

                            Entity tankPaint = new Entity(new TemplateAccessor(new TankPaintBattleItemTemplate(), "garage/paint/marine"),
                                new TankPaintBattleItemComponent(),
                                new TankGroupComponent(tank));
                            
                            
                            player.ReferencedEntities["round"] = round;
                            player.ReferencedEntities["tank"] = tank;

                            CommandManager.SendCommands(player, new Command[]
                            {
                                new EntityShareCommand(round),
                                new EntityShareCommand(redTeam),
                                new EntityShareCommand(blueTeam),
                                new EntityShareCommand(tank),
                                new EntityShareCommand(roundUser),
                                
                                new EntityShareCommand(weapon),
                                
                                new EntityShareCommand(hullSkin),
                                new EntityShareCommand(weaponSkin),
                                new EntityShareCommand(tankPaint),
                                new EntityShareCommand(weaponPaint),
                                
                                new EntityShareCommand(incarnation), //todo
                                
                                new ComponentAddCommand(battleUser, new UserReadyToBattleComponent())
                                // new ComponentAddCommand(player.User, new TankGroupComponent(tank)),
                                
                                
                                //todo look into this more, TankMovementReceiverSystem
                            });
                        }).Start();
                        
                        // TankActiveStateComponent, TankSpawnStateComponent, SelfTankComponent,
                        // TankDeadStateComponent, TankVisibleStateComponent, TankSemiActiveStateComponent
                        // TankNewStateComponent
                    }).Start();
                }
            }
        }
    }
}