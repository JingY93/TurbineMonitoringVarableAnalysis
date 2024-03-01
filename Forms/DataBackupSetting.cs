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
    public partial class DataBackupSetting : Form
    {
        public DataBackupSetting()
        {
            InitializeComponent();
            InitListBox();
        }

        public void InitListBox() 
        {
            this.listBox1.Items.Clear();
            this.listBox2.Items.Clear();

            #region 绑定已经选择的数据表名称
            
            ConfReadAndModify conf = new ConfReadAndModify();
            string DataTables = conf.getConfigSetting("BackupDataTables");
            //string DataTables = "test1,test2,test3";
            string[] tables = DataTables.Split(new char[]{','},StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tables.Length; i++)
            {
                listBox2.Items.Add(tables[i]);
            }
            #endregion
            #region 绑定可以选择的数据表
            string sql = "select table_name from information_schema.tables where table_schema='" + conf.getConfigSetting("DataBaseName") + "' and table_name not in ('" + DataTables + "');";
            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
            for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            {
                listBox1.Items.Add(ds.Tables[0].Rows[i][0].ToString());
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            #region 获取已经选择项目
            for (int i = listBox1.SelectedItems.Count - 1; i >= 0; i--)
            {      
                //要从后面往前删除
                listBox2.Items.Add(listBox1.SelectedItems[i]);
                listBox1.Items.Remove(listBox1.SelectedItems[i]);
                //SelectedItems直接获取选中项的文本
            }
            #endregion
        }

        private void button2_Click(object sender, EventArgs e)
        {
            #region 获取已经选择项目
            for (int i = listBox2.SelectedItems.Count - 1; i >= 0; i--)
            {
                //要从后面往前删除
                listBox1.Items.Add(listBox2.SelectedItems[i]);
                listBox2.Items.Remove(listBox2.SelectedItems[i]);
                //SelectedItems直接获取选中项的文本
            }
            #endregion
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string DataTables = "";
            for (int i = 0; i < listBox2.Items.Count; i++)
            {
                DataTables = DataTables + listBox2.Items[i].ToString() + ",";
            }
            ConfReadAndModify conf = new ConfReadAndModify();
            bool success = conf.modifyConfigSetting("BackupDataTables", DataTables);
            if (success)
            {
                MessageBox.Show("数据备份信息配置成功！","操作信息提示",MessageBoxButtons.OK);
            }
        }
    }
}
