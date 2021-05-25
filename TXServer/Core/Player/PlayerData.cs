using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using TXServer.Core.Commands;
using TXServer.Core.Data.Database;
using TXServer.Core.Database.NetworkEvents.PlayerAuth;
using TXServer.Core.Database.NetworkEvents.PlayerSettings;
using TXServer.Core.Logging;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types.Punishments;

namespace TXServer.Core
{
    public abstract class PlayerData : ICloneable
    {
        public PlayerData Original { get; protected set; }
        public Player Player { get; set; }

        public long UniqueId { get; }
        public string Email { get; protected set; }
        public bool EmailVerified { get; protected set; }
        public bool Subscribed { get; protected set; }
        public string Username { get; set; }
        public string HashedPassword { get; protected set; }
        public string HardwareId { get; protected set; }
        public string AutoLoginToken { get; protected set; }

        public string CountryCode { get; protected set; }
        public string Avatar { get; protected set; }

        public bool Admin { get; set; }
        public bool Beta { get; protected set; }

        public long XCrystals { get; protected set; }
        public long Crystals { get; protected set; }
        public long Experience { get; protected set; }
        public long Reputation { get; protected set; }
        public DateTime PremiumExpirationDate { get; protected set; }

        public List<long> AcceptedFriendIds { get; protected set; }
        public List<long> IncomingFriendIds { get; protected set; }
        public List<long> OutgoingFriendIds { get; protected set; }
        public List<long> BlockedPlayerIds { get; protected set; }
        public List<long> ReportedPlayerIds { get; protected set; }

        public List<Punishment> Punishments { get; protected set; }
        public List<bool> DeserterLog { get; protected set; }

        public ReadOnlyCollection<ChatMute> GetChatMutes(bool expired = false)
        {
            return Punishments.Where((ban) =>
            {
                if (ban is not ChatMute chatMute) return false;
                return expired || !chatMute.IsExpired;
            }).Cast<ChatMute>().ToList().AsReadOnly(); // XXX(Assasans): Rewrite without using so much LINQ?
        }

        public PlayerData(string uid)
        {
            Username = uid;
        }

        public PlayerData(long uniqueId)
            => UniqueId = uniqueId;

        public ConfirmedUserEmailComponent SetEmail(string email)
        {
            var component = SetValue<ConfirmedUserEmailComponent>(email);
            Email = Email;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetEmail() { uid = UniqueId, email = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(email) });
            return component;
        }

