using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon.Hammer
{
    [SerialVersionUID(2388237143993578319L)]
    public class MagazineStorageComponent : Component
    {
        public MagazineStorageComponent(int currentCartridgeCount)
        {
            CurrentCartridgeCount = currentCartridgeCount;
        }

        public int CurrentCartridgeCount { get; set; }
    }
}
