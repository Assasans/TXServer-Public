using Simple.Net;
using Simple.Net.Server;
using Core;
using TXDatabase.NetworkEvents.Communications;
using TXDatabase.NetworkEvents.PlayerAuth;
using TXDatabase.NetworkEvents.PlayerSettings;
using TXDatabase.Databases;

namespace TXDatabase {
    public static class Lobby {
        static Server server { get => Network.server; }
        const string authenticatedGroupName = "authComplete";
        public static void AddUser(User user) {
            user.groups.Add(authenticatedGroupName);

            Logger.LogNetwork($"User '{user.clientId}' logged in as '{(string)user.data["friendlyName"]}'", "Auth");
            
            user.on((UserLoggedInEvent packet) => {
                Logger.LogNetwork($"Player (of uid) '{packet.uid}' has logged into '{(string)user["friendlyName"]}'", "Broadcast");
                server.broadcastExcept(authenticatedGroupName, packet, user.clientId);
            });

            user.on(async (GetUserViaUsername packet) => {
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' accessing user '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' auth data", "DBAccess");
                UserRow data = await UserDatabase.Users.GetUserViaCallsign(user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername));
                if (data == UserRow.Empty) {
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' not found", "DBAccess");
                    user.emit(new UserInitialDataEvent() {
                        packetId = packet.packetId,
                        uid = -1
                    });
                }
                else {
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername)}' found", "DBAccess");
                    user.emit(new UserInitialDataEvent() {
                        packetId = packet.packetId,
                        uid = data.uid,
                        username = user.RSAEncryptionComponent.Encrypt(data.username),
                        hashedPassword = user.RSAEncryptionComponent.Encrypt(data.hashedPassword),
                        email = user.RSAEncryptionComponent.Encrypt(data.email),
                        emailVerified = data.emailVerified,
                        hardwareId = user.RSAEncryptionComponent.Encrypt(data.hardwareId),
                        hardwareToken = user.RSAEncryptionComponent.Encrypt(data.hardwareToken)
                    });
                }
            });
            user.on(async (UsernameAvailableRequest packet) => {
                string username = user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername);
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if '{username}' exists", "DBAccess");
                AvailableResult response = new AvailableResult()
                {
                    packetId = packet.packetId,
                    result = await UserDatabase.Users.UsernameAvailable(username)
                };
                if (response.result)
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' exists", "DBAccess");
                else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' does not exist", "DBAccess");
                user.emit(response);
            });
            user.on(async (EmailAvailableRequest packet) => {
                string username = user.RSADecryptionComponent.DecryptToString(packet.email);
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' checking if email '{username}' is availables", "DBAccess");
                AvailableResult response = new AvailableResult()
                {
                    packetId = packet.packetId,
                    result = await UserDatabase.Users.EmailAvailable(username)
                };
                if (response.result)
                    Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is available", "DBAccess");
                else Logger.LogNetwork($"'{(string)user["friendlyName"]}', '{username}' is not available", "DBAccess");
                user.emit(response);
            });
            user.on(async (RegsiterUserRequest packet) => {
                string username = user.RSADecryptionComponent.DecryptToString(packet.encryptedUsername);
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' creating user '{username}'", "DBAccess");
                UserRow data = await UserDatabase.Users.Create(
                    username,
                    user.RSADecryptionComponent.DecryptToString(packet.encryptedHashedPassword),
                    user.RSADecryptionComponent.DecryptToString(packet.encryptedEmail),
                    user.RSADecryptionComponent.DecryptToString(packet.encryptedHardwareId),
                    user.RSADecryptionComponent.DecryptToString(packet.encryptedHardwareToken)
                );
                _ = await UserDatabase.UserSettings.Get(data.uid); // Get will just make sure that the row exists (it will create a row if one for the uid does not exist, HEY, I am lazy)
                _ = await UserDatabase.UserSettings.SetCountryCode(data.uid, user.RSADecryptionComponent.DecryptToString(packet.encryptedCountryCode));
                _ = await UserDatabase.UserSettings.SetSubscribedState(data.uid, packet.subscribed);
                /* Shouldn't happen
                if (data == UserRow.Empty) {
                    Logger.Log($"'{(string)user["friendlyName"]}' failed to create user '{username}'", "DBAccess");
                }*/
                Logger.LogDebug(data.ToString());
                user.emit(new UserInitialDataEvent() {
                    packetId = packet.packetId,
                    uid = data.uid,
                    username = user.RSAEncryptionComponent.Encrypt(data.username),
                    hashedPassword = user.RSAEncryptionComponent.Encrypt(data.hashedPassword),
                    email = user.RSAEncryptionComponent.Encrypt(data.email),
                    emailVerified = data.emailVerified,
                    hardwareId = user.RSAEncryptionComponent.Encrypt(data.hardwareId),
                    hardwareToken = user.RSAEncryptionComponent.Encrypt(data.hardwareToken)
                });
            });
            user.on(async (GetUserSettingsRequest request) =>
            {
                Logger.LogNetwork($"'{(string)user["friendlyName"]}' getting user settings for uid '{request.uid}'", "DBAccess");
                UserSettingsData result = await UserDatabase.UserSettings.Get(request.uid);
                result.packetId = request.packetId;
                result.countryCode = user.RSAEncryptionComponent.Encrypt(result.countryCode);
                result.avatar = user.RSAEncryptionComponent.Encrypt(result.avatar);
                user.emit(result);
            });

            // Player account setters
            user.on(async (SetUsername request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing username of '{request.uid}'. Success: {(await UserDatabase.Users.SetUsername(request.uid, user.RSADecryptionComponent.DecryptToString(request.username)) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetHashedPassword request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing password of '{request.uid}'. Success: {(await UserDatabase.Users.SetHashedPassword(request.uid, user.RSADecryptionComponent.DecryptToString(request.hashedPassword)) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetEmail request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing email of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmail(request.uid, user.RSADecryptionComponent.DecryptToString(request.email)) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetEmailVerified request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing setting email verified state of '{request.uid}'. Success: {(await UserDatabase.Users.SetEmailVerified(request.uid, request.state) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetUserRememberMeCredentials request) 
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' set AutoLogin parameters for '{request.uid}'. Success: {(await UserDatabase.Users.SetRememberMe(request.uid, user.RSADecryptionComponent.DecryptToString(request.hardwareId), user.RSADecryptionComponent.DecryptToString(request.hardwareToken)) ? "TRUE" : "FALSE")}", "DBAccess"));

            // Player settings setters
            user.on(async (SetCountryCode request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing country code of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetCountryCode(request.uid, user.RSADecryptionComponent.DecryptToString(request.countryCode)) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetAvatar request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing avatar of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetAvatar(request.uid, user.RSADecryptionComponent.DecryptToString(request.avatar)) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetPremiumExpiration request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing premium expiration for '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetPremiumExpiration(request.uid, request.expiration) ? "TRUE" : "FALSE")}", "DBAccess"));
            user.on(async (SetSubscribed request)
                => Logger.LogNetwork($"'{(string)user["friendlyName"]}' changing country code of '{request.uid}'. Success: {(await UserDatabase.UserSettings.SetSubscribedState(request.uid, request.state) ? "TRUE" : "FALSE")}", "DBAccess"));
        }

        public static void RemoveUser(User user) { 
            if (user.groups.Contains(authenticatedGroupName))
                Logger.LogNetwork($"User '{user.clientId}' (aka '{(string)user["friendlyName"]}') has disconnected");
            else Logger.LogNetwork($"User '{user.clientId}' (unauthenticated) has disconnected");
        }
    }
}
