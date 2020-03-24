namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1544499423535)]
    public class FractionComponent : Component
    {
        public FractionComponent(string FractionName)
        {
            this.FractionName = FractionName;
        }

        [Protocol] public string FractionName { get; set; }
    }
}
