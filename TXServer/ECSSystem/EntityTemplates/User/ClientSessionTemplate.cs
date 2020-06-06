using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1429771189777)]
    public class ClientSessionTemplate : IEntityTemplate
    {
        public static Entity CreateEntity()
        {
            return new Entity(TemplateAccessor: new TemplateAccessor(new ClientSessionTemplate(), null),
                new ClientSessionComponent());
        }
    }
}
