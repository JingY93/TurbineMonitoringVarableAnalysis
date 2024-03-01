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
    public partial class SettingForElecLine : Form
    {
        public SettingForElecLine()
        {
            InitializeComponent();
            ConfReadAndModify conf = new ConfReadAndModify();
            string Default_Points_Line = conf.getConfigSetting("Default_Points_Line");
            string IQR_Factor = conf.getConfigSetting("IQR_Factor");
            this.textBox1.Text = Default_Points_Line;
            this.textBox2.Text = IQR_Factor;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (this.textBox1.Text.Trim().Equals("")||this.textBox2.Text.Trim().Equals(""))
            {
                MessageBox.Show("配置信息不能为空！", "操作信息提示", MessageBoxButtons.OK);
            }
            else
            {
                ConfReadAndModify conf = new ConfReadAndModify();
                bool success1 = conf.modifyConfigSetting("Default_Points_Line", this.textBox1.Text);
                bool success2 = conf.modifyConfigSetting("IQR_Factor", this.textBox2.Text);
                if (success1&&success2)
                {
                    MessageBox.Show("配置信息提交成功，已生效！", "操作信息提示", MessageBoxButtons.OK);
                    this.Close();
                    this.Dispose();
                }
            }
        }
    }
}
