using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(-5630755063511713066)]
    public class MapTemplate : IEntityTemplate
    {
        public static Entity CreateEntity(long id, string configPathEntry)
        {
            return new Entity(id, new TemplateAccessor(new MapTemplate(), "battle/map/" + configPathEntry),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(id));
        }
    }
}
