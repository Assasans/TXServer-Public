using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;
using TXServer.ECSSystem.Types;
using System;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1498569137147L)]
    public class ClientBattleParamsComponent : Component
    {
        public ClientBattleParamsComponent(ClientBattleParams customParams)
        {
            Params = customParams;
        }
        public ClientBattleParams Params { get; set; }
    }
}
