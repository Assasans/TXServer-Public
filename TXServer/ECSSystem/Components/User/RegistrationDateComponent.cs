using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components.User
{
    [SerialVersionUID(1439270018242)]
    public class RegistrationDateComponent : Component
    {
        public RegistrationDateComponent(DateTimeOffset date) {}

        [OptionalMapped] public DateTime? Date { get; set; } = null;
    }
}
