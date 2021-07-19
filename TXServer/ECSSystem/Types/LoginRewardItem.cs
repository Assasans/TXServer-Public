namespace TXServer.ECSSystem.Types
{
    public class LoginRewardItem
    {
        public LoginRewardItem(int day, long marketItemEntity, int amount)
        {
            Day = day;
            MarketItemEntity = marketItemEntity;
            Amount = amount;
        }

        public int Day { get; set; }
        public long MarketItemEntity { get; set; }
        public int Amount { get; set; }
    }
}
