using SkiaSharp;

namespace Lin.ScreenCapture
{
    public class DesktopVideo : IDisposable
    {
        private readonly Desktop _Desktop = new Desktop();
        public Action<SKBitmap>? OnFarme;

        public void Dispose()
        {
            Stop();
            _Desktop.Dispose();
        }

        private void OnError(Exception exception)
        {
            throw exception;
        }

        private void CoreAction()
        {
            var bitmap = _Desktop.GetSKBitmap();
            OnFarme?.Invoke(bitmap);
            bitmap.Dispose();
        }

        private void GcAction()
        {
            GC.Collect();
        }

        private TaskTimer start = null!;
        private TaskTimer gc = null!;

        public void Start(int fps)
        {
            var time = 1000 / fps;
            start = new TaskTimer(CoreAction, time, OnError);
            gc = new TaskTimer(GcAction, 1000, OnError);
            start.Start();
            gc.Start();
        }

        public void Stop()
        {
            start.Stop();
            gc.Stop();
        }
    }
}