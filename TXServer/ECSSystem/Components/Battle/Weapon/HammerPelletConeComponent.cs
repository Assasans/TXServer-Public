using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle.Weapon
{
    [SerialVersionUID(1464955716416L)]
    public class HammerPelletConeComponent : Component
    {
        public HammerPelletConeComponent(float horizontalConeHalfAngle, float verticalConeHalfAngle, int pelletCount)
        {
            HorizontalConeHalfAngle = horizontalConeHalfAngle;
            VerticalConeHalfAngle = verticalConeHalfAngle;
            PelletCount = pelletCount;
        }

        public float HorizontalConeHalfAngle { get; set; }
        public float VerticalConeHalfAngle { get; set; }
        public int PelletCount { get; set; }
    }
}