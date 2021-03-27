using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.GlobalEntities;
using System.Collections.Generic;
using System;
using TXServer.Core.Logging;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1490877430206L)]
	public class ActivatePromoCodeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			var rewards = new Dictionary<Entity, int> {};
			List<ICommand> commands = new List<ICommand> {};

			if (player.User.GetComponent<UserAdminComponent>() != null)
            {
				var currencieCodes = new Dictionary<string, Entity>
				{
					{ "c", ExtraItems.GlobalItems.Crystal },
					{ "x", ExtraItems.GlobalItems.Xcrystal }
				};
				
				// cheat codes
				int var;
				if (currencieCodes.ContainsKey(Convert.ToString(Code[0])))
                {
					if (int.TryParse(Code[1..], out var))
					{
						rewards.Add(currencieCodes[Convert.ToString(Code[0])], Convert.ToInt32(Code[1..]));
					}
                }
				if (Code.StartsWith("xp"))
				{
					if (int.TryParse(Code[2..], out var))
					{
						UserExperienceComponent userExperienceComponent = player.User.GetComponent<UserExperienceComponent>();
						userExperienceComponent.Experience += Convert.ToInt32(Code[2..]);
						Logger.Debug($"{player}: Changed experience to {userExperienceComponent.Experience}");
						commands.Add(new ComponentChangeCommand(player.User, userExperienceComponent));
						player.CheckRankUp(0);
					}
				}
			}

			foreach (KeyValuePair<Entity, int> item in rewards)
            {
				Entity notification = new Entity(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
				    new NotificationGroupComponent(entity),
				    new NewItemNotificationComponent(item.Key, item.Value),
				    new NotificationComponent(NotificationPriority.MESSAGE));
				commands.Add(new EntityShareCommand(notification));
				
				if (item.Key == ExtraItems.GlobalItems.Crystal)
				{
					UserMoneyComponent userMoneyComponent = player.Data.SetCrystals(player.Data.Crystals + item.Value);
					commands.Add(new ComponentChangeCommand(player.User, userMoneyComponent));
				}
				if (item.Key == ExtraItems.GlobalItems.Xcrystal)
                {
					UserXCrystalsComponent userXCrystalsComponent = player.Data.SetXCrystals(player.Data.XCrystals + item.Value);
					commands.Add(new ComponentChangeCommand(player.User, userXCrystalsComponent));
                }
			}

			CommandManager.SendCommands(player, commands);
		}
		public string Code { get; set; }
	}
}
