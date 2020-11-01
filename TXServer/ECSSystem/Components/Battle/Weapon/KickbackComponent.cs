using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1437989437781L)]
    public class KickbackComponent : Component
    {
        public KickbackComponent(float kickbackForce)
        {
            KickbackForce = kickbackForce;
        }

        public float KickbackForce { get; set; }
    }
}