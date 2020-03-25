using System.Collections.Generic;
using static TXServer.Core.ECSSystem.Entity;

namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1522236020112)]
    public class FavoriteEquipmentStatisticsComponent : Component
    {
        public Dictionary<long, long> HullStatistics { get; set; } = new Dictionary<long, long>()
        {
            { GlobalEntities.GARAGE_WEAPON_FLAMETHROWER.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_FREEZE.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_HAMMER.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_ISIS.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_RAILGUN.EntityId, 1 },
            { GlobalEntities.GARAGE_WEAPON_RICOCHET.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_SHAFT.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_SMOKY.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_THUNDER.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_TWINS.EntityId, 0 },
            { GlobalEntities.GARAGE_WEAPON_VULCAN.EntityId, 0 }
        };

        public Dictionary<long, long> TurretStatistics { get; set; } = new Dictionary<long, long>()
        {
            { GlobalEntities.GARAGE_TANK_WASP.EntityId, 0 },
            { GlobalEntities.GARAGE_TANK_HORNET.EntityId, 1 },
            { GlobalEntities.GARAGE_TANK_HUNTER.EntityId, 0 },
            { GlobalEntities.GARAGE_TANK_VIKING.EntityId, 0 },
            { GlobalEntities.GARAGE_TANK_TITAN.EntityId, 0 },
            { GlobalEntities.GARAGE_TANK_DICTATOR.EntityId, 0 },
            { GlobalEntities.GARAGE_TANK_MAMMOTH.EntityId, 0 }
        };
    }
}
