using System.Linq;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Events.Settings
{
	[SerialVersionUID(1490931976968L)]
	public class CheckPromoCodeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			PromoCodeCheckResult result = PromoCodeCheckResult.INVALID;

			if (player.Data.Admin)
                switch (Code)
                {
                    case { } s when (s.StartsWith("c") || s.StartsWith("x")) && !s.StartsWith("xp"):
                        if (int.TryParse(Code.Substring(1, Code.Length - 1), out int count))
                        {
                            if (Code.StartsWith("x") && !Code.StartsWith("xp") && player.Data.XCrystals + count < 0)
                                break;
                            if (Code.StartsWith("c") && player.Data.Crystals + count < 0) break;
                            result = PromoCodeCheckResult.VALID;
                        }
                        break;
                    case { } s when s.StartsWith("r"):
                        if (int.TryParse(Code.Substring(1, Code.Length - 1), out int number))
                            if (player.Data.Reputation + number >= 0)
                                result = PromoCodeCheckResult.VALID;
                        break;
                    case { } s when s.StartsWith("xp"):
                        if (int.TryParse(Code.Substring(2, Code.Length - 2), out int i))
                            if (player.Data.Experience + i >= 0)
                                result = PromoCodeCheckResult.VALID;
                        break;
                    default:
                        if (long.TryParse(Code, out long code))
                        {
                            Entity marketItem = player.EntityList.SingleOrDefault(e =>
                                e.EntityId == code && e.GetComponent<MarketItemGroupComponent>()?.Key == code);
                            if (marketItem is null) break;
                            result = player.Data.OwnsMarketItem(marketItem)
                                ? PromoCodeCheckResult.OWNED
                                : PromoCodeCheckResult.VALID;
                        }
                        break;
                }

            switch (Code)
			{
				case "7FA8-8E12-DB08":
					// easter egg: Tanki X discontinuation promo code
					result = PromoCodeCheckResult.VALID;
					break;
            }

			player.SendEvent(new PromoCodeCheckResultEvent(Code, result), entity);
		}
		public string Code { get; set; }
	}
}
