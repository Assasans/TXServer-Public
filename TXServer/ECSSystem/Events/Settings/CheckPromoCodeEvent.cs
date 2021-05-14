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
                result = Code switch
                {
                    "valid" => PromoCodeCheckResult.VALID,
                    "notFound" => PromoCodeCheckResult.NOT_FOUND,
                    "used" => PromoCodeCheckResult.USED,
                    "expired" => PromoCodeCheckResult.EXPIRED,
                    "owned" => PromoCodeCheckResult.OWNED,
                    _ => result
                };

                // cheat codes
				if (Code.StartsWith("c") | Code.StartsWith("x")) {
					if (int.TryParse(Code.Substring(1, Code.Length-1), out _))
                    {
						result = PromoCodeCheckResult.VALID;
                    }
				}
				else if (Code.StartsWith("xp"))
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
                case "teleport":
                    // for teleport command testing
                    // TODO: remove later when premium is fully integrated
                    result = PromoCodeCheckResult.VALID;
                    break;
            }

			player.SendEvent(new PromoCodeCheckResultEvent(Code, result), entity);
		}
		public string Code { get; set; }
	}
}
