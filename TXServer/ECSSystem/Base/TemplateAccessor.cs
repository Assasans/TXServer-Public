using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.Base
{
    public class TemplateAccessor
    {
        public TemplateAccessor(IEntityTemplate Template, string ConfigPath)
        {
            this.Template = Template;
            this.ConfigPath = ConfigPath;
        }

        [ProtocolFixed] public IEntityTemplate Template { get; set; }
        [ProtocolFixed] public string ConfigPath { get; set; }
    }
}
