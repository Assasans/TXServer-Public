using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Types;

namespace TXServer.Core.ChatCommands
{
    public class Punishment
    {
        public Punishment()
        {
        }

        public static Punishment Create(PunishmentType type, PlayerData victim, string reason, Dictionary<string, string> trTimes = null,
            DateTimeOffset? expirationTime = null, bool isPermanent = false)
        {
            if (type != PunishmentType.Warn && expirationTime == null)
                throw new ArgumentNullException(nameof(expirationTime), "Expiration time can only be null on a warn punishment");

            Punishment punishment = new Punishment
            {
                StartTime = DateTimeOffset.UtcNow,
                ExpirationTime = expirationTime ?? DateTimeOffset.UtcNow,
                TrTimes = trTimes ?? new Dictionary<string, string>(),
                IsPermanent = isPermanent,
                Reason = reason,
                Player = victim,
                Type = type
            };
            punishment.OriginalDuration = punishment.ExpirationTime - DateTimeOffset.UtcNow;

            return punishment;
        }

        public string CreateLogMsg(Player target)
        {
            string msg;
            switch (target.Data.CountryCode.ToLower())
            {
                case "de":
                    string deType = Type switch
                    {
                        PunishmentType.Ban => "Ban",
                        PunishmentType.Mute => "Mute",
                        _ => "Warnung"
                    };
                    msg = $"{StartTime:g}{(TrTimes.ContainsKey("de") ? $" {TrTimes["de"]}" : "")}: {deType}" +
                          $"{(Reason != null ? $" wegen '{Reason}'" : "")}\n";
                    if (ForceRemoved)
                        msg += $"{ForceRemoveTime:g}:{deType} wurde entfernt{(ForceRemovedReason != null ? $" wegen '{ForceRemovedReason}'" : "")}\n";
                    break;
                case "ru":
                    string ruType = Type switch
                    {
                        PunishmentType.Ban => "бан",
                        PunishmentType.Mute => "мут",
                        _ => "предупреждение"
                    };
                    msg = $"{StartTime:g}{(TrTimes.ContainsKey("ru") ? $" {TrTimes["ru"]}" : "")}: {ruType}" +
                          $"{(Reason != null ? $" по причине '{Reason}'" : "")}\n";
                    if (ForceRemoved)
                        msg += $"{ForceRemoveTime:g}:{ruType} был " +
                               $"убран{(ForceRemovedReason != null ? $" по причине '{ForceRemovedReason}'" : "")}\n";
                    break;
                default:
                    msg = $"{StartTime:g}{(TrTimes.ContainsKey("en") ? $" {TrTimes["en"]}" : "")}: {Type} " +
                          $"{(Reason != null ? $"because of '{Reason}'" : "")}\n";
                    if (ForceRemoved)
                        msg += $"{ForceRemoveTime:g}: Removed {Type.ToString().ToLower()}{(ForceRemovedReason != null ? $" because of '{ForceRemovedReason}'" : "")}\n";
                    break;
            }

            return msg;
        }

        private string CreateNewTimePunishmentMsg(Player target)
        {
            string countryCode = target.Data.CountryCode.ToLower();
            string trMsg;
            switch (countryCode)
            {
                case "de":
                    trMsg = $"'{Player.Username}' wurde für {TrTimes[countryCode]} {(Type == PunishmentType.Ban ? "gebannt" : "gemutet")}";
                    if (Reason is not null) trMsg += $". Grund: {Reason}";
                    break;
                case "ru":
                    trMsg = $"'{Player.Username}' получил {(Type == PunishmentType.Ban ? "бан" : "мут")} на {TrTimes["ru"]}";
                    if (Reason is not null) trMsg += $". Причина: {Reason}";
                    break;
                default:
                    trMsg = $"'{Player.Username}' was muted {(!IsPermanent ? "for " : "")}{TrTimes["en"]}";
                    if (Reason is not null) trMsg += $". Reason: {Reason}";
                    break;
            }

            return trMsg;
        }

        private string CreateUnTimePunishmentMsg(Player target)
        {
            return target.Data.CountryCode.ToLower() switch
            {
                "de" => $"'{Player.Username}' wurde {(Type == PunishmentType.Ban ? "entbannt" : "entmutet")}{(ForceRemovedReason != null ? $". Grund: {ForceRemovedReason}" : "")}",
                "ru" => $"'{Player.Username}' был {(Type == PunishmentType.Ban ? "разбанен" : "размучен")}{(ForceRemovedReason != null ? $". Причина: {ForceRemovedReason}" : "")}",
                _ => $"'{Player.Username}' was {(Type == PunishmentType.Ban ? "unbanned" : "unmuted")}{(ForceRemovedReason != null ? $". Reason: {ForceRemovedReason}" : "")}"
            };
        }

        private string CreateWarnMsg(Player target)
        {
            return target.Data.CountryCode.ToLower() switch
            {
                "de" => $"'{Player.Username}' wurde gewarnt{(Reason != null ? $". Grund: {Reason}" : "")}",
                "ru" => $"'{Player.Username}' был предупрежден{(Reason != null ? $". Причина: {Reason}" : "")}",
                _ => $"'{Player.Username}' was warned{(Reason != null ? $". Reason: {Reason}" : "")}"
            };
        }

        private string CreateUnWarnMsg(Player target)
        {
            return target.Data.CountryCode.ToLower() switch
            {
                "de" => $"'{Player.Username}' wurde entwarnt{(Reason != null ? $". Grund: {Reason}" : "")}",
                "ru" => $"Игроку '{Player.Username}' сняли предупреждение{(Reason != null ? $". Причина: {Reason}" : "")}",
                _ => $"'{Player.Username}' was unwarned{(Reason != null ? $". Reason: {Reason}" : "")}"
            };
        }

        private string CreateNewPunishmentMsg(Player target) =>
            Type == PunishmentType.Warn ? CreateWarnMsg(target) : CreateNewTimePunishmentMsg(target);

        private string CreateUnPunishmentMsg(Player target) =>
            Type == PunishmentType.Warn ? CreateUnWarnMsg(target) : CreateUnTimePunishmentMsg(target);

        public string CreatePunishmentMsg(Player target) =>
            ForceRemoved ? CreateUnPunishmentMsg(target) : CreateNewPunishmentMsg(target);


        public long PlayerId { get; set; }
        public virtual PlayerData Player { get; set; }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long PunishmentId { get; set; }

        public PunishmentType Type { get; set; }

        public DateTimeOffset StartTime { get; set; }
        public DateTimeOffset ExpirationTime { get; set; }
        public TimeSpan OriginalDuration { get; set; }
        [NotMapped] public Dictionary<string, string> TrTimes { get; set; }
        public TimeSpan Duration => ExpirationTime - DateTimeOffset.UtcNow;

        public bool ForceRemoved { get; set; }
        public DateTimeOffset ForceRemoveTime { get; set; }

        public bool IsActive => (DateTimeOffset.UtcNow < ExpirationTime || IsPermanent) && !ForceRemoved;
        public bool IsPermanent { get; set; }

        public string Reason { get; set; }
        public string ForceRemovedReason { get; set; }
    }
}
