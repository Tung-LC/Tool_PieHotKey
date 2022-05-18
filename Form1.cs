using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Windows.Input;
using System.Threading;
using System.IO;
using System.Collections;

namespace Tool_PieHotKey
{
    public partial class Form1 : Form
    {
        #region 全域變數/常數
        Keys[] NeedToModifiedKeys = new Keys[3] { Keys.ControlKey, Keys.ShiftKey, Keys.Menu };
        string[] MouseBtnChange = new string[3] { "LButton", "MButton", "RButton" };
        string[] MouseBtnChangeTo = new string[3] { "Left", "Middle", "Right" };
        public const int ActiveBtnN = 2;
        public const int UsingBtnN = 3;
        string ThisAppPath = System.Windows.Forms.Application.StartupPath;
        public IntPtr _ActiveWindow = IntPtr.Zero;
        Keys[] CompletedModifiedKeys = new Keys[ActiveBtnN];
        public Keys[] _ActiveKeys = new Keys[ActiveBtnN] { Keys.None, Keys.None };
        public Keys[] ActiveKeys
        {
            get { return _ActiveKeys; }
            set
            {
                for (int i = 0; i < ActiveBtnN; i++)
                {
                    try { _ActiveKeys[i] = value[i]; }
                    catch { _ActiveKeys[i] = Keys.None; Debug.WriteLine("ActiveKeys Error" + value[i]); }
                }
            }
        }
        public bool IsActiveByMouse;
        public bool[] IsKeyMousePress = new bool[ActiveBtnN] { false, false };
        bool IsShownChildrenHandle = false;
        bool IsBtnRelease = true;
        bool IsNeedToModifiedKeys = false;
        public List<ListViewData_To_DataInfo.DataInfo> AllDataInfo = new List<ListViewData_To_DataInfo.DataInfo>();
        public List<ListViewData_To_DataInfo.DataInfo> tempLocalDataInfo = new List<ListViewData_To_DataInfo.DataInfo>();
        public List<IntPtr> AllDataInfoHandle = new List<IntPtr>();
        public List<string> AllDataInfoHandleName = new List<string>();
        ListViewData_To_DataInfo.DataInfo ActiveWindowInfo = new ListViewData_To_DataInfo.DataInfo();
        private IntPtr ActiveWindow { get { return _ActiveWindow; } set { if (value != _ActiveWindow) { _ActiveWindow = value; ActiveWindowChanges(); } } }
        Form_ShowPieButton Fshow;
        private GlobalKeyboardHook _globalKeyboardHook;
        private GlobalMouseHook _globalMouseHook;
        FindAllWindows allwindowsdata;
        #endregion

