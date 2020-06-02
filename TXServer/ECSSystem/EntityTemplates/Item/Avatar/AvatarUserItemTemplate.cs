using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1544694265965L)]
	public class AvatarUserItemTemplate : IEntityTemplate, IMountableItemTemplate
	{
		public static Entity CreateEntity(Entity marketItem, Entity user)
        {
			return new Entity(new TemplateAccessor(new AvatarUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath),
				new MarketItemGroupComponent(marketItem),
				new UserGroupComponent(user));
        }
	}
}
