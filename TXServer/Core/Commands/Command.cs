using System;

namespace TXServer.Core.Commands
{
    public abstract class Command
    {
        public virtual void OnSend(Player player)
        {
            throw new NotImplementedException();
        }

        public virtual void OnReceive(Player player)
        {
            throw new NotImplementedException();
        }
    }
}
