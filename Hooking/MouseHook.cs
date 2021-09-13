namespace Memorandum.Hooking
{
    using System;

    public class MouseHook : LowLevelHook
    {
        public MouseHook()
            : base(HookTypes.MouseLowLevel)
        {
        }

        public event EventHandler<MouseHookedEventArgs> MouseHooked;

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
                var e = new MouseHookedEventArgs(message, state, this.GetForegroundWindow());

                if (!e.Injected)
                {
                    this.OnMouseHooked(e);
                    if (e.Cancel)
                    {
                        return -1;
                    }
                }
            }

            return base.HookCallback(code, message, state);
        }

        protected virtual void OnMouseHooked(MouseHookedEventArgs e)
        {
            if (this.MouseHooked != null)
            {
                this.MouseHooked(this, e);
            }
        }
    }
}