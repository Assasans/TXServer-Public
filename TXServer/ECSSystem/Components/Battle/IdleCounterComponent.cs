using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(2930474294118078222)]
    public class IdleCounterComponent : Component
    {
        public IdleCounterComponent(long skippedMillis)
        {
            SkippedMillis = skippedMillis;
        }
        
        [OptionalMapped]
        public long SkipBeginDate { get; set; }
        
        public long SkippedMillis { get; set; }
    }
}