        public Form1()
        {
            InitializeComponent();
            #region Global參數的預先設置
            string AllDataStr = new Load_DataInfoStr_From_File(ThisAppPath + "\\Data\\AllDataInfo.txt").Returnstr;
            if (AllDataStr == null)//沒東西的話就全部設置為None 
            {
                new FillInListViewWithNoneKeys(LV_Global_SetupShown, Convert.ToInt32(TBX_GlobalBtnN.Text));
                new FillInListViewWithNoneKeys(LV_Local_SetupShown, Convert.ToInt32(TBX_LocalBtnN.Text));
            }
            else
            {
                DataInfoStr_To_DataInfo a1 = new DataInfoStr_To_DataInfo(AllDataStr);
                AllDataInfo = a1.AllDataInfo;
                RDOBtnGlobal_ByMouseBtn.Checked = AllDataInfo[0].IsTriggerByMouse;
                RDOBtnGlobal_ByKeyBoard.Checked = !AllDataInfo[0].IsTriggerByMouse;
                int tempCheck = Array.IndexOf(MouseBtnChange, AllDataInfo[0].ActiveKey[0].ToString());
                int tempCheck1 = Array.IndexOf(MouseBtnChange, AllDataInfo[0].ActiveKey[1].ToString());
                TBX_Global_ActiveBtn1.Text = tempCheck >= 0 ? MouseBtnChangeTo[tempCheck] : AllDataInfo[0].ActiveKey[0].ToString();
                TBX_Global_ActiveBtn2.Text = tempCheck1 >= 0 ? MouseBtnChangeTo[tempCheck1] : AllDataInfo[0].ActiveKey[1].ToString();
                CKBX_GlobalActImmed.Checked = AllDataInfo[0].IsActiveImmed;
                TBX_GlobalInD.Text = AllDataInfo[0].InOutD[0].ToString();
                TBX_GlobalOutD.Text = AllDataInfo[0].InOutD[1].ToString();
                TBX_GlobalBtnN.Text = AllDataInfo[0].KeysInfo.Length.ToString();
                new PlotListView(LV_Global_SetupShown, new string[UsingBtnN + 1] { "按鈕", "按鍵1", "按鍵2", "按鍵3" }, new int[UsingBtnN + 1] { 45, 70, 70, 70 }, a1.ListViewString[0]);
                AllDataInfoHandle = a1.AllDataInfoHandle;
                AllDataInfoHandleName = a1.AllDataInfoHandleName;

                tempLocalDataInfo.AddRange(AllDataInfo);
                tempLocalDataInfo.RemoveAt(0);
                if (tempLocalDataInfo.Count > 0)
                {
                    string[] LV_LocalHandleTxt = new string[tempLocalDataInfo.Count];
                    for (int i = 0; i < tempLocalDataInfo.Count; i++)
                    {
                        LV_LocalHandleTxt[i] = tempLocalDataInfo[i].HandleName;
                    }
                    new PlotListView(LV_Local_HandleName, new string[1] { "視窗列" }, new int[1] { 85 }, LV_LocalHandleTxt, true, true, true);
                }
                else
                {
                    new PlotListView(LV_Local_HandleName, new string[1] { "視窗列" }, new int[1] { 85 }, true);
                }
                new FillInListViewWithNoneKeys(LV_Local_SetupShown, Convert.ToInt32(TBX_LocalBtnN.Text));
            }
            #endregion
            Refresh_LocalHandleSelect();
        }
        private void Refresh_LocalHandleSelect()
        {
            allwindowsdata = new FindAllWindows();
            CBBX_LocalHandle.Items.Clear(); //清空所有
            for (int i = 0; i < allwindowsdata.WindowList.Count; i++)
            {
                CBBX_LocalHandle.Items.Add(allwindowsdata.WindowList[i].Title);
            }


        }
        private void TBX_GlobalBtnN_TextChanged(object sender, EventArgs e)
        {
            string Text0 = (sender as TextBox).Text;
            if (Text0 == "") { return; }
            uint BtnN;
            try { BtnN = Convert.ToUInt32(Text0); }
            catch { MessageBox.Show("請輸入正整數"); (sender as TextBox).Text = ""; return; }
            if (BtnN > 99) { MessageBox.Show("不能大於99"); (sender as TextBox).Text = ""; return; }
            CBBX_GlobalBtnSelect.Items.Clear();
            for (int i = 0; i < BtnN; i++)
            {
                //CBBX_GlobalBtnSelect.Items.Add("按鈕" + i.ToString());
                CBBX_GlobalBtnSelect.Items.Add(i.ToString("00"));//將選擇按鈕號碼重新生成
            }
            //將按鈕設定欄位設定為None+None以防萬一
            new FillInListViewWithNoneKeys(LV_Global_SetupShown, (int)BtnN);
        }
        private void TBX_LocalBtnN_TextChanged(object sender, EventArgs e)
        {
            string Text0 = (sender as TextBox).Text;
            if (Text0 == "") { return; }
            uint BtnN;
            try { BtnN = Convert.ToUInt32(Text0); }
            catch { MessageBox.Show("請輸入正整數"); (sender as TextBox).Text = ""; return; }
            if (BtnN > 99) { MessageBox.Show("不能大於99"); (sender as TextBox).Text = ""; return; }
            CBBX_LocalBtnSelect.Items.Clear();
            for (int i = 0; i < BtnN; i++)
            {
                //CBBX_LocalBtnSelect.Items.Add("按鈕" + i.ToString());
                CBBX_LocalBtnSelect.Items.Add(i.ToString());
            }
            //將按鈕設定欄位設定為None+None以防萬一
            new FillInListViewWithNoneKeys(LV_Local_SetupShown, (int)BtnN);
        }
        private void TBX_Enter_ClearText(object sender, EventArgs e)
        {

        }
        private void TBX_MouseClick_ClearText(object sender, MouseEventArgs e)
        {
            TextBox tempTBX = (sender as TextBox);
            if (tempTBX != null) { tempTBX.Text = ""; }
        }
        private void TBX_KeyUp_ShowKeyCode(object sender, KeyEventArgs e)
        {
            TextBox tempTBX = (sender as TextBox);
            tempTBX.Enabled = false;
            /*
            if (e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Alt)
            {
                DialogResult a = MessageBox.Show("是左邊的" + e.KeyCode.ToString() + "嗎?", "詢問", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (a == DialogResult.Yes) { tempTBX.Text = "L" + e.KeyCode.ToString(); }
                else { tempTBX.Text = "R" + e.KeyCode.ToString(); }
            }
            else { tempTBX.Text = e.KeyCode.ToString(); }
            */
            tempTBX.Text = e.KeyCode.ToString();//不修改左右
            tempTBX.Enabled = true;
        }
        private void TBX_MouseDown_ShowMouseBtnCode(object sender, MouseEventArgs e)
        {
            TextBox tempTBX = (sender as TextBox);
            tempTBX.Enabled = false;
            tempTBX.Text = e.Button.ToString();
            tempTBX.Enabled = true;
        }
        private void Global_RDBtn_CheckChange(object sender, EventArgs e)
        {
            if (RDOBtnGlobal_ByMouseBtn.Checked)//使用滑鼠設定 則允許改變以hold 或是 trigger ((Hold意思為 當觸發按鈕放開時，Pie就會消失 
            {
                try
                {
                    TBX_Global_ActiveBtn1.KeyUp -= TBX_KeyUp_ShowKeyCode;
                    TBX_Global_ActiveBtn2.KeyUp -= TBX_KeyUp_ShowKeyCode;
                }
                catch { }
                TBX_Global_ActiveBtn1.MouseDown += TBX_MouseDown_ShowMouseBtnCode;
                TBX_Global_ActiveBtn2.MouseDown += TBX_MouseDown_ShowMouseBtnCode;
                groupBox1.Enabled = true;

            }
            else //用鍵盤設定 則只允許按鈕觸發以trigger 放開Active按鈕 PIE依然會存在 形式出現
            {
                try
                {
                    TBX_Global_ActiveBtn1.MouseDown -= TBX_MouseDown_ShowMouseBtnCode;
                    TBX_Global_ActiveBtn2.MouseDown -= TBX_MouseDown_ShowMouseBtnCode;
                }
                catch { }
                TBX_Global_ActiveBtn1.KeyUp += TBX_KeyUp_ShowKeyCode;
                TBX_Global_ActiveBtn2.KeyUp += TBX_KeyUp_ShowKeyCode;
                groupBox1.Enabled = false;
                RDBtn_Global_TriggerType_Trigger.Checked = true;
            }
        }
        private void Local_RDBtn_CheckChange(object sender, EventArgs e)
        {
            if (RDOBtnLocal_ByMouseBtn.Checked)//使用滑鼠設定 則允許改變以hold 或是 trigger ((Hold意思為 當觸發按鈕放開時，Pie就會消失 
            {
                try
                {
                    TBX_Local_ActiveBtn1.KeyUp -= TBX_KeyUp_ShowKeyCode;
                    TBX_Local_ActiveBtn2.KeyUp -= TBX_KeyUp_ShowKeyCode;
                }
                catch { }
                TBX_Local_ActiveBtn1.MouseDown += TBX_MouseDown_ShowMouseBtnCode;
                TBX_Local_ActiveBtn2.MouseDown += TBX_MouseDown_ShowMouseBtnCode;
                groupBox2.Enabled = true;

            }
            else //用鍵盤設定 則只允許按鈕觸發以trigger 放開Active按鈕 PIE依然會存在 形式出現
            {
                try
                {
                    TBX_Local_ActiveBtn1.MouseDown -= TBX_MouseDown_ShowMouseBtnCode;
                    TBX_Local_ActiveBtn2.MouseDown -= TBX_MouseDown_ShowMouseBtnCode;
                }
                catch { }
                TBX_Local_ActiveBtn1.KeyUp += TBX_KeyUp_ShowKeyCode;
                TBX_Local_ActiveBtn2.KeyUp += TBX_KeyUp_ShowKeyCode;
                groupBox2.Enabled = false;
                RDBtn_Local_TriggerType_Trigger.Checked = true;
            }
        }
        private void Btn_GlobalConfirm_Click(object sender, EventArgs e)
        {
            string[] DataInputToListView = new string[UsingBtnN + 1];
            if (CBBX_GlobalBtnSelect.SelectedIndex == -1) { return; }
            DataInputToListView[0] = CBBX_GlobalBtnSelect.SelectedIndex.ToString("00");//按鈕名稱
            DataInputToListView[1] = TBX_Global_Btn1.Text; //第一個操作鈕
            DataInputToListView[2] = TBX_Global_Btn2.Text; //第二個操作鈕
            DataInputToListView[3] = TBX_Global_Btn3.Text; //第二個操作鈕

            //判斷是否有之前的按鈕數據
            int itemN = LV_Global_SetupShown.Items.Count;
            int itemRepeatIndex;
            bool AddColumnHead = true;
            if (itemN != 0)//當ListView中有items 才進行判斷
            {
                AddColumnHead = false;
                string[] tempBtnIndex = new string[itemN];
                ListViewItem templistviewitem;
                for (int i = 0; i < itemN; i++)
                {
                    templistviewitem = LV_Global_SetupShown.Items[i];
                    tempBtnIndex[i] = templistviewitem.SubItems[0].Text;
                }
                itemRepeatIndex = Array.IndexOf(tempBtnIndex, DataInputToListView[0]);
                if (itemRepeatIndex != -1) { LV_Global_SetupShown.Items[itemRepeatIndex].Remove(); }
            }
            new PlotListView(LV_Global_SetupShown, new string[UsingBtnN + 1] { "按鈕", "按鍵1", "按鍵2", "按鈕3" }, new int[UsingBtnN + 1] { 45, 70, 70, 70 }, DataInputToListView, AddColumnHead);
        }
        private void Btn_LocalConfirm_Click(object sender, EventArgs e)
        {
            string[] DataInputToListView = new string[UsingBtnN + 1];
            if (CBBX_LocalBtnSelect.SelectedIndex == -1) { return; }
            DataInputToListView[0] = CBBX_LocalBtnSelect.SelectedIndex.ToString("00");//按鈕名稱
            DataInputToListView[1] = TBX_Local_Btn1.Text; //第一個操作鈕
            DataInputToListView[2] = TBX_Local_Btn2.Text; //第二個操作鈕
            DataInputToListView[3] = TBX_Local_Btn3.Text; //第二個操作鈕

            //判斷是否有之前的按鈕數據
            int itemN = LV_Local_SetupShown.Items.Count;
            int itemRepeatIndex;
            bool AddColumnHead = true;
            if (itemN != 0)//當ListView中有items 才進行判斷
            {
                AddColumnHead = false;
                string[] tempBtnIndex = new string[itemN];
                ListViewItem templistviewitem;
                for (int i = 0; i < itemN; i++)
                {
                    templistviewitem = LV_Local_SetupShown.Items[i];
                    tempBtnIndex[i] = templistviewitem.SubItems[0].Text;
                }
                itemRepeatIndex = Array.IndexOf(tempBtnIndex, DataInputToListView[0]);
                if (itemRepeatIndex != -1) { LV_Local_SetupShown.Items[itemRepeatIndex].Remove(); }
            }
            new PlotListView(LV_Local_SetupShown, new string[UsingBtnN + 1] { "按鈕", "按鍵1", "按鍵2", "按鈕3" }, new int[UsingBtnN + 1] { 45, 70, 70, 70 }, DataInputToListView, AddColumnHead);
        }
        private void Btn_LocalSaveData_Click(object sender, EventArgs e)
        {
            string[] tmpstrLocal = new string[ActiveBtnN] { TBX_Local_ActiveBtn1.Text, TBX_Local_ActiveBtn2.Text };
            if (TBX_Local_ActiveBtn1.Text == "" && TBX_Local_ActiveBtn2.Text == "") { MessageBox.Show("請輸入啟動鈕"); return; }
            int[] InOutD2 = new int[ActiveBtnN] { Convert.ToInt32(TBX_LocalInD.Text), Convert.ToInt32(TBX_LocalOutD.Text) };
            //IntPtr hdwd = FindWindowByCaption(IntPtr.Zero, CBBX_LocalHandle.Text);
            if (CBBX_LocalHandle.SelectedIndex == -1) { return; }
            ListViewData_To_DataInfo lvd2 = new ListViewData_To_DataInfo(LV_Local_SetupShown, allwindowsdata.WindowList[CBBX_LocalHandle.SelectedIndex].Hwnd, CBBX_LocalHandle.Text, tmpstrLocal, RDBtn_Local_TriggerType_Trigger.Checked, RDOBtnLocal_ByMouseBtn.Checked, InOutD2, CKBX_LocalActImmed.Checked);
            //new Write_Txt_To_File(ThisAppPath + "\\Data\\" + lvd2.dataInfo.HandleName + "DataInfo.txt", lvd2.ToString());
            tempLocalDataInfo.Add(lvd2.dataInfo);
            new PlotListView(LV_Local_HandleName, new string[1] { "視窗列" }, new int[1] { 85 }, new string[1] { lvd2.dataInfo.HandleName }, false);
        }
        private void Btn_LocalDeleteData_Click(object sender, EventArgs e)
        {
            if (LV_Local_HandleName.SelectedIndices[0] < 0) { return; }
            if (MessageBox.Show("確定要刪除 " + tempLocalDataInfo[LV_Local_HandleName.SelectedIndices[0]].HandleName + " 嗎?", "詢問", MessageBoxButtons.YesNo) != DialogResult.Yes)
            {
                return;
            }
            tempLocalDataInfo.RemoveAt(LV_Local_HandleName.SelectedIndices[0]);
            string[] temphandlename = new string[tempLocalDataInfo.Count];
            for (int i = 0; i < tempLocalDataInfo.Count; i++)
            {
                temphandlename[i] = tempLocalDataInfo[i].HandleName;
            }

            new PlotListView(LV_Local_HandleName, new string[1] { "視窗列" }, new int[1] { 85 }, temphandlename, true, true, true);
        }
        private void Btn_SaveData_Click(object sender, EventArgs e)
        {
            int HowManyLocalItem = LV_Local_HandleName.Items.Count;
            string[] tmpstrGlobal = new string[ActiveBtnN] { TBX_Global_ActiveBtn1.Text, TBX_Global_ActiveBtn2.Text };
            if (TBX_Global_ActiveBtn1.Text == "" && TBX_Global_ActiveBtn2.Text == "") { MessageBox.Show("請輸入Global啟動鈕"); return; }
            int[] InOutD = new int[ActiveBtnN] { Convert.ToInt32(TBX_GlobalInD.Text), Convert.ToInt32(TBX_GlobalOutD.Text) };
            ListViewData_To_DataInfo lvd1 = new ListViewData_To_DataInfo(LV_Global_SetupShown, IntPtr.Zero, "Global", tmpstrGlobal, RDBtn_Global_TriggerType_Trigger.Checked, RDOBtnGlobal_ByMouseBtn.Checked, InOutD, CKBX_GlobalActImmed.Checked);
            AllDataInfo.Clear();
            AllDataInfo.Add(lvd1.dataInfo);
            AllDataInfoHandle.Clear();
            AllDataInfoHandle.Add(lvd1.dataInfo.Handle);
            AllDataInfoHandleName.Clear();
            AllDataInfoHandleName.Add(lvd1.dataInfo.HandleName);
            bool IsNeedToUpdateHandleToTxt = false;
            for (int i = 0; i < HowManyLocalItem; i++)
            {
                //再次確認window的Handle 主要以字串為基準 若是找不到則以原本的為基準
                IntPtr tempHwnd = FindWindowByCaption(IntPtr.Zero, tempLocalDataInfo[i].HandleName);
                if (tempHwnd != IntPtr.Zero)//如果有重新找到這個標題的Handle 則以新的為基準 否則就以舊的為標準
                {
                    if (tempHwnd != tempLocalDataInfo[i].Handle)//找到的跟原本的不同 那就更新為新的
                    {
                        IsNeedToUpdateHandleToTxt = true; //必須要做修改
                    }
                    AllDataInfoHandle.Add(tempHwnd);
                }
                else
                {
                    AllDataInfoHandle.Add(tempLocalDataInfo[i].Handle);
                }
                AllDataInfoHandleName.Add(tempLocalDataInfo[i].HandleName);
            }
            AllDataInfo.AddRange(tempLocalDataInfo);
            if (IsNeedToUpdateHandleToTxt) //需要修改原本txt檔案的Handle參數
            {
                new Write_Txt_To_File(ThisAppPath + "\\Data\\AllDataInfo.txt", AllDataInfo, AllDataInfoHandle);
            }
            else
            {
                new Write_Txt_To_File(ThisAppPath + "\\Data\\AllDataInfo.txt", AllDataInfo);
            }
            _globalKeyboardHook = new GlobalKeyboardHook();
            _globalMouseHook = new GlobalMouseHook();
            int MaxBtnN = lvd1.dataInfo.KeysInfo.Length;
            PreCreatePieButton Fshow_Pie = new PreCreatePieButton(MaxBtnN, InOutD);//在開始的時候進行設置全部的按鈕
            Fshow = new Form_ShowPieButton(Fshow_Pie.ButtonsList, InOutD, MaxBtnN, lvd1.dataInfo.IsActiveImmed, lvd1.dataInfo.IsTypeTrigger);
            Btn_StartHotKey.Enabled = true;
        }
        private void Btn_StartHotKey_Click(object sender, EventArgs e)
        {
            ActiveWindow = IntPtr.Zero;
            Timer_GetCurrentWindows.Enabled = true;
            Btn_StartHotKey.Enabled = false;
            Btn_SaveData.Visible = false;
            Btn_Stop.Visible = true;
            //Thread.Sleep(400);
            _globalMouseHook.MousePressed += OnMouseBtnPressed;
            _globalKeyboardHook.KeyboardPressed += OnKeyPressed;
            ActiveWindow = GetForegroundWindow();
        }
        private void Btn_Stop_Click(object sender, EventArgs e)
        {
            Timer_GetCurrentWindows.Enabled = false;
            Btn_StartHotKey.Enabled = true;
            Btn_SaveData.Visible = true;
            Btn_Stop.Visible = false;
            try { _globalKeyboardHook.KeyboardPressed -= OnKeyPressed; } catch { }
            try { _globalMouseHook.MousePressed -= OnMouseBtnPressed; } catch { };
        }
        #region User32.dll windowAPI
        private void Timer_GetCurrentWindows_Tick(object sender, EventArgs e)
        {
            //IntPtr Hwnd = GetActiveWindow();
            IntPtr Hwnd = GetForegroundWindow();
            ActiveWindow = Hwnd;
            //Debug.WriteLine(ActiveWindowTitle());
        }
        [DllImport("user32.dll", SetLastError = true)]
        //static extern IntPtr GetActiveWindow();
        //[DllImport("user32.dll")]
        static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        static extern int GetWindowText(IntPtr hwnd, StringBuilder ss, int count);
        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(out Point lpPoint);
        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        // Find window by Caption only. Note you must pass IntPtr.Zero as the first parameter.
        [DllImport("user32.dll", EntryPoint = "FindWindow", SetLastError = true)]
        static extern IntPtr FindWindowByCaption(IntPtr ZeroOnly, string lpWindowName);

