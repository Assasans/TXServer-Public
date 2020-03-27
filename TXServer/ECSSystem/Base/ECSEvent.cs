using System;

namespace TXServer.ECSSystem.Base
{
    public abstract class ECSEvent
    {
        public virtual void Execute(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
