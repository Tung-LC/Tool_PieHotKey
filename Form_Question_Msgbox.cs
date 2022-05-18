using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    public partial class Form_Question_Msgbox : Form
    {
        public Form_Question_Msgbox()
        {
            InitializeComponent();
        }
        public Form_Question_Msgbox(string msg,string btn1,string btn2,string btn3)
        {
            InitializeComponent();
            Str_ASK = msg; Str_btn1 = btn1;Str_btn2 = btn2;Str_btn3 = btn3;
        }
        public Form_Question_Msgbox(string msg, string btn1, string btn2)
        {
            InitializeComponent();
            Str_ASK = msg; Str_btn1 = btn1; Str_btn2 = btn2;
            btn_3.Visible = false;
            btn_1.Size = new Size(100, 23);
            btn_2.Size = new Size(100, 23);
            btn_1.Location = new Point(25, 60);
            btn_2.Location = new Point(175, 60);
        }

        int _Result_Index = 0; //設定初始值為取消
        string _str_btndel = "刪除";
        string _str_btnedit = "編輯";
        string _str_btncancel = "取消";
        string _str_Ask= "刪除此項目或是編輯此項目？";
        bool _Is_btn1_Active=true;
        float _FontSizeAsk = 15.75F;
        public float FontSizeAsk { set { _FontSizeAsk = value; } get { return _FontSizeAsk; } }
        public int Result_Index
        {
            get { return _Result_Index; }
            set { _Result_Index = value; }
        }
        public string Str_btn1
        {
            get { return _str_btndel; }
            set { _str_btndel = value; }
        }
        public bool Is_Button1_Active
        {
            set { _Is_btn1_Active = value; }
        }
        public string Str_btn2
        {
            get { return _str_btnedit; }
            set { _str_btnedit = value; }
        }
        public string Str_btn3
        {
            get { return _str_btncancel; }
            set { _str_btncancel = value; }
        }
        public string Str_ASK
        {
            get { return _str_Ask; }
            set { _str_Ask = value; }
        }
        private void Button_Click(object sender, EventArgs e)
        {
            string btn_Name = (sender as Button).Name;
            if (btn_Name == "btn_1")
            {
                _Result_Index = 1;
            }
            else if (btn_Name == "btn_2")
            {
                _Result_Index = 2;
            }
            else
            {
                _Result_Index = 3;
            }
            this.Close();
        }

        private void Edit_Or_Delete_Items_Load(object sender, EventArgs e)
        {
            this.CenterToParent();
            label1.Text = _str_Ask;
            label1.Font = new Font("標楷體", FontSizeAsk, FontStyle.Regular, GraphicsUnit.Point);
            btn_3.Text = _str_btncancel;
            btn_1.Text = _str_btndel;
            btn_2.Text = _str_btnedit;
            btn_3.Focus();
            btn_1.Enabled = _Is_btn1_Active;
        }
    }
}
