using System.Reflection;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.EntityTemplates.Item.Module;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Modules
    {
        public static Items GlobalItems { get; } = new Items();

        public static Items GetUserItems(Entity user)
        {
            Items items = new Items();

            foreach (PropertyInfo info in typeof(Items).GetProperties())
            {
                Entity item = info.GetValue(items) as Entity;
                item.EntityId = Entity.GenerateId();

                switch (item.TemplateAccessor.ConfigPath.Split('/')[4])
                {
                    case "active":
                        if (item.TemplateAccessor.ConfigPath.Split('/')[3] == "common")
                        {
                            item.TemplateAccessor.Template = new GoldBonusModuleUserItemTemplate();
                            break;
                        }
                        item.TemplateAccessor.Template = new ModuleUserItemTemplate();
                        break;
                    case "passive":
                        item.TemplateAccessor.Template = new PassiveModuleUserItemTemplate();
                        break;
                    case "trigger":
                        item.TemplateAccessor.Template = new TriggerModuleUserItemTemplate();
                        break;
                }

                item.Components.Add(new UserGroupComponent(user.EntityId));
                item.Components.Add(new ModuleGroupComponent(item.EntityId));
                item.Components.Add(new ModuleUpgradeLevelComponent());
            }

            return items;
        }

        public class Items : ItemList
        {
            public Entity Absorbingarmor { get; } = new Entity(492941809, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/absorbingarmor"),
                new ParentGroupComponent(492941809),
                new MarketItemGroupComponent(492941809),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Repairkit { get; } = new Entity(-862259125, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/repairkit"),
                new ParentGroupComponent(-862259125),
                new MarketItemGroupComponent(-862259125),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Turbospeed { get; } = new Entity(-365494384, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/turbospeed"),
                new ParentGroupComponent(-365494384),
                new MarketItemGroupComponent(-365494384),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            public Entity Forcefield { get; } = new Entity(-1597839790, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/forcefield"),
                new ParentGroupComponent(-1597839790),
                new MarketItemGroupComponent(-1597839790),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            public Entity Invisibility { get; } = new Entity(137179508, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/invisibility"),
                new ParentGroupComponent(137179508),
                new MarketItemGroupComponent(137179508),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            public Entity Jumpimpact { get; } = new Entity(1327463523, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/jumpimpact"),
                new ParentGroupComponent(1327463523),
                new MarketItemGroupComponent(1327463523),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Firering { get; } = new Entity(1896579342, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/firering"),
                new ParentGroupComponent(1896579342),
                new MarketItemGroupComponent(1896579342),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Icetrap { get; } = new Entity(-1177680131, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/icetrap"),
                new ParentGroupComponent(-1177680131),
                new MarketItemGroupComponent(-1177680131),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Invulnerability { get; } = new Entity(1924597477, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/invulnerability"),
                new ParentGroupComponent(1924597477),
                new MarketItemGroupComponent(1924597477),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Backhitdefence { get; } = new Entity(-1962420821, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/1/backhitdefence"),
                new ParentGroupComponent(-1962420821),
                new MarketItemGroupComponent(-1962420821),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Tempblock { get; } = new Entity(596921121, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/1/tempblock"),
                new ParentGroupComponent(596921121),
                new MarketItemGroupComponent(596921121),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Acceleratedgears { get; } = new Entity(1365914179, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/2/acceleratedgears"),
                new ParentGroupComponent(1365914179),
                new MarketItemGroupComponent(1365914179),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Sapper { get; } = new Entity(-105040547, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/trigger/2/sapper"),
                new ParentGroupComponent(-105040547),
                new MarketItemGroupComponent(-105040547),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Emergencyprotection { get; } = new Entity(-357196071, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/trigger/3/emergencyprotection"),
                new ParentGroupComponent(-357196071),
                new MarketItemGroupComponent(-357196071),
                new ModuleTankPartComponent(TankPartModuleType.TANK),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Emp { get; } = new Entity(-1493372159, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/emp"),
                new ParentGroupComponent(-1493372159),
                new MarketItemGroupComponent(-1493372159),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Mine { get; } = new Entity(1133911248, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/mine"),
                new ParentGroupComponent(1133911248),
                new MarketItemGroupComponent(1133911248),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Sonar { get; } = new Entity(-1318192334, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/sonar"),
                new ParentGroupComponent(-1318192334),
                new MarketItemGroupComponent(-1318192334),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Externalimpact { get; } = new Entity(-1334156852, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/externalimpact"),
                new ParentGroupComponent(-1334156852),
                new MarketItemGroupComponent(-1334156852),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Increaseddamage { get; } = new Entity(676407818, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/increaseddamage"),
                new ParentGroupComponent(676407818),
                new MarketItemGroupComponent(676407818),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            public Entity Spidermine { get; } = new Entity(1458405023, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/spidermine"),
                new ParentGroupComponent(1458405023),
                new MarketItemGroupComponent(1458405023),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Drone { get; } = new Entity(1392039140, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/drone"),
                new ParentGroupComponent(1392039140),
                new MarketItemGroupComponent(1392039140),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Energyinjection { get; } = new Entity(1128679079, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/energyinjection"),
                new ParentGroupComponent(1128679079),
                new MarketItemGroupComponent(1128679079),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Explosivemass { get; } = new Entity(393550399, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/explosivemass"),
                new ParentGroupComponent(393550399),
                new MarketItemGroupComponent(393550399),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Backhitincrease { get; } = new Entity(-2075784110, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/1/backhitincrease"),
                new ParentGroupComponent(-2075784110),
                new MarketItemGroupComponent(-2075784110),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Engineer { get; } = new Entity(-1027949361, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/1/engineer"),
                new ParentGroupComponent(-1027949361),
                new MarketItemGroupComponent(-1027949361),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());
            /*public Entity Adrenaline { get; } = new Entity(1367280061, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/2/adrenaline"),
                new ParentGroupComponent(1367280061),
                new MarketItemGroupComponent(1367280061),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Rage { get; } = new Entity(1215656773, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/1/rage"),
                new ParentGroupComponent(1215656773),
                new MarketItemGroupComponent(1215656773),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            /*public Entity Kamikadze { get; } = new Entity(-1603423529, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/2/kamikadze"),
                new ParentGroupComponent(-1603423529),
                new MarketItemGroupComponent(-1603423529),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(1),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());*/
            public Entity Lifesteal { get; } = new Entity(-246333323, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/3/lifesteal"),
                new ParentGroupComponent(-246333323),
                new MarketItemGroupComponent(-246333323),
                new ModuleTankPartComponent(TankPartModuleType.WEAPON),
                new ModuleTierComponent(2),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
                new ModuleCardsCompositionComponent());
            public Entity Gold { get; } = new Entity(-150814762, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/prebuildmodule/common/active/1/gold"),
                new ParentGroupComponent(-150814762),
                new MarketItemGroupComponent(-150814762),
                new ModuleTankPartComponent(TankPartModuleType.COMMON),
                new ModuleTierComponent(0),
                new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
                new ModuleCardsCompositionComponent(),
                new ImmutableModuleItemComponent());
        }
    }
}
