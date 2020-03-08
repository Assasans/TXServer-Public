using System;

namespace TXServer.Core.ECSSystem.Events
{
    public abstract class ECSEvent
    {
        public virtual void Execute(Entity entity)
        {
            throw new NotImplementedException();
        }
    }
}
