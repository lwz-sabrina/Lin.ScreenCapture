using System.Drawing;
using System.Drawing.Drawing2D;
using SkiaSharp;

#pragma warning disable CA1416

namespace Lin.ScreenCapture
{
    partial class Desktop
    {
        private void Init_Windwos()
        {
            using var _windowDc = Vanara.PInvoke.User32.GetDC();
            if (_windowDc.IsInvalid)
            {
                throw new InvalidOperationException("Failed to get DC");
            }
            this.Width = Vanara.PInvoke.Gdi32.GetDeviceCaps(
                _windowDc,
                Vanara.PInvoke.Gdi32.DeviceCap.HORZRES
            );
            this.Height = Vanara.PInvoke.Gdi32.GetDeviceCaps(
                _windowDc,
                Vanara.PInvoke.Gdi32.DeviceCap.VERTRES
            );
        }

        private SKBitmap GetSKBitmap_Windwos()
        {
            var info = new SKImageInfo(Width, Height);
            var skiaBitmap = new SKBitmap(info);
            using var pixmap = skiaBitmap.PeekPixels();
            using var bmp = new Bitmap(
                info.Width,
                info.Height,
                info.RowBytes,
                System.Drawing.Imaging.PixelFormat.Format32bppPArgb,
                pixmap.GetPixels()
            );
            using var graphics = Graphics.FromImage(bmp);
            graphics.CompositingMode = CompositingMode.SourceCopy;
            graphics.CompositingQuality = CompositingQuality.HighSpeed;
            graphics.InterpolationMode = InterpolationMode.Low;
            using var screenDC = Vanara.PInvoke.User32.GetDC();
            if (screenDC.IsInvalid)
            {
                throw new InvalidOperationException("Failed to get DC");
            }
            Vanara.PInvoke.HDC targetDC = graphics.GetHdc();
            try
            {
                var result = Vanara.PInvoke.Gdi32.BitBlt(
                    targetDC,
                    0,
                    0,
                    Width,
                    Height,
                    screenDC,
                    0,
                    0,
                    Vanara.PInvoke.Gdi32.RasterOperationMode.SRCCOPY
#if NET6_0
                        | Vanara.PInvoke.Gdi32.RasterOperationMode.CAPTUREBLT
#endif
                );
                if (!result)
                {
                    if (IsInActiveSession())
                    {
                        SwitchToDesktop();
                    }
                    else
                    {
                        throw new InvalidOperationException("当前会话无效");
                    }
                }
            }
            finally
            {
                if (!targetDC.IsNull)
                {
                    graphics.ReleaseHdc();
                }
            }
            return skiaBitmap;
        }

        private bool IsInActiveSession()
        {
            uint activeSession = Vanara.PInvoke.Kernel32.WTSGetActiveConsoleSessionId();
            uint currentSession;
            Vanara.PInvoke.Kernel32.ProcessIdToSessionId(
                (uint)System.Diagnostics.Process.GetCurrentProcess().Id,
                out currentSession
            );
            return activeSession == currentSession;
        }

        private Vanara.PInvoke.Win32Error SwitchToDesktop()
        {
            // 打开接收用户输入的桌面。
            using var inputDesktop = Vanara.PInvoke.User32.OpenInputDesktop(
                0,
                false,
                Vanara.PInvoke.ACCESS_MASK.GENERIC_ALL
            );
            if (inputDesktop.IsInvalid)
                return Vanara.PInvoke.Kernel32.GetLastError();
            // 获取桌面名称
            var intput = Vanara.PInvoke.User32.GetUserObjectInformation<string>(
                inputDesktop.DangerousGetHandle(),
                Vanara.PInvoke.User32.UserObjectInformationType.UOI_NAME
            );
            // 获取当前线程的桌面句柄
            var currentDesktop = Vanara.PInvoke.User32.GetThreadDesktop(
                Vanara.PInvoke.Kernel32.GetCurrentThreadId()
            );
            if (currentDesktop.IsInvalid)
                return Vanara.PInvoke.Kernel32.GetLastError();

            // 获取桌面名称
            var current = Vanara.PInvoke.User32.GetUserObjectInformation<string>(
                currentDesktop.DangerousGetHandle(),
                Vanara.PInvoke.User32.UserObjectInformationType.UOI_NAME
            );

            if (intput != current)
            {
                if (!Vanara.PInvoke.User32.SwitchDesktop(inputDesktop))
                    return Vanara.PInvoke.Kernel32.GetLastError();

                if (!Vanara.PInvoke.User32.SetThreadDesktop(inputDesktop))
                    return Vanara.PInvoke.Kernel32.GetLastError();
            }

            return Vanara.PInvoke.Win32Error.ERROR_SUCCESS;
        }
    }
}

#pragma warning restore CA1416
