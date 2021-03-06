using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using TXServer.Core.Battles;
using TXServer.Core.Battles.Effect;
using TXServer.Core.Commands;
using TXServer.Core.ServerMapInformation;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Components.Battle;
using TXServer.ECSSystem.Components.Battle.Weapon;
using TXServer.ECSSystem.EntityTemplates.Battle;
using TXServer.ECSSystem.EntityTemplates.Battle.Effect;
using TXServer.ECSSystem.EntityTemplates.Battle.Weapon;
using TXServer.ECSSystem.Events.Battle;
using TXServer.ECSSystem.Events.Chat;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Types.Battle;
using static TXServer.Core.Battles.Battle;

namespace TXServer.Core.ChatCommands
{
    public static class ChatCommands
    {
        private static readonly Dictionary<string, (string, ChatCommandCondition[], Func<Player, string[], string>)>
            Commands = new()
        {
            { "help", ("help [opt: hackBattle]", new[]{ChatCommandCondition.None}, Help) },
            { "ping", (null, new[]{ChatCommandCondition.None}, Ping) },

            { "stats", (null, new[]{ChatCommandCondition.Tester}, Stats) },
            { "spawnInfo", (null, new[]{ChatCommandCondition.Tester, ChatCommandCondition.InMatch}, SpawnInfo) },

            { "hackBattle", ("hackBattle [everyone/onlyme]", new[]{ChatCommandCondition.TestServer, ChatCommandCondition.BattleOwner}, HackBattle) },
            { "bulletSpeed", ("bulletSpeed [max/stuck/norm/number] [target]", new[]{ChatCommandCondition.HackBattle}, BulletSpeed) },
            { "cases", ("cases (punishments)", new[]{ChatCommandCondition.None}, ListPunishments) },
            { "cheat", ("cheat [supply] [target]", new[]{ChatCommandCondition.HackBattle, ChatCommandCondition.ActiveTank}, Cheat) },
            { "flag", ("flag [color] [deliver/drop/return/give] [target]", new[]{ChatCommandCondition.HackBattle, ChatCommandCondition.Admin}, FlagAction) },
            { "gravity", ("gravity [number]", new[]{ChatCommandCondition.BattleOwner, ChatCommandCondition.HackBattle}, Gravity) },
            { "kickback", ("kickback [number] [target]", new[]{ChatCommandCondition.HackBattle}, Kickback) },
            { "kill", ("kill [target]", new[]{ChatCommandCondition.InMatch, ChatCommandCondition.Admin}, KillPlayer) },
            { "mode", ("kill [target]", new[]{ChatCommandCondition.BattleOwner, ChatCommandCondition.NonHackBattle, ChatCommandCondition.InactiveBattle}, Mode) },
            { "tp", ("teleport [target]", new[]{ChatCommandCondition.Premium, ChatCommandCondition.InMatch, ChatCommandCondition.HackBattle}, Teleport) },
            { "turretReload", ("turretReload [instant/never] [target]", new[]{ChatCommandCondition.HackBattle}, ReloadTime) },
            { "turretRotation", ("turretRotation [instant/stuck/norm/number]", new[]{ChatCommandCondition.HackBattle}, TurretRotation) },
            { "jump", ("jump [multiplier]", new[]{ChatCommandCondition.HackBattle, ChatCommandCondition.InMatch}, Jump) }
        };

        public static readonly Dictionary<ChatCommandCondition, string> ConditionErrors = new()
        {
            { ChatCommandCondition.ActiveTank, "Your tank needs to be active" },
            { ChatCommandCondition.Admin, "You are not an admin" },
            { ChatCommandCondition.BattleOwner, "You need to be the battle owner" },
            { ChatCommandCondition.HackBattle, "HackBattle is not enabled or you don't have permission to it" },
            { ChatCommandCondition.NonHackBattle, "This command is not allowed in HackBattles" },
            { ChatCommandCondition.InactiveBattle, "You need to be in a battle lobby with no active players in-battle" },
            { ChatCommandCondition.ActiveBattle, "You need to be in a battle with active players in-battle" },
            { ChatCommandCondition.InBattle, "You need to be in a battle" },
            { ChatCommandCondition.InMatch, "You need to be in a match" },
            { ChatCommandCondition.Premium, "You need an active premium pass" },
            { ChatCommandCondition.Tester, "You need to be a tester" },
            { ChatCommandCondition.TestServer, "This command is only available on test server" }
        };

