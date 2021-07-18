using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents.Experience
{
    [SerialVersionUID(1476865927439L)]
    public class UpgradeLevelsComponent : Component
    {
        public int[] LevelsExperiences { get; set; }
    }
}
