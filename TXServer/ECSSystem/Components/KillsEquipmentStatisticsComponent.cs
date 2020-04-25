using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.GlobalEntities;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1499175516647)]
    public class KillsEquipmentStatisticsComponent : Component
    {
        public Dictionary<Entity, long> HullStatistics { get; set; } = new Dictionary<Entity, long>()
        {
            { Weapons.GlobalItems.Flamethrower, 0 },
            { Weapons.GlobalItems.Freeze, 0 },
            { Weapons.GlobalItems.Hammer, 0 },
            { Weapons.GlobalItems.Isis, 0 },
            { Weapons.GlobalItems.Railgun, 0 },
            { Weapons.GlobalItems.Ricochet, 0 },
            { Weapons.GlobalItems.Shaft, 0 },
            { Weapons.GlobalItems.Smoky, 0 },
            { Weapons.GlobalItems.Thunder, 0 },
            { Weapons.GlobalItems.Twins, 0 },
            { Weapons.GlobalItems.Vulcan, 0 }
        };

        public Dictionary<Entity, long> TurretStatistics { get; set; } = new Dictionary<Entity, long>()
        {
            { Hulls.GlobalItems.Wasp, 0 },
            { Hulls.GlobalItems.Hornet, 0 },
            { Hulls.GlobalItems.Hunter, 0 },
            { Hulls.GlobalItems.Viking, 0 },
            { Hulls.GlobalItems.Titan, 0 },
            { Hulls.GlobalItems.Dictator, 0 },
            { Hulls.GlobalItems.Mammoth, 0 }
        };
    }
}
