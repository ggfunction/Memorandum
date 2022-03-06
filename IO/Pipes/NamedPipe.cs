namespace Memorandum.IO.Pipes
{
    using System;
    using System.Collections.Generic;
    using System.IO.Pipes;
    using System.Linq;
    using System.Threading;

    public class NamedPipe : IDisposable
    {
        private readonly object lockObject = new object();

        private readonly Semaphore semaphore;

        private readonly EventWaitHandle quitEvent;

        private bool disposedValue;

        private SynchronizationContext synchronizationContext;

        public NamedPipe()
            : this(string.Empty, SynchronizationContext.Current)
        {
        }

        public NamedPipe(string name)
            : this(name, SynchronizationContext.Current)
        {
        }

        public NamedPipe(SynchronizationContext context)
            : this(string.Empty, context)
        {
        }

        public NamedPipe(string name, SynchronizationContext context)
        {
            this.Name = string.IsNullOrEmpty(name) ?
                Guid.NewGuid().ToString() : name;

            this.semaphore = new Semaphore(1, 1, this.Name);
            this.IsPrimary = this.semaphore.WaitOne(0);
            this.synchronizationContext = context;
            this.quitEvent = new EventWaitHandle(false, EventResetMode.ManualReset);

            ThreadPool.QueueUserWorkItem(this.WaitCallback);
        }

        ~NamedPipe()
        {
            this.Dispose(false);
        }

        public event EventHandler<MessageReceivedEventArgs> MessageReceived;

        public event EventHandler PriorityChanged;

        public string Name { get; private set; }

        public bool IsPrimary { get; private set; }

        public static void SendTo(string text, System.Text.Encoding encoding, string serverName, string pipeName, int timeout)
        {
            var buffer = encoding.GetBytes(text);
            SendTo(buffer, 0, buffer.Length, serverName, pipeName, timeout);
        }

        public static void SendTo(System.IO.Stream stream, string serverName, string pipeName, int timeout)
        {
            var buffer = new byte[stream.Length];
            var count = stream.Read(buffer, 0, buffer.Length);
            SendTo(buffer, 0, count, serverName, pipeName, timeout);
        }

        public static void SendTo(byte[] buffer, int offset, int count, string serverName, string pipeName, int timeout)
        {
            using (var c = new NamedPipeClientStream(serverName, pipeName))
            {
                c.Connect(timeout);
                c.Write(buffer, offset, count);
                c.WaitForPipeDrain();
            }
        }

        public void SetSynchronizationContext(SynchronizationContext context)
        {
            lock (this.lockObject)
            {
                this.synchronizationContext = context;
            }
        }

        public void Send(string text)
        {
            this.Send(text, System.Text.Encoding.UTF8, Timeout.Infinite);
        }

        public void Send(string text, int timeout)
        {
            this.Send(text, System.Text.Encoding.UTF8, timeout);
        }

        public void Send(string text, System.Text.Encoding encoding, int timeout)
        {
            var buffer = encoding.GetBytes(text);
            this.Send(buffer, 0, buffer.Length, timeout);
        }

        public void Send(System.IO.Stream stream)
        {
            this.Send(stream, Timeout.Infinite);
        }

        public void Send(System.IO.Stream stream, int timeout)
        {
            var buffer = new byte[stream.Length];
            var count = stream.Read(buffer, 0, buffer.Length);
            this.Send(buffer, 0, count, timeout);
        }

        public void Send(byte[] buffer, int offset, int count)
        {
            this.Send(buffer, offset, count, Timeout.Infinite);
        }

        public void Send(byte[] buffer, int offset, int count, int timeout)
        {
            using (var c = new NamedPipeClientStream(".", this.Name))
            {
                c.Connect(timeout);
                c.Write(buffer, offset, count);
                c.WaitForPipeDrain();
            }
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected SynchronizationContext GetSynchronizationContext()
        {
            var context = default(SynchronizationContext);

            lock (this.lockObject)
            {
                context = this.synchronizationContext;
            }

            return context;
        }

        protected void Invoke(EventHandler handler, EventArgs e)
        {
            var context = this.GetSynchronizationContext();

            if (context != null)
            {
                this.synchronizationContext.Send(
                    o => handler.Invoke(this, (EventArgs)o), e);
            }
            else
            {
                handler.Invoke(this, e);
            }
        }

        protected void Invoke<T>(EventHandler<T> handler, T e)
            where T : EventArgs
        {
            var context = this.GetSynchronizationContext();

            if (context != null)
            {
                this.synchronizationContext.Send(
                    o => handler.Invoke(this, (T)o), e);
            }
            else
            {
                handler.Invoke(this, e);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.quitEvent.Set();
                    this.quitEvent.Close();

                    if (this.IsPrimary)
                    {
                        this.semaphore.Release();
                        this.IsPrimary = false;
                    }

                    this.semaphore.Close();
                }

                this.disposedValue = true;
            }
        }

        protected virtual void OnMessageReceived(MessageReceivedEventArgs e)
        {
            if (this.MessageReceived != null)
            {
                this.Invoke(
                    this.MessageReceived,
                    e);
            }
        }

        protected virtual void OnPriorityChanged(EventArgs e)
        {
            if (this.PriorityChanged != null)
            {
                this.Invoke(
                    this.PriorityChanged,
                    e);
            }
        }

        private WaitHandle BeginWaitForConnection()
        {
            var ar = default(IAsyncResult);
            var ss = default(NamedPipeServerStream);

            try
            {
                ss = new NamedPipeServerStream(
                    this.Name,
                    PipeDirection.InOut,
                    -1,
                    PipeTransmissionMode.Byte,
                    PipeOptions.Asynchronous);
                ar = ss.BeginWaitForConnection(
                    this.ConnectCallback,
                    ss);

                return ar.AsyncWaitHandle;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            throw new NotImplementedException();
        }

        private WaitHandle BeginWaitOne()
        {
            var waitHandle = new EventWaitHandle(false, EventResetMode.AutoReset);

            ThreadPool.QueueUserWorkItem(
                state =>
            {
                if (!this.IsPrimary)
                {
                    if (this.semaphore.WaitOne())
                    {
                        this.IsPrimary = true;
                        this.OnPriorityChanged(EventArgs.Empty);
                    }
                }

                ((EventWaitHandle)state).Set();
            },
                waitHandle);

            return waitHandle;
        }

        private void ConnectCallback(IAsyncResult ar)
        {
            var serverStream = ar.AsyncState as NamedPipeServerStream;
            var e = default(MessageReceivedEventArgs);

            try
            {
                serverStream.EndWaitForConnection(ar);
                e = new MessageReceivedEventArgs(serverStream);
                this.OnMessageReceived(e);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (e != null)
                {
                    e.Dispose();
                }

                serverStream.Dispose();
            }
        }

        private void WaitCallback(object state)
        {
            var requests = new HashSet<WaitHandle>
            {
                this.IsPrimary ?
                    this.BeginWaitForConnection() :
                    this.BeginWaitOne(),
            };

            while (true)
            {
                var waitHandles = new WaitHandle[] { this.quitEvent }
                    .Concat(requests)
                    .ToArray();
                var index = WaitHandle.WaitTimeout;
                bool exit;
                try
                {
                    index = WaitHandle.WaitAny(waitHandles);
                    exit = index == Array.IndexOf(waitHandles, this.quitEvent);
                }
                catch (ObjectDisposedException ex)
                {
                    exit = true;
                    Console.WriteLine(ex);
                }

                if (exit)
                {
                    break;
                }

                try
                {
                    requests.Remove(waitHandles[index]);
                    waitHandles[index].Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                requests.Add(this.BeginWaitForConnection());
            }
        }
    }
}