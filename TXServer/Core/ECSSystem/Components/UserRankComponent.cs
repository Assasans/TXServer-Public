namespace TXServer.Core.ECSSystem.Components
{
    [SerialVersionUID(-1413405458500615976)]
    public class UserRankComponent : Component
    {
        public UserRankComponent(int Rank)
        {
            this.Rank = Rank;
        }

        public int Rank { get; set; }
    }
}
