using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class OriginalDataView : Form
    {
        public string selectrowid = "";//一个全局变量，用于存储修改配置信息的选择行
        public int selectrow = 0;
        static DataTable dt = new DataTable();
        public OriginalDataView()
        {
            InitializeComponent();
            this.dateTimePicker1.Text = DateTime.Now.AddMonths(-3).ToString("yyyy-MM-dd");
            this.dateTimePicker2.Text = DateTime.Now.ToString("yyyy-MM-dd");
            BindTotalData();
        }

        public void BindTotalData() 
        {
            ConfReadAndModify conf=new ConfReadAndModify();
            string sql = "SELECT TurbineID,count(*) FROM " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where status in (" + conf.getConfigSetting("StatusRange") + ")  group by TurbineID";
            DataSet ds_var = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);

            DataTable dtt = new DataTable();
            this.dataGridView2.Columns.Clear();

            DataColumn dc1 = new DataColumn();
            dc1.Caption = "机组";
            dc1.ColumnName = "机组";
            dtt.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn();
            dc2.Caption = "数据量";
            dc2.ColumnName = "数据量";
            dtt.Columns.Add(dc2);

            for (int i = 0; i < ds_var.Tables[0].Rows.Count; i++)
            {
                List<string> list = new List<string>();
                DataRow dr = dtt.NewRow();
                for (int j = 0; j < ds_var.Tables[0].Columns.Count; j++)
                {
                    list.Add(ds_var.Tables[0].Rows[i][j].ToString());
                }
                dr.ItemArray = list.ToArray();
                dtt.Rows.Add(dr);
            }
            this.dataGridView2.DataSource = dtt;
        }


        public void  BindGridView(string starttime,string endtime,string turbineid)
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string sql = "select * from " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where turbineid='" + turbineid + "' and datatime between '" + starttime + "'  and '" + endtime + "' and status in (" + conf.getConfigSetting("StatusRange") + ")";
            if (starttime.Equals("")||endtime.Equals("")||turbineid.Equals(""))
            {
                sql = "select * from " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where status in (" + conf.getConfigSetting("StatusRange") + ")";
            }
            DataSet ds_var = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
            
            this.dataGridView1.Columns.Clear();

            dt.Rows.Clear();
            dt.Columns.Clear();

            for (int i = 0; i < ds_var.Tables[0].Columns.Count; i++)
            {
                DataColumn dc = new DataColumn();
                dc.Caption = ds_var.Tables[0].Columns[i].Caption;
                dc.ColumnName = ds_var.Tables[0].Columns[i].Caption;
                dt.Columns.Add(dc);
            }

            for (int i = 0; i < ds_var.Tables[0].Rows.Count; i++)
            {
                List<string> list = new List<string>();
                DataRow dr = dt.NewRow();
                for (int j = 0; j < ds_var.Tables[0].Columns.Count; j++)
                {
                    list.Add(ds_var.Tables[0].Rows[i][j].ToString());
                }
                dr.ItemArray = list.ToArray();
                dt.Rows.Add(dr);
            }
            this.dataGridView1.DataSource = dt;


        }

        private void button2_Click(object sender, EventArgs e)
        {
            BindGridView("", "", "");
            this.button1.Enabled = true;
        }

        private void dataGridView2_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    this.dataGridView2.CurrentCell = dataGridView2.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    this.dataGridView2.ClearSelection();
                    this.dataGridView2.Rows[e.RowIndex].Selected = true;
                    selectrowid = this.dataGridView2.Rows[e.RowIndex].Cells[0].Value.ToString();
                    selectrow = e.RowIndex;
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        private void 查看机组数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string starttime = DateTime.Parse(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " 00:00:00";
            string endtime = DateTime.Parse(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " 23:59:59";
            BindGridView(starttime, endtime, selectrowid);
            this.button1.Enabled = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "请选择文件夹";
            string filepath = "";
            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                filepath = dilog.SelectedPath;
            }
            string filename = "【" + selectrowid + "】【" + DateTime.Parse(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + "】【" + DateTime.Parse(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + "】"+DateTime.Now.ToString("yyyyMMddHHmmss")+"原始数据.csv";
            string result=ExcelHelper.SaveCSV(dt,filename,filepath);
            MessageBox.Show("结果已成功保存至：\r\n"+result, "操作信息提示", MessageBoxButtons.OK);
        }
    }
}
