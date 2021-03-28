using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components.Battle
{
    [SerialVersionUID(5192591761194414739L)]
    public class DurationComponent : Component
    {
        public DurationComponent(TXDate startedTime)
        {
            StartedTime = startedTime;
        }

        public TXDate StartedTime { get; set; }
    }
}