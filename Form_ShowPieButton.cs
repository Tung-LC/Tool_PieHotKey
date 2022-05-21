using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Input;
using System.Diagnostics;

namespace Tool_PieHotKey
{
    public partial class Form_ShowPieButton : Form
    {
        public Form_ShowPieButton(Button[] btnlist, int[] IOR, int maxBtnN, bool isImmedAct, bool triggerTypeTrig)
        {
            ButtonsList = btnlist;
            InOutD = IOR;
            MaxBtnN = maxBtnN;
            IsImmedAct = isImmedAct;
            IsTypeTrigger = triggerTypeTrig;
            {
                for (int i = 0; i < MaxBtnN; i++) //將按鈕修改成透明
                {
                    ButtonsList[i].BackColor = Color.WhiteSmoke;
                    ButtonsList[i].ForeColor = Color.Transparent;
                    ButtonsList[i].FlatAppearance.MouseDownBackColor = Color.AliceBlue;
                    ButtonsList[i].FlatAppearance.MouseOverBackColor = Color.AliceBlue;
                }
            }
            InitializeComponent();
            //建立所有按鈕
            this.Controls.AddRange(ButtonsList);
            this.Height = InOutD[1] + 50; //調整視窗大小
            this.Width = InOutD[1] + 50;
            this.BackColor = Color.Black;
            this.TransparencyKey = Color.Black;
            this.Visible = false;
            ButtonsName = ButtonsList.Select(x => x.Name).ToArray();
           
            //_globalKeyboardHook = new GlobalKeyboardHook();
            //_globalMouseHook = new GlobalMouseHook();
        }
        public Button[] ButtonsList { get; private set; }
        public bool IsTypeTrigger { get; set; }
        public int[] InOutD { get; private set; }
        public bool IsImmedAct { get; set; }
        public int MaxBtnN;
        public string[] ButtonsName;
        //private GlobalKeyboardHook _globalKeyboardHook;
        //private GlobalMouseHook _globalMouseHook;
        /// <summary>
        /// <param name="ActiveButtonName">有觸碰或點選到的按鈕名稱 從0開始排序</param>
        /// </summary>
        public string ActiveButtonName { get; private set; }
        /// <summary>
        /// <param name="ActiveButtonIndex">有觸碰或點選的按鈕是全部按鈕中的第幾個(0號按鈕 index = 0)</param>
        /// </summary>
        public int ActiveButtonIndex { get; private set; }
        public int temp_BtnN { get; private set; }
        public Keys[] ActiveKeys { get; private set; }
        private static GlobalKeyboardHook globalKeyBoardhook2;
        private static GlobalMouseHook globalMousehook2;
        public bool[] IsKeyMousePress;
        public bool IsBtnRelease = true; 
        //public MouseButtons[] ActiveMouseButton { get; private set; }
        /// <summary>
        /// 顯示出Pie狀按鈕並回傳啟動值，按下滑鼠右鍵則立刻停止
        /// </summary>
        /// <param name="IsActImedd">是否選擇即動，也就是當按鈕放掉時就直接回傳結果，不等待按鈕點擊</param>
        /// <param name="BtnN">總共有幾個按鈕要操作</param>
        /// <param name="Key1">啟動的按鈕為A+B的A</param>
        /// <param name="Key2">啟動的按鈕為A+B的B，若啟動鈕為單項，則此項為null</param>
        /// <returns></returns>
        internal void ShowButtonAndReturnSelect(int BtnN, Keys[] activeKeys, Region[] RegionList, Point MousePoint
            , GlobalMouseHook GMH, GlobalKeyboardHook GKBH)
        {
            ActiveButtonIndex = -1;
            ActiveButtonName = null;
            temp_BtnN = BtnN;
            ActiveKeys = activeKeys;
            IsKeyMousePress = new bool[Form1.ActiveBtnN] { true,true };
            IsBtnRelease = false;
            for (int i = 0; i < BtnN; i++)
            {
                ButtonsList[i].Visible = true;
                ButtonsList[i].Region = RegionList[i];
                //Debug.WriteLine(ButtonsList[i].Name);
            }
            this.Left = MousePoint.X - InOutD[1] / 2 - 5;
            this.Top = MousePoint.Y - InOutD[1] / 2 - 5;
            globalMousehook2 = GMH;
            globalKeyBoardhook2 = GKBH;
            if (!IsTypeTrigger)//按鈕視窗是否 不為 一直出現 == 按鈕視窗出現要按著按鈕才會顯現
            {
                globalKeyBoardhook2.KeyboardPressed += Form_ShowPieButton_KeyUp;
                globalMousehook2.MousePressed += Form_ShowPieButton_MouseUp;
                IsBtnRelease = false;
            }
            else
            {
                globalMousehook2.MousePressed += Form_ShowPieButton_MouseClick;
            }
            AddRemove_IsImmedActEvent(true);

        }
        private void AddRemove_IsImmedActEvent(bool IsAdd)
        {
            //Debug.WriteLine("INADD" + IsImmedAct.ToString());
            if (IsAdd)
            {
                if (IsImmedAct)//是否即動，是的話就使用Hover 否的話就使用Click
                {
                    for (int i = 0; i < MaxBtnN; i++)
                    {
                        //ButtonsList[i].MouseHover += ButtonHoverEvent;
                        ButtonsList[i].MouseMove += ButtonMoveEvent;
                    }
                }
                else
                {
                    for (int i = 0; i < MaxBtnN; i++)
                    {
                        ButtonsList[i].MouseClick += ButtonClickEvent;
                    }
                }
            }
            else
            {
                if (IsImmedAct)//是否即動，是的話就使用Hover 否的話就使用Click
                {
                    for (int i = 0; i < MaxBtnN; i++)
                    {
                        //ButtonsList[i].MouseHover += ButtonHoverEvent;
                        ButtonsList[i].MouseMove -= ButtonMoveEvent;
                    }
                }
                else
                {
                    for (int i = 0; i < MaxBtnN; i++)
                    {
                        ButtonsList[i].MouseClick -= ButtonClickEvent;
                    }
                }
            }
        }
        /// <summary>
        /// 當滑鼠移動到按鈕上進行的動作，將有動作之滑鼠名稱紀錄，若有其他的則持續覆蓋掉
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonHoverEvent(object sender, EventArgs e)
        {
            ActiveButtonName = ((Button)sender).Name;
            ActiveButtonIndex = Array.IndexOf(ButtonsName, ActiveButtonName);
            this.Hide();

        }
        private void ButtonClickEvent(object sender, EventArgs e)
        {
            ActiveButtonName = ((Button)sender).Name;
            ActiveButtonIndex = Array.IndexOf(ButtonsName, ActiveButtonName);
            VisibleFalse();
            this.Hide();
        }
        private void ButtonMoveEvent(object sender, EventArgs e)
        {
            ActiveButtonName = ((Button)sender).Name;
            ActiveButtonIndex = Array.IndexOf(ButtonsName, ActiveButtonName);
            VisibleFalse();
            this.Hide();
        }
        public void VisibleFalse()
        {
            for (int i = 0; i < temp_BtnN; i++)
            {
                ButtonsList[i].Visible = false;
            }
            if (!IsTypeTrigger)//按鈕視窗是否 不為 一直出現 == 按鈕視窗出現要按著按鈕才會顯現
            {
                globalKeyBoardhook2.KeyboardPressed -= Form_ShowPieButton_KeyUp;
                globalMousehook2.MousePressed -= Form_ShowPieButton_MouseUp;
            }
            else
            {
                globalMousehook2.MousePressed -= Form_ShowPieButton_MouseClick;
            }
            AddRemove_IsImmedActEvent(false);
            this.Visible = false;
        }

