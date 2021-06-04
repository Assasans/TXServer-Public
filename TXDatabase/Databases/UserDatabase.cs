using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using TXDatabase.Databases.UserTables;
using Core;

namespace TXDatabase.Databases {
    public static class UserDatabase {
        public static SQLiteConnection Connection;
        public static Users Users;
        public static UserSettings UserSettings;
        //public static UserSettings UserSettings;

        public static async void Load() {
            string connectionString = "URI=file:" + Path.Join(Environment.CurrentDirectory, "Data", "Users.db");
            Logger.LogDebug($"Using DB on path => '{connectionString}'", "UserDB");
            Connection = new SQLiteConnection(connectionString);
            await Connection.OpenAsync();
            Users = await new Users().Init();
            UserSettings = await new UserSettings().Init();
            //UserSettings = await new UserSettings().Init();
        }

        public static async void Dispose() {
            Users = null;
            await Connection.CloseAsync();
        }
    }
    public struct UserRow {
        public static UserRow Empty = new UserRow() { uid = -1 };
        public long uid;
        public string username;
        public string hashedPassword;
        public string email;
        public bool emailVerified;
        public string hardwareId;
        public string hardwareToken;

        
        public override bool Equals(object obj)
            => Equals((UserRow)obj);

        public bool Equals(UserRow other)
            => this == other;

        public static bool operator ==(UserRow lhs, UserRow rhs)
            => lhs.uid == rhs.uid;

        public static bool operator !=(UserRow lhs, UserRow rhs)
            => !(lhs == rhs);

        public override string ToString()
            => $"{{" +
               $"\n  uuid            => {uid}" +
               $"\n  username        => {username}" +
               $"\n  hashedPassword  => {hashedPassword}" +
               $"\n  email           => {email}" +
               $"\n  emailverified   => {emailVerified}" +
               $"\n  hardwareId      => {hardwareId}" +
               $"\n  hardwareToken   => {hardwareToken}" +
               $"\n}}";

        public override int GetHashCode()
        {
            int hashCode = 1975335690;
            hashCode *= -1521134295 + uid.GetHashCode();
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(username);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(hashedPassword);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(email);
            hashCode *= -1521134295 + EqualityComparer<bool>.Default.GetHashCode(emailVerified);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(hardwareId);
            hashCode *= -1521134295 + EqualityComparer<string>.Default.GetHashCode(hardwareToken);
            
            return hashCode;
        }
    }
}