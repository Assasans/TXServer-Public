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
            { Weapons.Flamethrower, 0 },
            { Weapons.Freeze, 0 },
            { Weapons.Hammer, 0 },
            { Weapons.Isis, 0 },
            { Weapons.Railgun, 0 },
            { Weapons.Ricochet, 0 },
            { Weapons.Shaft, 0 },
            { Weapons.Smoky, 0 },
            { Weapons.Thunder, 0 },
            { Weapons.Twins, 0 },
            { Weapons.Vulcan, 0 }
        };

        public Dictionary<Entity, long> TurretStatistics { get; set; } = new Dictionary<Entity, long>()
        {
            { Hulls.Wasp, 0 },
            { Hulls.Hornet, 0 },
            { Hulls.Hunter, 0 },
            { Hulls.Viking, 0 },
            { Hulls.Titan, 0 },
            { Hulls.Dictator, 0 },
            { Hulls.Mammoth, 0 }
        };
    }
}
