namespace Memorandum.UI
{
    using System;
    using System.Windows.Forms;

    public class ClipboardUpdatedEventArgs : EventArgs, IDisposable
    {
        private bool disposedValue;

        public ClipboardUpdatedEventArgs()
            : this(null, -1)
        {
        }

        public ClipboardUpdatedEventArgs(IDataObject dataObject, int clipboardSequenceNumber)
        {
            this.ClipboardSequenceNumber = clipboardSequenceNumber;
            this.DataObject = dataObject;
        }

        ~ClipboardUpdatedEventArgs()
        {
            this.Dispose(false);
        }

        public int ClipboardSequenceNumber { get; private set; }

        public IDataObject DataObject { get; private set; }

        public void Dispose()
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
                }

                this.DataObject = null;
                this.disposedValue = true;
            }
        }
    }
}