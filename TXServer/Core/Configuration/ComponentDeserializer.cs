using System;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using YamlDotNet.Core;
using YamlDotNet.Core.Events;
using YamlDotNet.Serialization;

namespace TXServer.Core.Configuration
{
    public class ComponentDeserializer : INodeTypeResolver, INodeDeserializer
    {
        private Type _type;

        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object retValue)
        {
            if (!typeof(Component).IsAssignableFrom(expectedType))
            {
                retValue = null;
                return false;
            }

            Component component = (Component)FormatterServices.GetUninitializedObject(expectedType);

            reader.MoveNext();
            while (reader.Current != null && reader.Current is not MappingEnd)
            {
                if (reader.Current is not Scalar scalar) continue;

                string key = scalar.Value[0].ToString().ToUpper() + scalar.Value[1..];
                var info = expectedType.GetProperty(key);

                reader.MoveNext();
                if (info == null)
                {
                    reader.SkipThisAndNestedEvents();
                    continue;
                }

                object value = nestedObjectDeserializer(reader, info.PropertyType);
                info.SetValue(component, value);

                Logger.Trace($">> {key}: {value}");
            }
            reader.MoveNext();

            retValue = component;
            return true;
        }

        public bool Resolve(NodeEvent nodeEvent, ref Type currentType)
        {
            if (_type != null)
            {
                var type = _type;
                _type = null;

                if (nodeEvent is not MappingStart)
                    return false;

                Logger.Trace($"> {type}");
                currentType = type;
                return true;
            }

            if (nodeEvent is not Scalar scalar || scalar.Value.Length < 2 || !Regex.IsMatch(scalar.Value, @"^[a-zA-Z]+$")) return false;

            string typeName = $"{scalar.Value[0].ToString().ToUpper()}{scalar.Value[1..]}Component";
            var types = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(x => x.Name == typeName);

            Type resolvedType = types.FirstOrDefault(type => !Attribute.IsDefined(type, typeof(SerialVersionUIDAttribute))) ?? types.FirstOrDefault();
            if (resolvedType != null)
                _type = resolvedType;

            return false;
        }
    }
}
