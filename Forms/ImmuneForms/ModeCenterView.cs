using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;

namespace TurbineMonitoringVarableAnalysis.Forms.ImmuneForms
{
    public partial class ModeCenterView : Form
    {
        public string selectrowid = "";//一个全局变量，用于存储修改配置信息的选择行
        public int selectrow = 0;

        static DataTable dt = new DataTable();
        public ModeCenterView()
        {
            InitializeComponent();
            BindTotalData();
        }

        public void BindTotalData()
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string sql = "select distinct turbineID from turbine_train_model";
            //string sql = "SELECT TurbineID,count(*) FROM " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where status in (" + conf.getConfigSetting("StatusRange") + ")  group by TurbineID";
            DataSet ds_var = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);

            DataTable dtt = new DataTable();
            this.dataGridView2.Columns.Clear();

            DataColumn dc1 = new DataColumn();
            dc1.Caption = "机组";
            dc1.ColumnName = "机组";
            dtt.Columns.Add(dc1);

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


        private void button2_Click(object sender, EventArgs e)
        {
            BindGridView("");
            this.dataGridView1.Columns[0].Width = 40;////系统ID
            this.dataGridView1.Columns[1].Width = 40;//风机编号
            this.dataGridView1.Columns[2].Width = 40;//状态
            this.button1.Enabled = true;
        }

        public void BindGridView(string turbineid)
        {
            string sql = "select * from Turbine_Train_Model where turbineid='" + turbineid + "'";
            if (turbineid.Equals(""))
            {
                sql = "select * from Turbine_Train_Model";
            }
            DataSet ds_var = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);


            this.dataGridView1.Columns.Clear();
            //for (int i = 0; i < ds_var.Tables[0].Columns.Count; i++)
            //{
            //    DataColumn dc = new DataColumn();
            //    dc.Caption = ds_var.Tables[0].Columns[i].Caption;
            //    dc.ColumnName = ds_var.Tables[0].Columns[i].Caption;
            //    dt.Columns.Add(dc);
            //}
            dt.Rows.Clear();
            dt.Columns.Clear();
            DataColumn dc1 = new DataColumn();
            dc1.Caption = "编号";
            dc1.ColumnName = "编号";
            dt.Columns.Add(dc1);

            DataColumn dc2 = new DataColumn();
            dc2.Caption = "风机";
            dc2.ColumnName = "风机";
            dt.Columns.Add(dc2);

            DataColumn dc3 = new DataColumn();
            dc3.Caption = "状态";
            dc3.ColumnName = "状态";
            dt.Columns.Add(dc3);

            DataColumn dc4 = new DataColumn();
            dc4.Caption = "变量";
            dc4.ColumnName = "变量";
            dt.Columns.Add(dc4);

            DataColumn dc5 = new DataColumn();
            dc5.Caption = "工况";
            dc5.ColumnName = "工况";
            dt.Columns.Add(dc5);

            DataColumn dc6 = new DataColumn();
            dc6.Caption = "区间";
            dc6.ColumnName = "区间";
            dt.Columns.Add(dc6);

            DataColumn dc7 = new DataColumn();
            dc7.Caption = "中心值";
            dc7.ColumnName = "中心值";
            dt.Columns.Add(dc7);

            DataColumn dc8 = new DataColumn();
            dc8.Caption = "标准差";
            dc8.ColumnName = "标准差";
            dt.Columns.Add(dc8);



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

        private void 查看机组模型训练结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BindGridView(selectrowid);
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
            string filename = "机组【" + selectrowid + "】" + DateTime.Now.ToString("yyyyMMddHHmmss") + "模型训练结果";
            string result = ExcelHelper.SaveCSV(dt, filename, filepath);
            MessageBox.Show("结果已成功保存至：\r\n" + result, "操作信息提示", MessageBoxButtons.OK);
        }
    }
}
