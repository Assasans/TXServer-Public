using System;
using System.Text;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Library;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1438590245672L)]
    public class RequestRegisterUserEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (!Server.Instance.Database.IsUsernameAvailable(Uid))
            {
                player.SendEvent(new RequestRegisterUserEvent());
                player.SendEvent(new RegistrationFailedEvent());
                return;
            }

            byte[] passwordHash = HexUtils.StringToBytes(player.EncryptionComponent.RsaDecryptString(Convert.FromBase64String(EncryptedPasswordDigest)));
            byte[] autoLoginToken = new byte[32];
            new Random().NextBytes(autoLoginToken);

            PlayerData data = new PlayerData(DateTimeOffset.UtcNow.Ticks);
            data.InitDefault();
            data.Username = Uid;
            data.PasswordHash = passwordHash;
            data.Email = Email;
            data.HardwareId = HardwareFingerprint;
            data.EmailSubscribed = Subscribed;
            data.AutoLoginToken = autoLoginToken;
            data.Save();

            byte[] autoLoginTokenEncrypted = player.EncryptionComponent.EncryptAutoLoginToken(autoLoginToken, data.PasswordHash);

            data.Player = player;
            player.Data = data;
            player.SendEvent(new SaveAutoLoginTokenEvent()
            {
                Uid = player.Data.Username,
                Token = autoLoginTokenEncrypted
            }, player.ClientSession);

            player.LogInWithDatabase(entity);

            // Server.Instance.Database.reg
            // PacketSorter.RegisterUser(new RegsiterUserRequest()
            // {
            //     encryptedUsername = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(Uid),
            //     encryptedHashedPassword = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(finalPass),
            //     encryptedEmail = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(Email),
            //     encryptedHardwareId = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(HardwareFingerprint),
            //     encryptedHardwareToken = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(generatedToken),
            //     encryptedCountryCode = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt("US"), // TODO SOON (c) 2021: Assign the country code based on IP location
            //     subscribed = Subscribed
            // }, response =>
            // {
            //     PlayerData data = new PlayerDataProxy(
            //         response.uid,
            //         Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.username),
            //         Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hashedPassword),
            //         Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.email),
            //         response.emailVerified,
            //         Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareId),
            //         Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareToken)
            //     );
            //     data.Player = player;
            //     player.Data = data;
            //     player.SendEvent(new SaveAutoLoginTokenEvent()
            //     {
            //         Uid = player.Data.Username,
            //         Token = Encoding.UTF8.GetBytes(generatedToken)
            //     }, player.ClientSession);
            //
            //     player.LogInWithDatabase(entity);
            // });
        }
        public string Uid { get; set; }
        public string EncryptedPasswordDigest { get; set; }
        public string Email { get; set; }
        public string HardwareFingerprint { get; set; }
        public bool Subscribed { get; set; }
        public bool Steam { get; set; }
        public bool QuickRegistration { get; set; }
    }
}
