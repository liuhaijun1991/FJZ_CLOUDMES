using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MESDataObject.Common;
using System.Data;

namespace MESReport
{
    public class ReportTable : ReportOutputBase
    {
        public String OutputType
        {
            get
            {
                return "ReportTable";
            }
        }

        /// <summary>
        /// Title of the data table
        /// </summary>
        public string Tittle = "";

        /// <summary>
        /// ???
        /// </summary>
        public List<TableRowView> Rows = new List<TableRowView>();

        [Obsolete("Please use ReportTable.TableHeaders", false)]
        /// <summary>
        /// Data field list, used to create table header
        /// </summary>
        public List<string> ColNames = new List<string>();

        /// <summary>
        /// Specify the search field
        /// </summary>
        public List<string> SearchCol = new List<string>();

        private bool _MergeCells = false;
        /// <summary>
        /// Merge Cells With RowSpan & ColSpan;
        /// Can't be used with pagination at the same time
        /// </summary>
        public bool MergeCells
        {
            get { return this._MergeCells; }
            set
            {
                this._MergeCells = value;
                if (value)
                {
                    this.pagination = !value;
                }
            }
        }

        /// <summary>
        /// Fixed Header
        /// </summary>
        public bool FixedHeader = false;

        /// <summary>
        /// Fixed number of columns from the left
        /// </summary>
        public int FixedCol = 0;

        private bool _pagination = true;
        /// <summary>
        /// Whether the table has pagination
        /// </summary>
        public bool pagination
        {
            get { return this._pagination; }
            set
            {
                this._pagination = value;
                if (value)
                {
                    this._MergeCells = !value;
                }
            }
        }

        /// <summary>
        /// Paging on the server
        /// </summary>
        public bool PaginationServer = false;

        /// <summary>
        /// Total number of data rows
        /// </summary>
        public int TotalRows = 0;

        /// <summary>
        /// 針對不需要樣式、鏈接，單純只有數據的報表
        /// </summary>
        /// <param name="dt"></param>
        public void LoadData(DataTable dt)
        {
            LoadData(dt, null);
        }

        public List<List<TableHeader>> TableHeaders = new List<List<TableHeader>>();

        /// <summary>
        /// 針對有樣式、鏈接的報表，樣式和鏈接放在第二個參數裡面
        /// </summary>
        /// <param name="DataT"></param>
        /// <param name="DataL"></param>
        public void LoadData(System.Data.DataTable DataT, System.Data.DataTable DataL = null)
        {
            List<TableHeader> hs = new List<TableHeader>();

            if (this.TableHeaders.Count == 0)
            {
                //List<TableHeader> hs = new List<TableHeader>();
                this.TableHeaders.Add(hs);
            }
            ColNames.Clear();
            for (int i = 0; i < DataT.Columns.Count; i++)
            {
                ColNames.Add(DataT.Columns[i].ColumnName);
                TableHeader h = new TableHeader();
                h.title = DataT.Columns[i].ColumnName;
                h.field = DataT.Columns[i].ColumnName;
                hs.Add(h);
            }
            Rows.Clear();
            for (int i = 0; i < DataT.Rows.Count; i++)
            {
                TableRowView row = new TableRowView();
                Rows.Add(row);
                for (int j = 0; j < ColNames.Count; j++)
                {

                    TableColView Item = new TableColView() { Value = DataT.Rows[i][j].ToString() };
                    if (DataL != null)
                    {
                        string[] LinkDatas = DataL.Rows[i][j].ToString().Split('#');
                        if (LinkDatas.Length > 1)
                            Item.LinkData = LinkDatas[1];
                        else
                            Item.LinkData = LinkDatas[0];
                        if (DataL.Rows[i][j].ToString() != "" && LinkDatas[0].Equals("Link"))
                            Item.LinkType = "Link";
                        else if (DataL.Rows[i][j].ToString() != "" && LinkDatas[0].Equals("Report"))
                            Item.LinkType = "Report";
                        else if (DataL.Rows[i][j].ToString() != "" && LinkDatas[0].Equals("Attachment"))
                            Item.LinkType = "Attachment";
                        else if (DataL.Rows[i][j].ToString() != "")
                            Item.LinkType = "Report";
                    }
                    row.Add(ColNames[j], Item);
                }
            }
        }

