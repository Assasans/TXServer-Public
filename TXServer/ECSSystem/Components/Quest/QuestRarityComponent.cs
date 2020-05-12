using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1495190227237L)]
    public class QuestRarityComponent : Component
    {
        public byte /* enum */ RarityType { get; set; } = 0;
    }
}
