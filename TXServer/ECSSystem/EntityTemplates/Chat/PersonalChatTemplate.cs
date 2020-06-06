using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(636469998634338659L)]
	public class PersonalChatTemplate : IEntityTemplate
	{
		public static Entity CreateEntity(params Entity[] users)
        {
			return new Entity(new TemplateAccessor(new PersonalChatTemplate(), "chat"),
				new ChatComponent(),
				new ChatParticipantsComponent(users));
		}
	}
}
