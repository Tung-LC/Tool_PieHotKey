using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Tool_PieHotKey
{
    class GlobalMouseHookEventArgs : HandledEventArgs
    {
        public GlobalMouseHook.MouseState MouseState { get; private set; }
        public GlobalMouseHook.LowLevelMouseInputEvent MouseData { get; private set; }

        public GlobalMouseHookEventArgs(
            GlobalMouseHook.LowLevelMouseInputEvent mouseData,
            GlobalMouseHook.MouseState mouseState)
        {
            MouseData = mouseData;
            MouseState = mouseState;
        }
    }

    //Based on https://gist.github.com/Stasonix
    class GlobalMouseHook : IDisposable
    {
        public event EventHandler<GlobalMouseHookEventArgs> MousePressed;

        // EDT: Added an optional parameter (registeredKeys) that accepts keys to restict
        // the logging mechanism.
        /// <summary>
        /// 
        /// </summary>
        /// <param name="registeredKeys">Keys that should trigger logging. Pass null for full logging.</param>

        public GlobalMouseHook(Keys[] registeredKeys = null)
        //public GlobalMouseHook(Keys[] registeredKeys)
        {
            //Debug.WriteLine("進入DD");
            RegisteredKeys = registeredKeys;
            _windowsHookHandle = IntPtr.Zero;
            _user32LibraryHandle = IntPtr.Zero;
            _hookProc = LowLevelMouseProc; // we must keep alive _hookProc, because GC is not aware about SetWindowsHookEx behaviour.

            _user32LibraryHandle = LoadLibrary("User32");
            if (_user32LibraryHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to load library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }



            _windowsHookHandle = SetWindowsHookEx(WH_Mouse_LL, _hookProc, _user32LibraryHandle, 0);
            if (_windowsHookHandle == IntPtr.Zero)
            {
                int errorCode = Marshal.GetLastWin32Error();
                throw new Win32Exception(errorCode, $"Failed to adjust Mouse hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                // because we can unhook only in the same thread, not in garbage collector thread
                if (_windowsHookHandle != IntPtr.Zero)
                {
                    if (!UnhookWindowsHookEx(_windowsHookHandle))
                    {
                        int errorCode = Marshal.GetLastWin32Error();
                        throw new Win32Exception(errorCode, $"Failed to remove Mouse hooks for '{Process.GetCurrentProcess().ProcessName}'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                    }
                    _windowsHookHandle = IntPtr.Zero;

                    // ReSharper disable once DelegateSubtraction
                    _hookProc -= LowLevelMouseProc;
                }
            }

            if (_user32LibraryHandle != IntPtr.Zero)
            {
                if (!FreeLibrary(_user32LibraryHandle)) // reduces reference to library by 1.
                {
                    int errorCode = Marshal.GetLastWin32Error();
                    throw new Win32Exception(errorCode, $"Failed to unload library 'User32.dll'. Error {errorCode}: {new Win32Exception(Marshal.GetLastWin32Error()).Message}.");
                }
                _user32LibraryHandle = IntPtr.Zero;
            }
        }

        ~GlobalMouseHook()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private IntPtr _windowsHookHandle;
        private IntPtr _user32LibraryHandle;
        private static HookProc _hookProc;

        delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll")]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private static extern bool FreeLibrary(IntPtr hModule);

        /// <summary>
        /// The SetWindowsHookEx function installs an application-defined hook procedure into a hook chain.
        /// You would install a hook procedure to monitor the system for certain types of events. These events are
        /// associated either with a specific thread or with all threads in the same desktop as the calling thread.
        /// </summary>
        /// <param name="idHook">hook type</param>
        /// <param name="lpfn">hook procedure</param>
        /// <param name="hMod">handle to application instance</param>
        /// <param name="dwThreadId">thread identifier</param>
        /// <returns>If the function succeeds, the return value is the handle to the hook procedure.</returns>
        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hMod, int dwThreadId);

        /// <summary>
        /// The UnhookWindowsHookEx function removes a hook procedure installed in a hook chain by the SetWindowsHookEx function.
        /// </summary>
        /// <param name="hhk">handle to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        public static extern bool UnhookWindowsHookEx(IntPtr hHook);

        /// <summary>
        /// The CallNextHookEx function passes the hook information to the next hook procedure in the current hook chain.
        /// A hook procedure can call this function either before or after processing the hook information.
        /// </summary>
        /// <param name="hHook">handle to current hook</param>
        /// <param name="code">hook code passed to hook procedure</param>
        /// <param name="wParam">value passed to hook procedure</param>
        /// <param name="lParam">value passed to hook procedure</param>
        /// <returns>If the function succeeds, the return value is true.</returns>
        [DllImport("USER32", SetLastError = true)]
        static extern IntPtr CallNextHookEx(IntPtr hHook, int code, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        internal struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct LowLevelMouseInputEvent
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        public const int WH_Mouse_LL = 14;
        //const int HC_ACTION = 0;
        /// <summary>
        /// <param name="MouseState">說明滑鼠的操作種類 像是按下等</param>
        /// </summary>
        public enum MouseState
        {
            //MouseMove = 0x0200,
            MouseWheel = 0x020A,
            LButtonDown = 0x0201,
            LButtonUp = 0x0202,
            LButtonDblClk = 0x0203,
            RButtonDown = 0x0204,
            RButtonUp = 0x0205,
            RButtonDblClk = 0x0206,
            MButtonDown = 0x0207,
            MButtonUp = 0x0208,
            MButtonDblClk = 0x0209,
            XButtonDown = 0x020B,
            XButtonUp = 0x020C
        }

        // EDT: Replaced VkSnapshot(int) with RegisteredKeys(Keys[])
        public static Keys[] RegisteredKeys;
        const int KfAltdown = 0x2000;
        public const int LlkhfAltdown = (KfAltdown >> 8);
        public IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam)
        {


            bool fEatKeyStroke = false;
            //Debug.WriteLine(wParam.ToInt32());
            var wparamTyped = wParam.ToInt32();
            if (Enum.IsDefined(typeof(MouseState), wparamTyped))
            {
                object o = Marshal.PtrToStructure(lParam, typeof(LowLevelMouseInputEvent));
                LowLevelMouseInputEvent p = (LowLevelMouseInputEvent)o;
                //Debug.WriteLine(p.pt.x + " ," + p.pt.y);
                //Debug.WriteLine(p.pt.x + " " + p.pt.y +" "+ p.mouseData.ToString());
                //Debug.WriteLine(wParam.ToString("X"));
                var eventArguments = new GlobalMouseHookEventArgs(p, (MouseState)wparamTyped);
                //Debug.WriteLine(p.flags);
                // EDT: Removed the comparison-logic from the usage-area so the user does not need to mess around with it.
                // Either the incoming key has to be part of RegisteredKeys (see constructor on top) or RegisterdKeys
                // has to be null for the event to get fired.

                //var key = (Keys)p.mouseData;
                //if (RegisteredKeys == null || RegisteredKeys.Contains(key))
                {
                    EventHandler<GlobalMouseHookEventArgs> handler = MousePressed;
                    handler?.Invoke(this, eventArguments);

                    fEatKeyStroke = eventArguments.Handled;
                }
            }
            try
            {
                return fEatKeyStroke ? (IntPtr)1 : CallNextHookEx(IntPtr.Zero, nCode, wParam, lParam);
            }
            catch (Exception)
            {
                MessageBox.Show("GC collection error");
                return (IntPtr)1;
            }

        }
    }
}
