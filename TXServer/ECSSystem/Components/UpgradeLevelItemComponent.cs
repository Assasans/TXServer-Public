using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1436343541876L)]
    public class UpgradeLevelItemComponent : Component
    {
        public int Level { get; set; } = 10;
    }
}
