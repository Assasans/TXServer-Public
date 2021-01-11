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

        public override string ToString()
        {
            return $"[{Template.GetType().Name}, \"{ConfigPath}\"]";
        }

        [ProtocolFixed] public IEntityTemplate Template { get; set; }
        [ProtocolFixed][OptionalMapped] public string ConfigPath { get; set; }
    }
}
