using System.Collections.Generic;

namespace TXServer.Core.HeightMaps
{
    public class HeightMap
    {
        public float MinX { get; set; }
        public float MaxX { get; set; }

        public float MinZ { get; set; }
        public float MaxZ { get; set; }

        public List<HeightMapLayer> Layers { get; set; }
    }
}
