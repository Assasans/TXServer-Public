using System.Collections.Generic;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(636174034381039231)]
    public class ItemsPackFromConfigComponent : Component
    {
        public List<long> Pack { get; set; } = new List<long>();
    }
}
