using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(643487924561268453)]
    public class DiscountComponent : Component
    {
        public DiscountComponent(float DiscountCoeff)
        {
            this.DiscountCoeff = DiscountCoeff;
        }

        public float DiscountCoeff { get; set; }
    }
}
