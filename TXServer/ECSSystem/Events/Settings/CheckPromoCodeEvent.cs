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
            {
				// for testing
				switch (Code)
				{
					case "valid":
						result = PromoCodeCheckResult.VALID;
						break;
					case "notFound":
						result = PromoCodeCheckResult.NOT_FOUND;
						break;
					case "used":
						result = PromoCodeCheckResult.USED;
						break;
					case "expired":
						result = PromoCodeCheckResult.EXPIRED;
						break;
					case "owned":
						result = PromoCodeCheckResult.OWNED;
						break;
				}

				// cheat codes
				if (Code.StartsWith("c") | Code.StartsWith("x")) {
					if (int.TryParse(Code.Substring(1, Code.Length-1), out _))
                    {
						result = PromoCodeCheckResult.VALID;
                    } 
				}
				if (Code.StartsWith("xp"))
                {
					if (int.TryParse(Code.Substring(2, Code.Length - 2), out _))
					{
						result = PromoCodeCheckResult.VALID;
					}
				}
			}

			switch (Code)
			{
				case "7FA8-8E12-DB08":
					// easter egg: Tanki X discontinuation promo code
					result = PromoCodeCheckResult.VALID;
					break;
				case "squad":
					// for squad testing
					// TODO: remove later when quads are tested & stable
					if (player.User.GetComponent<UserExperienceComponent>().Experience < 5000)
					    result = PromoCodeCheckResult.VALID;
					break;
			}

			player.SendEvent(new PromoCodeCheckResultEvent(Code, result), entity);
		}
		public string Code { get; set; }
	}
}
