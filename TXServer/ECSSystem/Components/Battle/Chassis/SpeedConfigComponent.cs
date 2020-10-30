using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Chassis
{
    [SerialVersionUID(-177474741853856725L)]
    public class SpeedConfigComponent : Component
    {
        public SpeedConfigComponent(float turnAcceleration, float sideAcceleration, float reverseAcceleration, float reverseTurnAcceleration)
        {
            ReverseAcceleration = reverseAcceleration;
            ReverseTurnAcceleration = reverseTurnAcceleration;
            SideAcceleration = sideAcceleration;
            TurnAcceleration = turnAcceleration;
        }

        public float ReverseAcceleration { get; set; }

        public float ReverseTurnAcceleration { get; set; }

        public float SideAcceleration { get; set; }

        public float TurnAcceleration { get; set; }
    }
}