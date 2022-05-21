using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tool_PieHotKey
{
    public class PlotListView
    {
        /// <summary>
        /// 重新繪製ListView 會將ListView內所有數據清空
        /// </summary>
        /// <param name="templistview">要繪製的ListView</param>
        /// <param name="columnhead">ListView標題</param>
        /// <param name="colwid">每個標題寬</param>
        /// <param name="Datainput">填入的資料</param>
        public PlotListView(ListView templistview, string[] columnhead, int[] colwid, string[][] Datainput)
        {
            templistview.BeginUpdate();
            templistview.Clear();
            #region 開始更新ListView 並設定ListView維Detail樣式
            templistview.GridLines = true;
            templistview.View = View.Details;
            templistview.HeaderStyle = ColumnHeaderStyle.Clickable;
            //templistview.OwnerDraw = true;
            #endregion
            #region 建立標題列
            //修改標題列的寬度
            if (colwid.Length != columnhead.Length) //改變column head的寬度
            {
                int[] tempMX = new int[columnhead.Length];
                for (int i = 0; i < columnhead.Length; i++)
                {
                    if (i > colwid.Length - 1) //已經沒有colwid的參數可以取得了
                    {
                        tempMX[i] = colwid[colwid.Length - 1];
                    }
                    else
                    {
                        tempMX[i] = colwid[i];
                    }
                }
                colwid = tempMX;
            }

            //ListView templistview = (ListView)this.Controls[templistviewstr];
            for (int i = 0; i < columnhead.Length; i++)
            {
                templistview.Columns.Add(columnhead[i], colwid[i]); //先建立標題列
            }
            #endregion
            #region 判斷是否有數據要填入ListView 沒有的話則離開
            if (columnhead.Length > Datainput[0].Length) //沒有數據需要繪製，或是columnhead多於DataInput
            {
                templistview.EndUpdate();
                return;
            }
            #endregion
            #region 將數據放入ListView
            for (int i = 0; i < Datainput.Length; i++)
            {
                string[] row = new string[columnhead.Length];
                for (int j = 0; j < columnhead.Length; j++)
                {
                    row[j] = Datainput[i][j];
                }
                var listViewItem = new ListViewItem(row);
                templistview.Items.Add(listViewItem);
            }
            #endregion
            templistview.Sorting = System.Windows.Forms.SortOrder.None;
            templistview.FullRowSelect = true;
            templistview.EndUpdate();
        }
        /// <summary>
        /// 堆疊ListView 不會將ListView內所有數據清空
        /// </summary>
        /// <param name="templistview">要繪製的ListView</param>
        /// <param name="columnhead">ListView標題</param>
        /// <param name="colwid">每個標題寬</param>
        /// <param name="Datainput">填入的資料</param>
        /// <param name="IsAddcolumnhead">是否要加入標題列 僅限第一次</param>
        public PlotListView(ListView templistview, string[] columnhead, int[] colwid, string[][] Datainput, bool IsAddcolumnhead)
        {
            templistview.BeginUpdate();
            #region 開始更新ListView 並設定ListView維Detail樣式
            templistview.GridLines = true;
            templistview.View = View.Details;
            templistview.HeaderStyle = ColumnHeaderStyle.Clickable;
            //templistview.OwnerDraw = true;
            #endregion
            #region 建立標題列
            //修改標題列的寬度
            if (IsAddcolumnhead)
            {
                if (colwid.Length != columnhead.Length) //改變column head的寬度
                {
                    int[] tempMX = new int[columnhead.Length];
                    for (int i = 0; i < columnhead.Length; i++)
                    {
                        if (i > colwid.Length - 1) //已經沒有colwid的參數可以取得了
                        {
                            tempMX[i] = colwid[colwid.Length - 1];
                        }
                        else
                        {
                            tempMX[i] = colwid[i];
                        }
                    }
                    colwid = tempMX;
                }
            }
            //ListView templistview = (ListView)this.Controls[templistviewstr];
            for (int i = 0; i < columnhead.Length; i++)
            {
                templistview.Columns.Add(columnhead[i], colwid[i]); //先建立標題列
            }
            #endregion
            #region 判斷是否有數據要填入ListView 沒有的話則離開
            if (columnhead.Length > Datainput[0].Length) //沒有數據需要繪製，或是columnhead多於DataInput
            {
                templistview.EndUpdate();
                return;
            }
            #endregion
            #region 將數據放入ListView
            for (int i = 0; i < Datainput.Length; i++)
            {
                string[] row = new string[columnhead.Length];
                for (int j = 0; j < columnhead.Length; j++)
                {
                    row[j] = Datainput[i][j];
                }
                var listViewItem = new ListViewItem(row);
                templistview.Items.Add(listViewItem);
            }
            #endregion
            templistview.Sorting = System.Windows.Forms.SortOrder.None;
            templistview.FullRowSelect = true;
            templistview.EndUpdate();
        }
        /// <summary>
        /// 堆疊ListView 可以選擇是否要新增標題列(建議 AddColumn = true只在ListView是乾淨的情況下使用)
        /// </summary>
        /// <param name="templistview"></param>
        /// <param name="columnhead"></param>
        /// <param name="colwid"></param>
        /// <param name="Datainput"></param>
        /// <param name="AddColumn"></param>
        public PlotListView(ListView templistview, string[] columnhead, int[] colwid, string[] Datainput, bool AddColumn)
        {
            templistview.BeginUpdate();
            //templistview.Clear();
            #region 開始更新ListView 並設定ListView維Detail樣式
            templistview.GridLines = true;
            templistview.View = View.Details;
            templistview.HeaderStyle = ColumnHeaderStyle.Clickable;
            //templistview.OwnerDraw = true;
            #endregion
            if (AddColumn)
            {
                #region 建立標題列
                //修改標題列的寬度
                if (colwid.Length != columnhead.Length) //改變column head的寬度
                {
                    int[] tempMX = new int[columnhead.Length];
                    for (int i = 0; i < columnhead.Length; i++)
                    {
                        if (i > colwid.Length - 1) //已經沒有colwid的參數可以取得了
                        {
                            tempMX[i] = colwid[colwid.Length - 1];
                        }
                        else
                        {
                            tempMX[i] = colwid[i];
                        }
                    }
                    colwid = tempMX;
                }

                //ListView templistview = (ListView)this.Controls[templistviewstr];
                for (int i = 0; i < columnhead.Length; i++)
                {
                    templistview.Columns.Add(columnhead[i], colwid[i]); //先建立標題列
                }
                #endregion
            }
            #region 判斷是否有數據要填入ListView 沒有的話則離開
            if (columnhead.Length > Datainput.Length) //沒有數據需要繪製，或是columnhead多於DataInput
            {
                templistview.EndUpdate();
                return;
            }
            #endregion
            #region 將數據放入ListView

            var listViewItem = new ListViewItem(Datainput);
            templistview.Items.Add(listViewItem);


            #endregion
            templistview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            templistview.FullRowSelect = true;
            templistview.EndUpdate();
        }
        /// <summary>
        /// 堆疊ListView的數據 可以選擇是否要建立標題列(僅限第一次) 並且ListView必須是想要變成直的 若是DataInputIsColumn為否 則不動作
        /// </summary>
        /// <param name="templistview"></param>
        /// <param name="columnhead"></param>
        /// <param name="colwid"></param>
        /// <param name="Datainput"></param>
        /// <param name="AddColumn"></param>
        /// <param name="DataInputIsColumn"></param>
        public PlotListView(ListView templistview, string[] columnhead, int[] colwid, string[] Datainput, bool AddColumn, bool DataInputIsColumn, bool IsClearListView)
        {
            if (!DataInputIsColumn)
            {
                return;
            }
            templistview.BeginUpdate();
            if (IsClearListView)
            {
                templistview.Clear();
            }
            //templistview.Clear();
            #region 開始更新ListView 並設定ListView維Detail樣式
            templistview.GridLines = true;
            templistview.View = View.Details;
            templistview.HeaderStyle = ColumnHeaderStyle.Clickable;
            //templistview.OwnerDraw = true;
            #endregion
            if (AddColumn)
            {
                #region 建立標題列
                //修改標題列的寬度
                if (colwid.Length != columnhead.Length) //改變column head的寬度
                {
                    int[] tempMX = new int[columnhead.Length];
                    for (int i = 0; i < columnhead.Length; i++)
                    {
                        if (i > colwid.Length - 1) //已經沒有colwid的參數可以取得了
                        {
                            tempMX[i] = colwid[colwid.Length - 1];
                        }
                        else
                        {
                            tempMX[i] = colwid[i];
                        }
                    }
                    colwid = tempMX;
                }

                //ListView templistview = (ListView)this.Controls[templistviewstr];
                for (int i = 0; i < columnhead.Length; i++)
                {
                    templistview.Columns.Add(columnhead[i], colwid[i]); //先建立標題列
                }
                #endregion
            }
            #region 判斷是否有數據要填入ListView 沒有的話則離開
            if (columnhead.Length > Datainput.Length) //沒有數據需要繪製，或是columnhead多於DataInput
            {
                templistview.EndUpdate();
                return;
            }
            #endregion
            #region 將數據放入ListView
            for (int i = 0; i < Datainput.Length; i++)
            {
                var listViewItem = new ListViewItem(Datainput[i]);
                templistview.Items.Add(listViewItem);
            }

            #endregion
            templistview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            templistview.FullRowSelect = true;
            templistview.EndUpdate();
        }
        /// <summary>
        /// 僅建立標題列，若AddColumn為false 則完全不動作
        /// </summary>
        /// <param name="templistview"></param>
        /// <param name="columnhead"></param>
        /// <param name="colwid"></param>
        /// <param name="AddColumn"></param>
        public PlotListView(ListView templistview, string[] columnhead, int[] colwid, bool AddColumn)
        {
            templistview.BeginUpdate();
            //templistview.Clear();
            #region 開始更新ListView 並設定ListView維Detail樣式
            templistview.GridLines = true;
            templistview.View = View.Details;
            templistview.HeaderStyle = ColumnHeaderStyle.Clickable;
            //templistview.OwnerDraw = true;
            #endregion
            if (AddColumn)
            {
                #region 建立標題列
                //修改標題列的寬度
                if (colwid.Length != columnhead.Length) //改變column head的寬度
                {
                    int[] tempMX = new int[columnhead.Length];
                    for (int i = 0; i < columnhead.Length; i++)
                    {
                        if (i > colwid.Length - 1) //已經沒有colwid的參數可以取得了
                        {
                            tempMX[i] = colwid[colwid.Length - 1];
                        }
                        else
                        {
                            tempMX[i] = colwid[i];
                        }
                    }
                    colwid = tempMX;
                }

                //ListView templistview = (ListView)this.Controls[templistviewstr];
                for (int i = 0; i < columnhead.Length; i++)
                {
                    templistview.Columns.Add(columnhead[i], colwid[i]); //先建立標題列
                }
                #endregion
            }
            templistview.Sorting = System.Windows.Forms.SortOrder.Ascending;
            templistview.FullRowSelect = true;
            templistview.EndUpdate();
        }
    }
}