        #endregion
        /// <summary>
        /// 經由Timer計時每0.1判斷當前Active視窗，若是視窗有變化，則會呼叫此程式
        /// </summary>
        private void ActiveWindowChanges()
        {
            //Debug.WriteLine("{0:X}", ActiveWindow);
            //先判斷是否此Handle有存在於Local 若是有 則以Local設定，若無，則以Global設定
            //先以global為主
            IsKeyMousePress = new bool[ActiveBtnN] { false, false };
            var lptrString = new StringBuilder(512);
            GetWindowText(ActiveWindow, lptrString, lptrString.Capacity);
            string ActiveWindowName = lptrString.ToString();
            //    ActiveWindowInfo = AllDataInfo1.GlobalDataInfo;
            if (AllDataInfoHandle.Contains(ActiveWindow)) //如果視窗存在於以儲存的視窗中
            {
                int indexHD = AllDataInfoHandle.IndexOf(ActiveWindow);
                ActiveWindowInfo = AllDataInfo[indexHD];
            }
            else if (AllDataInfoHandleName.Contains(ActiveWindowName))
            {
                int indexHD = AllDataInfoHandleName.IndexOf(ActiveWindowName);
                ActiveWindowInfo = AllDataInfo[indexHD];
            }
            else
            {
                ActiveWindowInfo = AllDataInfo[0]; //global設定
            }
            ActiveKeys = ActiveWindowInfo.ActiveKey;
            if (ActiveKeys.Select(x => NeedToModifiedKeys.Contains(x)).ToArray().Contains(true))//如果ActiveKeys包含著 Control Shift Alt等需要修改的Keys的話
            {
                IsNeedToModifiedKeys = true; //需要修改Keys就為真
                CompletedModifiedKeys = ActiveKeys;
                for (int i = 0; i < ActiveBtnN; i++)
                {
                    if (Array.IndexOf(NeedToModifiedKeys, CompletedModifiedKeys[i]) >= 0)
                    {
                        CompletedModifiedKeys[i] = (Keys)Enum.Parse(typeof(Keys), "L" + CompletedModifiedKeys[i].ToString());
                    }
                }
            }
            IsActiveByMouse = ActiveWindowInfo.IsTriggerByMouse;

            //Debug.WriteLine(ActiveWindow.ToString("x8"));


        }
        private void OnKeyPressed(object sender, GlobalKeyboardHookEventArgs e)
        {
            if (ActiveWindowInfo.IsTriggerByMouse) { return; }//由滑鼠觸發的話就跳掉
            Timer_GetCurrentWindows.Enabled = false;
            // EDT: No need to filter for VkSnapshot anymore. This now gets handled
            // through the constructor of GlobalKeyboardHook(...).
            //_globalKeyboardHook.KeyboardPressed -= OnKeyPressed;
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyDown)//如果有按下按鍵
            {
                // Now you can access both, the key and virtual code
                Keys loggedKey = e.KeyboardData.Key;
                //int loggedVkCode = e.KeyboardData.VirtualCode;
                //Debug.WriteLine("按下" + loggedKey + "\t" + loggedVkCode);
                for (int i = 0; i < ActiveBtnN; i++)
                {
                    if (IsNeedToModifiedKeys)
                    { if (loggedKey == CompletedModifiedKeys[i] || CompletedModifiedKeys[i] == Keys.None) { IsKeyMousePress[i] = true; } }
                    else
                    { if (loggedKey == ActiveKeys[i] || ActiveKeys[i] == Keys.None) { IsKeyMousePress[i] = true; } }
                }
            }
            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
            {
                Keys loggedKey = e.KeyboardData.Key;
                for (int i = 0; i < ActiveBtnN; i++)
                {
                    if (IsNeedToModifiedKeys)
                    { if (loggedKey == CompletedModifiedKeys[i] || CompletedModifiedKeys[i] == Keys.None) { IsKeyMousePress[i] = false; IsBtnRelease = true; } }
                    else
                    { if (loggedKey == ActiveKeys[i] || ActiveKeys[i] == Keys.None) { IsKeyMousePress[i] = false; IsBtnRelease = true; } }
                }
                if (Fshow.Visible && IsKeyMousePress.Contains(true) && ActiveWindowInfo.IsTypeTrigger == false)
                {
                    Fshow.VisibleFalse();
                }
            }
            //Debug.WriteLine(IsKeyMousePress[0] + " " + IsKeyMousePress[1]);
            if (!IsKeyMousePress.Contains(false) && Fshow.Visible == false && IsBtnRelease) //如果IsKeyMousePress全部為true
            {
                //IsKeyMousePress = new bool[2] { false, false };
                IsBtnRelease = false;//沒有放開按鍵
                ShowButton();
            }
            //_globalKeyboardHook.KeyboardPressed += OnKeyPressed;
            Timer_GetCurrentWindows.Enabled = true;

        }
        private void OnMouseBtnPressed(object sender, GlobalMouseHookEventArgs e)
        {
            //_globalMouseHook.MousePressed -= OnMouseBtnPressed;
            //Debug.WriteLine(e.MouseState);
            //if (e.MouseState == GlobalMouseHook.MouseState.MouseMove) { return; }
            if (!ActiveWindowInfo.IsTriggerByMouse) { return; }//不是由滑鼠觸發的話就跳掉
                                                               //Debug.WriteLine(e.MouseState);
            Timer_GetCurrentWindows.Enabled = false;
            string[] tempMouseBtnDown = new string[ActiveKeys.Length];
            string[] tempMouseBtnUp = new string[ActiveKeys.Length];
            for (int i = 0; i < ActiveKeys.Length; i++)
            {
                //if ( ActiveKeys[i] == Keys.XButton2|| ActiveKeys[i] == Keys.XButton1 || ActiveKeys[i] == Keys.None)
                if (ActiveKeys[i] == Keys.None)
                {
                    tempMouseBtnDown[i] = ActiveKeys[i].ToString();
                    tempMouseBtnUp[i] = ActiveKeys[i].ToString();
                }
                else
                {
                    tempMouseBtnDown[i] = ActiveKeys[i].ToString()[0] + "ButtonDown";
                    tempMouseBtnUp[i] = ActiveKeys[i].ToString()[0] + "ButtonUp";
                }
            }
            for (int i = 0; i < ActiveBtnN; i++)
            {
                if (e.MouseState == GlobalMouseHook.MouseState.XButtonDown && (ActiveKeys[i]==(Keys.XButton1) || ActiveKeys[i]==(Keys.XButton2)))
                {
                    if ((ActiveKeys[i] == Keys.XButton1 && e.MouseData.mouseData.ToString("X") == "10000") || (ActiveKeys[i] == Keys.XButton2 && e.MouseData.mouseData.ToString("X") == "20000"))
                    { IsKeyMousePress[i] = true; }
                }
                else
                {
                    if (e.MouseState.ToString() == tempMouseBtnDown[i] || tempMouseBtnDown[i] == "None") { IsKeyMousePress[i] = true; }
                }
                if (e.MouseState == GlobalMouseHook.MouseState.XButtonUp && (ActiveKeys[i]==(Keys.XButton1) || ActiveKeys[i]==(Keys.XButton2)))
                {
                    if ((ActiveKeys[i] == Keys.XButton1 && e.MouseData.mouseData.ToString("X") == "10000") || (ActiveKeys[i] == Keys.XButton2 && e.MouseData.mouseData.ToString("X") == "20000"))
                    { IsKeyMousePress[i] = false; IsBtnRelease = true; }
                }
                else
                {
                    if (e.MouseState.ToString() == tempMouseBtnUp[i]) { IsKeyMousePress[i] = false; IsBtnRelease = true; }
                }
            }

            if (Fshow.Visible && IsKeyMousePress.Contains(true) && ActiveWindowInfo.IsTypeTrigger == false)
            {
                Fshow.VisibleFalse();
            }
            Debug.WriteLine(IsKeyMousePress[0].ToString() + "\t" + IsKeyMousePress[1].ToString() + "\t" +"放開?"+ IsBtnRelease.ToString() + "\t" + Fshow.Visible.ToString()); ;
            //Debug.WriteLine(IsKeyMousePress[0] + " " + IsKeyMousePress[1]);
            if (!IsKeyMousePress.Contains(false) && Fshow.Visible == false && IsBtnRelease) //如果IsKeyMousePress全部為true
            {
                //IsKeyMousePress = new bool[2] { false, false };
                IsBtnRelease = false;
                ShowButton();
            }
            Timer_GetCurrentWindows.Enabled = true;
            //_globalMouseHook.MousePressed += OnMouseBtnPressed;

        }
        private void ShowButton()
        {
            int tempBtnN;
            try
            { tempBtnN = ActiveWindowInfo.KeysInfo.Length; }
            catch (NullReferenceException) { return; }
            CreateRegion R = new CreateRegion(tempBtnN, ActiveWindowInfo.InOutD);
            Point PT = GetCursorPosition();
            if (IsNeedToModifiedKeys)
            {
                Fshow.ShowButtonAndReturnSelect(tempBtnN, CompletedModifiedKeys, R.RegionsList, PT, _globalMouseHook, _globalKeyboardHook);//秀出按鈕
            }
            else
            {
                Fshow.ShowButtonAndReturnSelect(tempBtnN, ActiveWindowInfo.ActiveKey, R.RegionsList, PT, _globalMouseHook, _globalKeyboardHook);//秀出按鈕
            }
            Fshow.IsImmedAct = ActiveWindowInfo.IsActiveImmed;
            Fshow.IsTypeTrigger = ActiveWindowInfo.IsTypeTrigger;
            Fshow.StartPosition = FormStartPosition.Manual;
            Fshow.Left = PT.X - ActiveWindowInfo.InOutD[1] / 2;
            Fshow.Top = PT.Y - ActiveWindowInfo.InOutD[1] / 2;
            Fshow.TopMost = true;
            try { Fshow.ShowDialog(); }
            catch (System.InvalidOperationException) { return; }
            catch (System.ComponentModel.Win32Exception)//發生了SendInputMsg錯誤
            { return; }
            //Debug.WriteLine(Fshow.ActiveButtonIndex);
            if (IsKeyMousePress[0] == true) { IsKeyMousePress[0] = Fshow.IsKeyMousePress[0]; }//如果在外面還按著，那麼就有可能是在裡面釋放 所以檢查Fshow
            if (IsKeyMousePress[1] == true) { IsKeyMousePress[1] = Fshow.IsKeyMousePress[1]; }
            if (IsBtnRelease == false) { IsBtnRelease = Fshow.IsBtnRelease; } //如果外面還沒放開按鈕 那就去檢查Fshow
            if (Fshow.ActiveButtonIndex == -1) { return; }
            new SentInputData(ActiveWindow, ActiveWindowInfo.KeysInfo[Fshow.ActiveButtonIndex].KeyCode, true);

        }
        public static Point GetCursorPosition()
        {
            Point lpPoint;
            GetCursorPos(out lpPoint);
            // NOTE: If you need error handling
            // bool success = GetCursorPos(out lpPoint);
            // if (!success)
            //Debug.WriteLine(lpPoint.X + "  " + lpPoint.Y);
            return lpPoint;
        }
        private void ListView_DoubleClick(object sender, MouseEventArgs e)
        {
            ListView tempListView = sender as ListView; //操作中的ListView
            ListView.SelectedListViewItemCollection selectedListViewItem = tempListView.SelectedItems;
            string[] SelectItemStr = new string[UsingBtnN + 1];//多一項因為第一項是按鈕編號
            foreach (ListViewItem item in selectedListViewItem)
            {
                for (int i = 0; i < item.SubItems.Count; i++)
                {
                    SelectItemStr[i] = item.SubItems[i].Text;
                }
            }
            if (tempListView.Name.ToLower().Contains("global"))
            {
                CBBX_GlobalBtnSelect.SelectedIndex = Convert.ToInt32(SelectItemStr[0]);
                TBX_Global_Btn1.Text = SelectItemStr[1]; TBX_Global_Btn2.Text = SelectItemStr[2]; TBX_Global_Btn3.Text = SelectItemStr[3];
            }//如果是Global的話
            else
            {
                CBBX_LocalBtnSelect.SelectedIndex = Convert.ToInt32(SelectItemStr[0]);
                TBX_Local_Btn1.Text = SelectItemStr[1]; TBX_Local_Btn2.Text = SelectItemStr[2]; TBX_Local_Btn3.Text = SelectItemStr[3];
            }
        }
        private void ListView_LocalHandleName_DoubleClick(object sender, MouseEventArgs e)
        {
            ListView tempListView = sender as ListView; //操作中的ListView
            int LVLHNindex = tempListView.SelectedIndices[0];
            string[][] tempStr = new string[tempLocalDataInfo[LVLHNindex].KeysInfo.Length][];
            for (int i = 0; i < tempLocalDataInfo[LVLHNindex].KeysInfo.Length; i++)
            {
                tempStr[i] = new string[UsingBtnN + 1];
                tempStr[i][0] = i.ToString("00");
                for (int j = 1; j < UsingBtnN + 1; j++)
                {
                    tempStr[i][j] = tempLocalDataInfo[LVLHNindex].KeysInfo[i].KeyCode[j - 1].ToString();
                }
            }
            new PlotListView(LV_Local_SetupShown, new string[UsingBtnN + 1] { "按鈕", "按鍵1", "按鍵2", "按鍵3" }, new int[UsingBtnN + 1] { 45, 70, 70, 70 }, tempStr);
        }
        private void Btn_LocalHandleNameRefresh_Click(object sender, EventArgs e)
        {
            Refresh_LocalHandleSelect();
        }
    }
}