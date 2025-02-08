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
            var token = _tokenSource.Token;
            try
            {
                Task.Run(() =>
                {
                    while (!token.IsCancellationRequested)
                    {
                        Task.Delay(_timer, token)
                            .ContinueWith(t =>
                            {
                                if (t.IsCompleted)
                                {
                                    Task.Run(_action.Invoke, token);
                                }
                                if (t.Exception is not null)
                                {
                                    ErrorHandler?.Invoke(t.Exception);
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
