using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1496906087610)]
    public class UserEquipmentComponent : Component
    {
        public UserEquipmentComponent(long weaponId, long hullId)
        {
            WeaponId = weaponId;
            HullId = hullId;
        }
        
        public long WeaponId { get; set; }
        
        public long HullId { get; set; }
    }
}