        public static bool CheckForCommand(Player player, string message, out string reply)
        {
            reply = null;
            if (!message.StartsWith('/')) return false;
            string[] args = message[1..].Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length == 0) return false;

            if (Commands.TryGetValue(args[0], out var desc))
            {
                List<ChatCommandCondition> playerConditions = GetConditionsFor(player);

                foreach (var condition in Enum.GetValues<ChatCommandCondition>())
                    if (GetCompletedConditions(desc.Item2).Contains(condition) && !playerConditions.Contains(condition))
                        reply = ConditionErrors[condition];

                reply ??= desc.Item3(player, args[1..]);
            }

            reply ??= "Unknown command. Enter \"/help\" to view available commands";
            return true;
        }

        public static List<ChatCommandCondition> GetCompletedConditions(ChatCommandCondition[] commandConditions)
        {
            List<ChatCommandCondition> additionalConditions = new List<ChatCommandCondition>();
            List<ChatCommandCondition> inBattleConditions = new List<ChatCommandCondition>
                {ChatCommandCondition.BattleOwner, ChatCommandCondition.HackBattle, ChatCommandCondition.InMatch};

            if (commandConditions.Contains(ChatCommandCondition.ActiveTank))
                additionalConditions.Add(ChatCommandCondition.InMatch);
            if (commandConditions.Any(c => inBattleConditions.Contains(c)))
                additionalConditions.Add(ChatCommandCondition.InBattle);

            return additionalConditions;
        }

