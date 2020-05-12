using System;

namespace TXServer.ECSSystem.Types
{
    public class TXDate
    {
        public TXDate()
        {
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public TXDate(DateTimeOffset Time)
        {
            this.Time = Time.ToUnixTimeSeconds();
        }

        public TXDate(TimeSpan span)
        {
            Time = new DateTimeOffset(DateTimeOffset.Now.Ticks, span).ToUnixTimeSeconds();
        }

        public long Time { get; set; }
    }
}
