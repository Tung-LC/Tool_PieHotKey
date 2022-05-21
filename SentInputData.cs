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
                                    //wScan = (ScanCodeShort)System.Enum.Parse(typeof(ScanCodeShort), InputKeyStr[i]),
                                    dwFlags = KeyEventF.KeyDown,
                                    dwExtraInfo = GetMessageExtraInfo()
                                }
                            }
                        };
                        SendInput(1, inputs, INPUT.size);Thread.Sleep(5);
                    }
                    for (int i = InputKeyLength; i < InputKeyLength * 2; i++)
                    {
                        INPUT[] inputs = new INPUT[1];
                        inputs[0] = new INPUT()
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


}
