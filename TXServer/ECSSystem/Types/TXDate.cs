using System;

namespace TXServer.ECSSystem.Types
{
    public class TXDate
    {
        public TXDate()
        {
            Time = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public TXDate(DateTimeOffset time)
        {
            Time = time.ToUnixTimeMilliseconds();
        }

        public TXDate(TimeSpan span)
        {
            Time = DateTimeOffset.UtcNow.AddTicks(span.Ticks).ToUnixTimeMilliseconds();
        }

        public long Time { get; set; }
    }
}
