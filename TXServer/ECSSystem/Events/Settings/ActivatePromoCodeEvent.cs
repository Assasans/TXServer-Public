using System;
using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Settings
{
	[SerialVersionUID(1490877430206L)]
	public class ActivatePromoCodeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
            Dictionary<Entity, int> rewards = new();

			if (player.Data.Admin)
            {
				var currencyCodes = new Dictionary<string, Entity>
				{
					{ "c", ExtraItems.GlobalItems.Crystal },
					{ "x", ExtraItems.GlobalItems.Xcrystal }
				};

				// cheat codes
                if (currencyCodes.ContainsKey(Convert.ToString(Code[0])))
                {
					if (int.TryParse(Code[1..], out int _))
						rewards.Add(currencyCodes[Convert.ToString(Code[0])], Convert.ToInt32(Code[1..]));
                }
				if (Code.StartsWith("xp"))
				{
					if (int.TryParse(Code[2..], out int _))
					{
						player.User.ChangeComponent<UserExperienceComponent>(component =>
						{
							component.Experience += Convert.ToInt32(Code[2..]);
							Logger.Debug($"{player}: Changed experience to {component.Experience}");
						});

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
				case "squad":
				    // for squad testing
				    // TODO: remove later when quads are tested & stable
				    player.User.ChangeComponent<UserExperienceComponent>(component =>
				    {
					    component.Experience += 5000;
					    Logger.Debug($"{player}: Changed experience to {component.Experience}");
				    });

				    player.CheckRankUp();
				    break;
                case "teleport":
                    // for teleport command testing
                    // TODO: remove later when premium is fully integrated
                    rewards.Add(ExtraItems.GlobalItems.Premiumboost, 2);
                    player.Data.RenewPremium(new TimeSpan(1, 0, 0, 0));
                    break;
			}

			foreach (KeyValuePair<Entity, int> item in rewards)
            {
				Entity notification = new(new TemplateAccessor(new NewItemNotificationTemplate(), "notification/newitem"),
				    new NotificationGroupComponent(entity),
				    new NewItemNotificationComponent(item.Key, item.Value),
				    new NotificationComponent(NotificationPriority.MESSAGE));
				player.ShareEntities(notification);

				if (item.Key == ExtraItems.GlobalItems.Crystal)
					player.Data.SetCrystals(player.Data.Crystals + item.Value);
	            if (item.Key == ExtraItems.GlobalItems.Xcrystal)
		            player.Data.SetXCrystals(player.Data.XCrystals + item.Value);
	            if (item.Key == ExtraItems.GlobalItems.Premiumboost)
					player.Data.RenewPremium(new TimeSpan(item.Value, 0, 0, 0));
            }

			if (rewards.Count > 0)
				player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}

		public string Code { get; set; }
	}
}
