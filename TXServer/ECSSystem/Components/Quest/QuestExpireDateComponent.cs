using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1476707093577L)]
    public class QuestExpireDateComponent : Component
    {
        public TXDate Date { get; set; } = new TXDate(new TimeSpan(6, 0, 0));
    }
}
