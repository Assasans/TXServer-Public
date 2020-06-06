using System.Reflection;
using System.Runtime.Serialization;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.GlobalEntities
{
    public class Modules : ItemList
    {
        public static Modules GlobalItems { get; } = new Modules();

        public static ItemList GetUserItems(Entity user)
        {
            ItemList items = FormatterServices.GetUninitializedObject(typeof(Modules)) as ItemList;

            foreach (PropertyInfo info in typeof(Modules).GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                Entity marketItem = info.GetValue(GlobalItems) as Entity;

                Entity userItem = (marketItem.TemplateAccessor.Template as IMarketItemTemplate).GetUserItem(marketItem, user);
                info.SetValue(items, userItem);
            }

            return items;
        }

        public Entity Absorbingarmor { get; private set; } = ModuleMarketItemTemplate.CreateEntity(492941809, "tank/active/1/absorbingarmor",
            TankPartModuleType.TANK,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Repairkit { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-862259125, "tank/active/1/repairkit",
            TankPartModuleType.TANK,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Turbospeed { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-365494384, "tank/active/1/turbospeed",
            TankPartModuleType.TANK,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Forcefield { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1597839790, "tank/active/2/forcefield",
            TankPartModuleType.TANK,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Invisibility { get; private set; } = ModuleMarketItemTemplate.CreateEntity(137179508, "tank/active/2/invisibility",
            TankPartModuleType.TANK,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Jumpimpact { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1327463523, "tank/active/2/jumpimpact",
            TankPartModuleType.TANK,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Firering { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1896579342, "tank/active/3/firering",
            TankPartModuleType.TANK,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Icetrap { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1177680131, "tank/active/3/icetrap",
            TankPartModuleType.TANK,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Invulnerability { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1924597477, "tank/active/3/invulnerability",
            TankPartModuleType.TANK,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Backhitdefence { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1962420821, "tank/passive/1/backhitdefence",
            TankPartModuleType.TANK,
            tier: 0,
            ModuleBehaviourType.PASSIVE);
        public Entity Tempblock { get; private set; } = ModuleMarketItemTemplate.CreateEntity(596921121, "tank/passive/1/tempblock",
            TankPartModuleType.TANK,
            tier: 0,
            ModuleBehaviourType.PASSIVE);
        public Entity Acceleratedgears { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1365914179, "tank/passive/2/acceleratedgears",
            TankPartModuleType.TANK,
            tier: 1,
            ModuleBehaviourType.PASSIVE);
        public Entity Sapper { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-105040547, "tank/trigger/2/sapper",
            TankPartModuleType.TANK,
            tier: 1,
            ModuleBehaviourType.PASSIVE);
        public Entity Emergencyprotection { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-357196071, "tank/trigger/3/emergencyprotection",
            TankPartModuleType.TANK,
            tier: 2,
            ModuleBehaviourType.PASSIVE);
        public Entity Emp { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1493372159, "weapon/active/1/emp",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Mine { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1133911248, "weapon/active/1/mine",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Sonar { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1318192334, "weapon/active/1/sonar",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.ACTIVE);
        public Entity Externalimpact { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1334156852, "weapon/active/2/externalimpact",
            TankPartModuleType.WEAPON,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Increaseddamage { get; private set; } = ModuleMarketItemTemplate.CreateEntity(676407818, "weapon/active/2/increaseddamage",
            TankPartModuleType.WEAPON,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Spidermine { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1458405023, "weapon/active/2/spidermine",
            TankPartModuleType.WEAPON,
            tier: 1,
            ModuleBehaviourType.ACTIVE);
        public Entity Drone { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1392039140, "weapon/active/3/drone",
            TankPartModuleType.WEAPON,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Energyinjection { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1128679079, "weapon/active/3/energyinjection",
            TankPartModuleType.WEAPON,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Explosivemass { get; private set; } = ModuleMarketItemTemplate.CreateEntity(393550399, "weapon/active/3/explosivemass",
            TankPartModuleType.WEAPON,
            tier: 2,
            ModuleBehaviourType.ACTIVE);
        public Entity Backhitincrease { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-2075784110, "weapon/passive/1/backhitincrease",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.PASSIVE);
        public Entity Engineer { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1027949361, "weapon/passive/1/engineer",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.PASSIVE);
        public Entity Adrenaline { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1367280061, "weapon/passive/2/adrenaline",
            TankPartModuleType.WEAPON,
            tier: 1,
            ModuleBehaviourType.PASSIVE);
        public Entity Rage { get; private set; } = ModuleMarketItemTemplate.CreateEntity(1215656773, "weapon/trigger/1/rage",
            TankPartModuleType.WEAPON,
            tier: 0,
            ModuleBehaviourType.PASSIVE);
        public Entity Kamikadze { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-1603423529, "weapon/trigger/2/kamikadze",
            TankPartModuleType.WEAPON,
            tier: 1,
            ModuleBehaviourType.PASSIVE);
        public Entity Lifesteal { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-246333323, "weapon/trigger/3/lifesteal",
            TankPartModuleType.WEAPON,
            tier: 2,
            ModuleBehaviourType.PASSIVE);
        public Entity Gold { get; private set; } = ModuleMarketItemTemplate.CreateEntity(-150814762, "common/active/1/gold",
            TankPartModuleType.COMMON,
            tier: 0,
            ModuleBehaviourType.PASSIVE,
            immutable: true);
    }
}
