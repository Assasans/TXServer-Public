﻿using System;
using TXServer.Core;
using TXServer.Core.Logging;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Events
{
    [SerialVersionUID(1453867134827)]
    public class EnterScreenEvent : ECSEvent
    {
        public string Screen { get; set; }

        public void Execute(Player player, Entity entity)
        {
            Logger.Debug($"{player}: Entering screen {Screen}");
        }
    }
}
