﻿using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Components;
using TXServer.ECSSystem.EntityTemplates;

namespace TXServer.ECSSystem.GlobalEntities
{
    public static class Modules
    {
        public static readonly Entity AbsorbingArmor = new Entity(492941809, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/absorbingarmor"),
            new ParentGroupComponent(492941809),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(492941809),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity RepairKit = new Entity(-862259125, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/repairkit"),
            new ParentGroupComponent(-862259125),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-862259125),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity TurboSpeed = new Entity(-365494384, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/1/turbospeed"),
            new ParentGroupComponent(-365494384),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-365494384),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity ForceField = new Entity(-1597839790, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/forcefield"),
            new ParentGroupComponent(-1597839790),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-1597839790),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Invisibility = new Entity(137179508, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/invisibility"),
            new ParentGroupComponent(137179508),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(137179508),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity JumpImpact = new Entity(1327463523, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/2/jumpimpact"),
            new ParentGroupComponent(1327463523),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(1327463523),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity FireRing = new Entity(1896579342, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/firering"),
            new ParentGroupComponent(1896579342),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(1896579342),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity IceTrap = new Entity(-1177680131, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/icetrap"),
            new ParentGroupComponent(-1177680131),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-1177680131),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Invulnerability = new Entity(1924597477, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/active/3/invulnerability"),
            new ParentGroupComponent(1924597477),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(1924597477),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity BackHitDefence = new Entity(-1962420821, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/1/backhitdefence"),
            new ParentGroupComponent(-1962420821),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-1962420821),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity TempBlock = new Entity(596921121, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/1/tempblock"),
            new ParentGroupComponent(596921121),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(596921121),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity AcceleratedGears = new Entity(1365914179, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/passive/2/acceleratedgears"),
            new ParentGroupComponent(1365914179),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(1365914179),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Sapper = new Entity(-105040547, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/trigger/2/sapper"),
            new ParentGroupComponent(-105040547),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-105040547),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity EmergencyProtection = new Entity(-357196071, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/tank/trigger/3/emergencyprotection"),
            new ParentGroupComponent(-357196071),
            new ModuleTankPartComponent(TankPartModuleType.TANK),
            new MarketItemGroupComponent(-357196071),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity EMP = new Entity(-1493372159, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/emp"),
            new ParentGroupComponent(-1493372159),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-1493372159),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Mine = new Entity(1133911248, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/mine"),
            new ParentGroupComponent(1133911248),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1133911248),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Sonar = new Entity(-1318192334, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/1/sonar"),
            new ParentGroupComponent(-1318192334),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-1318192334),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity ExternalImpact = new Entity(-1334156852, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/externalimpact"),
            new ParentGroupComponent(-1334156852),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-1334156852),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity IncreasedDameage = new Entity(676407818, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/increaseddamage"),
            new ParentGroupComponent(676407818),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(676407818),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity SpiderMine = new Entity(1458405023, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/2/spidermine"),
            new ParentGroupComponent(1458405023),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1458405023),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Drone = new Entity(1392039140, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/drone"),
            new ParentGroupComponent(1392039140),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1392039140),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity EnergyIngection = new Entity(1128679079, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/energyinjection"),
            new ParentGroupComponent(1128679079),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1128679079),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity ExplosiveMass = new Entity(393550399, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/active/3/explosivemass"),
            new ParentGroupComponent(393550399),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(393550399),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity BackHitIncrease = new Entity(-2075784110, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/1/backhitincrease"),
            new ParentGroupComponent(-2075784110),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-2075784110),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Engineer = new Entity(-1027949361, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/1/engineer"),
            new ParentGroupComponent(-1027949361),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-1027949361),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Adrenaline = new Entity(1367280061, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/passive/2/adrenaline"),
            new ParentGroupComponent(1367280061),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1367280061),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Rage = new Entity(1215656773, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/1/rage"),
            new ParentGroupComponent(1215656773),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(1215656773),
            new ModuleTierComponent(0),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Kamikadze = new Entity(-1603423529, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/2/kamikadze"),
            new ParentGroupComponent(-1603423529),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-1603423529),
            new ModuleTierComponent(1),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity LifeSteal = new Entity(-246333323, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/module/weapon/trigger/3/lifesteal"),
            new ParentGroupComponent(-246333323),
            new ModuleTankPartComponent(TankPartModuleType.WEAPON),
            new MarketItemGroupComponent(-246333323),
            new ModuleTierComponent(2),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.PASSIVE),
            new ModuleCardsCompositionComponent());
        public static readonly Entity Gold = new Entity(-150814762, new TemplateAccessor(new ModuleMarketItemTemplate(), "garage/module/prebuildmodule/common/active/1/gold"),
            new ParentGroupComponent(-150814762),
            new ModuleTankPartComponent(TankPartModuleType.COMMON),
            new MarketItemGroupComponent(-150814762),
            new ModuleTierComponent(0),
            new ImmutableModuleItemComponent(),
            new ModuleBehaviourTypeComponent(ModuleBehaviourType.ACTIVE),
            new ModuleCardsCompositionComponent());
    }
}
