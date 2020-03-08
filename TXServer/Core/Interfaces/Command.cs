using System;

namespace TXServer.Core.Commands
{
    public abstract class Command
    {
        public virtual void BeforeWrap()
        {
            throw new NotImplementedException();
        }

        public virtual void AfterUnwrap()
        {
            throw new NotImplementedException();
        }
    }
}
