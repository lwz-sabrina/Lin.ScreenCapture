using System;
using System.Drawing;
using System.Threading;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public class DesktopVideo : IDisposable
    {
        public Action<SKBitmap>? OnFarme;
        private readonly Desktop Desktop = new Desktop();
        private readonly CancellationTokenSource cancellationTokenSource =
            new CancellationTokenSource();
        public int Width => Desktop.Width;
        public int Height => Desktop.Height;
        public Size Size => Desktop.Size;
        public double Scale => Desktop.Scale;

        public void ScaleSize(double scale) => Desktop.ScaleSize(scale);

        public void Dispose()
        {
            cancellationTokenSource.Dispose();
            Desktop.Dispose();
        }

        private void GcAction()
        {
            GC.Collect();
        }

        public void Start(int fps)
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                cancellationTokenSource.TryReset();
            }
            foreach (var bitmap in GetBitmaps(fps, cancellationTokenSource.Token))
            {
                OnFarme?.Invoke(bitmap);
            }
        }

        public void Stop()
        {
            cancellationTokenSource.Cancel();
        }

        public IEnumerable<SKBitmap> GetBitmaps(
            int fps,
            CancellationToken cancellationToken = default
        )
        {
            int index = 0;
            var time = TimeSpan.FromMilliseconds(1000 / fps);
            while (!cancellationToken.IsCancellationRequested)
            {
                var bitmap = Desktop.GetSKBitmap();
                yield return bitmap;
                index++;
                if (index == fps)
                {
                    GcAction();
                    index = 0;
                }
                bitmap.Dispose();
                Thread.Sleep(time);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}
