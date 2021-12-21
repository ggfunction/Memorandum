namespace Memorandum.Threading
{
    using System;
    using System.Threading;

    public static class Delay
    {
        public static WaitHandle Invoke(Action<object> action, object state, DateTime onTime)
        {
            return Delay.Invoke(action, state, onTime, SynchronizationContext.Current);
        }

        public static WaitHandle Invoke(Action<object> action, object state, DateTime onTime, SynchronizationContext synchronizationContext)
        {
            return Delay.Invoke(action, state, onTime - DateTime.Now, synchronizationContext);
        }

        public static WaitHandle Invoke(Action<object> action, object state, TimeSpan dueTime)
        {
            return Delay.Invoke(action, state, dueTime, SynchronizationContext.Current);
        }

        public static WaitHandle Invoke(Action<object> action, object state, TimeSpan dueTime, SynchronizationContext synchronizationContext)
        {
            return Delay.Invoke(action, state, (int)dueTime.TotalMilliseconds, synchronizationContext);
        }

        public static WaitHandle Invoke(Action<object> action, object state, int dueTime)
        {
            return Delay.Invoke(action, state, dueTime, SynchronizationContext.Current);
        }

        public static WaitHandle Invoke(Action<object> action, object state, int dueTime, SynchronizationContext synchronizationContext)
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            var startTime = DateTime.Now;

            ThreadPool.QueueUserWorkItem(
                _s1 =>
            {
                dueTime = Math.Max(
                    Math.Min(
                        (int)(startTime.AddMilliseconds(dueTime) - DateTime.Now).TotalMilliseconds,
                        dueTime),
                    0);

                using (var timer = new Timer(_s2 => waitHandle.Set(), null, dueTime, Timeout.Infinite))
                {
                    waitHandle.WaitOne();
                    action.InvokeIfNeeded(state, synchronizationContext);
                }
            });
            return waitHandle;
        }

        public static WaitHandle Signal(DateTime onTime)
        {
            return Delay.Signal(onTime - DateTime.Now);
        }

        public static WaitHandle Signal(TimeSpan dueTime)
        {
            return Delay.Signal((int)dueTime.TotalMilliseconds);
        }

        public static WaitHandle Signal(int dueTime)
        {
            return Delay.Invoke(null, null, dueTime);
        }

        public static void Sleep(DateTime onTime)
        {
            Delay.Signal(onTime).WaitOne();
        }

        public static void Sleep(TimeSpan dueTime)
        {
            Delay.Signal(dueTime).WaitOne();
        }

        public static void Sleep(int dueTime)
        {
            Delay.Signal(dueTime).WaitOne();
        }

        private static void InvokeIfNeeded(this Action<object> action, object state, SynchronizationContext synchronizationContext)
        {
            if (action != null)
            {
                if (synchronizationContext != null)
                {
                    synchronizationContext.Send(action.Invoke, state);
                }
                else
                {
                    action.Invoke(state);
                }
            }
        }
    }
}