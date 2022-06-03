using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Windows.Forms;
using System.Threading;

namespace Tool_PieHotKey
{

    public class SentInputData
    {
        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint SendInput(uint nInput, INPUT[] pInput, int cbSize);
        [DllImport("user32.dll")]
        private static extern UIntPtr GetMessageExtraInfo();
        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);
        /* 先收起來 以後要用到再拿出來
        public SentInputData(IntPtr Hwnd, Keys[] InputKey, bool IsInputByKeyBoard)
        {

            int InputKeyLength = InputKey.Length;
            string[] InputKeyStr = InputKey.Select(x => x.ToString().ToUpper()).ToArray();
            for (int i = InputKeyLength-1; i >0-1 ; i--)
            {
                if (InputKeyStr[i] == "NONE") { InputKeyStr[i].Remove(i); InputKeyLength--; }
            }
            if(InputKeyLength == 0) { return; }
            INPUT[] inputs = new INPUT[InputKeyLength * 2];
            if (IsInputByKeyBoard)//模擬鍵盤輸入
            {
                for (int i = 0; i < InputKeyLength; i++)
                {
                    inputs[i] = new INPUT()
                    {
                        type = (uint)InputType.Keyboard,
                        union = new InputUnion()
                        {
                            ki = new KEYBDINPUT()
                            {
                                wVk = (VirtualKeyShort)System.Enum.Parse(typeof(VirtualKeyShort), InputKeyStr[i]),
                                wScan = (ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKeyStr[i]),
                                dwFlags = KeyEventF.KeyDown,
                                dwExtraInfo = GetMessageExtraInfo()
                            }
                        }
                    };
                }
                for (int i = InputKeyLength; i < InputKeyLength * 2; i++)
                {
                    inputs[i] = new INPUT()
                    {
                        type = (uint)InputType.Keyboard,
                        union = new InputUnion()
                        {
                            ki = new KEYBDINPUT()
                            {
                                wVk = (VirtualKeyShort)System.Enum.Parse(typeof(VirtualKeyShort), InputKeyStr[i - InputKeyLength]),
                                wScan = (ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKeyStr[i - InputKeyLength]),
                                dwFlags = KeyEventF.KeyUp,
                                dwExtraInfo = GetMessageExtraInfo()                                
                            }
                        }
                    };
                }
            }
            Debug.WriteLine("進入SentMsg");
            SetForegroundWindow(Hwnd);
            SendInput((uint)inputs.Length, inputs, INPUT.size);
        }
        */
        /* 
         * 由於一次傳遞所有的按鈕，因此導致某些程式會沒感應到，因此修改寫法讓他變成可以有sleep(5)
        public SentInputData(IntPtr Hwnd, Keys[] InputKey, bool IsInputByKeyBoard)
        {

            int InputKeyLength = InputKey.Length;
            if (InputKey[0] == Keys.None && InputKey[1] == Keys.None && InputKey[2] == Keys.None) { return; }
            INPUT[] inputs = new INPUT[InputKeyLength * 2];
            try
            {
                if (IsInputByKeyBoard)//模擬鍵盤輸入
                {
                    for (int i = 0; i < InputKeyLength; i++)
                    {
                        inputs[i] = new INPUT()
                        {
                            type = (uint)InputType.Keyboard,
                            union = new InputUnion()
                            {
                                ki = new KEYBDINPUT()
                                {
                                    wVk = (ushort)InputKey[i],
                                    //(ushort)Convert.ToInt16(InputKey[0].ToString("X"),16)
                                    //wScan = (ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKeyStr[i]),
                                    dwFlags = KeyEventF.KeyDown,
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                            }
                        };
                    }
                    for (int i = InputKeyLength; i < InputKeyLength * 2; i++)
                    {
                        inputs[i] = new INPUT()
                        {
                            type = (uint)InputType.Keyboard,
                            union = new InputUnion()
                            {
                                ki = new KEYBDINPUT()
                                {
                                    //wVk = (VirtualKeyShort)System.Enum.Parse(typeof(VirtualKeyShort), InputKeyStr[i - InputKeyLength]),
                                    wVk = (ushort)InputKey[i - InputKeyLength],
                                    //wScan = (ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKeyStr[i - InputKeyLength]),
                                    dwFlags = KeyEventF.KeyUp,
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                            }
                        };
                    }
                }
            }
            catch (Exception) { MessageBox.Show("設定按鍵輸出失敗"); return; }
            Debug.WriteLine("進入SentMsg");
            SetForegroundWindow(Hwnd);
            SendInput((uint)inputs.Length, inputs, INPUT.size);
        }
        */
        public SentInputData(IntPtr Hwnd, Keys[] InputKey, bool IsInputByKeyBoard)
        {

            int InputKeyLength = InputKey.Length;
            if (InputKey[0] == Keys.None && InputKey[1] == Keys.None && InputKey[2] == Keys.None) { return; }
            //INPUT[] inputs = new INPUT[InputKeyLength * 2];
            //try
            {
                SetForegroundWindow(Hwnd);
                if (IsInputByKeyBoard)//模擬鍵盤輸入
                {
                    for (int i = 0; i < InputKeyLength; i++)
                    {
                        if(InputKey[i] == Keys.None) { continue; }
                        INPUT[] inputs = new INPUT[1];
                        inputs[0] = new INPUT()
                        {
                            type = (uint)InputType.Keyboard,
                            union = new InputUnion()
                            {
                                ki = new KEYBDINPUT()
                                {
                                    wVk = (ushort)InputKey[i],
                                    //(ushort)Convert.ToInt16(InputKey[0].ToString("X"),16)
                                    wScan = (short)((ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKey[i].ToString().ToUpper())),
                                    dwFlags = KeyEventF.KeyDown,
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                            }
                        };
                        SendInput(1, inputs, INPUT.size);Thread.Sleep(5);
                    }
                    for (int i = InputKeyLength-1; i > -1; i--)
                    {
                        if (InputKey[i] == Keys.None) { continue; }
                        INPUT[] inputs = new INPUT[1];
                        inputs[0] = new INPUT()
                        {
                            type = (uint)InputType.Keyboard,
                            union = new InputUnion()
                            {
                                ki = new KEYBDINPUT()
                                {
                                    //wVk = (VirtualKeyShort)System.Enum.Parse(typeof(VirtualKeyShort), InputKeyStr[i - InputKeyLength]),
                                    wVk = (ushort)InputKey[i],
                                    wScan = (short)((ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKey[i].ToString().ToUpper())),
                                    dwFlags = KeyEventF.KeyUp,
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                            }
                        };
                        SendInput(1, inputs, INPUT.size); Thread.Sleep(5);
                    }
                }
            }
            //catch (Exception) { MessageBox.Show("設定按鍵輸出失敗"); return; }
            Debug.WriteLine("進入SentMsg");
            //SetForegroundWindow(Hwnd);
            //SendInput((uint)inputs.Length, inputs, INPUT.size);
        }

    }
    [StructLayout(LayoutKind.Sequential)]
    public struct INPUT
    {
        internal uint type;
        internal InputUnion union;
        internal static int size
        {
            get { return Marshal.SizeOf(typeof(INPUT)); }
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct InputUnion
    {
        [FieldOffset(0)]
        internal MOUSEINPUT mi;
        [FieldOffset(0)]
        internal KEYBDINPUT ki;
        [FieldOffset(0)]
        internal HARDWAREINPUT hi;
    }


    //https://www.codeproject.com/Articles/5264831/How-to-Send-Inputs-using-Csharp
    [StructLayout(LayoutKind.Sequential)]
    internal struct KEYBDINPUT
    {
        //internal VirtualKeyShort wVk;
        //internal ScanCodeShort wScan;
        internal ushort wVk;
        internal short wScan;
        internal KeyEventF dwFlags;
        internal int time;
        internal UIntPtr dwExtraInfo;
    }
    internal struct MOUSEINPUT
    {
        internal int dx;
        internal int dy;
        internal int mouseData;
        internal MouseEventF dwFlags;
        internal uint time;
        internal UIntPtr dwExtraInfo;
    }
    [StructLayout(LayoutKind.Sequential)]
    internal struct HARDWAREINPUT
    {
        internal int uMsg;
        internal short wParamL;
        internal short wParamH;
    }
    [Flags]
    public enum InputType
    {
        Mouse = 0,
        Keyboard = 1,
        Hardware = 2
    }
    [Flags]
    public enum KeyEventF
    {
        KeyDown = 0x0000,
        ExtendedKey = 0x0001,
        KeyUp = 0x0002,
        Unicode = 0x0004,
        Scancode = 0x0008
    }
    [Flags]
    internal enum MouseEventF : uint
    {
        Absolute = 0x8000,
        HWheel = 0x01000,
        Move = 0x0001,
        MoveNoCoalesce = 0x2000,
        LeftDown = 0x0002,
        LeftUp = 0x0004,
        RightDown = 0x0008,
        RightUp = 0x0010,
        MiddleDown = 0x0020,
        MiddleUp = 0x0040,
        VirtualDesk = 0x4000,
        Wheel = 0x0800,
        XDown = 0x0080,
        XUp = 0x0100
    }
    internal enum ScanCodeShort : short
    {
        LBUTTON = 0,
        RBUTTON = 0,
        CANCEL = 70,
        MBUTTON = 0,
        XBUTTON1 = 0,
        XBUTTON2 = 0,
        BACK = 14,
        TAB = 15,
        CLEAR = 76,
        RETURN = 28,
        SHIFT = 42,
        SHIFTKEY = 42,   //自己額外加的
        CONTROLKEY = 29, //額外
        CONTROL = 29,
        MENU = 56,
        PAUSE = 0,
        CAPITAL = 58,
        KANA = 0,
        HANGUL = 0,
        JUNJA = 0,
        FINAL = 0,
        HANJA = 0,
        KANJI = 0,
        ESCAPE = 1,
        CONVERT = 0,
        NONCONVERT = 0,
        ACCEPT = 0,
        MODECHANGE = 0,
        SPACE = 57,
        PRIOR = 73,
        NEXT = 81,
        END = 79,
        HOME = 71,
        LEFT = 75,
        UP = 72,
        RIGHT = 77,
        DOWN = 80,
        SELECT = 0,
        PRINT = 0,
        EXECUTE = 0,
        SNAPSHOT = 84,
        INSERT = 82,
        DELETE = 83,
        HELP = 99,
        D0 = 11,
        D1 = 2,
        D2 = 3,
        D3 = 4,
        D4 = 5,
        D5 = 6,
        D6 = 7,
        D7 = 8,
        D8 = 9,
        D9 = 10,
        A = 30,
        B = 48,
        C = 46,
        D = 32,
        E = 18,
        F = 33,
        G = 34,
        H = 35,
        I = 23,
        J = 36,
        K = 37,
        L = 38,
        M = 50,
        N = 49,
        O = 24,
        P = 25,
        Q = 16,
        R = 19,
        S = 31,
        T = 20,
        U = 22,
        V = 47,
        W = 17,
        X = 45,
        Y = 21,
        Z = 44,
        LWIN = 91,
        RWIN = 92,
        APPS = 93,
        SLEEP = 95,
        NUMPAD0 = 82,
        NUMPAD1 = 79,
        NUMPAD2 = 80,
        NUMPAD3 = 81,
        NUMPAD4 = 75,
        NUMPAD5 = 76,
        NUMPAD6 = 77,
        NUMPAD7 = 71,
        NUMPAD8 = 72,
        NUMPAD9 = 73,
        MULTIPLY = 55,
        ADD = 78,
        SEPARATOR = 0,
        SUBTRACT = 74,
        DECIMAL = 83,
        DIVIDE = 53,
        F1 = 59,
        F2 = 60,
        F3 = 61,
        F4 = 62,
        F5 = 63,
        F6 = 64,
        F7 = 65,
        F8 = 66,
        F9 = 67,
        F10 = 68,
        F11 = 87,
        F12 = 88,
        F13 = 100,
        F14 = 101,
        F15 = 102,
        F16 = 103,
        F17 = 104,
        F18 = 105,
        F19 = 106,
        F20 = 107,
        F21 = 108,
        F22 = 109,
        F23 = 110,
        F24 = 118,
        NUMLOCK = 69,
        SCROLL = 70,
        LSHIFT = 42,
        RSHIFT = 54,
        LCONTROL = 29,
        RCONTROL = 29,
        LMENU = 56,
        RMENU = 56,
        BROWSER_BACK = 106,
        BROWSER_FORWARD = 105,
        BROWSER_REFRESH = 103,
        BROWSER_STOP = 104,
        BROWSER_SEARCH = 101,
        BROWSER_FAVORITES = 102,
        BROWSER_HOME = 50,
        VOLUME_MUTE = 32,
        VOLUME_DOWN = 46,
        VOLUME_UP = 48,
        MEDIA_NEXT_TRACK = 25,
        MEDIA_PREV_TRACK = 16,
        MEDIA_STOP = 36,
        MEDIA_PLAY_PAUSE = 34,
        LAUNCH_MAIL = 108,
        LAUNCH_MEDIA_SELECT = 109,
        LAUNCH_APP1 = 107,
        LAUNCH_APP2 = 33,
        OEM1 = 39,
        OEMPLUS = 13,
        OEMCOMMA = 51,
        OEMMINUS = 12,
        OEMPERIOD = 52,
        OEM2 = 53,
        OEM3 = 41,
        OEM4 = 26,
        OEM5 = 43,
        OEM6 = 27,
        OEM7 = 40,
        OEM8 = 0,
        OEM102 = 86,
        PROCESSKEY = 0,
        PACKET = 0,
        ATTN = 0,
        CRSEL = 0,
        EXSEL = 0,
        EREOF = 93,
        PLAY = 0,
        ZOOM = 98,
        NONAME = 0,
        PA1 = 0,
        OEM_CLEAR = 0,
    }


}
