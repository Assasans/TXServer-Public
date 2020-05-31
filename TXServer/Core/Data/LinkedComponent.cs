using System;
using TXServer.ECSSystem.Base;

namespace TXServer.Core.Data.Database
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LinkedComponent : Attribute
    {
        public Component instance;
        public LinkedComponent(Component instance)
        {
            if (!instance.GetType().IsAssignableFrom(typeof(Component)))
            {
                throw new InvalidOperationException("ComponentType has to be a Component");
            }
            this.instance = instance;
        }
    }
}