using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;
using MySql.Data.MySqlClient;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class DataBackup : Form
    {
        public DataBackup()
        {
            InitializeComponent();
            InitListBox();
        }

        public void InitListBox() 
        {
            this.listBox1.Items.Clear();
            #region 绑定已经选择的数据表名称

            ConfReadAndModify conf = new ConfReadAndModify();
            string DataTables = conf.getConfigSetting("BackupDataTables");
            //string DataTables = "test1,test2,test3";
            string[] tables = DataTables.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < tables.Length; i++)
            {
                listBox1.Items.Add(tables[i]);
            }
            #endregion
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确定要备份上面的数据表吗？\r\n\r\n若有新的需求，请重新配置即可。", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = trans;
                try
                {
                    string versioninfo = DateTime.Now.ToString("yyyyMMddHHmmss");
                    for (int i = 0; i < this.listBox1.Items.Count; i++)
                    {
                        string oldtable = this.listBox1.Items[i].ToString();
                        string newtable = oldtable + "_" + versioninfo;
                        cmd.CommandText = "CREATE TABLE " + newtable + " like " + oldtable + "; INSERT INTO " + newtable + " SELECT * FROM " + oldtable + ";";
                        int y = cmd.ExecuteNonQuery();
                        this.textBox1.Text = "数据表" + oldtable + "已成功备份...\r\n\r\n" + this.textBox1.Text;
                        Application.DoEvents();
                    }
                    trans.Commit();
                    conn.Close();
                    MessageBox.Show("数据表备份完成，请在数据库中查看！", "操作信息提示", MessageBoxButtons.OK);
                }
                catch (Exception ee)
                {
                    trans.Rollback();
                    conn.Close();
                    LogRecord.WriteLog(ee.ToString());
                    this.textBox1.Text = "数据表备份过程出错，所有操作已回滚...\r\n\r\n" + this.textBox1.Text;
                    Application.DoEvents();
                    MessageBox.Show("数据表备份过程出错！！", "操作信息提示", MessageBoxButtons.OK);
                }
            }
        }
    }
}
