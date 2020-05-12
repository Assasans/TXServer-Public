using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1493196614850L)]
    public class QuestRewardComponent : Component
    {
        public QuestRewardComponent(Dictionary<Entity, int> Reward)
        {
            this.Reward = Reward;
        }

        public Dictionary<Entity, int> Reward { get; set; }
    }
}
