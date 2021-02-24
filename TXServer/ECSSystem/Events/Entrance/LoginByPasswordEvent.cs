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
using TXServer.Core.RemoteDatabase;
using TXServer.Utils;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public async void Execute(Player player, Entity clientSession)
		{
			Console.WriteLine("Hashed Password:\t" + PasswordEncipher + 
							"\nHashed PassOnDB:\t" + player.tempRow.hashedPassword + " => (" + (PasswordEncipher == player.tempRow.hashedPassword) + ")");
			// If the database is disabled, if not compare the passwords
			if (!RemoteDatabase.isInitilized || PasswordEncipher == player.tempRow.hashedPassword) {

				PlayerData data = new UserInfo(player.tempRow.username);

				data.Player = player;
				player.Data = data;
				player.LogIn(clientSession);
			}
			else CommandManager.SendCommands(player,
				new SendEventCommand(new InvalidPasswordEvent(), clientSession),
				new SendEventCommand(new LoginFailedEvent(), clientSession));
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
