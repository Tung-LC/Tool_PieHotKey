using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    internal partial class Form_Recheck_Local_Handle_And_Name : Form
    {
        public List<ListViewData_To_DataInfo.DataInfo> Result_LocalDataInfo = new List<ListViewData_To_DataInfo.DataInfo>();//要回傳回去的
        private List<ListViewData_To_DataInfo.DataInfo> temp_Result_LocalDataInfo = new List<ListViewData_To_DataInfo.DataInfo>();
        public List<IntPtr> Result_AllDataInfoHandle = new List<IntPtr>();
        public List<string> Result_AllDataInfoHandleName = new List<string>();
        List<LocalHandleReCheck> HRecheck = new List<LocalHandleReCheck>();//用來畫ListView的 包含原本的名字片段 以及 最相似的名字整體與Handle

        public Form_Recheck_Local_Handle_And_Name(List<ListViewData_To_DataInfo.DataInfo> LocalDataInfo, FindAllWindows AllwindowsData)
        {
            InitializeComponent();//初始化

            List<IntPtr> tempDataInfoHandle = new List<IntPtr>();
            List<string> tempDataInfoHandleName = new List<string>();
            string[] tempAllhandlename = new string[AllwindowsData.WindowList.Count]; //全部已開啟視窗的資訊
            IntPtr[] tempAllhandle = new IntPtr[AllwindowsData.WindowList.Count];
            for (int i = 0; i < tempAllhandlename.Length; i++)
            {
                tempAllhandlename[i] = AllwindowsData.WindowList[i].Title;
                tempAllhandle[i] = AllwindowsData.WindowList[i].Hwnd;
            }


            for (int i = 0; i < LocalDataInfo.Count; i++)//一個一個重新檢查與LocalDataInfo擁有完全一致的HandleName
            {
                IntPtr tempHwnd = FindWindowByCaption(IntPtr.Zero, LocalDataInfo[i].HandleName);
                if (tempHwnd == IntPtr.Zero) //如果名字沒有完全一致，但可以原本的Handle就已經存在，則當作擁有完全一致的HandleName
                {
                    if (Is_Handle_Already_Exist(tempAllhandle, LocalDataInfo[i].Handle))
                    {
                        tempDataInfoHandle.Add(LocalDataInfo[i].Handle);
                        tempDataInfoHandleName.Add(LocalDataInfo[i].HandleName);
                    }
                    else
                    {
                        tempDataInfoHandle.Add(tempHwnd);
                        tempDataInfoHandleName.Add(LocalDataInfo[i].HandleName);
                    }
                }
                else
                {
                    tempDataInfoHandle.Add(tempHwnd);
                    tempDataInfoHandleName.Add(LocalDataInfo[i].HandleName);
                }

            }


            for (int i = 0; i < LocalDataInfo.Count; i++)
            {
                if (tempDataInfoHandle[i] != IntPtr.Zero) //已經有找到的 HandleName完全一樣
                {
                    ListViewData_To_DataInfo.DataInfo tempReturnInfo = new ListViewData_To_DataInfo.DataInfo
                    {
                        Handle = tempDataInfoHandle[i], //只有Handle要做修改，其餘完全跟原本一樣
                        InOutD = LocalDataInfo[i].InOutD,
                        ActiveKey = LocalDataInfo[i].ActiveKey,
                        HandleName = LocalDataInfo[i].HandleName,
                        IsActiveImmed = LocalDataInfo[i].IsActiveImmed,
                        IsTriggerByMouse = LocalDataInfo[i].IsTriggerByMouse,
                        IsTypeTrigger = LocalDataInfo[i].IsTypeTrigger,
                        KeysInfo = LocalDataInfo[i].KeysInfo,
                    };
                    Result_LocalDataInfo.Add(tempReturnInfo);
                    Result_AllDataInfoHandleName.Add(tempReturnInfo.HandleName);
                    Result_AllDataInfoHandle.Add(tempReturnInfo.Handle);

                }
                else//如果沒辦法從Caption找到的話
                {
                    List<int> tempint = new List<int>();
                    tempint = Find_Partial_String_in_String(tempAllhandlename, tempDataInfoHandleName[i]);//將原本的HandleName跟現在所開啟的全部視窗的HandleName做比較
                    if (tempint.Count <= 1)
                    {//找不到 或是只找到一項 就不進行人工判斷了 拿原本或新的就好
                        if (tempint.Count == 1)//剛好找到一項相似的
                        {
                            ListViewData_To_DataInfo.DataInfo tempReturnInfo = new ListViewData_To_DataInfo.DataInfo
                            {
                                Handle = AllwindowsData.WindowList[tempint[0]].Hwnd, //只有Handle要做修改，其餘完全跟原本一樣
                                InOutD = LocalDataInfo[i].InOutD,
                                ActiveKey = LocalDataInfo[i].ActiveKey,
                                HandleName = LocalDataInfo[i].HandleName,
                                IsActiveImmed = LocalDataInfo[i].IsActiveImmed,
                                IsTriggerByMouse = LocalDataInfo[i].IsTriggerByMouse,
                                IsTypeTrigger = LocalDataInfo[i].IsTypeTrigger,
                                KeysInfo = LocalDataInfo[i].KeysInfo,
                            };
                            Result_LocalDataInfo.Add(tempReturnInfo);
                            Result_AllDataInfoHandleName.Add(tempReturnInfo.HandleName);
                            Result_AllDataInfoHandle.Add(tempReturnInfo.Handle);
                        }
                        else
                        {
                            ListViewData_To_DataInfo.DataInfo tempReturnInfo = new ListViewData_To_DataInfo.DataInfo
                            {
                                Handle = LocalDataInfo[i].Handle, //找不到 完全跟原本一樣
                                InOutD = LocalDataInfo[i].InOutD,
                                ActiveKey = LocalDataInfo[i].ActiveKey,
                                HandleName = LocalDataInfo[i].HandleName,
                                IsActiveImmed = LocalDataInfo[i].IsActiveImmed,
                                IsTriggerByMouse = LocalDataInfo[i].IsTriggerByMouse,
                                IsTypeTrigger = LocalDataInfo[i].IsTypeTrigger,
                                KeysInfo = LocalDataInfo[i].KeysInfo,
                            };
                            Result_LocalDataInfo.Add(tempReturnInfo);
                            Result_AllDataInfoHandleName.Add(tempReturnInfo.HandleName);
                            Result_AllDataInfoHandle.Add(tempReturnInfo.Handle);
                        }
                    }
                    else //從部分文字進行視窗搜尋 找到不只一項相似的
                    {
                        LocalHandleReCheck tempLHRE = new LocalHandleReCheck
                        {
                            Original = new _HandleData
                            {
                                HandleName = LocalDataInfo[i].HandleName,
                                Handle = LocalDataInfo[i].Handle,
                            },
                            Similar = new _HandleData[tempint.Count],
                        };
                        for (int j = 0; j < tempint.Count; j++)
                        {
                            tempLHRE.Similar[j].Handle = AllwindowsData.WindowList[tempint[j]].Hwnd;
                            tempLHRE.Similar[j].HandleName = AllwindowsData.WindowList[tempint[j]].Title;
                        }
                        HRecheck.Add(tempLHRE);
                        temp_Result_LocalDataInfo.Add(LocalDataInfo[i]);
                        //---------------------------沒心力寫
                        /*
                        ListViewData_To_DataInfo.DataInfo tempReturnIndo = new ListViewData_To_DataInfo.DataInfo
                        {
                            Handle = AllwindowsData.WindowList[tempint[0]].Hwnd, //只有Handle要做修改，其餘完全跟原本一樣
                            InOutD = LocalDataInfo[i].InOutD,
                            ActiveKey = LocalDataInfo[i].ActiveKey,
                            HandleName = LocalDataInfo[i].HandleName,
                            IsActiveImmed = LocalDataInfo[i].IsActiveImmed,
                            IsTriggerByMouse = LocalDataInfo[i].IsTriggerByMouse,
                            IsTypeTrigger = LocalDataInfo[i].IsTypeTrigger,
                            KeysInfo = LocalDataInfo[i].KeysInfo,
                        };
                        Result_LocalDataInfo.Add(tempReturnIndo);
                        Result_AllDataInfoHandleName.Add(LocalDataInfo[i].HandleName);
                        Result_AllDataInfoHandle.Add(AllwindowsData.WindowList[tempint[0]].Hwnd);
                        */
                        //----------------------------直接拿第一項

                    }
                }
            }
            if (HRecheck.Count == 0)
            {
                IsComplete = true;
                return;
            }
            tempOriginal = new string[HRecheck.Count][];
            for (int i = 0; i < HRecheck.Count; i++)
            {
                tempOriginal[i] = new string[3];
                tempOriginal[i][0] = HRecheck[i].Original.HandleName;
                tempOriginal[i][1] = "";
                tempOriginal[i][2] = HRecheck[i].Original.Handle.ToString("X");
            }
            new PlotListView(LV_OriginalHandleName, new string[3] { "原視窗名", "已確認", "Handle" }, new int[3] { 180, 60, 0 }, tempOriginal);
        }
        public bool IsComplete = false;
        string[][] tempOriginal;
        public struct LocalHandleReCheck
        {
            public _HandleData Original;
            public _HandleData[] Similar;
        }
        public struct _HandleData
        {
            public IntPtr Handle;
            public string HandleName;
        }
        private List<int> Find_Partial_String_in_String(string[] strmtx, string str)
        {
            int a = strmtx.Length;
            List<int> result = new List<int>();
            for (int i = 0; i < a; i++)
            {
                Debug.WriteLine(strmtx[i]);
                if (strmtx[i].IndexOf(str) != -1)
                { result.Add(i); }
            }
            return result;
        }
        private bool Is_Handle_Already_Exist(IntPtr[] tempallhandle, IntPtr localhandle)
        {
            bool IsRight = false;
            for (int i = 0; i < tempallhandle.Length; i++)
            {
                if (tempallhandle[i] == localhandle)
                {
                    IsRight = true;
                }
            }
            return IsRight;
        }
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);


        private void LV_OriginalHandleName_Click(object sender, EventArgs e)
        {
            ListView tempListView = sender as ListView; //操作中的ListView
            //ListView.SelectedListViewItemCollection selectedListViewItem = tempListView.SelectedItems;
            int index = tempListView.SelectedIndices[0];
            string[][] tempSimilar = new string[HRecheck[index].Similar.Length][];
            for (int i = 0; i < HRecheck[index].Similar.Length; i++)
            {
                tempSimilar[i] = new string[2];
                tempSimilar[i][0] = HRecheck[index].Similar[i].HandleName;
                tempSimilar[i][1] = HRecheck[index].Similar[i].Handle.ToString("X");
            }
            new PlotListView(LV_SimilarHandleName, new string[2] { "最接近之視窗名", "Handle" }, new int[2] { 240, 0 }, tempSimilar);
        }

        private void button1_Click(object sender, EventArgs e)
        {
            int index_Similar = LV_SimilarHandleName.SelectedIndices[0];
            int index = LV_OriginalHandleName.SelectedIndices[0];
            if (index < 0 || index_Similar < 0) { MessageBox.Show("請確定兩邊視窗都有選擇"); return; }
            ListView.SelectedListViewItemCollection selectedListViewItem = LV_SimilarHandleName.SelectedItems;
            string[] SelectItemStr = new string[2];//多一項因為第一項是按鈕編號
            foreach (ListViewItem item in selectedListViewItem)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    SelectItemStr[i] = item.SubItems[i].Text;
                }
            }
            tempOriginal[index][1] = "Y";
            tempOriginal[index][2] = SelectItemStr[1];
            new PlotListView(LV_OriginalHandleName, new string[3] { "原視窗名", "已確認", "Handle" }, new int[3] { 180, 60, 0 }, tempOriginal);
        }

        private void Btn_Completed_Click(object sender, EventArgs e)
        {

            for (int i = 0; i < HRecheck.Count; i++)
            {
                ListViewData_To_DataInfo.DataInfo tempReturnInfo = new ListViewData_To_DataInfo.DataInfo
                {
                    Handle = (IntPtr)Convert.ToInt32(tempOriginal[i][2], 16), //只有Handle要做修改，其餘完全跟原本一樣
                    InOutD = temp_Result_LocalDataInfo[i].InOutD,
                    ActiveKey = temp_Result_LocalDataInfo[i].ActiveKey,
                    HandleName = temp_Result_LocalDataInfo[i].HandleName,
                    IsActiveImmed = temp_Result_LocalDataInfo[i].IsActiveImmed,
                    IsTriggerByMouse = temp_Result_LocalDataInfo[i].IsTriggerByMouse,
                    IsTypeTrigger = temp_Result_LocalDataInfo[i].IsTypeTrigger,
                    KeysInfo = temp_Result_LocalDataInfo[i].KeysInfo,
                };
                Result_LocalDataInfo.Add(tempReturnInfo);
                Result_AllDataInfoHandleName.Add(tempReturnInfo.HandleName);
                Result_AllDataInfoHandle.Add(tempReturnInfo.Handle);
            }
            this.Close();
        }
    }
}
