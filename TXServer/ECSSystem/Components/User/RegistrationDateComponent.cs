using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1439270018242)]
    public class RegistrationDateComponent : Component
    {
        [OptionalMapped]
        public TXDate Date { get; set; } = null;
    }
}
