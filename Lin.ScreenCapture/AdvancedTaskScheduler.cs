public class AdvancedTaskScheduler : IDisposable
{
    private readonly PriorityQueue<ScheduledTask, DateTime> _taskQueue = new();
    private readonly object _queueLock = new();
    private readonly Thread _schedulerThread;
    private readonly ManualResetEventSlim _taskSignal = new(false);
    private bool _isDisposed;

    public AdvancedTaskScheduler()
    {
        _schedulerThread = new Thread(SchedulerLoop)
        {
            IsBackground = true,
            Name = "TaskScheduler"
        };
        _schedulerThread.Start();
    }

    public Guid ScheduleTask(Action action, DateTime executionTime, TimeSpan? interval = null)
    {
        var task = new ScheduledTask(
            action,
            executionTime.ToUniversalTime(),
            interval,
            Guid.NewGuid()
        );

        lock (_queueLock)
        {
            bool isEarliestTask =
                _taskQueue.Count == 0 || executionTime < _taskQueue.Peek().ExecutionTime;
            _taskQueue.Enqueue(task, task.ExecutionTime);

            if (isEarliestTask)
            {
                _taskSignal.Set();
            }
        }

        return task.Id;
    }

    public bool RemoveTask(Guid taskId)
    {
        lock (_queueLock)
        {
            // 由于PriorityQueue不支持直接删除，这里使用临时列表实现
            var tempList = new List<ScheduledTask>();
            bool found = false;

            while (_taskQueue.TryDequeue(out var task, out var priority))
            {
                if (task.Id == taskId)
                {
                    found = true;
                    break;
                }
                tempList.Add(task);
            }

            // 将未删除的任务重新入队
            foreach (var task in tempList)
            {
                _taskQueue.Enqueue(task, task.ExecutionTime);
            }

            if (found)
                _taskSignal.Set(); // 唤醒调度线程重新检查队列
            return found;
        }
    }

    private void SchedulerLoop()
    {
        while (!_isDisposed)
        {
            ScheduledTask? nextTask = null;
            DateTime nextExecution = DateTime.MaxValue;

            lock (_queueLock)
            {
                if (_taskQueue.TryPeek(out var task, out var executionTime))
                {
                    nextTask = task;
                    nextExecution = executionTime;
                }
            }

            if (nextTask == null)
            {
                _taskSignal.Wait();
                _taskSignal.Reset();
                continue;
            }

            var now = DateTime.UtcNow;
            var waitTime = nextExecution - now;

            if (waitTime <= TimeSpan.Zero)
            {
                HandleTaskExecution(nextTask);
            }
            else
            {
                _taskSignal.Wait(waitTime);
                _taskSignal.Reset();
            }
        }
    }

    private void HandleTaskExecution(ScheduledTask task)
    {
        lock (_queueLock)
        {
            if (_taskQueue.TryDequeue(out var dequeuedTask, out _) && dequeuedTask.Id == task.Id)
            {
                // 如果是周期性任务，重新计算下次执行时间
                if (task.Interval.HasValue)
                {
                    var newTask = new ScheduledTask(
                        task.Action,
                        task.ExecutionTime + task.Interval.Value,
                        task.Interval,
                        Guid.NewGuid()
                    );
                    _taskQueue.Enqueue(newTask, newTask.ExecutionTime);
                }
            }
        }

        // 使用线程池执行任务
        ThreadPool.UnsafeQueueUserWorkItem(
            _ =>
            {
                try
                {
                    task.Action();
                }
                catch (Exception ex)
                {
                    // 可在此处添加日志记录或错误处理
                    Console.WriteLine($"Task execution failed: {ex.Message}");
                }
            },
            null
        );
    }

    public void Dispose()
    {
        _isDisposed = true;
        _taskSignal.Set();
        _schedulerThread.Join();
        _taskSignal.Dispose();
        GC.SuppressFinalize(this);
    }

    private record ScheduledTask(
        Action Action,
        DateTime ExecutionTime,
        TimeSpan? Interval,
        Guid Id
    );
}
