using System.IO;
using static TXServer.Core.ECSSystem.EntityTemplates;

namespace TXServer.Core.ECSSystem
{
    public class TemplateAccessor
    {
        [Protocol] public IEntityTemplate Template { get; set; }
        [Protocol] public string ConfigPath { get; set; }

        public TemplateAccessor(IEntityTemplate Template, string ConfigPath)
        {
            this.Template = Template;
            this.ConfigPath = ConfigPath;
        }

        public void Wrap(BinaryWriter writer)
        {
            writer.Write(SerialVersionUIDTools.GetId(Template.GetType()));
            writer.Write(ConfigPath);
        }
    }
}
