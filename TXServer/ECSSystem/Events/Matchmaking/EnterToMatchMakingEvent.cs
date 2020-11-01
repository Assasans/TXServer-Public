using System;
using System.Net.Http.Headers;
using System.Numerics;
using System.Threading;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Chassis;
using TXServer.ECSSystem.Components.Battle.Energy;
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
        public void Execute(Player player, Entity mode)
        {
            // Validate given mode entity
            // (I don't think chis check is really necessary)
            bool modeValid = false;
            foreach (Entity existingMode in MatchmakingModes.GlobalItems.GetAllItems())
            {
                if (mode.EntityId == existingMode.EntityId)
                {
                    modeValid = true;
                    break;
                }
            }
            if (!modeValid) throw new ArgumentException($"Invalid mode entity: {mode.TemplateAccessor.Template.GetType().Name} ({mode.TemplateAccessor.ConfigPath})");

            Entity battleLobby = MatchMakingLobbyTemplate.CreateEntity(Maps.GlobalItems.Rio, BattleMode.CTF, 2, 9.81f, GravityType.EARTH);

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
                new SendEventCommand(new EnteredToMatchMakingEvent(), mode),
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

                Entity battle = CTFTemplate.CreateEntity(battleLobby, 5, 600, 120);
                Entity redTeam = TeamTemplate.CreateEntity(TeamColor.RED, battle);
                Entity blueTeam = TeamTemplate.CreateEntity(TeamColor.BLUE, battle);

                Entity battleUser = new Entity(new TemplateAccessor(new BattleUserTemplate(), "battle/battleuser"),
                    new UserGroupComponent(player.User),
                    new BattleGroupComponent(battle),
                    new UserInBattleAsTankComponent(),
                    new BattleUserComponent(),
                    // new UserInBattleAsSpectatorComponent(99L),//todo
                    new IdleCounterComponent(10000L), //todo this is the kick time after becoming idle
                    new TeamGroupComponent(blueTeam)
                );

                

                new Thread(() =>
                {
                    Thread.Sleep(3000);

                    Entity round = new Entity(new TemplateAccessor(new RoundTemplate(), ""),
                        new RoundComponent(),
                        new BattleGroupComponent(battle),

                        // WarmingUpTimerSystem
                        new RoundStopTimeComponent(DateTimeOffset.Now.AddSeconds(40)),
                        new RoundActiveStateComponent()
                        // new RoundWarmingUpStateComponent() todo
                    );

                    // new TankIncarnationComponent()); // WeaponRotationSystem, might need to create an incarnation entity

                    // RoundUserEquipment#HullNode & TurretNode

                    Entity tank = TankTemplate.CreateEntity(player.CurrentPreset.HullItem, battleUser);
                    Entity weapon = WeaponTemplate.CreateEntity(player.CurrentPreset.WeaponItem, tank);
                    Entity hullSkin = HullSkinBattleItemTemplate.CreateEntity(player.CurrentPreset.HullSkins[player.CurrentPreset.HullItem], tank);
                    Entity weaponSkin = WeaponSkinBattleItemTemplate.CreateEntity(player.CurrentPreset.WeaponSkins[player.CurrentPreset.WeaponItem], tank);
                    Entity weaponPaint = WeaponPaintBattleItemTemplate.CreateEntity(player.CurrentPreset.WeaponPaint, tank);
                    Entity tankPaint = TankPaintBattleItemTemplate.CreateEntity(player.CurrentPreset.TankPaint, tank);
                    Entity shell = ShellBattleItemTemplate.CreateEntity(player.CurrentPreset.WeaponShells[player.CurrentPreset.WeaponItem], tank);

                    Entity roundUser = new Entity(new TemplateAccessor(new RoundUserTemplate(), "battle/round/rounduser"),
                        new RoundUserStatisticsComponent(1, 100, 1, 0, 1),
                        new RoundUserComponent(),
                        new UserGroupComponent(player.User),
                        new TeamGroupComponent(redTeam),
                        new BattleGroupComponent(battle),
                        new TankGroupComponent(tank));

                    player.ReferencedEntities["round"] = round;
                    player.ReferencedEntities["tank"] = tank;

                    CommandManager.SendCommands(player, new Command[]
                    {
                        new EntityShareCommand(redTeam),
                        new EntityShareCommand(blueTeam),

                        new EntityShareCommand(tank),
                        new EntityShareCommand(weapon),
                        new EntityShareCommand(hullSkin),
                        new EntityShareCommand(weaponSkin),
                        new EntityShareCommand(tankPaint),
                        new EntityShareCommand(weaponPaint),
                        new EntityShareCommand(shell),

                        new EntityShareCommand(battle),
                        new EntityShareCommand(battleUser),

                        new EntityShareCommand(round),
                        new EntityShareCommand(roundUser)
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