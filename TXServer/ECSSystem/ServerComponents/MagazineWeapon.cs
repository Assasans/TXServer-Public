using TXServer.ECSSystem.Components.Battle.Weapon;

namespace TXServer.ECSSystem.ServerComponents
{
    public static class MagazineWeapon
    {
        public class ReloadMagazineTimePropertyComponent : RangedComponent, IConvertibleComponent<MagazineWeaponComponent>
        {
            public void Convert(MagazineWeaponComponent component) => component.ReloadMagazineTimePerSec = 1 / FinalValue;
        }

        public class MagazineSizePropertyComponent : RangedComponent, IConvertibleComponent<MagazineWeaponComponent>, IConvertibleComponent<MagazineStorageComponent>
        {
            public void Convert(MagazineWeaponComponent component) => component.MaxCartridgeCount = (int)FinalValue;

            public void Convert(MagazineStorageComponent component) => component.CurrentCartridgeCount = (int)FinalValue;
        }
    }
}
