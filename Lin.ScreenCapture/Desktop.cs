using System.Drawing;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public partial class Desktop : IDisposable
    {
        public const double DefultScale = 1;
        private int width;
        private int height;
        public int Width => (int)(width * Scale);
        public int Height => (int)(height * Scale);
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
            GC.Collect();
            GC.WaitForPendingFinalizers();
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
            if (Scale == DefultScale)
            {
                return bitmap;
            }
            else
            {
                var resizedBitmap = bitmap.Resize(
                    new SKSizeI(Width, Height),
                    SKSamplingOptions.Default
                );
                bitmap.Dispose(); // 释放原始的SKBitmap
                return resizedBitmap;
            }
        }

        public void ScaleSize(double scale) => Scale = scale;
    }
}
