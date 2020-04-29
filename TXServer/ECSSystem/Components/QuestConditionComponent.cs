using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1493901546731L)]
    public class QuestConditionComponent : Component
    {
        public Dictionary<QuestConditionType, Entity> Condition { get; set; } = new Dictionary<QuestConditionType, Entity>();
    }
}
