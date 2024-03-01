using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;
using System.IO;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class SkinSelection : Form
    {
        public SkinSelection()
        {
            InitializeComponent();
            LoadSkin();
        }
        public void LoadSkin()
        {
            DirectoryInfo di = new DirectoryInfo((new DirectoryInfo(Application.StartupPath)).Parent.Parent.FullName + "\\skins\\");
            FileInfo[] fis = di.GetFiles();
            this.comboBox1.Items.Clear();
            for (int i = 0; i < fis.Length; i++)
            {
                if (fis[i].Extension.Equals(".ssk"))
                {
                    ComBoxItem item = new ComBoxItem();
                    item.Text = fis[i].Name.Substring(0, fis[i].Name.IndexOf('.'));
                    item.Value = fis[i].Name;
                    this.comboBox1.Items.Add(item);
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                ComBoxItem item = this.comboBox1.SelectedItem as ComBoxItem;
                ConfReadAndModify conf = new ConfReadAndModify();
                bool success = conf.modifyConfigSetting("SkinName", StringEncodeAndDecode.Encode(item.Value.ToString()));
                if (success)
                {
                    MessageBox.Show("换肤成功，下次启动后生效！", "操作信息提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ee)
            {
                MessageBox.Show("配置信息临时存储出错！\r\n" + ee.Message, "配置信息提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                LogRecord.WriteLog(this.Text + "  配置信息临时存储出错！---" + ee.Message);
            }
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComBoxItem item = this.comboBox1.SelectedItem as ComBoxItem;
            SkinPreview sp = new SkinPreview(item.Value.ToString());
            sp.ShowDialog();
        }
    }
}
