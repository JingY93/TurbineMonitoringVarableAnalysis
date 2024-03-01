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
    public partial class ConfigEditing : Form
    {
        string record_id = "";
        public ConfigEditing(string recordid)
        {
            InitializeComponent();
            record_id = recordid;
            GetCurrentConf();
        }

        public void GetCurrentConf() 
        {
            string sql_conditionvar = "select turbineid,analysisvar,conditionvar,conditionrange from turbine_conf where recordid='"+record_id+"'  ";
            DataSet ds_conditionvar = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_conditionvar, null);

            this.textBox1.Text=ds_conditionvar.Tables[0].Rows[0][0].ToString();
            this.textBox2.Text = ds_conditionvar.Tables[0].Rows[0][1].ToString();
            this.textBox3.Text = ds_conditionvar.Tables[0].Rows[0][2].ToString();
            this.textBox4.Text = ds_conditionvar.Tables[0].Rows[0][3].ToString();
 
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Equals("")||this.textBox2.Text.Equals("")||this.textBox3.Text.Equals("")||this.textBox4.Text.Equals(""))
            {
                MessageBox.Show("配置信息不能为空！", "操作信息提示", MessageBoxButtons.OK);
            }
            else
            {
                string sql_update = "update turbine_conf set turbineid='" + this.textBox1.Text.Trim() + "', analysisvar='" + this.textBox2.Text + "', conditionvar='" + this.textBox3.Text + "', conditionrange='" + this.textBox4.Text + "'  where recordid='" + record_id + "'  ";

                int t = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, sql_update, null);
                MessageBox.Show("配置信息已更新！", "操作信息提示", MessageBoxButtons.OK);
            }
           
        }
    }
}
