﻿using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1439270018242)]
    public class RegistrationDateComponent : Component
    {
        [OptionalMapped]
        public DateTime? Date { get; set; } = null;
    }
}
