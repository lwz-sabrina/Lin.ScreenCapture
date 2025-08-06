using System.Drawing;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public partial class Desktop : IDisposable
    {
        private const double DefultScale = 1;
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size => new Size(Width, Height);
        public double Scale { get; private set; } = DefultScale;

        public Desktop()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                Init_Windwos();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Init_Linux();
            }
            else
            {
                throw new PlatformNotSupportedException("不支持的操作系统");
            }
        }

        public void Dispose()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) { }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Dispose_Linux();
            }
            else
            {
                throw new PlatformNotSupportedException("不支持的操作系统");
            }
            GC.SuppressFinalize(this);
        }

        public SKBitmap GetSKBitmap()
        {
            SKBitmap bitmap;
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                bitmap = GetSKBitmap_Windwos();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                bitmap = GetSKBitmap_Linux();
            }
            else
            {
                throw new PlatformNotSupportedException("不支持的操作系统");
            }
            int ScaleWith = (int)(Width * Scale);
            int ScaleHeight = (int)(Height * Scale);
            var resizedBitmap = bitmap.Resize(
                new SKSizeI(ScaleWith, ScaleHeight),
                SKSamplingOptions.Default
            );
            bitmap.Dispose();
            return resizedBitmap;
        }

        public void ScaleSize(double scale) => Scale = scale;

        ~Desktop()
        {
            Dispose();
        }
    }
}
