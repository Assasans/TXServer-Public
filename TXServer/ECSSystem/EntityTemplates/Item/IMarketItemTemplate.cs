using System;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    public interface IMarketItemTemplate : IEntityTemplate
    {
        Type UserItemType { get; }
    }
}
