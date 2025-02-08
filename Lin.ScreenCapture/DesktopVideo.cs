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

        private TaskTimer start = null!;

        public void Start(int fps)
        {
            var time = 1000 / fps;
            start = new TaskTimer(CoreAction, time, OnError);
            start.Start();
        }

        public void Stop()
        {
            start.Stop();
        }
    }
}
