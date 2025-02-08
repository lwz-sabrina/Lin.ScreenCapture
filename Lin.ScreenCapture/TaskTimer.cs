using System.Diagnostics;

namespace Lin.ScreenCapture
{
    internal class TaskTimer
    {
        private readonly Action _action;

        private CancellationTokenSource? _tokenSource;
        private Action<Exception>? ErrorHandler;
        public int _timer { get; set; }
        private bool _running;

        public TaskTimer(Action action, int timer, Action<Exception>? error = null)
        {
            _action = action;
            _timer = timer;
            ErrorHandler = error;
        }

        public void Start()
        {
            if (_running)
                return;
            _running = true;
            _tokenSource = new CancellationTokenSource();
            try
            {
                Task.Run(() =>
                {
                    while (!_tokenSource.IsCancellationRequested)
                    {
                        Task.Delay(_timer, _tokenSource.Token)
                            .ContinueWith(t =>
                            {
                                if (t.IsCompleted)
                                {
                                    _action.Invoke();
                                }
                            });
                    }
                });
            }
            catch (Exception ex)
            {
                ErrorHandler?.Invoke(ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (!_tokenSource!.IsCancellationRequested && _running)
                {
                    _tokenSource.Cancel();
                }
            }
            finally
            {
                _running = false;
                _tokenSource?.Dispose();
            }
        }
    }
}