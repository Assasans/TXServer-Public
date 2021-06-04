using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.Core.Database;
using System;
using System.Text;
using TXDatabase.NetworkEvents.PlayerAuth;
using TXServer.Core.Logging;

namespace TXServer.ECSSystem.Events.Entrance
{
    [SerialVersionUID(1438590245672L)]
    public class RequestRegisterUserEvent : ECSEvent
    {
        public void Execute(Player player, Entity entity)
        {
            if (Server.DatabaseNetwork.IsReady)
            {
                PacketSorter.UsernameAvailable(Uid,
                    packet =>
                    {
                        if (packet.result) // Username Available
                        {
                            Logger.Debug(EncryptedPasswordDigest);
                            string generatedToken = Guid.NewGuid().ToString();
                            byte[] decryptedPass = player.EncryptionComponent.Decrypt(EncryptedPasswordDigest);
                            byte[] xor = player.EncryptionComponent.XorArrays(decryptedPass, Convert.FromBase64String(new PersonalPasscodeEvent().Passcode));
                            byte[] concat = player.EncryptionComponent.ConcatenateArrays(xor, decryptedPass);
                            string finalPass = Convert.ToBase64String(player.EncryptionComponent.DIGEST.ComputeHash(concat));
                            PacketSorter.RegisterUser(new RegsiterUserRequest()
                            {
                                encryptedUsername = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(Uid),
                                encryptedHashedPassword = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(finalPass),
                                encryptedEmail = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(Email),
                                encryptedHardwareId = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(HardwareFingerprint),
                                encryptedHardwareToken = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(generatedToken),
                                encryptedCountryCode = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt("US"), // TODO SOON (c) 2021: Assign the country code based on IP location
                                subscribed = Subscribed
                            }, response =>
                            {
                                PlayerData data = new PlayerDataProxy(
                                    response.uid,
                                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.username),
                                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hashedPassword),
                                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.email),
                                    response.emailVerified,
                                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareId),
                                    Server.DatabaseNetwork.Socket.RSADecryptionComponent.DecryptToString(response.hardwareToken)
                                );
                                data.Player = player;
                                player.Data = data;
                                player.SendEvent(new SaveAutoLoginTokenEvent()
                                {
                                    Uid = player.Data.Username,
                                    Token = Encoding.UTF8.GetBytes(generatedToken)
                                }, player.ClientSession);

                                player.LogInWithDatabase(entity);
                            });
                        }
                        else
                        {
                            player.SendEvent(new RequestRegisterUserEvent());
                            player.SendEvent(new RegistrationFailedEvent());
                        }
                    }
                );
            }
            else { /* Idk what to do */}
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
