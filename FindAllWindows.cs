using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tool_PieHotKey
{
    internal class FindAllWindows
    {
        public FindAllWindows()
        {
            ArrayList arrayList = GetWindows();
            var windowsList = new List<WindowInfo>();
            for (int i = 0; i < arrayList.Count; i++)
            {
                if (IsWindowVisible((IntPtr)arrayList[i]) && (GetParent((IntPtr)arrayList[i]) == IntPtr.Zero))//如果視窗是可見的
                                                                                                              //    if (IsWindowVisible((IntPtr)arrayList[i]))//如果視窗是可見的
                {
                    var lptrString = new StringBuilder(512);
                    GetWindowText((IntPtr)arrayList[i], lptrString, lptrString.Capacity);
                    var title = lptrString.ToString();
                    if (title.ToString() == "") { continue; }

                    windowsList.Add(new WindowInfo(title, (IntPtr)arrayList[i]));
                }
            }
            WindowList = windowsList;
        }
        public List<WindowInfo> WindowList { get; set; }
        public readonly struct WindowInfo
        {
            public WindowInfo(string _Title, IntPtr _Hwnd)
            {
                Title = _Title;
                Hwnd = _Hwnd;
            }
            public string Title { get; }
            public IntPtr Hwnd { get; }
        }

        public delegate bool EnumedWindow(IntPtr handleWindow, ArrayList handles);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool EnumWindows(EnumedWindow lpEnumFunc, ArrayList lParam);
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder ss, int count);
        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hwnd);
        [DllImport("user32.dll")]
        private static extern IntPtr GetParent(IntPtr hWnd);
        public static ArrayList GetWindows()
        {
            ArrayList windowHandles = new ArrayList();
            EnumedWindow callBackPtr = GetWindowHandle;
            EnumWindows(callBackPtr, windowHandles);

            return windowHandles;
        }

        private static bool GetWindowHandle(IntPtr windowHandle, ArrayList windowHandles)
        {
            windowHandles.Add(windowHandle);
            return true;
        }
    }
}
