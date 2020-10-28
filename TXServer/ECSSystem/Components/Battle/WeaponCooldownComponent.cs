using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(7115193786389139467)]
    public class WeaponCooldownComponent : Component
    {
        public WeaponCooldownComponent(float cooldownIntervalSec)
        {
            CooldownIntervalSec = cooldownIntervalSec;
        }
        
        public float CooldownIntervalSec { get; set; }
    }
}