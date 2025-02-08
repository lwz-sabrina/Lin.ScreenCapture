using System.Runtime.InteropServices;
using SkiaSharp;
using X11;

namespace Lin.ScreenCapture
{
    partial class Desktop
    {
        private IntPtr Display { get; set; }
        private Window RootWindow { get; set; }

        private void Init_Linux()
        {
            Display = Xlib.XOpenDisplay(null);
            if (Display == IntPtr.Zero)
            {
                throw new X11Exception("无法打开显示器");
            }
            int screen = Xlib.XDefaultScreen(Display);
            RootWindow = Xlib.XRootWindow(Display, screen);
            Xlib.XGetWindowAttributes(Display, RootWindow, out XWindowAttributes windowAttributes);
            this.Width = (int)windowAttributes.width;
            this.Height = (int)windowAttributes.height;
        }

        private SKBitmap GetSKBitmap_Linux()
        {
            XImage image = Xlib.XGetImage(
                Display,
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
            Xlib.XCloseDisplay(Display);
        }
    }
}