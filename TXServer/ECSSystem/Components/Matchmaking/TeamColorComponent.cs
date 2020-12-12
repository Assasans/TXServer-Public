using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(6258344835131144773)]
    public class TeamColorComponent : Component
    {
        public TeamColorComponent(TeamColor color)
        {
            TeamColor = color;
        }
        
        public TeamColor TeamColor { get; set; }
    }
}