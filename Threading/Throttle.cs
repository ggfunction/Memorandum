namespace Memorandum.Threading
{
    using System;
    using System.Threading;

    public sealed class Throttle<T>
    {
        private readonly Action<T> action;

        private readonly SynchronizationContext context;

        private readonly TimeSpan dueTime;

        private long count;

        public Throttle(Action<T> action, int dueTime)
            : this(action, TimeSpan.FromMilliseconds(dueTime), SynchronizationContext.Current)
        {
        }

        public Throttle(Action<T> action, int dueTime, SynchronizationContext context)
            : this(action, TimeSpan.FromMilliseconds(dueTime), context)
        {
        }

        public Throttle(Action<T> action, TimeSpan dueTime)
            : this(action, dueTime, SynchronizationContext.Current)
        {
        }

        public Throttle(Action<T> action, TimeSpan dueTime, SynchronizationContext context)
        {
            this.action = action;
            this.dueTime = dueTime;
            this.context = context;
        }

        public void Push()
        {
            this.Push(default(T));
        }

        public void Push(T value)
        {
            var token = Interlocked.Increment(ref this.count);

            ThreadPool.QueueUserWorkItem(_ =>
            {
                Thread.Sleep(this.dueTime);

                var latest = Interlocked.Read(ref this.count);
                if (token == latest)
                {
                    if (this.context != null)
                    {
                        this.context.Post(state => this.action((T)state), value);
                    }
                    else
                    {
                        this.action(value);
                    }
                }
            });
        }
    }
}