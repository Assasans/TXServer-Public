﻿using System;
using TXServer.Core.Protocol;
using TXServer.ECSSystem.Base;

namespace TXServer.ECSSystem.Components
{
    [SerialVersionUID(1504598280798L)]
    public class ExpireDateComponent : Component
    {
        public TXDate Date { get; set; } = new TXDate(new TimeSpan(6, 0, 0));
    }
}
