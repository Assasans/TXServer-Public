using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1544956558339)]
    public class RestrictionByUserFractionComponent : Component
    {
        public long FractionId { get; set; }
    }
}
