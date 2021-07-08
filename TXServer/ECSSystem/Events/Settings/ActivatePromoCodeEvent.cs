using System;
using System.Collections.Generic;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

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

                switch (Code)
                {
                    case { } s when (s.StartsWith("c") || s.StartsWith("x")) && !s.StartsWith("xp"):
                        if (int.TryParse(Code[1..], out int count))
                            rewards.Add(currencyCodes[Convert.ToString(Code[0])], count);
                        break;
                    case { } s when s.StartsWith("r"):
                        if (int.TryParse(Code[1..], out int number))
                            player.Data.Reputation += number;
                        break;
                    case { } s when s.StartsWith("xp"):
                        if (int.TryParse(Code.Substring(2, Code.Length - 2), out int i))
                        {
                            player.Data.SetExperience(player.Data.Experience + i);
                            Logger.Debug($"{player}: Changed experience to {player.Data.Experience}");
                        }
                        break;
                }
            }

			switch (Code)
			{
				case "7FA8-8E12-DB08":
					rewards.Add(ExtraItems.GlobalItems.Crystal, 10000);
					rewards.Add(ExtraItems.GlobalItems.Xcrystal, 10000);
					rewards.Add(ExtraItems.GlobalItems.Premiumboost, 7);
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
                player.ShareEntities(NewItemNotificationTemplate.CreateEntity(entity, item));

				if (item.Key == ExtraItems.GlobalItems.Crystal)
					player.Data.Crystals += item.Value;
	            else if (item.Key == ExtraItems.GlobalItems.Xcrystal)
		            player.Data.XCrystals += item.Value;
	            else if (item.Key == ExtraItems.GlobalItems.Premiumboost)
					player.Data.RenewPremium(new TimeSpan(item.Value, 0, 0, 0));
            }

			if (rewards.Count > 0)
				player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}

		public string Code { get; set; }
	}
}
