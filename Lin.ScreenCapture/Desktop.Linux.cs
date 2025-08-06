using System.Runtime.InteropServices;
using SkiaSharp;
using X11;

namespace Lin.ScreenCapture
{
    partial class Desktop
    {
        private class SafeDisplay : SafeHandle
        {
            private SafeDisplay(nint invalidHandleValue, bool ownsHandle)
                : base(invalidHandleValue, ownsHandle) { }

            public SafeDisplay()
                : this(default, true)
            {
                handle = Xlib.XOpenDisplay(null);
            }

            public static readonly SafeDisplay Null = new(default, false) { handle = IntPtr.Zero };
            public override bool IsInvalid => handle == IntPtr.Zero;

            protected override bool ReleaseHandle()
            {
                try
                {
                    Xlib.XCloseDisplay(handle);
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        private SafeDisplay Display = SafeDisplay.Null;
        private Window RootWindow;

        private void Init_Linux()
        {
            Display = new SafeDisplay();
            if (Display.IsInvalid)
            {
                throw new X11Exception("无法打开显示器");
            }
            int screen = Xlib.XDefaultScreen(Display.DangerousGetHandle());
            RootWindow = Xlib.XRootWindow(Display.DangerousGetHandle(), screen);
            XWindowAttributes windowAttributes;
            Xlib.XGetWindowAttributes(
                Display.DangerousGetHandle(),
                RootWindow,
                out windowAttributes
            );
            Width = (int)windowAttributes.width;
            Height = (int)windowAttributes.height;
        }

        private SKBitmap GetSKBitmap_Linux()
        {
            // 获取屏幕图像
            XImage image = Xlib.XGetImage(
                Display.DangerousGetHandle(),
                RootWindow,
                0,
                0,
                (uint)Width,
                (uint)Height,
                0,
                PixmapFormat.ZPixmap
            );
            SKBitmap bitmap = new SKBitmap(Width, Height, SKColorType.Bgra8888, SKAlphaType.Premul);
            IntPtr pixels = bitmap.GetPixels();
            for (int y = 0; y < Height; y++)
            {
                for (int x = 0; x < Width; x++)
                {
                    int pixel = Marshal.ReadInt32(image.data, y * image.bytes_per_line + x * 4);
                    Marshal.WriteInt32(pixels, (y * Width + x) * 4, pixel);
                }
            }
            return bitmap;
        }

        private void Dispose_Linux()
        {
            Display.Dispose();
        }

        private class X11Exception : Exception
        {
            public X11Exception(string? message)
                : base(message) { }
        }
    }
}
