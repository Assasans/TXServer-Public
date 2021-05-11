using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ICSharpCode.SharpZipLib.GZip;
using ICSharpCode.SharpZipLib.Tar;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.ServerComponents;
using YamlDotNet.Serialization;

namespace TXServer.Core.Configuration
{
    public static partial class Config
    {
        private const string _configPath = "StateServer/config/master-48606/en/config.tar.gz";

        private static Dictionary<(string, Type), Component> _cachedComponents = new();
        private static ConfigNode _rootNode = new();

        public static void Init()
        {
            Logger.Log("Initializing config...");

            _cachedComponents = new();
            _rootNode = new();

            var deserializer = new DeserializerBuilder()
                .WithNodeTypeResolver(new ComponentDeserializer())
                .WithNodeDeserializer(new ComponentDeserializer())
                .IgnoreUnmatchedProperties()
                .Build();

            Dictionary<string, object> components = new();
            Dictionary<string, long?> ids = new();

            // Read config file
            Logger.Debug("Reading config files...");
            TarInputStream stream = new(new GZipInputStream(new FileStream(_configPath, FileMode.Open)), Encoding.UTF8);
            while (true)
            {
                TarEntry entry = stream.GetNextEntry();
                if (entry == null) break;

                if (entry.Name.EndsWith("id.yml"))
                {
                    var reader = new StreamReader(stream);
                    reader.Read(new char[4]);
                    ids[entry.Name[0..^7]] = long.Parse(reader.ReadLine());
                }

                if (!entry.Name.EndsWith("public.yml")) continue;
                Logger.Trace($"Reading {entry.Name}...");
                components.Add(entry.Name[0..^11], deserializer.Deserialize(new StreamReader(stream)));
            }


            // Initialize nodes
            Logger.Debug("Preparing config nodes...");
            foreach (var pair in components)
            {
                if (pair.Value is not Dictionary<object, object> dict || !dict.Values.Any(x => x is Component)) continue;

                // Add nodes if need
                var currentNode = _rootNode;
                foreach (string part in pair.Key.Split('/'))
                {
                    if (currentNode.ChildNodes.ContainsKey(part))
                    {
                        currentNode = currentNode.ChildNodes[part];
                    }
                    else
                    {
                        ids.TryGetValue(pair.Key, out long? id);
                        ConfigNode node = new() { Id = id };

                        currentNode.ChildNodes.Add(part, node);
                        currentNode = node;
                    }
                }

                // Add components to nodes
                foreach (object obj in dict.Values)
                {
                    if (obj is not Component component) continue;

                    if (Attribute.IsDefined(component.GetType(), typeof(SerialVersionUIDAttribute)))
                        currentNode.Components.Add(component.GetType(), component);
                    else
                        currentNode.ServerComponents.Add(component.GetType(), component);
                }

                // Convert server-only components to shared components
                foreach (Component component in currentNode.ServerComponents.Values)
                {
                    foreach (Type type in component.GetType().GetInterfaces()
                        .Where(iType => iType.IsGenericType && iType.GetGenericTypeDefinition() == typeof(IConvertibleComponent<>)))
                    {
                        Type resultType = type.GetGenericArguments()[0];

                        currentNode.Components.TryGetValue(resultType, out Component resultComponent);
                        resultComponent ??= (Component)FormatterServices.GetUninitializedObject(resultType);

                        type.GetMethod("Convert").Invoke(component, new[] { resultComponent });

                        currentNode.Components.TryAdd(resultType, resultComponent);
                    }
                }
            }

            Logger.Log("Config is ready.");
        }

        private static ConfigNode GetNodeByPath(string path)
        {
            var currentNode = _rootNode;
            foreach (string part in path.Split('/'))
                currentNode = currentNode.ChildNodes[part];

            return currentNode;
        }

        public static T LoadComponent<T>(string path) where T : Component
        {
            var node = GetNodeByPath(path);
            if (!node.Components.TryGetValue(typeof(T), out Component component))
                component = node.ServerComponents[typeof(T)];

            return (T)component.Clone();
        }

        public static IEnumerable<string> GetSubPaths(string path)
        {
            throw new NotImplementedException();
        }
    }
}
