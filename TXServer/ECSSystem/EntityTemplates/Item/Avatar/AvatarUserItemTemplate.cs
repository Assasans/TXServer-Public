using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1544694265965L)]
	public class AvatarUserItemTemplate : UserItemTemplate, IMountableItemTemplate
	{
		public static Entity CreateEntity(Entity marketItem, Entity user)
        {
			Entity userItem = new Entity(new TemplateAccessor(new AvatarUserItemTemplate(), marketItem.TemplateAccessor.ConfigPath), marketItem.Components);
			userItem.Components.Add(new UserGroupComponent(user));

			AddToUserItems(typeof(Avatars), userItem);
			return userItem;
        }
	}
}
