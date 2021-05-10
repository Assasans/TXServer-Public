using System;
using System.Collections.Generic;
using System.Linq;
using TXServer.Core.Battles;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.Types;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.Commands
{
    public static class ChatCommands
    {
        private static readonly Dictionary<string, (string, ChatCommandConditions, Func<Player, string[], string>)> Commands = new()
        {
            { "help", ("help [opt: hackBattle]", ChatCommandConditions.None, Help) },
            { "ping", (null, ChatCommandConditions.None, Ping) },

            { "stats", (null, ChatCommandConditions.Tester, Stats) },
            { "spawnInfo", (null, ChatCommandConditions.Tester | ChatCommandConditions.InMatch, SpawnInfo) },

            { "hackBattle", ("hackBattle [everyone/onlyme]", ChatCommandConditions.BattleOwner, HackBattle) },
            { "bulletSpeed", ("bulletSpeed [max/stuck/norm/number] [target]", ChatCommandConditions.HackBattle, BulletSpeed) },
            { "cheat", ("cheat [supply] [target]", ChatCommandConditions.HackBattle | ChatCommandConditions.ActiveTank, Cheat) },
            { "flag", ("flag [color] [deliver/drop/return/give] [target]", ChatCommandConditions.HackBattle | ChatCommandConditions.Admin, FlagAction) },
            { "gravity", ("gravity [number]", ChatCommandConditions.HackBattle | ChatCommandConditions.BattleOwner, Gravity) },
            { "kickback", ("kickback [number] [target]", ChatCommandConditions.HackBattle, Kickback) },
            { "kill", ("kill [target]", ChatCommandConditions.InMatch | ChatCommandConditions.Admin, KillPlayer) },
            { "turretReload", ("turretReload [instant/never] [target]", ChatCommandConditions.HackBattle, ReloadTime) },
            { "turretRotation", ("turretRotation [instant/stuck/norm/number]", ChatCommandConditions.HackBattle, TurretRotation) },
            { "jump", ("jump [multiplier]", ChatCommandConditions.HackBattle | ChatCommandConditions.InMatch, Jump) }
        };

        private static readonly Dictionary<ChatCommandConditions, string> ConditionErrors = new()
        {
            { ChatCommandConditions.Admin, "You are not an admin" },
            { ChatCommandConditions.Tester, "You are not a tester" },
            { ChatCommandConditions.InBattle, "You are not in battle" },
            { ChatCommandConditions.BattleOwner, "You do not own this battle" },
            { ChatCommandConditions.HackBattle, "HackBattle is not enabled or you don't have permission to it" },
            { ChatCommandConditions.InMatch, "You are not in match" },
            { ChatCommandConditions.ActiveTank, "Your tank is not active" },
        };

        /// <summary>
        /// Preprocesses command conditions
        /// </summary>
        static ChatCommands()
        {
            foreach (var _command in Commands)
            {
                var command = _command.Value;

                if (command.Item2.HasFlag(ChatCommandConditions.ActiveTank))
                    command.Item2 |= ChatCommandConditions.InMatch;
                if ((command.Item2 & (ChatCommandConditions.BattleOwner | ChatCommandConditions.HackBattle | ChatCommandConditions.InMatch)) != 0)
                    command.Item2 |= ChatCommandConditions.InBattle;

                Commands[_command.Key] = command;
            }
        }

        public static string CheckForCommand(Player player, string message)
        {
            if (!message.StartsWith('/')) return null;
            string[] args = message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return null;

            if (Commands.TryGetValue(args[0], out var desc))
            {
                var playerConditions = GetConditionsFor(player);

                foreach (var condition in Enum.GetValues<ChatCommandConditions>())
                {
                    if ((desc.Item2 & condition) == condition && (playerConditions & condition) != condition)
                        return ConditionErrors[condition];
                }

                return desc.Item3(player, args[1..]);
            }

            return "Unknown command. Enter \"/help\" to view available commands";
        }
        private static string Help(Player player, string[] args)
        {
            ChatCommandConditions playerConditions = GetConditionsFor(player);
            string message;

            if (!args.Any())
            {
                message = '/' + string.Join("\n/", from command in Commands
                                                   where playerConditions.HasFlag(command.Value.Item2) &&
                                                         !command.Value.Item2.HasFlag(ChatCommandConditions.HackBattle)
                                                   select command.Value.Item1 ?? command.Key);
            }
            else
            {
                ChatCommandConditions condition;
                switch (args[0])
                {
                    case "admin":
                        condition = ChatCommandConditions.Admin;
                        if (!player.Data.Admin)
                            return ConditionErrors[condition];
                        break;
                    case "hackBattle":
                        condition = ChatCommandConditions.HackBattle;
                        if (!playerConditions.HasFlag(ChatCommandConditions.HackBattle))
                            return ConditionErrors[condition];
                        break;
                    case "target":
                        message = "Valid arguments for 'target' in HackBattle arguments: 'username', 'all', " +
                                  "'blue'/'red' (only in team battles), 'others'";
                        if (player.Data.Admin)
                            message += "\nFor battles: 'all/custom/mm/others/this' or nothing";
                        return message;
                    case "test":
                        condition = ChatCommandConditions.Tester;
                        if (!player.Data.Beta)
                            return ConditionErrors[condition];
                        break;
                    default:
                        return "Invalid command, specific help message wasn't found";
                }

                message = '/' + string.Join("\n/", from command in Commands
                                                   where playerConditions.HasFlag(command.Value.Item2) &&
                                                         command.Value.Item2.HasFlag(condition)
                                                   select command.Value.Item1 ?? command.Key);
            }
            return message;
        }


        private static string BulletSpeed(Player player, string[] args)
        {
            float? bulletSpeed;
            string targetName;
            Battle battle = player.BattlePlayer.Battle;

            switch (args.Length)
            {
                case < 1:
                    return "Parsing error, '/bulletSpeed' needs 'max/stuck/norm' or numbers as argument";
                case < 2:
                    targetName = player.Data.Username;
                    break;
                default:
                    targetName = args[1];
                    break;
            }
            switch (args[0])
            {
                case "norm":
                    bulletSpeed = null;
                    break;
                case "max":
                    bulletSpeed = 1000;
                    break;
                case "stuck":
                    bulletSpeed = 0;
                    break;
                default:
                    bool successfullyParsed = float.TryParse(args[0], out float floatyKickback);
                    if (!successfullyParsed)
                        return "Parsing error, '/bulletSpeed' only allows 'max/stuck/norm' or numbers as argument";
                    if (floatyKickback < 0 || floatyKickback > 1000)
                        return "Command error, '/bulletSpeed' is limited to a range from 0 to 1000";
                    bulletSpeed = floatyKickback;
                    break;
            }

            List<BattleTankPlayer> targets = FindTargets(targetName, player);
            if (!targets.Any())
                return $"Error, the target '{targetName}' wasn't found in this battle";
            if (targets.Count == 1 &&
                targets[0].MatchPlayer?.Weapon.TemplateAccessor.Template is not RicochetBattleItemTemplate and not
                    TwinsBattleItemTemplate)
            {
                targets[0].BulletSpeed = bulletSpeed;
                return $"Error, '{targets[0].Player.Data.Username}' isn't equipped with Ricochet or Twins. " +
                       "Cheat gets applied as soon the target equips a bullet shooting turret";
            }

            bool targetWithoutBulletTurret = false;
            foreach (BattleTankPlayer target in targets)
            {
                target.BulletSpeed = bulletSpeed;

                if (!target.Player.IsInMatch) continue;
                if (bulletSpeed == null)
                {
                    battle.KeepRunning = true;
                    target.Rejoin = true;
                    target.SendEvent(new KickFromBattleEvent(), target.MatchPlayer.BattleUser);
                    continue;
                }

                if (target.MatchPlayer?.Weapon.TemplateAccessor.Template is RicochetBattleItemTemplate or
                    TwinsBattleItemTemplate)
                {
                    target.MatchPlayer.Weapon.ChangeComponent<WeaponBulletShotComponent>(component =>
                        component.BulletSpeed = (float)bulletSpeed);
                }
                else
                    targetWithoutBulletTurret = true;
            }

            string bulletSpeedWritten = bulletSpeed == null ? "back to normal" : $"to {bulletSpeed}";
            string message = targets.Count > 1
                ? $"Set the bullet speed for {targets.Count} players {bulletSpeedWritten}"
                : $"Set '{targets[0].Player.Data.Username}'s bullet speed {bulletSpeedWritten}";
            return targetWithoutBulletTurret
                ? message + ". Warning: At least one target isn't equipped with a bullet shooting turret (Ricochet/Twins)"
                : message;
        }

        private static string Cheat(Player player, string[] args)
        {
            if (!args.Any())
                return "Parsing error, '/cheat' needs at least the desired cheat as argument";

            string targetName = args.Length > 1 ? args[1] : player.Data.Username;
            List<BattleTankPlayer> targets = FindTargets(targetName, player);
            if (!targets.Any())
                return $"Error, the target '{targetName}' wasn't found in this battle";

            switch (args[0].ToUpper())
            {
                case "ARMOR" or "DAMAGE" or "REPAIR" or "SPEED" or "ALL":
                    List<BonusType> cheats = new();
                    if (Enum.TryParse(args[0].ToUpper(), out BonusType bonusType))
                        cheats.Add(bonusType);
                    else if (args[0].ToUpper() == "ALL")
                        cheats = new List<BonusType>((BonusType[])Enum.GetValues(typeof(SupplyType)));

                    foreach (BattleTankPlayer target in targets)
                        foreach (BonusType bonusCheat in cheats)
                            _ = new SupplyEffect(bonusCheat, target.MatchPlayer, true);

                    string cheatWritten = cheats.Count == 1
                        ? char.ToUpper(bonusType.ToString().First()) + bonusType.ToString().Substring(1).ToLower() +
                          " cheat"
                        : "Multiple cheats";
                    string hasHaveWritten = cheats.Count == 1 ? "has" : "have";
                    return targets.Count <= 1
                        ? $"{cheatWritten} {hasHaveWritten} been activated for '{targets[0].Player.Data.Username}'"
                        : $"{cheatWritten} {hasHaveWritten} been activated for multiple people";
                case "DISABLE":
                    int amount = 0;
                    foreach (SupplyEffect supplyEffect in targets.SelectMany(target =>
                        target.MatchPlayer.SupplyEffects.ToArray().Where(supply => supply.Cheat)))
                    {
                        amount += 1;
                        supplyEffect.Remove();
                    }
                    string cheatString = amount == 1 ? "cheat" : "cheats";
                    string hasHaveString = amount == 1 ? "has" : "have";
                    return targets.Count > 1
                        ? $"{amount} {cheatString} {hasHaveString} deactivated for multiple people"
                        : $"{amount} {cheatString} {hasHaveString} been deactivated for '{targets[0].Player.Data.Username}'";
                default:
                    return "Parsing error, valid cheats are only: 'armor', 'damage', 'repair', & 'speed'";
            }
        }

        private static string FlagAction(Player player, string[] args)
        {
            if (player.BattlePlayer?.Battle.Params.BattleMode is not BattleMode.CTF)
                return "Flag command is only available in CTF";
            if (args.Length < 2)
                return "Parsing error, '/flag' needs at least 2 arguments";
            if (args[0].ToUpper() is not "BLUE" and not "RED")
                return $"Parsing error, didn't find flag with the color '{args[0]}'";

            Battle battle = player.BattlePlayer.Battle;
            CTFHandler modeHandler = (CTFHandler)battle.ModeHandler;
            BattleView teamView = modeHandler.BattleViewFor(player.BattlePlayer);
            Flag flag = args[0].ToUpper() == "BLUE" ? teamView.AllyTeamFlag : teamView.EnemyTeamFlag;
            Entity enemyTeamOfFlag = args[0].ToUpper() == "BLUE" ? teamView.EnemyTeamEntity : teamView.AllyTeamEntity;

            switch (args[1])
            {
                case "deliver":
                    Flag oppositeFlag = args[0].ToUpper() == "BLUE"
                        ? teamView.EnemyTeamFlag
                        : teamView.AllyTeamFlag;

                    switch (oppositeFlag.State)
                    {
                        case FlagState.Captured:
                            oppositeFlag.Drop(false);
                            oppositeFlag.Return();
                            break;
                        case FlagState.Dropped:
                            oppositeFlag.Return();
                            break;
                    }
                    switch (flag.State)
                    {
                        case FlagState.Captured:
                            string carrierUid = flag.Carrier.Player.Data.Username;
                            flag.Deliver();
                            battle.UpdateScore(enemyTeamOfFlag);
                            return $"'{carrierUid}' delivered the {args[0].ToLower()} flag";
                        case FlagState.Dropped:
                            flag.Return(silent: true);
                            break;
                    }

                    battle.PlayersInMap.SendEvent(new FlagDeliveryEvent(), flag.FlagEntity);
                    battle.UpdateScore(enemyTeamOfFlag);
                    return $"Delivered the {args[0].ToLower()} flag";
                case "drop":
                    if (flag.State != FlagState.Captured)
                        return "Flag state error, flag needs to be captured";

                    flag.Drop(false);
                    return $"Dropped the {args[0].ToLower()} flag";
                case "return":
                    switch (flag.State)
                    {
                        case FlagState.Captured:
                            flag.Drop(false);
                            flag.Return();
                            break;
                        case FlagState.Dropped:
                            flag.Return();
                            break;
                        case FlagState.Home:
                            return "Flag state error, flag needs to be captured or dropped";
                    }
                    return $"Returned the {args[0].ToLower()} flag to its base";
                case "give":
                    string targetName;
                    targetName = args.Length < 3 ? player.Data.Username : args[2];
                    List<BattleTankPlayer> targets = FindTargets(targetName, player);
                    if (targets.Count == 0)
                        return $"Error, the target '{targetName}' wasn't found in this battle";
                    if (targets.Count > 1)
                        return $"Logical error, found {targets.Count} targets but expected one";
                    if (targets[0].Team == flag.Team)
                        return $"Logical error, {targetName} can't capture the flag of this team";
                    if (flag.Carrier == player.BattlePlayer)
                        return $"Logical error, {targetName} already has the {args[0].ToLower()} flag";

                    switch (flag.State)
                    {
                        case FlagState.Captured:
                            flag.Drop(false);
                            flag.Pickup(targets[0]);
                            break;
                        case FlagState.Home:
                            flag.Capture(targets[0]);
                            break;
                        case FlagState.Dropped:
                            flag.Pickup(targets[0]);
                            break;
                    }
                    return $"Gave flag to user '{targetName}'";
                default:
                    return $"Parsing error, '/flag' doesn't have an argument named '{args[1]}'";
            }
        }

        private static string Gravity(Player player, string[] args)
        {
            Battle battle = player.BattlePlayer.Battle;

            string notification = "Gravity changed ";
            float gravity;
            if (args.Length == 0)
            {
                gravity = battle.GravityTypes[battle.Params.Gravity];
                notification += "to params gravity";
            }
            else
            {
                bool successfullyParsed = float.TryParse(args[0], out gravity);
                if (!successfullyParsed)
                    return "Parsing error, '/gravity' only allows numbers or nothing as argument";
                notification += $"to {gravity}";
            }

            foreach (BaseBattlePlayer battlePlayer in battle.PlayersInMap)
            {
                battle.KeepRunning = true;
                battlePlayer.Rejoin = true;
                battlePlayer.SendEvent(new KickFromBattleEvent(), (battlePlayer as BattleTankPlayer)?.MatchPlayer.BattleUser ?? ((Spectator)battlePlayer).BattleUser);
            }

            battle.BattleEntity.ChangeComponent<GravityComponent>(component => component.Gravity = gravity);

            Dictionary<IEnumerable<Player>, Entity> targets = new()
            {
                { battle.JoinedTankPlayers.Select(b => b.Player), battle.BattleLobbyChatEntity }
            };

            ChatMessageReceivedEvent.SystemMessageOtherPlayers(notification, targets, player);

            return notification;
        }

        private static string HackBattle(Player player, string[] args)
        {
            if (player.BattlePlayer.Battle.IsMatchMaking)
                return "HackBattle cannot be enabled in matchmaking battles";

            CustomBattleHandler handler = (CustomBattleHandler)player.BattlePlayer.Battle.TypeHandler;
            string permissionInfo = "";

            if (args.Length == 1)
            {
                switch (args[0])
                {
                    case "everyone":
                        handler.HackBattleDemocracy = true;
                        permissionInfo = " for everyone to use";
                        break;
                    case "onlyme":
                        handler.HackBattleDemocracy = false;
                        permissionInfo = " for only you to use";
                        break;
                    default:
                        return "";
                }
                if (handler.HackBattle)
                    return "Changed HackBattle's permissions";
                handler.HackBattle = true;
            }
            else
                handler.HackBattle = !handler.HackBattle;

            Dictionary<IEnumerable<Player>, Entity> targets = new()
            {
                {
                    player.BattlePlayer.Battle.JoinedTankPlayers.Select(b => b.Player),
                    player.BattlePlayer.Battle.BattleLobbyChatEntity
                }
            };
            if (player.BattlePlayer.Battle.PlayersInMap.Any())
                targets.Add(player.BattlePlayer.Battle.PlayersInMap.Select(b => b.Player),
                    player.BattlePlayer.Battle.GeneralBattleChatEntity);
            string notification = handler.HackBattle
                ? "This is now a \"HackBattle\"! The owner can cheat & change a lot of things. Note: this is very experimental."
                : "HackBattle was disabled";
            ChatMessageReceivedEvent.SystemMessageOtherPlayers(notification, targets, player);

            return handler.HackBattle ? $"HackBattle was enabled{permissionInfo}" : $"HackBattle was disabled{permissionInfo}";
        }

        private static string Kickback(Player player, string[] args)
        {
            if (args.Length < 1)
                return "Parsing error, '/kickback' needs a number or 'norm' as argument";

            float? kickback;
            string targetName;
            Battle battle = player.BattlePlayer.Battle;

            switch (args.Length)
            {
                case < 1:
                    return "Parsing error, '/kickback' needs a number or 'norm' as argument";
                case < 2:
                    targetName = player.Data.Username;
                    break;
                default:
                    targetName = args[1];
                    break;
            }
            switch (args[0])
            {
                case "norm":
                    kickback = null;
                    break;
                default:
                    bool successfullyParsed = float.TryParse(args[0], out float floatyKickback);
                    if (!successfullyParsed)
                        return "Parsing error, '/kickback' only allows numbers or 'norm' as argument";
                    if (floatyKickback < 0 || floatyKickback > 1000)
                        return "Command error, '/kickback' only allowed in a range from 0 - 1000";
                    kickback = floatyKickback;
                    break;
            }

            List<BattleTankPlayer> targets = FindTargets(targetName, player);
            if (!targets.Any()) return $"Error, the user '{targetName}' wasn't found in this battle";
            foreach (BattleTankPlayer target in targets)
            {
                target.TurretKickback = kickback;

                if (!target.Player.IsInMatch) continue;
                if (kickback == null)
                {
                    battle.KeepRunning = true;
                    target.Rejoin = true;
                    target.SendEvent(new KickFromBattleEvent(), target.MatchPlayer.BattleUser);
                    continue;
                }

                target.MatchPlayer.Weapon.ChangeComponent<KickbackComponent>(component =>
                    component.KickbackForce = (float)kickback);
            }

            string kickbackWritten = kickback == null ? "back to normal" : $"to {kickback}";
            return targets.Count > 1
                ? $"Set turret kickback for multiple players {kickbackWritten}"
                : $"Set '{targets[0].Player.Data.Username}'s turret kickback {kickbackWritten}";
        }

        private static string KillPlayer(Player player, string[] args)
        {
            string targetName = args.Length < 1 ? player.Data.Username : args[0];

            List<BattleTankPlayer> targets = FindTargets(targetName, player);
            if (!targets.Any()) return $"Error, the user '{targetName}' wasn't found in this battle";
            foreach (BattleTankPlayer target in targets)
                target.MatchPlayer.SelfDestructionTime = DateTime.Now.AddSeconds(0);

            return targets.Count > 1
                ? $"{targets.Count} players have been killed"
                : $"'{targets[0].Player.Data.Username}' has been killed'";
        }

        private static string Ping(Player player, string[] args)
        {
            if (args.Length <= 0 || args[0] == player.Data.Username) return $"Network latency: {player.Connection.Ping} ms";
            if (!player.Data.Admin)
                return ConditionErrors[ChatCommandConditions.Admin];

            Player targetPlayer = Server.Instance.FindPlayerByUid(args[0]);
            return targetPlayer == null
                ? $"Error, the user '{args[0]}' wasn't found or isn't online yet"
                : $"Network latency of '{args[0]}': {targetPlayer.Connection.Ping} ms";
        }

        private static string ReloadTime(Player player, string[] args)
        {
            string targetName;
            Battle battle = player.BattlePlayer.Battle;
            float? unloadEnergyPerShot;

            switch (args.Length)
            {
                case < 1:
                    return "Parsing error, '/turretReload' needs & allows only 'instant', 'never' or 'norm' " +
                           "as first argument";
                case < 2:
                    targetName = player.Data.Username;
                    break;
                default:
                    targetName = args[1];
                    break;
            }
            switch (args[0])
            {
                case "instant" or "0":
                    unloadEnergyPerShot = 0;
                    break;
                case "never":
                    unloadEnergyPerShot = float.MaxValue;
                    break;
                case "norm":
                    unloadEnergyPerShot = null;
                    break;
                default:
                    return "Parsing error, '/turretReload' needs & allows only 'instant', 'never' or 'norm' " +
                           "as first argument";
            }

            List<BattleTankPlayer> targets = FindTargets(targetName, player);
            if (!targets.Any()) return $"Error, the user '{targetName}' wasn't found in this battle";
            foreach (BattleTankPlayer target in targets)
            {
                target.TurretUnloadEnergyPerShot = unloadEnergyPerShot;

                if (!target.Player.IsInMatch) continue;
                if (unloadEnergyPerShot == null)
                {
                    battle.KeepRunning = true;
                    target.Rejoin = true;
                    target.SendEvent(new KickFromBattleEvent(), target.MatchPlayer.BattleUser);
                    continue;
                }

                if (target.MatchPlayer.Weapon.GetComponent<StreamWeaponComponent>() != null)
                    target.MatchPlayer.Weapon.ChangeComponent<StreamWeaponEnergyComponent>(component =>
                        component.UnloadEnergyPerSec = (float)unloadEnergyPerShot);
                else
                    target.MatchPlayer.Weapon.ChangeComponent<DiscreteWeaponEnergyComponent>(component =>
                        component.UnloadEnergyPerShot = (float)unloadEnergyPerShot);
                target.MatchPlayer.Weapon.ChangeComponent<WeaponCooldownComponent>(component =>
                    component.CooldownIntervalSec = (float)unloadEnergyPerShot);

                switch (target.MatchPlayer.Weapon.TemplateAccessor.Template)
                {
                    case RailgunBattleItemTemplate:
                        target.MatchPlayer.Weapon.ChangeComponent<RailgunChargingWeaponComponent>(component =>
                            component.ChargingTime = (float)unloadEnergyPerShot);
                        break;
                    case ShaftBattleItemTemplate:
                        target.MatchPlayer.Weapon.ChangeComponent<ShaftEnergyComponent>(component =>
                        {
                            component.PossibleUnloadEnergyPerAimingShot = 0;
                            component.ReloadEnergyPerSec = 1;
                            component.UnloadAimingEnergyPerSec = 0;
                            component.UnloadEnergyPerQuickShot = 0;
                        });
                        break;
                }
            }

            return $"Changed reload time for '{targetName}' to {args[0]}";
        }

        private static string SpawnInfo(Player player, string[] args)
        {
            SpawnPoint lastSpawnPoint = player.BattlePlayer.MatchPlayer.LastSpawnPoint;
            return $"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, {player.BattlePlayer.Battle.Params.BattleMode}, " +
                   $"{player.User.GetComponent<TeamColorComponent>().TeamColor}, Number: {lastSpawnPoint.Number}";
        }

        private static string Stats(Player player, string[] args)
        {
            return $"Online players: {Server.Instance.Connection.Pool.Count(x => x.IsActive)}\n" +
                $"MM battles: {ServerConnection.BattlePool.Count(b => b.IsMatchMaking)}\n" +
                $"Custom battles: {ServerConnection.BattlePool.Count(b => !b.IsMatchMaking)}\n";
        }

        private static string TurretRotation(Player player, string[] args)
        {
            string notification = "Changed turret rotation speed";
            float? rotationSpeed;

            if (args.Length == 0)
            {
                rotationSpeed = null;
                notification += "to normal";
            }
            else
            {
                switch (args[0])
                {
                    case "instant":
                        rotationSpeed = 1000;
                        notification += "to instant";
                        break;
                    case "norm":
                        rotationSpeed = null;
                        notification += "to normal";
                        break;
                    case "stuck":
                        rotationSpeed = 0;
                        notification += "to be stuck";
                        break;
                    default:
                        bool successfullyParsed = float.TryParse(args[0], out float temp);
                        if (!successfullyParsed)
                            return "Parsing error, '/turretRotation' only allows 'instant/norm/stuck' or a number as argument";
                        if (temp is > 1000 or < 0)
                            return "Out of range, '/turretRotation' only allows a range from 0 to 1000";
                        rotationSpeed = temp;
                        notification += $"to {rotationSpeed}";
                        break;
                }
            }

            player.BattlePlayer.TurretRotationSpeed = rotationSpeed;

            player.BattlePlayer.Battle.KeepRunning = true;
            player.BattlePlayer.Rejoin = true;
            player.BattlePlayer.SendEvent(new KickFromBattleEvent(), player.BattlePlayer.MatchPlayer.BattleUser);

            return notification;
        }

        private static string Jump(Player player, string[] args)
        {
            MatchPlayer matchPlayer = player.BattlePlayer.MatchPlayer;
            Battle battle = player.BattlePlayer.Battle;

            float multiplier;
            if (args.Length == 0)
                multiplier = 15.0f;
            else
            {
                bool successfullyParsed = float.TryParse(args[0], out multiplier);
                if (!successfullyParsed)
                    return "Parsing error, '/jump' only allows numbers or nothing as argument";
                if (multiplier is > 200 or < -200)
                    return "Out of range, '/jump' only allows a range from -200 to 200";
            }

            if (battle.BattleState is BattleState.WarmUp && battle.CountdownTimer <= 4 ||
                player.BattlePlayer.MatchPlayer.TankState != TankState.Active)
                return "Command error, you can't do that yet";

            Entity effect = JumpEffectTemplate.CreateEntity(matchPlayer, multiplier);

            foreach (BaseBattlePlayer battlePlayer in battle.PlayersInMap)
                battlePlayer.ShareEntities(effect);

            matchPlayer.Battle.Schedule(() =>
            {
                foreach (BaseBattlePlayer battlePlayer in battle.PlayersInMap)
                    battlePlayer.UnshareEntities(effect);
            });

            return $"Jumped successfully (multiplier: {multiplier})";
        }


        private static ChatCommandConditions GetConditionsFor(Player player)
        {
            ChatCommandConditions conditions = 0;

            if (player.Data.Admin)
                conditions |= ChatCommandConditions.Admin;
            if (player.Data.Beta || player.Data.Admin)
                conditions |= ChatCommandConditions.Tester;

            if (player.IsInBattle)
            {
                conditions |= ChatCommandConditions.InBattle;

                if (player.IsBattleOwner || player.Data.Admin)
                    conditions |= ChatCommandConditions.BattleOwner;
                if (player.BattlePlayer.Battle.TypeHandler is CustomBattleHandler handler)
                {
                    if (handler.HackBattle && player.IsBattleOwner || handler.HackBattle && handler.HackBattleDemocracy)
                        conditions |= ChatCommandConditions.HackBattle;
                }
                if (player.Data.Admin)
                    conditions |= ChatCommandConditions.HackBattle;

                if (player.IsInMatch)
                    conditions |= ChatCommandConditions.InMatch;
                if (player.BattlePlayer?.MatchPlayer?.TankState == TankState.Active)
                    conditions |= ChatCommandConditions.ActiveTank;
            }

            return conditions;
        }

        private static List<BattleTankPlayer> FindTargets(string targetName, Player player)
        {
            Battle battle = player.BattlePlayer.Battle;
            List<BattleTankPlayer> targets = new();
            switch (targetName)
            {
                case "all":
                    targets.AddRange(battle.MatchTankPlayers);
                    break;
                case "blue" or "red":
                    if (player.BattlePlayer.Battle.Params.BattleMode != BattleMode.CTF) return targets;
                    CTFHandler modeHandler = (CTFHandler)battle.ModeHandler;
                    BattleView teamView = modeHandler.BattleViewFor(player.BattlePlayer);
                    targets.AddRange(targetName.ToUpper() == "BLUE" ? teamView.AllyTeamPlayers : teamView.EnemyTeamPlayers);
                    break;
                case "others":
                    targets.AddRange(battle.MatchTankPlayers.Where(p => p.Player != player));
                    break;
                default:
                    Player target = battle.FindPlayerByUid(targetName);
                    if (target != null && !target.BattlePlayer.IsCheatImmune || target == player)
                        targets.Add(target.BattlePlayer);
                    break;
            }

            targets = targets.Where(t => !t.IsCheatImmune || t.Player == player).ToList();
            return targets;
        }
    }
}