        private void Form_ShowPieButton_KeyUp(object sender, GlobalKeyboardHookEventArgs e)
        {
            bool ToBeClose = false;

            if (e.KeyboardState == GlobalKeyboardHook.KeyboardState.KeyUp)
            {
                Keys loggedKey = e.KeyboardData.Key;
                for (int i = 0; i < ActiveKeys.Length; i++)
                {
                    if (loggedKey == ActiveKeys[i])
                    {
                        ToBeClose = true;
                        IsKeyMousePress[i] = false;
                        IsBtnRelease = true;
                        break;
                    }
                }

            }
            if (ToBeClose)
            {
                ActiveButtonIndex = -1;
                ActiveButtonName = null;
                VisibleFalse();
                this.Hide();
            }
        }
        private void Form_ShowPieButton_MouseUp(object sender, GlobalMouseHookEventArgs e)
        {
            bool ToBeClose = false;


            for (int i = 0; i < ActiveKeys.Length; i++)
            {
                if (e.MouseState.ToString() == ActiveKeys[i].ToString()[0] + "ButtonUp")
                {
                    ToBeClose = true;
                    IsKeyMousePress[i] = false;
                    IsBtnRelease =true;
                    break;
                }

            }
            if (ToBeClose)
            {
                ActiveButtonIndex = -1;
                ActiveButtonName = null;
                VisibleFalse();
                this.Hide();
            }
        }
        private void Form_ShowPieButton_MouseClick(object sender, GlobalMouseHookEventArgs e)
        {
            if (e.MouseState == GlobalMouseHook.MouseState.RButtonDown)
            {
                ActiveButtonIndex = -1;
                ActiveButtonName = null;
                VisibleFalse();
                this.Hide();
            }

        }
    }
}
