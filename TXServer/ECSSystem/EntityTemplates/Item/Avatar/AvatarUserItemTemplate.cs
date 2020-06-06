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
			Entity userItem = new Entity(new TemplateAccessor(new AvatarUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
			userItem.Components.Add(new UserGroupComponent(user));

			return userItem;
        }
	}
}
