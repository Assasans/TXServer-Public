using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components.Groups;


namespace TXServer.ECSSystem.EntityTemplates.Squad
{
    [SerialVersionUID(1507120664314L)]
    public class SquadTemplate : IEntityTemplate
    {
        public static Entity CreateEntity()
        {
            Entity entity = new(new TemplateAccessor(new SquadTemplate(), "squad"));
            entity.AddComponent(new SquadGroupComponent(entity));
            return entity;
        }
    }
}