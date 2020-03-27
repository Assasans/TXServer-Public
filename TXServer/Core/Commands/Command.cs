using System;

namespace TXServer.Core.Commands
{
    public abstract class Command
    {
        public virtual void OnSend()
        {
            throw new NotImplementedException();
        }

        public virtual void OnReceive()
        {
            throw new NotImplementedException();
        }
    }
}
