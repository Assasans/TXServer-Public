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

			if (player.Data.Admin)
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
						player.CheckRankUp();
					}
				}
			}

			switch (Code)
			{
				case "7FA8-8E12-DB08":
					rewards.Add(ExtraItems.GlobalItems.Crystal, 10000);
					rewards.Add(ExtraItems.GlobalItems.Xcrystal, 10000);
					rewards.Add(ExtraItems.GlobalItems.Premiumboost, 7);
					break;
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
				if (item.Key == ExtraItems.GlobalItems.Premiumboost)
				{
					if (player.User.GetComponent<PremiumAccountBoostComponent>() == null)
						player.User.AddComponent(new PremiumAccountBoostComponent(endDate: new TXDate(new TimeSpan(item.Value * 24, 0, 0))));
					else
						player.User.ChangeComponent<PremiumAccountBoostComponent>(component => component.EndDate.Time += item.Value);
				}
			}

			CommandManager.SendCommands(player, commands);
			if (rewards.Count > 0)
				player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}
		public string Code { get; set; }
	}
}
