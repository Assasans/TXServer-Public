using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1544499423535)]
    public class FractionComponent : Component
    {
        public FractionComponent(string FractionName)
        {
            this.FractionName = FractionName;
        }

        public string FractionName { get; set; }
    }
}
