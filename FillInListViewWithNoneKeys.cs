using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    public class FillInListViewWithNoneKeys
    {
        public FillInListViewWithNoneKeys(ListView tempListView, int BtnN)
        {
            int tempItemsN = tempListView.Items.Count;
            if (tempItemsN > BtnN)
            {
                for (int i = tempItemsN; i > (int)BtnN; i--)
                {
                    tempListView.Items[i - 1].Remove();
                }

            }
            if (tempItemsN < BtnN)
            {
                if (tempItemsN == 0)//第一次要先建立標題列
                {
                    new PlotListView(tempListView, new string[Form1.UsingBtnN+1] { "按鈕", "按鍵1", "按鍵2","按鈕3" }, new int[Form1.UsingBtnN + 1] { 45, 70, 70,70 }, true);
                }
                for (int i = tempItemsN; i < BtnN; i++)
                {
                    new PlotListView(tempListView, new string[Form1.UsingBtnN + 1] { "按鈕", "按鍵1", "按鍵2","按鈕3" }, new int[Form1.UsingBtnN + 1] { 45, 70, 70 ,70}, new string[Form1.UsingBtnN + 1] { i.ToString("00"), "None", "None","None" }, false);
                }
            }
        }

    }
}
