using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    internal class Write_Txt_To_File
    {
        internal Write_Txt_To_File(string fileFullPath, string writeTxt)
        {
            if (new FileInfo(fileFullPath).Exists)//此路徑已有檔案
            {
                Form_Question_Msgbox fqm = new Form_Question_Msgbox("已存在預設檔案，要覆蓋或是另存新檔？", "覆蓋", "不覆蓋", "另存新檔");
                fqm.ShowDialog();
                if (fqm.Result_Index == 3)//不進行覆蓋，也就代表另存新檔
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    saveFileDialog.DefaultExt = ".txt";
                    if (saveFileDialog.ShowDialog() == DialogResult.Cancel) //另存新檔又按下取消 則完全不儲存
                    {
                        return;
                    }
                    else //否則將路徑更新到新的
                    {
                        fileFullPath = saveFileDialog.FileName;
                    }
                }
                if (fqm.Result_Index == 2) //不覆蓋不另存
                {
                    return;
                }
            }
            FileInfo fileInfo = new FileInfo(fileFullPath);
            StreamWriter sw = fileInfo.CreateText();
            sw.Write(writeTxt); sw.Flush(); sw.Close();
        }
        internal Write_Txt_To_File(string fileFullPath, List<ListViewData_To_DataInfo.DataInfo> allDataInfo)
        {
            if (new FileInfo(fileFullPath).Exists)//此路徑已有檔案
            {
                Form_Question_Msgbox fqm = new Form_Question_Msgbox("已存在預設檔案，要覆蓋或是另存新檔？", "覆蓋", "不覆蓋", "另存新檔");
                fqm.ShowDialog();
                if (fqm.Result_Index == 3)//不進行覆蓋，也就代表另存新檔
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    saveFileDialog.DefaultExt = ".txt";
                    if (saveFileDialog.ShowDialog() == DialogResult.Cancel) //另存新檔又按下取消 則完全不儲存
                    {
                        return;
                    }
                    else //否則將路徑更新到新的
                    {
                        fileFullPath = saveFileDialog.FileName;
                    }
                }
                if (fqm.Result_Index == 2) //不覆蓋不另存
                {
                    return;
                }
            }
            FileInfo fileInfo = new FileInfo(fileFullPath);
            StreamWriter sw = fileInfo.CreateText();
            string tempstr = "";
            for (int i = 0; i < allDataInfo.Count; i++)
            {

                tempstr += WriteTostr(allDataInfo[i]);
                if (i == allDataInfo.Count - 1)
                { }
                else
                { tempstr += "------------------------EndSection------------------------" + "\r\n"; }
            }
            sw.Write(tempstr); sw.Flush(); sw.Close();
        }
        internal Write_Txt_To_File(string fileFullPath, List<ListViewData_To_DataInfo.DataInfo> allDataInfo,List<IntPtr> handleInfo)//需要修改Handle參數
        {
            if (new FileInfo(fileFullPath).Exists)//此路徑已有檔案
            {
                Form_Question_Msgbox fqm = new Form_Question_Msgbox("已存在預設檔案，要覆蓋或是另存新檔？", "覆蓋", "不覆蓋", "另存新檔");
                fqm.ShowDialog();
                if (fqm.Result_Index == 3)//不進行覆蓋，也就代表另存新檔
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog();
                    saveFileDialog.Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*";
                    saveFileDialog.DefaultExt = ".txt";
                    if (saveFileDialog.ShowDialog() == DialogResult.Cancel) //另存新檔又按下取消 則完全不儲存
                    {
                        return;
                    }
                    else //否則將路徑更新到新的
                    {
                        fileFullPath = saveFileDialog.FileName;
                    }
                }
                if (fqm.Result_Index == 2) //不覆蓋不另存
                {
                    return;
                }
            }
            FileInfo fileInfo = new FileInfo(fileFullPath);
            StreamWriter sw = fileInfo.CreateText();
            string tempstr = "";
            for (int i = 0; i < allDataInfo.Count; i++)
            {
                tempstr += WriteTostr(allDataInfo[i], handleInfo[i]);
                if (i == allDataInfo.Count - 1)
                { }
                else
                { tempstr += "------------------------EndSection------------------------" + "\r\n"; }
            }
            sw.Write(tempstr); sw.Flush(); sw.Close();
        }
        public string WriteTostr(ListViewData_To_DataInfo.DataInfo dataInfo)
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
        public string WriteTostr(ListViewData_To_DataInfo.DataInfo dataInfo,IntPtr newhandle)
        {

            string tempstr = "";
            tempstr += "HandleName: " + dataInfo.HandleName + "\r\n";
            tempstr += "Handle: " + newhandle.ToString("X") + "\r\n";
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
