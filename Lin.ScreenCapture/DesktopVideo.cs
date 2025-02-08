using System.Drawing;
using SkiaSharp;

namespace Lin.ScreenCapture
{
    public class DesktopVideo : IDisposable
    {
        public Action<SKBitmap>? OnFarme;
        private readonly Desktop Desktop = new Desktop();
        private readonly AdvancedTaskScheduler _Scheduler = new AdvancedTaskScheduler();
        public int Width => Desktop.Width;
        public int Height => Desktop.Height;
        public Size Size => Desktop.Size;
        public double Scale => Desktop.Scale;

        public void Dispose()
        {
            Desktop.Dispose();
            _Scheduler.Dispose();
        }

        private void GcAction()
        {
            GC.Collect();
        }

        private void CoreAction()
        {
            var bitmap = Desktop.GetSKBitmap();
            OnFarme?.Invoke(bitmap);
            bitmap.Dispose();
        }

        private Guid? Core_Guid;
        private Guid? GC_Guid;

        public void Start(int fps)
        {
            var time = TimeSpan.FromMilliseconds(1000 / fps);
            Core_Guid = _Scheduler.ScheduleTask(CoreAction, System.DateTime.Now, time);
            GC_Guid = _Scheduler.ScheduleTask(
                GcAction,
                System.DateTime.Now,
                TimeSpan.FromMilliseconds(1000)
            );
        }

        public void Stop()
        {
            if (Core_Guid.HasValue)
                _Scheduler.RemoveTask(Core_Guid.Value);
            if (GC_Guid.HasValue)
                _Scheduler.RemoveTask(GC_Guid.Value);
        }

        public void ScaleSize(double scale) => Desktop.ScaleSize(scale);
    }
}
