using System.Drawing;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public class DesktopVideo : IDisposable
    {
        public int Width => Desktop.Width;
        public int Height => Desktop.Height;
        public Size Size => Desktop.Size;
        public double Scale => Desktop.Scale;
        public Desktop Desktop { get; init; }

        public DesktopVideo(Desktop desktop)
        {
            Desktop = desktop;
        }

        public void ScaleSize(double scale) => Desktop.ScaleSize(scale);

        public IEnumerable<SKBitmap> GetBitmaps(
            int fps,
            CancellationToken cancellationToken = default
        )
        {
            int index = 0;
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
            while (!cancellationToken.IsCancellationRequested)
            {
                stopwatch.Restart();
                // 获取当前屏幕截图
                using (var bitmap = Desktop.GetSKBitmap())
                {
                    yield return bitmap;
                }
                index++;
                // 控制帧率
                int delay = Math.Max(0, (1000 / fps) - (int)stopwatch.ElapsedMilliseconds);
                Thread.Sleep(delay);
                if (index == fps)
                {
                    index = 0;
                }
            }
            stopwatch.Stop();
        }

        public void Dispose()
        {
            Desktop.Dispose();
        }

        ~DesktopVideo()
        {
            Dispose();
        }
    }
}
