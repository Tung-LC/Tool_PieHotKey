using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    internal class Load_DataInfoStr_From_File
    {
        public Load_DataInfoStr_From_File()
        {
            OpenFileDialog file1 = new OpenFileDialog()
            {
                Filter = "文字檔 (*.txt)|*.txt|所有檔案 (*.*)|*.*",
                DefaultExt = ".txt"
            };
            if (file1.ShowDialog() == DialogResult.OK)
            {
                StreamReader streamReader = new StreamReader(file1.FileName);
                Returnstr = streamReader.ReadToEnd();
                streamReader.Close();
            }
            else { return; }
        }
        public Load_DataInfoStr_From_File(string Fullpath)
        {
            if (File.Exists(Fullpath))
            {
                StreamReader streamReader = new StreamReader(Fullpath);
                Returnstr = streamReader.ReadToEnd();
                streamReader.Close();
            }
            else
            {
                Returnstr = null;
            }
        }

        public string Returnstr { get; private set; }
    }
}
