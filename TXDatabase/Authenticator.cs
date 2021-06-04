using Core;
using TXDatabase.NetworkEvents.Security;
using Simple.Net;
using Simple.Net.Server;
using SimpleJSON;
using System.Net;
using TXDatabase.Databases;

namespace TXDatabase {
    public static class Authenticator {
        static JSONNode Config { get => Program.Config["Authenticator"]; }
        public static void Process(User user) {
            Logger.LogNetwork($"New connection from '{(user.socket.RemoteEndPoint as IPEndPoint).Address}' assigned id '{user.clientId}'", "Auth");
            user.emit<RSAPublicKey>(new RSAPublicKey() { Key = user.RSADecryptionComponent.publicKey });
            user.onOnce<RSAPublicKey>((RSAPublicKey packet) => {
                user.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key);
                user.on<RSAPublicKey>((RSAPublicKey packet)
                    => user.RSAEncryptionComponent = new RSAEncryptCompoenent(packet.Key)
                );
                
                user.on<LoginEvent>(async (LoginEvent packet) => {
                    string APIKey = user.RSADecryptionComponent.DecryptToString(packet.encryptedAPIKey);
                    string APIToken = user.RSADecryptionComponent.DecryptToString(packet.encryptedAPIToken);

                    // Find in DB and search 
                    ServerRow data = await ServerDatabase.Servers.Get(HashUtil.MD5(APIKey));
                    if (data == ServerRow.Empty) {
                        Logger.LogNetwork($"User '{user.clientId}' sent an invalid key '{data.name}'", "Auth");
                        user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid API key" : string.Empty });
                        return;
                    }
                    Logger.LogNetwork($"User '{user.clientId}' attempting to login as '{data.name}'", "Auth");
                    HashUtilCheckResult result = HashUtil.Check(data.token, APIToken);
                    if (!result.verified) {
                    Logger.LogNetwork($"User '{user.clientId}' failed to login as '{data.name}', token error", "Auth");
                        user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid API token" : string.Empty });
                        return;
                    }
                    if (result.needsUpgrade) {
                        Logger.Log($"Server '{data.name}' API Token has been upgraded", "Net");
                        data.token = HashUtil.Compute(APIKey);
                        await ServerDatabase.Servers.Save(data);
                    }
                    if (!data.addresses.Contains($"{(user.socket.RemoteEndPoint as IPEndPoint).Address}")){
                        Logger.LogNetwork($"User '{user.clientId}' failed to login as '{data.name}', IP Check failed", "Auth");
                        user.emit<LoginFailedEvent>(new LoginFailedEvent() { reason = Config["EmitLoginErrorReason"] ? "Invalid remote endpoint" : string.Empty });
                        return;
                    }

                    // Passed all checks... so allow access
                    // cleanup
                    user.removeEvent<LoginEvent>();
                    // /cleanup
                    user["uid"] = data.uid;
                    user["friendlyName"] = data.name;
                    user.emit<LoginSuccessEvent>(new LoginSuccessEvent());
                    Lobby.AddUser(user);
                });
            });
        }
    }
}
