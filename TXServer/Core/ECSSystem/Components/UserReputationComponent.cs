namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(1502716170372)]
    public class UserReputationComponent : Component
    {
        public UserReputationComponent(double Reputation)
        {
            this.Reputation = Reputation;
        }

        public double Reputation { get; set; }
    }
}
