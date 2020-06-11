using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Shells : ItemList
    {
        public static Shells GlobalItems { get; } = new Shells();

        public static Shells GetUserItems(Entity user)
        {
            Shells items = FormatterServices.GetUninitializedObject(typeof(Shells)) as Shells;

            foreach (PropertyInfo info in typeof(Shells).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity FlamethrowerAcid { get; private set; } = ShellMarketItemTemplate.CreateEntity(692677861, "flamethrower/acid", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerOrange { get; private set; } = ShellMarketItemTemplate.CreateEntity(357929046, "flamethrower/orange", Weapons.GlobalItems.Flamethrower);
        public Entity FlamethrowerRed { get; private set; } = ShellMarketItemTemplate.CreateEntity(-947470487, "flamethrower/red", Weapons.GlobalItems.Flamethrower);
        public Entity FreezeIndigo { get; private set; } = ShellMarketItemTemplate.CreateEntity(224610499, "freeze/indigo", Weapons.GlobalItems.Freeze);
        public Entity FreezeSkyblue { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1408603862, "freeze/skyblue", Weapons.GlobalItems.Freeze);
        public Entity FreezeWhite { get; private set; } = ShellMarketItemTemplate.CreateEntity(-395640808, "freeze/white", Weapons.GlobalItems.Freeze);
        public Entity HammerPapercracker { get; private set; } = ShellMarketItemTemplate.CreateEntity(142652989, "hammer/papercracker", Weapons.GlobalItems.Hammer);
        public Entity HammerStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(530945311, "hammer/standard", Weapons.GlobalItems.Hammer);
        public Entity HammerThor { get; private set; } = ShellMarketItemTemplate.CreateEntity(1317881529, "hammer/thor", Weapons.GlobalItems.Hammer);
        public Entity IsisEmerald { get; private set; } = ShellMarketItemTemplate.CreateEntity(2065928272, "isis/emerald", Weapons.GlobalItems.Isis);
        public Entity IsisShine { get; private set; } = ShellMarketItemTemplate.CreateEntity(-132552041, "isis/shine", Weapons.GlobalItems.Isis);
        public Entity IsisStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(48235025, "isis/standard", Weapons.GlobalItems.Isis);
        public Entity RailgunGlitch { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1975536348, "railgun/glitch", Weapons.GlobalItems.Railgun);
        public Entity RailgunGloom { get; private set; } = ShellMarketItemTemplate.CreateEntity(-375403918, "railgun/gloom", Weapons.GlobalItems.Railgun);
        public Entity RailgunGreen { get; private set; } = ShellMarketItemTemplate.CreateEntity(-660457061, "railgun/green", Weapons.GlobalItems.Railgun);
        public Entity RailgunOrange { get; private set; } = ShellMarketItemTemplate.CreateEntity(1229590166, "railgun/orange", Weapons.GlobalItems.Railgun);
        public Entity RailgunPaleblue { get; private set; } = ShellMarketItemTemplate.CreateEntity(366763244, "railgun/paleblue", Weapons.GlobalItems.Railgun);
        public Entity RailgunPurple { get; private set; } = ShellMarketItemTemplate.CreateEntity(1261498404, "railgun/purple", Weapons.GlobalItems.Railgun);
        public Entity RailgunSmoke { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1195981301, "railgun/smoke", Weapons.GlobalItems.Railgun);
        public Entity RicochetAcid { get; private set; } = ShellMarketItemTemplate.CreateEntity(-492753567, "ricochet/acid", Weapons.GlobalItems.Ricochet);
        public Entity RicochetAurulent { get; private set; } = ShellMarketItemTemplate.CreateEntity(139800007, "ricochet/aurulent", Weapons.GlobalItems.Ricochet);
        public Entity RicochetCannonball { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1899861235, "ricochet/cannonball", Weapons.GlobalItems.Ricochet);
        public Entity RicochetCoral { get; private set; } = ShellMarketItemTemplate.CreateEntity(577198848, "ricochet/coral", Weapons.GlobalItems.Ricochet);
        public Entity RicochetMoon { get; private set; } = ShellMarketItemTemplate.CreateEntity(-585038633, "ricochet/moon", Weapons.GlobalItems.Ricochet);
        public Entity RicochetRicsnowball { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1073342998, "ricochet/ricsnowball", Weapons.GlobalItems.Ricochet);
        public Entity ShaftBlue { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1488314890, "shaft/blue", Weapons.GlobalItems.Shaft);
        public Entity ShaftLighting { get; private set; } = ShellMarketItemTemplate.CreateEntity(-250346840, "shaft/lighting", Weapons.GlobalItems.Shaft);
        public Entity ShaftRed { get; private set; } = ShellMarketItemTemplate.CreateEntity(-47995019, "shaft/red", Weapons.GlobalItems.Shaft);
        public Entity ShaftStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(70311513, "shaft/standard", Weapons.GlobalItems.Shaft);
        public Entity SmokyFumes { get; private set; } = ShellMarketItemTemplate.CreateEntity(860375257, "smoky/fumes", Weapons.GlobalItems.Smoky);
        public Entity SmokyLightning { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1260549513, "smoky/lightning", Weapons.GlobalItems.Smoky);
        public Entity SmokyStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(-966935184, "smoky/standard", Weapons.GlobalItems.Smoky);
        public Entity SmokyThermite { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1165747871, "smoky/thermite", Weapons.GlobalItems.Smoky);
        public Entity ThunderDragon { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1852311024, "thunder/dragon", Weapons.GlobalItems.Thunder);
        public Entity ThunderLightning { get; private set; } = ShellMarketItemTemplate.CreateEntity(794635633, "thunder/lightning", Weapons.GlobalItems.Thunder);
        public Entity ThunderStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(1067800943, "thunder/standard", Weapons.GlobalItems.Thunder);
        public Entity TwinsBlue { get; private set; } = ShellMarketItemTemplate.CreateEntity(807172229, "twins/blue", Weapons.GlobalItems.Twins);
        public Entity TwinsOrange { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1319432295, "twins/orange", Weapons.GlobalItems.Twins);
        public Entity TwinsTurquoise { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1173768746, "twins/turquoise", Weapons.GlobalItems.Twins);
        public Entity TwinsTwinssnowball { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1092514341, "twins/twinssnowball", Weapons.GlobalItems.Twins);
        public Entity TwinsViolet { get; private set; } = ShellMarketItemTemplate.CreateEntity(-1126924822, "twins/violet", Weapons.GlobalItems.Twins);
        public Entity TwinsWhite { get; private set; } = ShellMarketItemTemplate.CreateEntity(-728200866, "twins/white", Weapons.GlobalItems.Twins);
        public Entity VulcanAcid { get; private set; } = ShellMarketItemTemplate.CreateEntity(189181410, "vulcan/acid", Weapons.GlobalItems.Vulcan);
        public Entity VulcanBlue { get; private set; } = ShellMarketItemTemplate.CreateEntity(189220223, "vulcan/blue", Weapons.GlobalItems.Vulcan);
        public Entity VulcanRed { get; private set; } = ShellMarketItemTemplate.CreateEntity(-963712308, "vulcan/red", Weapons.GlobalItems.Vulcan);
        public Entity VulcanStandard { get; private set; } = ShellMarketItemTemplate.CreateEntity(1322064226, "vulcan/standard", Weapons.GlobalItems.Vulcan);
    }
}
