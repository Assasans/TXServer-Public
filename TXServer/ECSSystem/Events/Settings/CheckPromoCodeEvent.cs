using TXServer.Core;
using TXServer.Core.Commands;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.Events
{
	[SerialVersionUID(1490931976968L)]
	public class CheckPromoCodeEvent : ECSEvent
	{
		public void Execute(Player player, Entity entity)
		{
			PromoCodeCheckResult result = PromoCodeCheckResult.INVALID;
			// TODO: easter egg: 7FA8-8E12-DB08 (Tanki X discontinuation promo code, let's give away some crystals)

			if (player.User.GetComponent<UserAdminComponent>() != null)
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
				int var;
				if (Code.StartsWith("c") | Code.StartsWith("x")) {
					if (int.TryParse(Code.Substring(1, Code.Length-1), out var))
                    {
						result = PromoCodeCheckResult.VALID;
                    } 
				}
				if (Code.StartsWith("xp"))
                {
					if (int.TryParse(Code.Substring(2, Code.Length - 2), out var))
					{
						result = PromoCodeCheckResult.VALID;
					}
				}
			}

			CommandManager.SendCommands(player,
				new SendEventCommand(new PromoCodeCheckResultEvent(Code, result), entity));
		}
		public string Code { get; set; }
	}
}
