using System.Reflection;
using System.Runtime.Serialization;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class ModuleCards : ItemList
    {
        public static ModuleCards GlobalItems { get; } = new ModuleCards();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(ModuleCards)) as ItemList;

            foreach (PropertyInfo info in typeof(ModuleCards).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Acceleratedgears { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1379340193, "acceleratedgears", Modules.GlobalItems.Acceleratedgears);
        public Entity Adrenaline { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1384519921, "adrenaline", Modules.GlobalItems.Adrenaline);
        public Entity Backhitdefence { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-1143816664, "backhitdefence", Modules.GlobalItems.Backhitdefence);
        public Entity Drone { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(69326151, "drone", Modules.GlobalItems.Drone);
        public Entity Emergencyprotection { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-443509498, "emergencyprotection", Modules.GlobalItems.Emergencyprotection);
        public Entity Emp { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-1756465500, "emp", Modules.GlobalItems.Emp);
        public Entity Energyinjection { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-1116052693, "energyinjection", Modules.GlobalItems.Energyinjection);
        public Entity Engineer { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1464064213, "engineer", Modules.GlobalItems.Engineer);
        public Entity Explosivemass { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-206369995, "explosivemass", Modules.GlobalItems.Explosivemass);
        public Entity Externalimpact { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1032613237, "externalimpact", Modules.GlobalItems.Externalimpact);
        public Entity Firering { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-627265270, "firering", Modules.GlobalItems.Firering);
        public Entity Forcefield { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(2100727840, "forcefield", Modules.GlobalItems.Forcefield);
        public Entity Icetrap { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1914815875, "icetrap", Modules.GlobalItems.Icetrap);
        public Entity Increaseddamage { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(465438765, "increaseddamage", Modules.GlobalItems.Increaseddamage);
        public Entity Invisibility { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1625485211, "invisibility", Modules.GlobalItems.Invisibility);
        public Entity Invulnerability { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-1128519711, "invulnerability", Modules.GlobalItems.Invulnerability);
        public Entity Jumpimpact { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-1596570946, "jumpimpact", Modules.GlobalItems.Jumpimpact);
        public Entity Kamikadze { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(489948195, "kamikadze", Modules.GlobalItems.Kamikadze);
        public Entity Lifesteal { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(2010921135, "lifesteal", Modules.GlobalItems.Lifesteal);
        public Entity Mine { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1384378871, "mine", Modules.GlobalItems.Mine);
        public Entity Rage { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-786092160, "rage", Modules.GlobalItems.Rage);
        public Entity Sapper { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-887306593, "sapper", Modules.GlobalItems.Sapper);
        public Entity Sonar { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-28208097, "sonar", Modules.GlobalItems.Sonar);
        public Entity Spidermine { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(709690204, "spidermine", Modules.GlobalItems.Spidermine);
        public Entity Tempblock { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(-23536107, "tempblock", Modules.GlobalItems.Tempblock);
        public Entity Turbospeed { get; private set; } = ModuleCardMarketItemTemplate.CreateEntity(1378523021, "turbospeed", Modules.GlobalItems.Turbospeed);
    }
}
