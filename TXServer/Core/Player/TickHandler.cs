using System;

namespace TXServer.Core
{
    public class TickHandler
    {
        public TickHandler(DateTimeOffset time, Action action)
        {
            Time = time;
            Action = action;
        }

        public DateTimeOffset Time { get; }
        public Action Action { get; }
    }
}
