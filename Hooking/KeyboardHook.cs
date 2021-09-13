namespace Memorandum.Hooking
{
    using System;

    public class KeyboardHook : LowLevelHook
    {
        public KeyboardHook()
            : base(HookTypes.KeyboardLowLevel)
        {
        }

        public event EventHandler<KeyboardHookedEventArgs> KeyboardHooked;

        public override void Dispose()
        {
            base.Dispose();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }

        protected override int HookCallback(int code, IntPtr message, IntPtr state)
        {
            if (code >= 0)
            {
                var e = new KeyboardHookedEventArgs(message, state, this.GetForegroundWindow());

                if (!e.Injected)
                {
                    this.OnKeyboardHooked(e);
                    if (e.Cancel)
                    {
                        return -1;
                    }
                }
            }

            return base.HookCallback(code, message, state);
        }

        protected virtual void OnKeyboardHooked(KeyboardHookedEventArgs e)
        {
            if (this.KeyboardHooked != null)
            {
                this.KeyboardHooked(this, e);
            }
        }
    }
}