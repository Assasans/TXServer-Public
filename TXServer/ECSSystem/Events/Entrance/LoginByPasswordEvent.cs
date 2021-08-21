using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
    public class LoginByPasswordEvent : ECSEvent
    {
        public void Execute(Player player, Entity clientSession)
        {
            if (!player.EncryptionComponent.GetLoginPasswordHash(player.Data.PasswordHash).SequenceEqual(Convert.FromBase64String(PasswordEncipher)))
            {
                player.SendEvent(new InvalidPasswordEvent(), clientSession);
                player.SendEvent(new LoginFailedEvent(), clientSession);
                return;
            }

            if (RememberMe)
            {
                byte[] autoLoginToken = new byte[32];
                new Random().NextBytes(autoLoginToken);

                player.Data.RememberMe = true;
                player.Data.HardwareId = HardwareFingerprint;
                player.Data.AutoLoginToken = autoLoginToken;

                byte[] autoLoginTokenEncrypted = player.EncryptionComponent.EncryptAutoLoginToken(autoLoginToken, player.Data.PasswordHash);

                player.SendEvent(new SaveAutoLoginTokenEvent()
                {
                    Uid = player.Data.Username,
                    Token = autoLoginTokenEncrypted
                }, player.ClientSession);
            }

            if (!player.IsBanned)
                player.LogInWithDatabase(clientSession);
            else
                player.SendEvent(new LoginFailedEvent(), clientSession);


            // {
            //     if (!player.IsBanned)
            //     {
            //         player.Data.RememberMe = RememberMe;
            //         player.Data.HashedPassword = PasswordEncipher;
            //         player.Data.HardwareId = HardwareFingerprint;
            //         player.LogIn(clientSession);
            //     }
            //     else
            //         player.SendEvent(new LoginFailedEvent(), clientSession);
            //
            //
            // }
        }

        public string HardwareFingerprint { get; set; }
        public string PasswordEncipher { get; set; }
        public bool RememberMe { get; set; }
    }
}
