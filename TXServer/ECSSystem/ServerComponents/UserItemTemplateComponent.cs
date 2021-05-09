using System;
using System.Linq;
using System.Reflection;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.ServerComponents
{
    public class UserItemTemplateComponent : Component
    {
        public long UserItemTemplate
        {
            get => _userItemTemplate;
            set
            {
                _userItemTemplate = value;
                _entityTemplate = (IEntityTemplate)Activator.CreateInstance(
                    Assembly.GetExecutingAssembly()
                        .GetTypes()
                        .First(type => type.GetCustomAttribute<SerialVersionUIDAttribute>()?.Id == value));
            }
        }
        private long _userItemTemplate;

        public IEntityTemplate EntityTemplate => _entityTemplate;
        private IEntityTemplate _entityTemplate;
    }
}
