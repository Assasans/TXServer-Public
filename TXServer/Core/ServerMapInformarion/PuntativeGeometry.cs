﻿using System.Numerics;

namespace TXServer.Core.ServerMapInformation
{
    public class PuntativeGeometry
    {
        public int Number { get; set; }
        public Vector3 Position { get; set; }
        public Quaternion Rotation { get; set; }
        public Vector3 Size { get; set; }
        public Vector3 Center { get; set; }
    }
}
