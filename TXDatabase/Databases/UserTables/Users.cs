using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Data;
using System.Data.Common;
using Core;

namespace TXDatabase.Databases.UserTables {
    public class Users {
        //public string Escape(string value) => Db
        public async Task<Users> Init() {
            using (SQLiteCommand request = new SQLiteCommand(
                "CREATE TABLE IF NOT EXISTS `users`(" +
                " uid INTEGER PRIMARY KEY AUTOINCREMENT," +
                " username VARCHAR(16) NOT NULL COLLATE NOCASE," +
                " hashedPassword VARCHAR(44) NOT NULL," +
                " email VARCHAR(50) NOT NULL," +
                " emailVerified TINYINT(1) NOT NULL DEFAULT 0," +
                " hardwareId VARCHAR(100) NOT NULL," +
                " hardwareToken VARCHAR(100) NOT NULL" +
                ");", UserDatabase.Connection))
                await request.ExecuteNonQueryAsync();
            Logger.Log("Table 'UserDatabase.users` initilized", "UserDB");
            return this;
        }

        public async Task<bool> UsernameAvailable(string username) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT uid FROM users WHERE username = '{username}' COLLATE NOCASE;",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                bool result = !response.HasRows;
                await response.CloseAsync();
                return result;
            }
        }

        public async Task<bool> EmailAvailable(string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT uid FROM users WHERE email = '{email}';",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                bool result = !response.HasRows;
                await response.CloseAsync();
                return result;
            }
        }

        public async Task<UserRow> Create(string username, string hashedPassword, string email = null, string hardwareId = null, string token = null) {
            if (email == null) email = string.Empty;
            if (hardwareId == null) hardwareId = string.Empty;
            if (token == null) token = string.Empty;
            else if (!await EmailAvailable(email)) throw new ArgumentException("Email Taken!");
            using (SQLiteCommand request = new SQLiteCommand(
                $"INSERT INTO users(username, hashedPassword, email, hardwareId, hardwareToken) VALUES('{username}', '{hashedPassword}', '{email}', '{hardwareId}', '{token}');",
                UserDatabase.Connection
            )) {
                await request.ExecuteNonQueryAsync();

                return await GetUserViaCallsign(username);
            }
        }

        public async Task<UserRow> GetUserViaCallsign(string username) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT * FROM users WHERE username = '{username}'",
                UserDatabase.Connection
            )) {
                using (DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow)) {
                    Dictionary<string, object> result = new Dictionary<string, object>();
                    if (!response.HasRows) {
                        result.Add("error", "User not found");
                        return UserRow.Empty;
                    }
                    
                    await response.ReadAsync();
                    
                    return new UserRow() {
                        uid = response.GetInt32("uid"),
                        username = response.GetString("username"),
                        hashedPassword = response.GetString("hashedPassword"),
                        email = response.GetString("email"),
                        emailVerified = response.GetBoolean("emailVerified"),
                        hardwareId = response.GetString("hardwareId"),
                        hardwareToken = response.GetString("hardwareToken")
                    };

                        /*result.Add("uid", (long)response.GetInt32("uid"));
                        result.Add("username", response.GetString("username"));
                        result.Add("hashedPassword", response.GetString("hashedPassword"));
                        result.Add("email", response.GetString("email"));
                        result.Add("emailVerified", response.GetBoolean("emailVerified"));
                        result.Add("hardwareId", response.GetString("hardwareId"));
                        result.Add("hardwareToken", response.GetString("hardwareToken"));*/
                }
            }
        }

        public async Task<Dictionary<string, object>> GetUserViaEmail(string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"SELECT * FROM users WHERE email = '{email}'",
                UserDatabase.Connection
            )) {
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.SingleRow);
                Dictionary<string, object> result = new Dictionary<string, object>();
                if (response.HasRows) {
                    await response.ReadAsync();

                    result.Add("uid", (long)response.GetInt32("uid"));
                    result.Add("username", response.GetString("username"));
                    result.Add("hashedPassword", response.GetString("hashedPassword"));
                    result.Add("email", response.GetString("email"));
                    result.Add("emailVerified", response.GetBoolean("emailVerified"));
                    result.Add("hardwareId", response.GetString("hardwareId"));
                    result.Add("hardwareToken", response.GetString("hardwareToken"));
                }
                else result.Add("error", "User not found");
                await response.CloseAsync();

                return result;
            }
        }

        public async Task<Queue<string>> GetEmailList() {
            using (SQLiteCommand request = new SQLiteCommand(
                "SELECT users.email AS email FROM users, `user-settings` WHERE users.uid = `user-settings`.uid AND email != '' AND `user-settings`.subscribed AND users.emailVerified;",
                UserDatabase.Connection
            )) {
                Queue<string> result = new Queue<string>();
                DbDataReader response = await request.ExecuteReaderAsync(CommandBehavior.Default);

                while (response.HasRows) {
                    await response.ReadAsync();
                    result.Enqueue(response.GetString("email"));
                    await response.NextResultAsync();
                }

                return result;
            }
        }

        /// <summary>
        /// Will change the username of a user, returns true if the operation was successful
        /// </summary>
        public async Task<bool> SetUsername(long uid, string newUsername) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET username = '{newUsername}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetHashedPassword(long uid, string hashedPassword) {
            if (hashedPassword.Length > 44) throw new ArgumentException("Parameter 'hashedPassword' is too long!");

            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET hashedPassword = '{hashedPassword}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        // Will set the emailVerified = false
        public async Task<bool> SetEmail(long uid, string email) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET email = '{email}', emailVerified = 0 WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetEmailVerified(long uid, bool value) {
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET emailVerified = {(value ? 1 : 0)} WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetHardwareId(long uid, string hardwareId) {
            if (hardwareId.Length > 100) throw new ArgumentException("Parameter hardwareId cannot not be over 100 characters");
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET hardwareId = '{hardwareId}' WHERE uid = {uid}",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }

        public async Task<bool> SetRememberMe(long uid, string hardwareId, string token) {
            if (hardwareId.Length > 100) throw new ArgumentException("Parameter hardwareId cannot not be over 100 characters");
            using (SQLiteCommand request = new SQLiteCommand(
                $"UPDATE users SET hardwareId = '{hardwareId}', hardwareToken = '{token}' WHERE uid = {uid};",
                UserDatabase.Connection
            )) return await request.ExecuteNonQueryAsync() > 0;
        }
    }
}