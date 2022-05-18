using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    internal class DataInfoStr_To_DataInfo
    {
        /*
        public DataInfoStr_To_DataInfo(string DataInfoStr)
        {
            string[] strData = DataInfoStr.Replace("\r\n", "@").Split('@');
            if (strData[0].ToLower() == "handlename: global")//如果是global項
            {
                _datainfo.HandleName = "Global";
                _datainfo.Handle = IntPtr.Zero;
                int ActiveKeyN = Convert.ToInt32(ReturnStrAfterSymbol(strData[2], ':').Trim());
                _datainfo.ActiveKey = new Keys[ActiveKeyN];
                string tempActiveKeyStr = strData[3].Substring(0, strData[3].Length - 1);
                string[] ActiveKeyStr = ReturnStrAfterSymbol(tempActiveKeyStr, ':').Split(';');
                for (int i = 0; i < ActiveKeyN; i++)//Active Keys Number
                {
                    _datainfo.ActiveKey[i] = (Keys)(Enum.Parse(typeof(Keys), ActiveKeyStr[i])); //裝入ActiveKey
                }
                int KeyInfoN = Convert.ToInt32(ReturnStrAfterSymbol(strData[4], ':').Trim());
                _datainfo.KeysInfo = new ListViewData_To_DataInfo.KeysInfo[KeyInfoN];
                int KeyCodeN = 3;//strData[5].ToCharArray().Count(x => x == ';');//找每個KeyInfo內有幾個KeyCode
                _ListViewString = new string[KeyInfoN][];
                for (int i = 0; i < KeyInfoN; i++)
                {
                    _datainfo.KeysInfo[i].KeyCode = new Keys[KeyCodeN];
                    _ListViewString[i] = new string[KeyCodeN+1];//第一項要放按鈕編號
                    _ListViewString[i][0] = i.ToString("00");
                    string tempBtnStr = strData[5 + i].Substring(0, strData[5 + i].Length - 1);
                    string[] BtnStr = ReturnStrAfterSymbol(tempBtnStr, ':').Split(';');
                    for (int j = 0; j < KeyCodeN; j++)
                    {
                        _datainfo.KeysInfo[i].KeyCode[j] = (Keys)Enum.Parse(typeof(Keys), BtnStr[j]);//塞入KeyCode
                        _ListViewString[i][j+1] = BtnStr[j];
                    }
                }
                //InOutD的欄位 是在[4]在往後KeyInfoN+1 
                string tempInOutStr = ReturnStrAfterSymbol(strData[5 + KeyInfoN], ':');
                string[] InOutStr = tempInOutStr.Split(';');
                _datainfo.InOutD = new int[2]; _datainfo.InOutD[0] = Convert.ToInt32(InOutStr[0]); _datainfo.InOutD[1] = Convert.ToInt32(InOutStr[1]);
                _datainfo.IsTypeTrigger = Convert.ToBoolean(ReturnStrAfterSymbol(strData[6 + KeyInfoN], ':').Trim());
                _datainfo.IsTriggerByMouse = Convert.ToBoolean(ReturnStrAfterSymbol(strData[7 + KeyInfoN], ':').Trim());
                _datainfo.IsActiveImmed = Convert.ToBoolean(ReturnStrAfterSymbol(strData[8 + KeyInfoN], ':').Trim());
            }
        }
        */
        public DataInfoStr_To_DataInfo(string DataInfoStr)
        {
            string[] strDataAll = DataInfoStr.Replace("------------------------EndSection------------------------", "@").Split('@');
            _ListViewString = new string[strDataAll.Length][][];
            for (int i = 0; i < strDataAll.Length; i++)//總共有多少組DataInfo
            {
                string[] strData = strDataAll[i].Replace("\r\n", "@").Trim().Split('@');
                strData = strData.Where(x => x != "").ToArray();
                _datainfo.HandleName = ReturnStrAfterSymbol(strData[0], ':').Trim();
                _datainfo.Handle = (IntPtr)Convert.ToInt32(ReturnStrAfterSymbol(strData[1], ':').Trim(), 16);
                AllDataInfoHandle.Add(_datainfo.Handle);
                AllDataInfoHandleName.Add(_datainfo.HandleName);
                int ActiveKeyN = Convert.ToInt32(ReturnStrAfterSymbol(strData[2], ':').Trim());
                _datainfo.ActiveKey = new Keys[ActiveKeyN];
                string tempActiveKeyStr = strData[3].Substring(0, strData[3].Length - 1);
                string[] ActiveKeyStr = ReturnStrAfterSymbol(tempActiveKeyStr, ':').Split(';');
                for (int ii1 = 0; ii1 < ActiveKeyN; ii1++)//Active Keys Number
                {
                    _datainfo.ActiveKey[ii1] = (Keys)(Enum.Parse(typeof(Keys), ActiveKeyStr[ii1])); //裝入ActiveKey
                }
                int KeyInfoN = Convert.ToInt32(ReturnStrAfterSymbol(strData[4], ':').Trim());
                _datainfo.KeysInfo = new ListViewData_To_DataInfo.KeysInfo[KeyInfoN];
                int KeyCodeN = 3;//strData[5].ToCharArray().Count(x => x == ';');//找每個KeyInfo內有幾個KeyCode
                _ListViewString[i] = new string[KeyInfoN][];
                for (int ii1 = 0; ii1 < KeyInfoN; ii1++)
                {
                    _datainfo.KeysInfo[ii1].KeyCode = new Keys[KeyCodeN];
                    _ListViewString[i][ii1] = new string[KeyCodeN + 1];//第一項要放按鈕編號
                    _ListViewString[i][ii1][0] = ii1.ToString("00");
                    string tempBtnStr = strData[5 + ii1].Substring(0, strData[5 + ii1].Length - 1);
                    string[] BtnStr = ReturnStrAfterSymbol(tempBtnStr, ':').Split(';');
                    for (int j = 0; j < KeyCodeN; j++)
                    {
                        _datainfo.KeysInfo[ii1].KeyCode[j] = (Keys)Enum.Parse(typeof(Keys), BtnStr[j]);//塞入KeyCode
                        _ListViewString[i][ii1][j + 1] = BtnStr[j];
                    }
                }
                //InOutD的欄位 是在[4]在往後KeyInfoN+1 
                string tempInOutStr = ReturnStrAfterSymbol(strData[5 + KeyInfoN], ':');
                string[] InOutStr = tempInOutStr.Split(';');
                _datainfo.InOutD = new int[2]; _datainfo.InOutD[0] = Convert.ToInt32(InOutStr[0]); _datainfo.InOutD[1] = Convert.ToInt32(InOutStr[1]);
                _datainfo.IsTypeTrigger = Convert.ToBoolean(ReturnStrAfterSymbol(strData[6 + KeyInfoN], ':').Trim());
                _datainfo.IsTriggerByMouse = Convert.ToBoolean(ReturnStrAfterSymbol(strData[7 + KeyInfoN], ':').Trim());
                _datainfo.IsActiveImmed = Convert.ToBoolean(ReturnStrAfterSymbol(strData[8 + KeyInfoN], ':').Trim());
                AllDataInfo.Add(_datainfo);
            }


        }
        public List<ListViewData_To_DataInfo.DataInfo> AllDataInfo = new List<ListViewData_To_DataInfo.DataInfo>();
        private ListViewData_To_DataInfo.DataInfo _datainfo;
        public List<IntPtr> AllDataInfoHandle = new List<IntPtr>();
        public List<string> AllDataInfoHandleName = new List<string>();
        //public ListViewData_To_DataInfo.DataInfo DataInfo { get { return _datainfo; } }
        private string[][][] _ListViewString;
        public string[][][] ListViewString { get { return _ListViewString; } }
        private string ReturnStrAfterSymbol(string str, char c)
        {
            int index0 = str.IndexOf(c);
            return str.Substring(index0 + 1);
        }
    }

}
