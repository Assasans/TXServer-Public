using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;
using TXServer.Core.Database;
using TXDatabase.NetworkEvents.PlayerAuth;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public void Execute(Player player, Entity clientSession)
		{
            if (Server.DatabaseNetwork.IsReady)
            {
                if (player.Data.HashedPassword == PasswordEncipher)
                {
                    if (RememberMe)
                    {
                        string generatedToken = Guid.NewGuid().ToString();
                        Server.DatabaseNetwork.Socket.emit(new SetUserRememberMeCredentials()
                        {
                            uid = player.Data.UniqueId,
                            hardwareId = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(HardwareFingerprint),
                            hardwareToken = Server.DatabaseNetwork.Socket.RSAEncryptionComponent.Encrypt(generatedToken)
                        });
                        player.SendEvent(new SaveAutoLoginTokenEvent()
                        {
                            Uid = player.Data.Username,
                            Token = System.Text.Encoding.UTF8.GetBytes(generatedToken)
                        }, player.ClientSession);
                    }
                    if (!player.IsBanned)
                        player.LogInWithDatabase(clientSession);
                    else
                        player.SendEvent(new LoginFailedEvent(), clientSession);

                }
                else
                {
                    player.SendEvent(new InvalidPasswordEvent(), clientSession);
                    player.SendEvent(new LoginFailedEvent(), clientSession);
                }
            }
            else
            {
                if (!player.IsBanned)
                {
                    player.Data.RememberMe = RememberMe;
                    player.Data.HashedPassword = PasswordEncipher;
                    player.Data.HardwareId = HardwareFingerprint;
                    player.LogIn(clientSession);
                }
                else
                    player.SendEvent(new LoginFailedEvent(), clientSession);


            }
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
