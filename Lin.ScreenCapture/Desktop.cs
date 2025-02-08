using System.Drawing;
using System.Runtime.InteropServices;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public partial class Desktop : IDisposable
    {
        public int Width { get; private set; }
        public int Height { get; private set; }
        public Size Size => new Size(Width, Height);

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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return GetSKBitmap_Windwos();
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return GetSKBitmap_Linux();
            }
            else
            {
                throw new PlatformNotSupportedException("不支持的操作系统");
            }
        }
    }
}