        public ConfirmedUserEmailComponent SetSubscribed(bool subscribed)
        {
            var component = SetValue<ConfirmedUserEmailComponent>(subscribed);
            Subscribed = subscribed;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetSubscribed() { uid = UniqueId, state = subscribed });
            return component;
        }

        public void SetUsername(string username)
        {
            Username = username;
            Player.User.ChangeComponent(new UserUidComponent(username));
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetUsername() { uid = UniqueId, username = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(username) });
        }

        public void SetHashedPassword(string hashedPassword)
        {
            HashedPassword = hashedPassword;
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetHashedPassword() { uid = UniqueId, hashedPassword = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(hashedPassword) });
        }

        public void SetCountryCode(string countryCode)
        {
            CountryCode = countryCode;
            Player.User.ChangeComponent(new UserCountryComponent(countryCode));
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetCountryCode() { uid = UniqueId, countryCode = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(countryCode) });
        }

        public void SetAvatar(string avatarId)
        {
            Avatar = avatarId;
            Player.User.ChangeComponent(new UserAvatarComponent(avatarId));
            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetAvatar() { uid = UniqueId, avatar = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(avatarId) });
        }

        public void SetAdmin(bool admin)
        {
            if (admin && !Admin)
                Player.User.Components.Add(new UserAdminComponent());
            else if (!admin && Admin)
                Player.User.Components.Remove(new UserAdminComponent());
            Admin = admin;
        }
        public void SetBeta(bool beta)
        {
            if (beta && !Beta)
                Player.User.Components.Add(new ClosedBetaQuestAchievementComponent());
            else if (!beta && Beta)
                Player.User.Components.Remove(new ClosedBetaQuestAchievementComponent());
            Beta = beta;
        }

        public void SetCrystals(long value)
        {
            Crystals = value;
            Player.User.ChangeComponent(new UserMoneyComponent(value));
        }
        public void SetXCrystals(long value)
        {
            XCrystals = value;
            Player.User.ChangeComponent(new UserXCrystalsComponent(value));
        }
        public void SetExperience(long value)
        {
            Experience = value;
            Player.User.ChangeComponent(new UserExperienceComponent(value));
        }
        public void SetReputation(long value)
        {
            Reputation = value;
            Player.User.ChangeComponent(new UserReputationComponent(value));
        }
        public void RenewPremium(TimeSpan additionalPremiumTime)
        {
            if (Player.IsPremium)
                PremiumExpirationDate += additionalPremiumTime;
            else
                PremiumExpirationDate = DateTime.UtcNow + additionalPremiumTime;

            PremiumAccountBoostComponent component = new() { EndDate = PremiumExpirationDate };

            if (Player.User.GetComponent<PremiumAccountBoostComponent>() == null)
                Player.User.AddComponent(component);
            else
                Player.User.ChangeComponent(component);

            if (Server.DatabaseNetwork.IsReady)
                Server.DatabaseNetwork.Socket.emit(new SetPremiumExpiration() { uid = UniqueId, expiration = PremiumExpirationDate.Ticks });
        }

        public void AddIncomingFriend(long userId)
        {
            IncomingFriendIds.Add(userId);
        }
        public void AddOutgoingFriend(long userId)
        {
            OutgoingFriendIds.Add(userId);
        }
        public void AddAcceptedFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Add(userId);
        }
        public void RemoveFriend(long userId)
        {
            IncomingFriendIds.Remove(userId);
            OutgoingFriendIds.Remove(userId);
            AcceptedFriendIds.Remove(userId);
        }
        public void ChangeBlockedPlayer(long userId)
        {
            if (BlockedPlayerIds.Contains(userId))
                BlockedPlayerIds.Remove(userId);
            else
                BlockedPlayerIds.Add(userId);
        }
        public void AddReportedPlayer(long userId)
        {
            ReportedPlayerIds.Add(userId);
        }

        private T SetValue<T>(object value) where T : Component
        {
            T component = Player.User.GetComponent<T>();
            if (component == null)
            {
                component = Activator.CreateInstance<T>();
                Player.User.Components.Add(component);
            }

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.PropertyType != value.GetType()) continue;
                Logger.Debug($"{Player}: Updated user value \"{info.Name}\"");
                info.SetValue(component, value);
            }

            return component;
        }


        public abstract PlayerData From(object dataReader);

        public bool Save()
        {
            return Player.Server.Database.SavePlayerData(this);
        }

        public void Apply()
        {
            Entity user = Player.User;
            List<ICommand> commands = new List<ICommand>();

            foreach (var change in RawChanges(Original))
            {
                FieldInfo info = typeof(PlayerData).GetField(change.Key);
                LinkedComponent link = info.GetCustomAttribute(typeof(LinkedComponent)) as LinkedComponent;

                if (link != null)
                {
                    Component component;
                    Player.User.Components.TryGetValue(link.instance, out component);

                    bool isNew = info.GetValue(Original) == null;

                    if (component != null)
                    {
                        commands.Add(isNew ?
                            (ICommand) new ComponentChangeCommand(user, component) :
                            new ComponentAddCommand(user, component));
                        continue;
                    }
                    commands.Add(isNew ?
                        (ICommand) new ComponentAddCommand(user, component) :
                        new ComponentRemoveCommand(user, component.GetType())
                    );
                }
            }
        }

        protected IDictionary<string, object> RawChanges(PlayerData originalData)
        {
            IDictionary<string, object> changes = new Dictionary<string, object>();

            foreach (var field in typeof(PlayerData).GetFields(BindingFlags.GetField))
            {
                var value = field.GetValue(this);
                if (value.Equals(field.GetValue(originalData))) continue;
                changes.Add(field.Name, value);
            }

            return changes;
        }

        protected void SetValue(string field, object value)
        {
            GetType().GetField(field).SetValue(this, value);
        }

        public object Clone()
        {
            var clone = (PlayerData)MemberwiseClone();
            clone.Original = null;

            return clone;
        }
    }
}
