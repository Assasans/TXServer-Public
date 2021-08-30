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

            byte[] passwordHash = player.EncryptionComponent.RsaDecrypt(Convert.FromBase64String(EncryptedPasswordDigest));
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

            lock (player.Server.Database)
            {
                player.Server.Database.Players.Add(data);
            }

            byte[] autoLoginTokenEncrypted = player.EncryptionComponent.EncryptAutoLoginToken(autoLoginToken, data.PasswordHash);

            data.Player = player;
            player.Data = data;
            player.SendEvent(new SaveAutoLoginTokenEvent()
            {
                Uid = player.Data.Username,
                Token = autoLoginTokenEncrypted
            }, player.ClientSession);

            player.LogInWithDatabase(entity);
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
