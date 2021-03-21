using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events.Battle
{
    [SerialVersionUID(1447764683298L)]
    public class SelfTankExplosionEvent : ECSEvent
    {
        public bool CanDetachWeapon { get; set; } = true;
    }
}