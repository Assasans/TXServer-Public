using System.Runtime.InteropServices;
using TXServer.Core.ECSSystem.EntityTemplates;

namespace TXServer.Core.ECSSystem
{
    [StructLayout(LayoutKind.Sequential)]
    public class TemplateAccessor
    {
        public TemplateAccessor(IEntityTemplate Template, string ConfigPath)
        {
            this.Template = Template;
            this.ConfigPath = ConfigPath;
        }

        [Protocol] public IEntityTemplate Template { get; set; }
        [Protocol] public string ConfigPath { get; set; }
    }
}
