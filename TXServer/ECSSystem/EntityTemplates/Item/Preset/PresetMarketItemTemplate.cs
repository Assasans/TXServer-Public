using System;
using TXServer.Core.Protocol;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1493972656490L)]
    public class PresetMarketItemTemplate : IMarketItemTemplate
    {
        public Type UserItemType => typeof(PresetUserItemTemplate);
    }
}
