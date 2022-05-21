using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    public class ListViewData_To_DataInfo
    {
        public ListViewData_To_DataInfo(ListView global_ListView, IntPtr Hwnd, string HandleName, string[] ActiveBtn, bool IsTypeTrigger, bool Istriggerbymouse, int[] InOutD, bool IsActImmed)
        {
            string[][] tempListViewData = new string[global_ListView.Items.Count][];
            ListViewItem templistviewitem;
            for (int i = 0; i < global_ListView.Items.Count; i++)
            {
                templistviewitem = global_ListView.Items[i];
                tempListViewData[i] = new string[Form1.UsingBtnN + 1];
                for (int j = 0; j < Form1.UsingBtnN + 1; j++)
                {
                    tempListViewData[i][j] = templistviewitem.SubItems[j].Text; //提取資料
                }
            }//第一個subitem為button number 第二第三為Keycode
            DataInfo tempDataInfo = new DataInfo
            {
                Handle = Hwnd,
                HandleName = HandleName,
                ActiveKey = new Keys[ActiveBtn.Length],
                IsTypeTrigger = IsTypeTrigger,
                IsTriggerByMouse = Istriggerbymouse,
                InOutD = InOutD,
                IsActiveImmed = IsActImmed
            };
            for (int i = 0; i < ActiveBtn.Length; i++) //啟動按鈕，在這個條件下全部都一樣
            {
                try
                {
                    if (MouseBtnName.Contains(ActiveBtn[i])) { ActiveBtn[i] = ActiveBtn[i].ToString()[0] + "Button"; }
                    tempDataInfo.ActiveKey[i] = (Keys)Enum.Parse(typeof(Keys), ActiveBtn[i], true);
                }
                catch (ArgumentException)
                {
                    tempDataInfo.ActiveKey[i] = Keys.None;
                }
            }
            tempDataInfo.KeysInfo = new KeysInfo[global_ListView.Items.Count];//總共有幾個按鈕的KeyCode要記錄 這些紀錄存放在KeysInfo
            for (int i = 0; i < global_ListView.Items.Count; i++)
            {
                tempDataInfo.KeysInfo[i].KeyCode = new Keys[Form1.UsingBtnN];
                for (int j = 1; j < Form1.UsingBtnN + 1; j++) //KeyCode存在 1.2項 第0項為按鈕編號
                {
                    try
                    {
                        tempDataInfo.KeysInfo[i].KeyCode[j - 1] = (Keys)Enum.Parse(typeof(Keys), tempListViewData[i][j], true);
                    }
                    catch (System.ArgumentException)
                    {
                        tempDataInfo.KeysInfo[i].KeyCode[j - 1] = Keys.None;
                    }
                }
            }
            dataInfo = tempDataInfo;
        }
        public ListViewData_To_DataInfo()
        { }
        static string[] MouseBtnName = new string[3] { "Left", "Middle", "Right" };
        public DataInfo dataInfo { get; set; }
        public struct DataInfo
        {
            public IntPtr Handle;
            public string HandleName;
            //public int[] BtnIndex;
            public Keys[] ActiveKey;
            public KeysInfo[] KeysInfo;
            public int[] InOutD;
            public bool IsTypeTrigger;
            public bool IsTriggerByMouse;
            public bool IsActiveImmed;
        }
        public struct KeysInfo
        {
            public Keys[] KeyCode;
        }

        public override string ToString()
        {
            string tempstr = "";
            tempstr += "HandleName: " + dataInfo.HandleName + "\r\n";
            tempstr += "Handle: " + dataInfo.Handle.ToString("X") + "\r\n";
            tempstr += "ActiveKeys Number: " + Form1.ActiveBtnN.ToString() + "\r\n";
            tempstr += "ActiveKeys:";
            for (int i = 0; i < Form1.ActiveBtnN; i++)
            {
                tempstr += " " + dataInfo.ActiveKey[i].ToString() + ";";
            }
            tempstr += "\r\n";
            tempstr += "KeysInfo Number: " + dataInfo.KeysInfo.Length.ToString() + "\r\n";
            for (int i = 0; i < dataInfo.KeysInfo.Length; i++)
            {
                tempstr += "KeyCode:";
                for (int j = 0; j < Form1.UsingBtnN; j++)
                {
                    tempstr += " " + dataInfo.KeysInfo[i].KeyCode[j].ToString() + ";";
                }
                tempstr += "\r\n";
            }
            tempstr += "InOutD: " + dataInfo.InOutD[0].ToString() + ";" + " " + dataInfo.InOutD[1].ToString() + ";" + "\r\n";
            tempstr += "IsTypeTrigger: " + dataInfo.IsTypeTrigger.ToString() + "\r\n";
            tempstr += "IsTriggerByMouse: " + dataInfo.IsTriggerByMouse.ToString() + "\r\n";
            tempstr += "IsActiveImmed: " + dataInfo.IsActiveImmed.ToString() + "\r\n";

            return tempstr;
        }
    }
}
