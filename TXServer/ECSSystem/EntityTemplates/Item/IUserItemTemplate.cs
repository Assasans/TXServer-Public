using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    public interface IUserItemTemplate : IEntityTemplate
    {
        void AddUserItemComponents(Entity item);
    }
}