        private static string Help(Player player, string[] args)
        {
            List<ChatCommandCondition> playerConditions = GetConditionsFor(player);
            string message;

            if (!args.Any())
            {
                message = '/' + string.Join("\n/", from command in Commands
                                                   where !command.Value.Item2.Except(playerConditions).Any() &&
                                                         !command.Value.Item2.Contains(ChatCommandCondition.HackBattle)
                                                   select command.Value.Item1 ?? command.Key);
            }
            else
            {
                ChatCommandCondition condition;
                switch (args[0])
                {
                    case "admin":
                        condition = ChatCommandCondition.Admin;
                        if (!player.Data.Admin)
                            return ConditionErrors[condition];
                        break;
                    case "hackBattle":
                        condition = ChatCommandCondition.HackBattle;
                        if (!playerConditions.Contains(ChatCommandCondition.HackBattle))
                            return ConditionErrors[condition];
                        break;
                    case "target":
                        message = "Valid arguments for 'target' in HackBattle arguments: 'username', 'all', " +
                                  "'blue'/'red' (only in team battles), 'others'";
                        if (player.Data.Admin)
                            message += "\nFor battles: 'all/custom/mm/others/this' or nothing";
                        return message;
                    case "tp":
                        condition = ChatCommandCondition.Premium;
                        if (!playerConditions.Contains(condition))
                            return ConditionErrors[condition];
                        if (!player.IsInBattle)
                            return "Teleport your tank to a player or a teleport point. Use '/help tp' " +
                                   "in-battle to view available teleport points";
                        message = player.BattlePlayer.Battle.CurrentMapInfo.TeleportPoints.Aggregate(
                            "Available teleport points on this map: ", (current, tp) => current + $"{tp.Name}, ");
                        message = message.Remove(message.Length - 2);
                        return message;
                    default:
                        return "Invalid command, specific help message wasn't found";
                }

                message = '/' + string.Join("\n/", from command in Commands
                                                   where !command.Value.Item2.Except(playerConditions).Any() &&
                                                         command.Value.Item2.Contains(condition)
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
                targets[0].MatchPlayer?.WeaponEntity.TemplateAccessor.Template is not RicochetBattleItemTemplate and not
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

                if (target.MatchPlayer?.WeaponEntity.TemplateAccessor.Template is RicochetBattleItemTemplate or
                    TwinsBattleItemTemplate)
                {
                    target.MatchPlayer.WeaponEntity.ChangeComponent<WeaponBulletShotComponent>(component =>
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
                    foreach ((Type, Entity) desc in cheats.Select(bonusCheat => BonusToModule[bonusCheat]))
                    {
                        if (target.MatchPlayer.TankState is not TankState.Active)
                            return "error: tank of at least one target isn't active";

                        if (!target.MatchPlayer.TryGetModule(desc.Item1, out BattleModule module))
                        {
                            module = (BattleModule) Activator.CreateInstance(desc.Item1, target.MatchPlayer,
                                    desc.Item2);
                            target.MatchPlayer.Modules.Add(module);
                        }

                        Debug.Assert(module != null, nameof(module) + " != null");
                        module.IsCheat = true;
                        module.Activate();
                    }

                    string cheatWritten = cheats.Count == 1
                        ? char.ToUpper(bonusType.ToString().First()) + bonusType.ToString()[1..].ToLower() +
                          " cheat"
                        : "Multiple cheats";
                    string hasHaveWritten = cheats.Count == 1 ? "has" : "have";
                    return targets.Count <= 1
                        ? $"{cheatWritten} {hasHaveWritten} been activated for '{targets[0].Player.Data.Username}'"
                        : $"{cheatWritten} {hasHaveWritten} been activated for multiple people";
                case "DISABLE":
                    int amount = 0;
                    foreach (BattleModule effect in targets.SelectMany(target =>
                        target.MatchPlayer.Modules.ToArray().Where(effect => effect.IsCheat)))
                    {
                        amount += 1;
                        effect.DeactivateCheat = true;
                        effect.TickHandlers.Clear();
                        effect.Deactivate();
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

                    switch (targets.Count)
                    {
                        case 0:
                            return $"Error, the target '{targetName}' wasn't found in this battle";
                        case > 1:
                            return $"Logical error, found {targets.Count} targets but expected one";
                    }
                    if (targets[0].Team == flag.Team)
                        return $"Logical error, {targetName} can't capture the flag of this team";
                    if (flag.State == FlagState.Captured && flag.Carrier.Player.Data.Username == targetName)
                        return $"Logical error, {targetName} already has the {args[0].ToLower()} flag";

                    switch (flag.State)
                    {
                        case FlagState.Captured:
                            if (flag.Carrier.IsCheatImmune) return "Command error, not possible with this carrier";
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

                target.MatchPlayer.WeaponEntity.ChangeComponent<KickbackComponent>(component =>
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
                target.MatchPlayer.SelfDestructionTime = DateTime.UtcNow.AddSeconds(0);

            return targets.Count > 1
                ? $"{targets.Count} players have been killed"
                : $"'{targets[0].Player.Data.Username}' has been killed'";
        }

        private static string ListPunishments(Player player, string[] args)
        {
            if (!player.Data.Punishments.Any())
                return "'Error', you don't have any cases or punishments";
            return player.Data.Punishments.Aggregate("",
                    (current, punishment) => current + punishment.CreateLogMsg(player));
        }

        private static string Mode(Player player, string[] args)
        {
            if (args.Length < 1) return "Command error, missing argument: mode";

            ExtendedBattleMode newBattleMode;
            ClientBattleParams oldParams = player.BattlePlayer.Battle.Params;

            switch (args[0])
            {
                case "hps":
                    newBattleMode = ExtendedBattleMode.HPS;
                    oldParams.BattleMode = BattleMode.DM;
                    break;
                default:
                    return $"Command error: didn't find battle mode '{args[0]}'";
            }

            if (player.BattlePlayer.Battle.ExtendedBattleMode == newBattleMode)
                return "Very funny";

            player.BattlePlayer.Battle.ExtendedBattleMode = newBattleMode;
            player.BattlePlayer.Battle.UpdateParams(player.BattlePlayer.Battle.Params, true);

            return $"Changed battleMode to {newBattleMode.ToString()}";
        }

        private static string Ping(Player player, string[] args)
        {
            if (args.Length <= 0 || args[0] == player.Data.Username)
                return $"Network latency: {player.Connection.Ping} ms";
            if (!player.Data.Admin)
                return ConditionErrors[ChatCommandCondition.Admin];

            Player targetPlayer = Server.Instance.FindPlayerByUsername(args[0]);
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

                if (target.MatchPlayer.WeaponEntity.HasComponent<StreamWeaponComponent>())
                    target.MatchPlayer.WeaponEntity.ChangeComponent<StreamWeaponEnergyComponent>(component =>
                        component.UnloadEnergyPerSec = (float)unloadEnergyPerShot);
                else
                {
                    target.MatchPlayer.WeaponEntity.ChangeComponent<DiscreteWeaponEnergyComponent>(component =>
                        component.UnloadEnergyPerShot = (float)unloadEnergyPerShot);
                    target.MatchPlayer.WeaponEntity.ChangeComponent<WeaponCooldownComponent>(component =>
                        component.CooldownIntervalSec = (float)unloadEnergyPerShot);
                }

                switch (target.MatchPlayer.WeaponEntity.TemplateAccessor.Template)
                {
                    case RailgunBattleItemTemplate:
                        target.MatchPlayer.WeaponEntity.ChangeComponent<RailgunChargingWeaponComponent>(component =>
                            component.ChargingTime = (float)unloadEnergyPerShot);
                        break;
                    case ShaftBattleItemTemplate:
                        target.MatchPlayer.WeaponEntity.ChangeComponent<ShaftEnergyComponent>(component =>
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
            SpawnPoint lastSp = player.BattlePlayer.MatchPlayer.LastSpawnPoint;
            TeleportPoint lasTp = player.BattlePlayer.MatchPlayer.LastTeleportPoint;

            if (lastSp != null)
                return
                    $"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, {player.BattlePlayer.Battle.Params.BattleMode}, " +
                    $"{player.User.GetComponent<TeamColorComponent>().TeamColor}, SP: {lastSp.Number}";

            return $"{player.BattlePlayer.Battle.CurrentMapInfo.Name}, TP: {lasTp.Name}";
        }

        private static string Stats(Player player, string[] args)
        {
            return $"Online players: {Server.Instance.Connection.Pool.Count(x => x.IsActive)}\n" +
                $"MM battles: {ServerConnection.BattlePool.Count(b => b.IsMatchMaking)}\n" +
                $"Custom battles: {ServerConnection.BattlePool.Count(b => !b.IsMatchMaking)}\n";
        }

        private static string Teleport(Player player, string[] args)
        {
            string message = "Successfully teleported to ";

            if (args.Length > 0)
            {
                TeleportPoint teleportPoint = null;

                // CTF specific teleport points
                if (player.BattlePlayer.Battle.Params.BattleMode is BattleMode.CTF)
                {
                    TeamBattleHandler teamHandler = player.BattlePlayer.Battle.ModeHandler as TeamBattleHandler;
                    Debug.Assert(teamHandler != null, nameof(teamHandler) + " != null");
                    BattleView teamView = teamHandler.BattleViewFor(player.BattlePlayer);

                    teleportPoint = args[0].ToLower() switch
                    {
                        "blueflag" => new TeleportPoint("allyFlag", GetFlagPosition(teamView.AllyTeamFlag),
                            new Quaternion()),
                        "bluepedestal" => new TeleportPoint("allyPedestal",
                            teamView.AllyTeamFlag.PedestalEntity.GetComponent<FlagPedestalComponent>().Position,
                            new Quaternion()),
                        "redflag" => new TeleportPoint("enemyFlag", GetFlagPosition(teamView.EnemyTeamFlag)
                            ,
                            new Quaternion()),
                        "redpedestal" => new TeleportPoint("enemyPedestal",
                            teamView.EnemyTeamFlag.PedestalEntity.GetComponent<FlagPedestalComponent>().Position,
                            new Quaternion()),
                        _ => null
                    };

                    if (teleportPoint != null)
                        teleportPoint.Position = new Vector3(teleportPoint.Position.X, teleportPoint.Position.Y + 1,
                            teleportPoint.Position.Z);
                }

                // map specific points of interest
                teleportPoint ??=
                    player.BattlePlayer.Battle.CurrentMapInfo.TeleportPoints.SingleOrDefault(tp => tp.Name == args[0]);

                // other players
                if (teleportPoint == null)
                {
                    List<BattleTankPlayer> targets = FindTargets(args[0], player);
                    if (targets.Count == 1)
                    {
                        if (!targets[0].Player.IsInMatch)
                            return "Command error, target is in the battle lobby";
                        if (targets[0].MatchPlayer.TankState is TankState.Dead or TankState.Spawn)
                            return "Command error, target is currently spawning";
                        if (targets[0] == player.BattlePlayer)
                            return "Nope";

                        teleportPoint = new TeleportPoint($"Player {targets[0].Player.Data.Username}",
                            targets[0].MatchPlayer.TankPosition, targets[0].MatchPlayer.TankQuaternion);
                    }
                }

                // specific spawn point
                if (player.Data.Admin && teleportPoint is null)
                {
                    if (Int32.TryParse(args[0], out int number))
                    {
                        SpawnPoint spawnPoint = player.BattlePlayer.MatchPlayer.SpawnCoordinates.SingleOrDefault(s =>
                            s.Number == number);

                        if (spawnPoint is not null)
                            teleportPoint = new TeleportPoint($"spawnPoint {spawnPoint.Number.ToString()}",
                                spawnPoint.Position, spawnPoint.Rotation);
                    }
                }

                if (teleportPoint == null)
                    return $"Command error, didn't find target '{args[0]}'";

                message += teleportPoint.Name;
                player.BattlePlayer.MatchPlayer.NextTeleportPoint = teleportPoint;
            }
            else
                message += "the next spawn point";

            player.BattlePlayer.MatchPlayer.KeepDisabled = false;
            player.BattlePlayer.MatchPlayer.TankState = TankState.Spawn;

            return message;
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



        public static List<ChatCommandCondition> GetConditionsFor(Player player)
        {
            List<ChatCommandCondition> conditions = new();

            if (player.Server.Settings.TestServer || player.Data.Admin)
                conditions.Add(ChatCommandCondition.TestServer);

            if (player.Data.Admin)
                conditions.Add(ChatCommandCondition.Admin);
            if (player.Data.Beta || player.Data.Admin)
                conditions.Add(ChatCommandCondition.Tester);
            if (player.Data.IsPremium)
                conditions.Add(ChatCommandCondition.Premium);

            if (player.IsInBattle)
            {
                conditions.Add(ChatCommandCondition.InBattle);

                if (player.IsBattleOwner || player.Data.Admin)
                    conditions.Add(ChatCommandCondition.BattleOwner);
                if (player.BattlePlayer.Battle.TypeHandler is CustomBattleHandler handler)
                    if (handler.HackBattle && player.IsBattleOwner || handler.HackBattle && handler.HackBattleDemocracy)
                        conditions.Add(ChatCommandCondition.HackBattle);
                if (player.Data.Admin)
                    conditions.Add(ChatCommandCondition.HackBattle);

                if (player.IsInMatch)
                    conditions.Add(ChatCommandCondition.InMatch);
                if (player.BattlePlayer?.MatchPlayer?.TankState == TankState.Active)
                    conditions.Add(ChatCommandCondition.ActiveTank);
                conditions.Add(InactiveBattleStates.Contains(player.BattlePlayer.Battle.BattleState)
                    ? ChatCommandCondition.InactiveBattle
                    : ChatCommandCondition.ActiveBattle);
            }

            return conditions;
        }
        private static Vector3 GetFlagPosition(Flag flag)
        {
            return flag.State is FlagState.Captured
                ? flag.Carrier.MatchPlayer.TankPosition
                : flag.FlagEntity.GetComponent<FlagPositionComponent>().Position;
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
                    Player target = battle.FindPlayerByUsername(targetName);
                    if (target != null && !target.BattlePlayer.IsCheatImmune || target == player)
                        targets.Add(target.BattlePlayer);
                    break;
            }

            targets = targets.Where(t => !t.IsCheatImmune || t.Player == player).ToList();
            return targets;
        }

        public static readonly BattleState[] InactiveBattleStates =
        {
            BattleState.CustomNotStarted, BattleState.NotEnoughPlayers, BattleState.StartCountdown, BattleState.Starting
        };

        private static readonly Dictionary<BonusType, (Type, Entity)> BonusToModule = new()
        {
            { BonusType.ARMOR, (typeof(AbsorbingArmorModule), Modules.GlobalItems.Absorbingarmor) },
            { BonusType.DAMAGE, (typeof(IncreasedDamageModule), Modules.GlobalItems.Increaseddamage) },
            { BonusType.REPAIR, (typeof(RepairKitModule), Modules.GlobalItems.Repairkit) },
            { BonusType.SPEED, (typeof(TurboSpeedModule), Modules.GlobalItems.Turbospeed) }
        };
    }
}
