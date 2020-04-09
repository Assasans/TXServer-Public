using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Maps
    {
        public static readonly Entity Silence = new Entity(-321842153, new TemplateAccessor(new MapTemplate(), "battle/map/silence"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-321842153));
        public static readonly Entity NightIran = new Entity(343745828, new TemplateAccessor(new MapTemplate(), "battle/map/nightiran"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(343745828));
        public static readonly Entity AcidLake = new Entity(485053206, new TemplateAccessor(new MapTemplate(), "battle/map/silence"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(485053206));
        public static readonly Entity AcidLakeHalloween = new Entity(-820833801, new TemplateAccessor(new MapTemplate(), "battle/map/acidlakehalloween"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-820833801));
        public static readonly Entity Testbox = new Entity(458045295, new TemplateAccessor(new MapTemplate(), "battle/map/testbox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(458045295));
        public static readonly Entity Sandbox = new Entity(-549069251, new TemplateAccessor(new MapTemplate(), "battle/map/sandbox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-549069251));
        public static readonly Entity Iran = new Entity(-51480736, new TemplateAccessor(new MapTemplate(), "battle/map/iran"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-51480736));
        public static readonly Entity WestPrime = new Entity(980475942, new TemplateAccessor(new MapTemplate(), "battle/map/westprime"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(980475942));
        public static readonly Entity Boombox = new Entity(1945237110, new TemplateAccessor(new MapTemplate(), "battle/map/boombox"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(1945237110));
        public static readonly Entity SilenceMoon = new Entity(933129112, new TemplateAccessor(new MapTemplate(), "battle/map/silencemoon"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(933129112));
        public static readonly Entity Rio = new Entity(-1664220274, new TemplateAccessor(new MapTemplate(), "battle/map/rio"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-1664220274));
        public static readonly Entity MassacreMarsBG = new Entity(989096365, new TemplateAccessor(new MapTemplate(), "battle/map/massacremarsBG"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(989096365));
        public static readonly Entity Massacre = new Entity(-1551247853, new TemplateAccessor(new MapTemplate(), "battle/map/massacre"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(-1551247853));
        public static readonly Entity Kungur = new Entity(2127033418, new TemplateAccessor(new MapTemplate(), "battle/map/kungur"),
                new MapEnabledInCustomGameComponent(),
                new MapComponent(),
                new MapGroupComponent(2127033418));
    }
}
