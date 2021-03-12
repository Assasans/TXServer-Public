﻿using System;
using TXServer.Core;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.EntityTemplates
{
    [SerialVersionUID(1496905821016)]
    public class SetEquipmentEvent : ECSEvent
    {
        public long WeaponId { get; set; }
        
        public long HullId { get; set; }
    }
}