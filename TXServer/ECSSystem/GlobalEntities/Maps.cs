using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Maps
    {
        public static Items GlobalItems { get; } = new Items();

        public class Items : ItemList
        {
            public Entity Silence { get; } = new Entity(-321842153, new TemplateAccessor(new MapTemplate(), "battle/map/silence"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-321842153));
            public Entity Nightiran { get; } = new Entity(343745828, new TemplateAccessor(new MapTemplate(), "battle/map/nightiran"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(343745828));
            public Entity Acidlake { get; } = new Entity(485053206, new TemplateAccessor(new MapTemplate(), "battle/map/acidlake"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(485053206));
            public Entity Acidlakehalloween { get; } = new Entity(-820833801, new TemplateAccessor(new MapTemplate(), "battle/map/acidlakehalloween"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-820833801));
            public Entity Testbox { get; } = new Entity(458045295, new TemplateAccessor(new MapTemplate(), "battle/map/testbox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(458045295));
            public Entity Sandbox { get; } = new Entity(-549069251, new TemplateAccessor(new MapTemplate(), "battle/map/sandbox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-549069251));
            public Entity Iran { get; } = new Entity(-51480736, new TemplateAccessor(new MapTemplate(), "battle/map/iran"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-51480736));
            public Entity Area159 { get; } = new Entity(1133979230, new TemplateAccessor(new MapTemplate(), "battle/map/area159"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(1133979230));
            public Entity Repin { get; } = new Entity(-1587964040, new TemplateAccessor(new MapTemplate(), "battle/map/repin"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-1587964040));
            public Entity Westprime { get; } = new Entity(980475942, new TemplateAccessor(new MapTemplate(), "battle/map/westprime"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(980475942));
            public Entity Boombox { get; } = new Entity(1945237110, new TemplateAccessor(new MapTemplate(), "battle/map/boombox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(1945237110));
            public Entity Silencemoon { get; } = new Entity(933129112, new TemplateAccessor(new MapTemplate(), "battle/map/silencemoon"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(933129112));
            public Entity Rio { get; } = new Entity(-1664220274, new TemplateAccessor(new MapTemplate(), "battle/map/rio"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-1664220274));
            public Entity MassacremarsBG { get; } = new Entity(989096365, new TemplateAccessor(new MapTemplate(), "battle/map/massacremarsBG"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(989096365));
            public Entity Massacre { get; } = new Entity(-1551247853, new TemplateAccessor(new MapTemplate(), "battle/map/massacre"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-1551247853));
            public Entity Kungur { get; } = new Entity(2127033418, new TemplateAccessor(new MapTemplate(), "battle/map/kungur"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(2127033418));
        }
    }
}
