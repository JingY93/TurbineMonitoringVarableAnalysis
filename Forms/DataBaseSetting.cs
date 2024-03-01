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
    public partial class DataBaseSetting : Form
    {
        public DataBaseSetting()
        {
            InitializeComponent();
            Getconfiguration();
        }
        public void Getconfiguration()
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            this.textBox1.Text = conf.getConfigSetting("DataConnection");
            this.textBox2.Text = conf.getConfigSetting("Port");
            this.textBox3.Text = conf.getConfigSetting("DataBaseName");
            this.textBox4.Text = conf.getConfigSetting("UserName");
            this.textBox5.Text = conf.getConfigSetting("PassWord");
            this.textBox6.Text = conf.getConfigSetting("TrainDataTable");
        }
        private void button1_Click(object sender, EventArgs e)
        {
            this.textBox1.Text = "";
            this.textBox2.Text = "";
            this.textBox3.Text = "";
            this.textBox4.Text = "";
            this.textBox5.Text = "";
            this.textBox6.Text = "";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals("") || this.textBox2.Text.Trim().Equals("") || this.textBox3.Text.Trim().Equals("") || this.textBox4.Text.Trim().Equals("") || this.textBox5.Text.Trim().Equals("") || this.textBox6.Text.Trim().Equals(""))
            {
                MessageBox.Show("配置信息不全，请补全信息！");
            }
            else
            {
                string connstr = "Server='" + this.textBox1.Text.Trim() + "';Port='" + this.textBox2.Text.Trim() + "';Database='" + this.textBox3.Text.Trim() + "';Uid='" + this.textBox4.Text.Trim() + "';Pwd='" + this.textBox5.Text.Trim() + "';Connect Timeout=30;";
                MySqlConnection conn = null;
                try
                {
                    conn = new MySqlConnection(connstr);
                    conn.Open();
                    MessageBox.Show("数据库连接测试成功！");
                }
                catch (Exception ee)
                {
                    LogRecord.WriteLog(ee.ToString());
                }
                finally
                {
                    conn.Close();
                }
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals("") || this.textBox2.Text.Trim().Equals("") || this.textBox3.Text.Trim().Equals("") || this.textBox4.Text.Trim().Equals("") || this.textBox5.Text.Trim().Equals(""))
            {
                MessageBox.Show("配置信息不全，请补全信息！");
            }
            else
            {
                try
                {
                    ConfReadAndModify conf = new ConfReadAndModify();
                    bool success1 = conf.modifyConfigSetting("DataConnection", this.textBox1.Text.Trim());
                    bool success2 = conf.modifyConfigSetting("Port", this.textBox2.Text.Trim());
                    bool success3 = conf.modifyConfigSetting("DataBaseName", this.textBox3.Text.Trim());
                    bool success4 = conf.modifyConfigSetting("UserName", this.textBox4.Text.Trim());
                    bool success5 = conf.modifyConfigSetting("PassWord", this.textBox5.Text.Trim());
                    bool success6 = conf.modifyConfigSetting("TrainDataTable", this.textBox6.Text.Trim());

                    if (success1 && success2 && success3 && success4 && success5)
                    {
                        MessageBox.Show("数据库配置信息写入成功！", "操作信息提示", MessageBoxButtons.OK);
                    }
                    else
                    {
                        MessageBox.Show("数据库配置信息写入出错！", "操作信息提示", MessageBoxButtons.OK);
                    }
                }
                catch (Exception ee)
                {

                    LogRecord.WriteLog(ee.ToString());
                    MessageBox.Show(ee.ToString());
                }
            }
        }
    }
}
