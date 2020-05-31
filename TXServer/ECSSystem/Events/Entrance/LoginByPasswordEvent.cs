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

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1437480091995)]
	public class LoginByPasswordEvent : ECSEvent
	{
		public void Execute(Player player, Entity clientSession)
		{
			//todo get PlayerData
			player.LogIn(clientSession);
		}

		public string HardwareFingerprint { get; set; }
		public string PasswordEncipher { get; set; }
		public bool RememberMe { get; set; }
	}
}
