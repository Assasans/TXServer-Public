using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1493974995307L)]
	public class PresetNameComponent : Component
	{
        public PresetNameComponent(string Name)
        {
            this.Name = Name;
        }

        public string Name { get; set; }
	}
}
