using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Events.Settings
{
	[SerialVersionUID(1490877430206L)]
	public class ActivatePromoCodeEvent : ECSEvent
	{
        private static readonly ILogger Logger = Log.Logger.ForType<ActivatePromoCodeEvent>();

		public void Execute(Player player, Entity entity)
		{
            Dictionary<Entity, int> rewards = new();

			if (player.Data.IsAdmin)
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
                            Logger.WithPlayer(player).Debug("Changed experience to {Experience}", player.Data.Experience);
                        }
                        break;
                    default:
                        if (long.TryParse(Code, out long code))
                        {
                            Entity marketItem = player.EntityList.SingleOrDefault(e =>
                                e.EntityId == code && e.GetComponent<MarketItemGroupComponent>()?.Key == code);
                            if (marketItem is null) break;
                            rewards.Add(marketItem, 1);
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
            }

			foreach (KeyValuePair<Entity, int> item in rewards)
            {
                player.ShareEntities(NewItemNotificationTemplate.CreateEntity(entity, item.Key, item.Value));

				if (item.Key == ExtraItems.GlobalItems.Crystal)
					player.Data.Crystals += item.Value;
	            else if (item.Key == ExtraItems.GlobalItems.Xcrystal)
		            player.Data.XCrystals += item.Value;
	            else if (item.Key == ExtraItems.GlobalItems.Premiumboost)
					player.Data.RenewPremium(new TimeSpan(item.Value, 0, 0, 0));
                else
                    player.SaveNewMarketItem(item.Key, item.Value);
            }

			if (rewards.Count > 0)
				player.SendEvent(new ShowNotificationGroupEvent(1), entity);
		}

		public string Code { get; set; }
	}
}
