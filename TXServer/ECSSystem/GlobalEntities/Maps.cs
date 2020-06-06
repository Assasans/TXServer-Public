using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Maps : ItemList
    {
        public static Maps GlobalItems { get; } = new Maps();

        public Entity Silence { get; } = MapTemplate.CreateEntity(-321842153, "silence");
        public Entity Nightiran { get; } = MapTemplate.CreateEntity(343745828, "nightiran");
        public Entity Acidlake { get; } = MapTemplate.CreateEntity(485053206, "acidlake");
        public Entity Acidlakehalloween { get; } = MapTemplate.CreateEntity(-820833801, "acidlakehalloween");
        public Entity Testbox { get; } = MapTemplate.CreateEntity(458045295, "testbox");
        public Entity Sandbox { get; } = MapTemplate.CreateEntity(-549069251, "sandbox");
        public Entity Iran { get; } = MapTemplate.CreateEntity(-51480736, "iran");
        public Entity Area159 { get; } = MapTemplate.CreateEntity(1133979230, "area159");
        public Entity Repin { get; } = MapTemplate.CreateEntity(-1587964040, "repin");
        public Entity Westprime { get; } = MapTemplate.CreateEntity(980475942, "westprime");
        public Entity Boombox { get; } = MapTemplate.CreateEntity(1945237110, "boombox");
        public Entity Silencemoon { get; } = MapTemplate.CreateEntity(933129112, "silencemoon");
        public Entity Rio { get; } = MapTemplate.CreateEntity(-1664220274, "rio");
        public Entity MassacremarsBG { get; } = MapTemplate.CreateEntity(989096365, "massacremarsBG");
        public Entity Massacre { get; } = MapTemplate.CreateEntity(-1551247853, "massacre");
        public Entity Kungur { get; } = MapTemplate.CreateEntity(2127033418, "kungur");
    }
}
