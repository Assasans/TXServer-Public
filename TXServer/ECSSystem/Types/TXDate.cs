using System;

namespace TXServer.ECSSystem.Types
{
    public class TXDate
    {
        public TXDate()
        {
            Time = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        public TXDate(DateTimeOffset time)
        {
            Time = time.ToUnixTimeMilliseconds();
        }

        public TXDate(TimeSpan span)
        {
            Time = new DateTimeOffset(DateTimeOffset.Now.Ticks, span).ToUnixTimeMilliseconds();
        }

        public long Time { get; set; }
    }
}
