using System;

namespace TXServer.ECSSystem.Base
{
    public class TXDate
    {
        public TXDate(DateTimeOffset Time)
        {
            this.Time = Time.ToUnixTimeSeconds();
        }

        public long Time { get; set; }
    }
}
