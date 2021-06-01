using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;

namespace TXServer.Core.HeightMaps
{
    public class HeightMapLayer
    {
        public Image<Rgb24> Image { get; set; }

        public string Path { get; set; }

        public float MaxY { get; set; }
        public float MinY { get; set; }
    }
}
