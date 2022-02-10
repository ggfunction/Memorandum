namespace Memorandum.IO.Pipes
{
    using System;
    using System.IO;

    public class MessageReceivedEventArgs : EventArgs, IDisposable
    {
        private bool disposedValue;

        public MessageReceivedEventArgs()
        {
            this.Content = new MemoryStream();
        }

        public MessageReceivedEventArgs(System.IO.Pipes.PipeStream stream)
            : this()
        {
            this.CopyStream(stream, this.Content);
            this.Content.Seek(0, 0);
        }

        ~MessageReceivedEventArgs()
        {
            this.Dispose(false);
        }

        public MemoryStream Content { get; private set; }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    this.Content.Dispose();
                }

                this.disposedValue = true;
            }
        }

        private void CopyStream(Stream source, Stream destination)
        {
            const int BufferSize = 1024;
            var buffer = new byte[BufferSize];
            int count;
            do
            {
                count = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, count);
            }
            while (count == BufferSize);
        }
    }
}