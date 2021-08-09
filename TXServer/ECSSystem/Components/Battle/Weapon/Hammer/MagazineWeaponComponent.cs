using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon.Hammer
{
    [SerialVersionUID(4355651182908057733L)]
    public class MagazineWeaponComponent : Component
    {
        public MagazineWeaponComponent(int maxCartridgeCount, float reloadMagazineTimePerSec)
        {
            MaxCartridgeCount = maxCartridgeCount;
            ReloadMagazineTimePerSec = reloadMagazineTimePerSec;
        }

        public int MaxCartridgeCount { get; set; }

        public float ReloadMagazineTimePerSec { get; set; }
    }
}
