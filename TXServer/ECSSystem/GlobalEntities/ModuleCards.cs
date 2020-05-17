using System.Reflection;
using TXServer.Core;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class ModuleCards
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Player.GenerateId();

                item.TemplateAccessor.Template = new ModuleCardUserItemTemplate();

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new UserItemCounterComponent(1));
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Absorbingarmor { get; } = new Entity(1070497259, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/absorbingarmor"),
                new ParentGroupComponent(Modules.GlobalItems.Absorbingarmor),
                new MarketItemGroupComponent(1070497259));
            public Entity Acceleratedgears { get; } = new Entity(1379340193, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/acceleratedgears"),
                new ParentGroupComponent(Modules.GlobalItems.Acceleratedgears),
                new MarketItemGroupComponent(1379340193));
            public Entity Adrenaline { get; } = new Entity(1384519921, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/adrenaline"),
                new ParentGroupComponent(Modules.GlobalItems.Adrenaline),
                new MarketItemGroupComponent(1384519921));
            public Entity Backhitdefence { get; } = new Entity(-1143816664, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/backhitdefence"),
                new ParentGroupComponent(Modules.GlobalItems.Backhitdefence),
                new MarketItemGroupComponent(-1143816664));
            public Entity Drone { get; } = new Entity(69326151, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/drone"),
                new ParentGroupComponent(Modules.GlobalItems.Drone),
                new MarketItemGroupComponent(69326151));
            public Entity Emergencyprotection { get; } = new Entity(-443509498, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/emergencyprotection"),
                new ParentGroupComponent(Modules.GlobalItems.Emergencyprotection),
                new MarketItemGroupComponent(-443509498));
            public Entity Emp { get; } = new Entity(-1756465500, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/emp"),
                new ParentGroupComponent(Modules.GlobalItems.Emp),
                new MarketItemGroupComponent(-1756465500));
            public Entity Energyinjection { get; } = new Entity(-1116052693, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/energyinjection"),
                new ParentGroupComponent(Modules.GlobalItems.Energyinjection),
                new MarketItemGroupComponent(-1116052693));
            public Entity Engineer { get; } = new Entity(1464064213, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/engineer"),
                new ParentGroupComponent(Modules.GlobalItems.Engineer),
                new MarketItemGroupComponent(1464064213));
            public Entity Explosivemass { get; } = new Entity(-206369995, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/explosivemass"),
                new ParentGroupComponent(Modules.GlobalItems.Explosivemass),
                new MarketItemGroupComponent(-206369995));
            public Entity Externalimpact { get; } = new Entity(1032613237, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/externalimpact"),
                new ParentGroupComponent(Modules.GlobalItems.Externalimpact),
                new MarketItemGroupComponent(1032613237));
            public Entity Firering { get; } = new Entity(-627265270, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/firering"),
                new ParentGroupComponent(Modules.GlobalItems.Firering),
                new MarketItemGroupComponent(-627265270));
            public Entity Forcefield { get; } = new Entity(2100727840, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/forcefield"),
                new ParentGroupComponent(Modules.GlobalItems.Forcefield),
                new MarketItemGroupComponent(2100727840));
            public Entity Icetrap { get; } = new Entity(1914815875, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/icetrap"),
                new ParentGroupComponent(Modules.GlobalItems.Icetrap),
                new MarketItemGroupComponent(1914815875));
            public Entity Increaseddamage { get; } = new Entity(465438765, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/increaseddamage"),
                new ParentGroupComponent(Modules.GlobalItems.Increaseddamage),
                new MarketItemGroupComponent(465438765));
            public Entity Invisibility { get; } = new Entity(1625485211, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/invisibility"),
                new ParentGroupComponent(Modules.GlobalItems.Invisibility),
                new MarketItemGroupComponent(1625485211));
            public Entity Invulnerability { get; } = new Entity(-1128519711, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/invulnerability"),
                new ParentGroupComponent(Modules.GlobalItems.Invulnerability),
                new MarketItemGroupComponent(-1128519711));
            public Entity Jumpimpact { get; } = new Entity(-1596570946, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/jumpimpact"),
                new ParentGroupComponent(Modules.GlobalItems.Jumpimpact),
                new MarketItemGroupComponent(-1596570946));
            public Entity Kamikadze { get; } = new Entity(489948195, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/kamikadze"),
                new ParentGroupComponent(Modules.GlobalItems.Kamikadze),
                new MarketItemGroupComponent(489948195));
            public Entity Lifesteal { get; } = new Entity(2010921135, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/lifesteal"),
                new ParentGroupComponent(Modules.GlobalItems.Lifesteal),
                new MarketItemGroupComponent(2010921135));
            public Entity Mine { get; } = new Entity(1384378871, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/mine"),
                new ParentGroupComponent(Modules.GlobalItems.Mine),
                new MarketItemGroupComponent(1384378871));
            public Entity Rage { get; } = new Entity(-786092160, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/rage"),
                new ParentGroupComponent(Modules.GlobalItems.Rage),
                new MarketItemGroupComponent(-786092160));
            public Entity Sapper { get; } = new Entity(-887306593, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/sapper"),
                new ParentGroupComponent(Modules.GlobalItems.Sapper),
                new MarketItemGroupComponent(-887306593));
            public Entity Sonar { get; } = new Entity(-28208097, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/sonar"),
                new ParentGroupComponent(Modules.GlobalItems.Sonar),
                new MarketItemGroupComponent(-28208097));
            public Entity Spidermine { get; } = new Entity(709690204, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/spidermine"),
                new ParentGroupComponent(Modules.GlobalItems.Spidermine),
                new MarketItemGroupComponent(709690204));
            public Entity Tempblock { get; } = new Entity(-23536107, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/tempblock"),
                new ParentGroupComponent(Modules.GlobalItems.Tempblock),
                new MarketItemGroupComponent(-23536107));
            public Entity Turbospeed { get; } = new Entity(1378523021, new TemplateAccessor(new ModuleCardMarketItemTemplate(), "garage/module/card/turbospeed"),
                new ParentGroupComponent(Modules.GlobalItems.Turbospeed),
                new MarketItemGroupComponent(1378523021));
        }
    }
}
