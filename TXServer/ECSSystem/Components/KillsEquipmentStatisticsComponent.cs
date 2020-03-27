using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using static TXServer.ECSSystem.Base.Entity;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1499175516647)]
    public class KillsEquipmentStatisticsComponent : Component
    {
        public Dictionary<Entity, long> HullStatistics { get; set; } = new Dictionary<Entity, long>()
        {
            { GlobalEntities.GARAGE_WEAPON_FLAMETHROWER, 0 },
            { GlobalEntities.GARAGE_WEAPON_FREEZE, 0 },
            { GlobalEntities.GARAGE_WEAPON_HAMMER, 0 },
            { GlobalEntities.GARAGE_WEAPON_ISIS, 0 },
            { GlobalEntities.GARAGE_WEAPON_RAILGUN, 0 },
            { GlobalEntities.GARAGE_WEAPON_RICOCHET, 0 },
            { GlobalEntities.GARAGE_WEAPON_SHAFT, 0 },
            { GlobalEntities.GARAGE_WEAPON_SMOKY, 0 },
            { GlobalEntities.GARAGE_WEAPON_THUNDER, 0 },
            { GlobalEntities.GARAGE_WEAPON_TWINS, 0 },
            { GlobalEntities.GARAGE_WEAPON_VULCAN, 0 }
        };

        public Dictionary<Entity, long> TurretStatistics { get; set; } = new Dictionary<Entity, long>()
        {
            { GlobalEntities.GARAGE_TANK_WASP, 0 },
            { GlobalEntities.GARAGE_TANK_HORNET, 0 },
            { GlobalEntities.GARAGE_TANK_HUNTER, 0 },
            { GlobalEntities.GARAGE_TANK_VIKING, 0 },
            { GlobalEntities.GARAGE_TANK_TITAN, 0 },
            { GlobalEntities.GARAGE_TANK_DICTATOR, 0 },
            { GlobalEntities.GARAGE_TANK_MAMMOTH, 0 }
        };
    }
}