        /// <summary>
        /// 執行分頁
        /// </summary>
        /// <param name="data_all">待分頁的table</param>
        /// <param name="page_number">number</param>
        /// <param name="page_size">size</param>
        public void MakePagination(DataTable data_report, DataTable data_link, int page_number, int page_size)
        {
            List<TableHeader> hs = new List<TableHeader>();
            if (this.TableHeaders.Count == 0)
            {

                this.TableHeaders.Add(hs);
            }
            PaginationServer = true;
            TotalRows = data_report.Rows.Count;
            int totalPage = (TotalRows / page_size) + (TotalRows % page_size > 0 ? 1 : 0);
            int currnetPage = page_number;
            currnetPage = (currnetPage > totalPage ? totalPage : currnetPage);
            currnetPage = (currnetPage <= 0 ? 1 : currnetPage);
            DataTable currentPageTable = data_report.Clone();
            DataTable currentLikTable = (data_link == null) ? null : data_link.Clone();
            int rowBegin = (currnetPage - 1) * page_size;
            int rowEnd = (currnetPage * page_size > TotalRows) ? TotalRows : currnetPage * page_size;
            DataRow newRow = null;
            DataRow oldRow = null;

            DataRow newLinkRow = null;
            DataRow oldLinkRow = null;
            for (int i = rowBegin; i <= rowEnd - 1; i++)
            {
                newRow = currentPageTable.NewRow();
                oldRow = data_report.Rows[i];
                foreach (DataColumn column in data_report.Columns)
                {
                    newRow[column.ColumnName] = oldRow[column.ColumnName];
                    TableHeader h = new TableHeader();
                    h.title = column.ColumnName;
                    h.field = column.ColumnName;
                    hs.Add(h);
                }
                currentPageTable.Rows.Add(newRow);

                if (data_link != null)
                {
                    newLinkRow = currentLikTable.NewRow();
                    oldLinkRow = data_link.Rows[i];
                    foreach (DataColumn column in data_link.Columns)
                    {
                        newLinkRow[column.ColumnName] = oldLinkRow[column.ColumnName];
                    }
                    currentLikTable.Rows.Add(newLinkRow);
                }
            }
            LoadData(currentPageTable, currentLikTable);
        }
    }

    public class TableColView
    {
        public string ColName;
        public string Value;
        public string LinkType;
        public string LinkData;
        public Dictionary<string, object> CellStyle = new Dictionary<string, object>();
        public string FontStyle;
        public int RowSpan = 1;
        public int ColSpan = 1;
        public String OutputType
        {
            get
            {
                return "TableColView";
            }
        }
    }
    public class TableRowView : IDictionary<string, TableColView>
    {
        public String OutputType
        {
            get
            {
                return "TableRowView";
            }
        }
        public Dictionary<string, TableColView> RowData = new Dictionary<string, TableColView>();
        public string RowStyle = "";

        public TableColView this[string key]
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData)[key];
            }

            set
            {
                ((IDictionary<string, TableColView>)RowData)[key] = value;
            }
        }

        public int Count
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).IsReadOnly;
            }
        }

        public ICollection<string> Keys
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Keys;
            }
        }

        public ICollection<TableColView> Values
        {
            get
            {
                return ((IDictionary<string, TableColView>)RowData).Values;
            }
        }

        public void Add(KeyValuePair<string, TableColView> item)
        {
            ((IDictionary<string, TableColView>)RowData).Add(item);
        }

        public void Add(string key, TableColView value)
        {
            ((IDictionary<string, TableColView>)RowData).Add(key, value);
        }

        public void Clear()
        {
            ((IDictionary<string, TableColView>)RowData).Clear();
        }

        public bool Contains(KeyValuePair<string, TableColView> item)
        {
            return ((IDictionary<string, TableColView>)RowData).Contains(item);
        }

        public bool ContainsKey(string key)
        {
            return ((IDictionary<string, TableColView>)RowData).ContainsKey(key);
        }

        public void CopyTo(KeyValuePair<string, TableColView>[] array, int arrayIndex)
        {
            ((IDictionary<string, TableColView>)RowData).CopyTo(array, arrayIndex);
        }

        public IEnumerator<KeyValuePair<string, TableColView>> GetEnumerator()
        {
            return ((IDictionary<string, TableColView>)RowData).GetEnumerator();
        }

        public bool Remove(KeyValuePair<string, TableColView> item)
        {
            return ((IDictionary<string, TableColView>)RowData).Remove(item);
        }

        public bool Remove(string key)
        {
            return ((IDictionary<string, TableColView>)RowData).Remove(key);
        }

        public bool TryGetValue(string key, out TableColView value)
        {
            return ((IDictionary<string, TableColView>)RowData).TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IDictionary<string, TableColView>)RowData).GetEnumerator();
        }
    }

    public class TableHeader
    {
        /// <summary>
        /// 向下合并行數量
        /// </summary>
        public int rowspan { get; set; } = 0;
        /// <summary>
        /// 向右合并單元格數量
        /// </summary>
        public int colspan { get; set; } = 0;
        /// <summary>
        /// 取值字段名
        /// </summary>
        public string field { get; set; }
        /// <summary>
        /// 表頭顯示
        /// </summary>
        public string title { get; set; }
        /// <summary>
        /// 是否可搜索
        /// </summary>
        public bool searchable { get; set; } = true;
        /// <summary>
        /// 是否可排序
        /// </summary>
        public bool sortable { get; set; } = false;

    }


}
