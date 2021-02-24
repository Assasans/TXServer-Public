using System;
using System.Collections.Generic;
using System.Reflection;
using TXServer.Core.Commands;
using TXServer.Core.Data.Database;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.Core
{
    public abstract class PlayerData : ICloneable
    {
        public PlayerData Original { get; protected set; }
        public Player Player { get; set; }

        public string UniqueId { get; }
        public string Email { get; protected set; }
        public bool Subscribed { get; protected set; }
        public string Username { get; protected set; }
        public string HashedPassword { get; protected set; }

        public string CountryCode { get; protected set; }
        public string Avatar { get; protected set; }

        public bool Admin { get; protected set; }
        public bool Beta { get; protected set; }

        public long Score { get; protected set; }
        public long XCrystals { get; protected set; }
        public long Crystals { get; protected set; }

        public PlayerData(string uid)
        {
            UniqueId = uid;
        }

        public ConfirmedUserEmailComponent SetEmail(string email)
        {
            var component = SetValue<ConfirmedUserEmailComponent>(email);
            Email = email;
            return component;
        }

        public ConfirmedUserEmailComponent SetSubscribed(bool subscribed)
        {
            var component = SetValue<ConfirmedUserEmailComponent>(subscribed);
            Subscribed = subscribed;
            return component;
        }

        public void SetUsername(string username)
        {
            Username = username;
        }

        public void SetHashedPassword(string hashedPassword)
        {
            HashedPassword = hashedPassword;
        }

        public UserCountryComponent SetCountryCode(string countryCode)
        {
            var component = SetValue<UserCountryComponent>(countryCode);
            CountryCode = countryCode;
            return component;
        }

        public UserAvatarComponent SetAvatar(string avatarId)
        {
            var component = SetValue<UserAvatarComponent>(avatarId);
            Avatar = avatarId;
            return component;
        }

        public void SetAdmin(bool admin)
        {
            if (admin)
            {
                Player.User.Components.Add(new UserAdminComponent());
                return;
            }

            //Player.User.Components.Remove(new UserAdminComponent());
            // did you see them get called before? or did you start to try that
            // The first errors was because of this as well, so we are at square 1 again
        } 

        public void SetBeta(bool beta)
        {
            if (beta)
            {
                Player.User.Components.Add(new ClosedBetaQuestAchievementComponent());
                return;
            }

            //Player.User.Components.Remove(new ClosedBetaQuestAchievementComponent());
        }
        
        public UserMoneyComponent SetCrystals(long value)
        {
            var component = SetValue<UserMoneyComponent>(value);
            Crystals = value;
            return component;
        }

        public UserExperienceComponent SetScore(long value)
        {
            var component = SetValue<UserExperienceComponent>(value);
            // Component tmpComponent;
            // Player.User.Components.TryGetValue(new UserXCrystalsComponent(0), out tmpComponent);
            // UserXCrystalsComponent component = (UserXCrystalsComponent) tmpComponent;
            // component.Money = value;
            Score = value;
            return component;
        }

        public UserXCrystalsComponent SetXCrystals(long value)
        {
            var component = SetValue<UserXCrystalsComponent>(value);
            // Component tmpComponent;
            // Player.User.Components.TryGetValue(new UserXCrystalsComponent(0), out tmpComponent);
            // UserXCrystalsComponent component = (UserXCrystalsComponent) tmpComponent;
            // component.Money = value;
            XCrystals = value;
            return component;
        }

        private T SetValue<T>(object value) where T : Component
        {
            T component = Player.User.GetComponent<T>(); // This is the error and idk, its because Player.User is null and idk how to change that
            if (component == null) // Did you se the breakpoint? no
            {
                component = Activator.CreateInstance<T>();
                Player.User.Components.Add(component);
            }

            foreach (PropertyInfo info in typeof(T).GetProperties())
            {
                if (info.PropertyType != value.GetType()) continue;
                Console.WriteLine(info.Name);
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
            PlayerData clone = (PlayerData) Activator.CreateInstance(GetType(), UniqueId);
            
            foreach (var field in typeof(PlayerData).GetFields())
            {
                if (field.Name.Equals("Original")) continue;
                field.SetValue(clone, field.GetValue(this));
            }

            return clone;
        }
    }
}