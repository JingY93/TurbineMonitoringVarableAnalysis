using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TurbineMonitoringVarableAnalysis.CommonCode;
using MySql.Data;
using MySql.Data.MySqlClient;
using System.IO;
using TurbineMonitoringVarableAnalysis.Forms;
using TurbineMonitoringVarableAnalysis.Forms.ImmuneForms;

namespace TurbineMonitoringVarableAnalysis
{
    public partial class MainForm : Form
    {
        static DataTable dt_conf = new DataTable();

        public string selectrowid = "";//一个全局变量，用于存储修改配置信息的选择行
        public int selectrow = 0;

        public MainForm()
        {
            InitializeComponent();
            this.dateTimePicker1.Text = System.DateTime.Now.AddYears(-1).ToString("yyyy-MM-dd");
            this.dateTimePicker2.Text = System.DateTime.Now.AddMonths(-9).ToString("yyyy-MM-dd");
             
            this.dateTimePicker3.Text = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd");
            this.dateTimePicker4.Text = System.DateTime.Now.ToString("yyyy-MM-dd");

            GetBasicInfo();
            BindingConfInfo();
            BindingTrainInfo();
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string SkinName = conf.getConfigSetting("SkinName");

            if (SkinName.Equals(""))
            {
                SkinName = "Midsummer.ssk";
            }

            DirectoryInfo di = new DirectoryInfo(Application.StartupPath);

            skinEngine1.SkinFile = di.Parent.Parent.FullName + "\\skins\\" + SkinName;
        }

        /// <summary>
        /// 绑定基础配置信息
        /// </summary>
        public void BindingConfInfo()
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string CreateTable = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Conf`  (   `RecordID` int NOT NULL AUTO_INCREMENT COMMENT '记录编号',   `TurbineID` varchar(100) NULL COMMENT '风机ID',   `AnalysisVar` varchar(150) NULL COMMENT '分析变量名',   `ConditionVar` varchar(150) NULL COMMENT '工况变量名', `ConditionRange` varchar(100) NULL COMMENT '工况区间',  PRIMARY KEY (`RecordID`) );";
            string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Conf';";
            string sql = "SELECT * FROM Turbine_Conf";
            DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);
            if (ds_isexist != null)
            {
                if (ds_isexist.Tables[0].Rows.Count > 0)//存在这这表
                {
                    #region 绑定配置数据

                    DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                    if (ds != null)
                    {
                        DataTable dt = new DataTable();

                        DataColumn RecordID = new DataColumn();
                        RecordID.Caption = "序号";
                        RecordID.ColumnName = "序号";

                        DataColumn turbineID = new DataColumn();
                        turbineID.Caption = "编号";
                        turbineID.ColumnName = "编号";

                        DataColumn AnalysisVar = new DataColumn();
                        AnalysisVar.Caption = "分析变量";
                        AnalysisVar.ColumnName = "分析变量";

                        DataColumn ConditionVar = new DataColumn();
                        ConditionVar.Caption = "工况变量";
                        ConditionVar.ColumnName = "工况变量";

                        DataColumn ConditionRange = new DataColumn();
                        ConditionRange.Caption = "工况区间";
                        ConditionRange.ColumnName = "工况区间";

                        dt.Columns.Add(RecordID);
                        dt.Columns.Add(turbineID);
                        dt.Columns.Add(AnalysisVar);
                        dt.Columns.Add(ConditionVar);
                        dt.Columns.Add(ConditionRange);

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            List<string> list = new List<string>();
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                            {
                                list.Add(ds.Tables[0].Rows[i][j].ToString());
                            }
                            dr.ItemArray = list.ToArray();
                            dt.Rows.Add(dr);
                        }
                        //DataGridViewCheckBoxColumn colbx = new DataGridViewCheckBoxColumn();
                        //colbx.HeaderText = "选择";
                        //colbx.FalseValue = 0;
                        //colbx.TrueValue = 1;
                        //this.dataGridView1.Columns.Clear();
                        //this.dataGridView1.Columns.Add(colbx);
                        this.dataGridView1.DataSource = dt;
                        this.groupBox2.Text = "分析变量配置信息（" + dt.Rows.Count + ")";
                        this.dataGridView1.Columns[0].Width = 60;////系统ID
                        this.dataGridView1.Columns[1].Width = 60;//风机编号
                        this.dataGridView1.Columns[2].Width = 180;//分析变量
                        this.dataGridView1.Columns[3].Width = 180;//工况变量
                        this.dataGridView1.Columns[4].Width = 218;//工况区间
                    }
                    #endregion
                }
                else//不存在，则创建
                {
                    int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, CreateTable, null);

                    #region 创建完在绑定
                    #region 绑定配置数据

                    DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                    if (ds != null)
                    {
                        DataTable dt = new DataTable();

                        DataColumn RecordID = new DataColumn();
                        RecordID.Caption = "序号";
                        RecordID.ColumnName = "序号";

                        DataColumn turbineID = new DataColumn();
                        turbineID.Caption = "编号";
                        turbineID.ColumnName = "编号";

                        DataColumn AnalysisVar = new DataColumn();
                        AnalysisVar.Caption = "分析变量";
                        AnalysisVar.ColumnName = "分析变量";

                        DataColumn ConditionVar = new DataColumn();
                        ConditionVar.Caption = "工况变量";
                        ConditionVar.ColumnName = "工况变量";

                        DataColumn ConditionRange = new DataColumn();
                        ConditionRange.Caption = "工况区间";
                        ConditionRange.ColumnName = "工况区间";

                        dt.Columns.Add(RecordID);
                        dt.Columns.Add(turbineID);
                        dt.Columns.Add(AnalysisVar);
                        dt.Columns.Add(ConditionVar);
                        dt.Columns.Add(ConditionRange);

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            List<string> list = new List<string>();
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                            {
                                list.Add(ds.Tables[0].Rows[i][j].ToString());
                            }
                            dr.ItemArray = list.ToArray();
                            dt.Rows.Add(dr);
                        }
                        //DataGridViewCheckBoxColumn colbx = new DataGridViewCheckBoxColumn();
                        //colbx.HeaderText = "选择";
                        //colbx.FalseValue = 0;
                        //colbx.TrueValue = 1;
                        //this.dataGridView1.Columns.Clear();
                        //this.dataGridView1.Columns.Add(colbx);
                        this.dataGridView1.DataSource = dt;
                        this.groupBox2.Text = "分析变量配置信息（" + dt.Rows.Count + ")";
                        this.dataGridView1.Columns[0].Width = 60;////系统ID
                        this.dataGridView1.Columns[1].Width = 60;//风机编号
                        this.dataGridView1.Columns[2].Width = 180;//分析变量
                        this.dataGridView1.Columns[3].Width = 180;//工况变量
                        this.dataGridView1.Columns[4].Width = 218;//工况区间
                    }
                    #endregion
                    #endregion
                }
            }
        }

        /// <summary>
        /// 绑定风机和数据量基础信息
        /// </summary>
        public void GetBasicInfo()
        {
            string sql = "SELECT count(distinct TurbineID),count(*) FROM " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable();
            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
            if (ds != null)
            {
                if (ds.Tables[0].Rows.Count > 0)
                {
                    this.textBox1.Text = ds.Tables[0].Rows[0][0].ToString();
                    this.textBox3.Text = ds.Tables[0].Rows[0][1].ToString();
                }
            }
        }

        /// <summary>
        /// 选择配置文件路径
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            OpenFileDialog of = new OpenFileDialog();
            of.Title = "选择分析配置文件";
            of.InitialDirectory = @"C:\";
            of.Filter = "CSV|*.csv";
            if (of.ShowDialog() == DialogResult.OK)
            {
                this.textBox2.Text = of.FileName;
            }
        }

        /// <summary>
        /// 打开配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (!this.textBox2.Text.Trim().Equals(""))
            {
                DataTable dt = ExcelHelper.OpenCSV(this.textBox2.Text.Trim(), 1);//从第一行开始读取数据
                if (dt != null)
                {
                    #region 绑定配置数据

                    dt_conf.Rows.Clear();
                    dt_conf.Columns.Clear();

                    DataColumn RecordID = new DataColumn();
                    RecordID.Caption = "序号";
                    RecordID.ColumnName = "序号";

                    DataColumn turbineID = new DataColumn();
                    turbineID.Caption = "风机";
                    turbineID.ColumnName = "风机";

                    DataColumn AnalysisVar = new DataColumn();
                    AnalysisVar.Caption = "分析变量";
                    AnalysisVar.ColumnName = "分析变量";

                    DataColumn ConditionVar = new DataColumn();
                    ConditionVar.Caption = "工况变量";
                    ConditionVar.ColumnName = "工况变量";

                    DataColumn ConditionRange = new DataColumn();
                    ConditionRange.Caption = "工况区间";
                    ConditionRange.ColumnName = "工况区间";

                    dt_conf.Columns.Add(RecordID);
                    dt_conf.Columns.Add(turbineID);
                    dt_conf.Columns.Add(AnalysisVar);
                    dt_conf.Columns.Add(ConditionVar);
                    dt_conf.Columns.Add(ConditionRange);

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        List<string> list = new List<string>();
                        DataRow dr = dt_conf.NewRow();
                        list.Add((i + 1).ToString());
                        for (int j = 0; j < dt.Columns.Count; j++)
                        {
                            list.Add(dt.Rows[i][j].ToString());
                        }
                        dr.ItemArray = list.ToArray();
                        dt_conf.Rows.Add(dr);
                    }
                    //DataGridViewCheckBoxColumn colbx = new DataGridViewCheckBoxColumn();
                    //colbx.HeaderText = "选择";
                    //colbx.FalseValue = 0;
                    //colbx.TrueValue = 1;
                    //this.dataGridView1.Columns.Clear();
                    //this.dataGridView1.Columns.Add(colbx);
                    this.dataGridView1.DataSource = dt_conf;


                    this.dataGridView1.Columns[0].Width = 60;////系统ID
                    this.dataGridView1.Columns[1].Width = 60;//风机编号
                    this.dataGridView1.Columns[2].Width = 180;//分析变量
                    this.dataGridView1.Columns[3].Width = 180;//工况变量
                    this.dataGridView1.Columns[4].Width = 200;//工况区间

                    #endregion
                }
                this.textBox4.Text = "数据配置文件打开成功\r\n" + this.textBox4.Text;
                this.button3.Enabled = true;
            }
            else
            {
                MessageBox.Show("请先选择配置文件！", "操作信息提示", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 导入配置信息进数据库
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            if (dt_conf != null)
            {
                MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = trans;
                try
                {
                    //2023.1.4 这里不应该删除所有的配置信息。而是应该覆盖已有机组的配置信息
                    //cmd.CommandText = "delete from turbine_conf";
                    //int y = cmd.ExecuteNonQuery();
                    for (int i = 0; i < dt_conf.Rows.Count; i++)
                    {
                        cmd.CommandText = "delete from turbine_conf where turbineid='" + dt_conf.Rows[i][1].ToString() + "' and analysisvar='" + dt_conf.Rows[i][2].ToString() + "' and conditionvar='" + dt_conf.Rows[i][3].ToString() + "' ";
                        int y = cmd.ExecuteNonQuery();
                        cmd.CommandText = "insert into turbine_conf (Turbineid, analysisvar,conditionvar,conditionrange) values('" + dt_conf.Rows[i][1].ToString() + "','" + dt_conf.Rows[i][2].ToString() + "','" + dt_conf.Rows[i][3].ToString() + "','" + dt_conf.Rows[i][4].ToString() + "')";
                        int x = cmd.ExecuteNonQuery();
                    }
                    trans.Commit();
                    conn.Close();
                    BindingConfInfo();
                    this.textBox4.Text = "配置信息已成功写入数据库\r\n" + this.textBox4.Text; ;
                    MessageBox.Show("配置信息已成功写入数据库！", "操作信息提示", MessageBoxButtons.OK);
                }
                catch (Exception ee)
                {
                    trans.Rollback();
                    conn.Close();
                    LogRecord.WriteLog(ee.ToString());
                    this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text; ;
                    MessageBox.Show("配置信息写入数据库出错！", "操作信息提示", MessageBoxButtons.OK);
                }
            }
        }

        /// <summary>
        /// 绑定已经训练好的模型信息
        /// </summary>
        public void BindingTrainInfo()
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string createtable = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Train`  (  `recordid` int NOT NULL AUTO_INCREMENT COMMENT '记录ID',  `turbineid` varchar(100) NULL COMMENT '风机ID', `status` varchar(10) NULL COMMENT '风机状态标识',`analysisvar` varchar(150) NULL COMMENT '分析变量',  `conditionvar` varchar(150) NULL COMMENT '工况变量',  `conditionrange` varchar(100) NULL COMMENT '工况区间',  `conditioncenter` float NULL COMMENT '分析变量中心值',  `standarddev` float NULL COMMENT '标准差',  PRIMARY KEY (`recordid`));";

            string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Train';";
            string sql = "SELECT recordid, turbineid,status,analysisvar,conditionvar, conditionrange FROM Turbine_Train order by turbineid, status";
            DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);
            if (ds_isexist != null)
            {
                if (ds_isexist.Tables[0].Rows.Count > 0)//存在这这表
                {
                    DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                    if (ds != null)
                    {
                        #region 绑定配置数据
                        DataTable dt = new DataTable();

                        DataColumn RecordID = new DataColumn();
                        RecordID.Caption = "序号";
                        RecordID.ColumnName = "序号";

                        DataColumn turbineID = new DataColumn();
                        turbineID.Caption = "编号";
                        turbineID.ColumnName = "编号";

                        DataColumn status = new DataColumn();
                        status.Caption = "状态";
                        status.ColumnName = "状态";

                        DataColumn AnalysisVar = new DataColumn();
                        AnalysisVar.Caption = "分析变量";
                        AnalysisVar.ColumnName = "分析变量";

                        DataColumn ConditionVar = new DataColumn();
                        ConditionVar.Caption = "工况变量";
                        ConditionVar.ColumnName = "工况变量";

                        DataColumn ConditionRange = new DataColumn();
                        ConditionRange.Caption = "工况区间";
                        ConditionRange.ColumnName = "工况区间";


                        dt.Columns.Add(RecordID);
                        dt.Columns.Add(turbineID);
                        dt.Columns.Add(status);
                        dt.Columns.Add(AnalysisVar);
                        dt.Columns.Add(ConditionVar);
                        dt.Columns.Add(ConditionRange);



                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            List<string> list = new List<string>();
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                            {
                                list.Add(ds.Tables[0].Rows[i][j].ToString());
                            }
                            dr.ItemArray = list.ToArray();
                            dt.Rows.Add(dr);
                        }
                        //DataGridViewCheckBoxColumn colbx = new DataGridViewCheckBoxColumn();
                        //colbx.HeaderText = "选择";
                        //colbx.FalseValue = 0;
                        //colbx.TrueValue = 1;
                        //this.dataGridView1.Columns.Clear();
                        //this.dataGridView1.Columns.Add(colbx);
                        this.dataGridView2.DataSource = dt;
                        this.groupBox5.Text = "配置信息解析结果（" + dt.Rows.Count + ")";

                        this.dataGridView2.Columns[0].Width = 40;////系统ID
                        this.dataGridView2.Columns[1].Width = 40;//风机编号
                        this.dataGridView2.Columns[2].Width = 40;//风机状态
                        this.dataGridView2.Columns[3].Width = 180;//分析变量
                        this.dataGridView2.Columns[4].Width = 200;//工况变量
                        this.dataGridView2.Columns[5].Width = 198;//工况区间

                        #endregion
                    }
                }
                else//不存在，则创建
                {
                    int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, createtable, null);

                    #region 创建完再绑定
                    DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                    if (ds != null)
                    {
                        #region 绑定配置数据
                        DataTable dt = new DataTable();

                        DataColumn RecordID = new DataColumn();
                        RecordID.Caption = "序号";
                        RecordID.ColumnName = "序号";

                        DataColumn turbineID = new DataColumn();
                        turbineID.Caption = "编号";
                        turbineID.ColumnName = "编号";

                        DataColumn status = new DataColumn();
                        status.Caption = "状态";
                        status.ColumnName = "状态";

                        DataColumn AnalysisVar = new DataColumn();
                        AnalysisVar.Caption = "分析变量";
                        AnalysisVar.ColumnName = "分析变量";

                        DataColumn ConditionVar = new DataColumn();
                        ConditionVar.Caption = "工况变量";
                        ConditionVar.ColumnName = "工况变量";

                        DataColumn ConditionRange = new DataColumn();
                        ConditionRange.Caption = "工况区间";
                        ConditionRange.ColumnName = "工况区间";

                        dt.Columns.Add(RecordID);
                        dt.Columns.Add(turbineID);
                        dt.Columns.Add(status);
                        dt.Columns.Add(AnalysisVar);
                        dt.Columns.Add(ConditionVar);
                        dt.Columns.Add(ConditionRange);

                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            List<string> list = new List<string>();
                            DataRow dr = dt.NewRow();
                            for (int j = 0; j < ds.Tables[0].Columns.Count; j++)
                            {
                                list.Add(ds.Tables[0].Rows[i][j].ToString());
                            }
                            dr.ItemArray = list.ToArray();
                            dt.Rows.Add(dr);
                        }
                        //DataGridViewCheckBoxColumn colbx = new DataGridViewCheckBoxColumn();
                        //colbx.HeaderText = "选择";
                        //colbx.FalseValue = 0;
                        //colbx.TrueValue = 1;
                        //this.dataGridView1.Columns.Clear();
                        //this.dataGridView1.Columns.Add(colbx);
                        this.dataGridView2.DataSource = dt;
                        this.groupBox5.Text = "配置信息解析结果（" + dt.Rows.Count + ")";

                        this.dataGridView2.Columns[0].Width = 40;////系统ID
                        this.dataGridView2.Columns[1].Width = 40;//风机编号
                        this.dataGridView2.Columns[2].Width = 40;//状态
                        this.dataGridView2.Columns[3].Width = 180;//分析变量
                        this.dataGridView2.Columns[4].Width = 200;//工况变量
                        this.dataGridView2.Columns[5].Width = 198;//工况区间
                        #endregion
                    }
                    #endregion
                }
            }
        }

        /// <summary>
        /// 一键训练
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button4_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作会覆盖所有的已训练模型信息，您确认要重新训练吗？", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                this.timer2.Enabled = false;//先关闭自动分析计时器
                #region 首先判断训练数据是否存在
                ConfReadAndModify conf = new ConfReadAndModify();
                string train_data_table = conf.getConfigSetting("TrainDataTable");
                if (train_data_table.Equals(""))
                {
                    MessageBox.Show("还未配置模型训练数据集，请配置后操作！", "操作信息提示",MessageBoxButtons.OK);
                }
                else
                {
                    string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='" + train_data_table + "';";
                    DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);
                    if (ds_isexist != null && ds_isexist.Tables[0].Rows.Count > 0)
                    {
                        DateTime time_start = DateTime.Now;//模型训练开始的时间

                        ConditionResolve();
                        BindingTrainInfo();
                        TrainModel(DateTime.Parse(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Parse(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " 23:59:59");
                        //TrainModel("2021-01-01 00:00:00", "2021-01-31 23:59:59");

                        DateTime time_end = DateTime.Now;//模型训练结束的时间
                        TimeSpan tp = time_end - time_start;
                        this.textBox4.Text = "所有机组的分析模型已经训练完成，并已写入数据库！耗时：" + Convert.ToInt32(tp.Days * 24*60*60 + tp.Hours*3600+tp.Minutes*60+tp.Seconds) + "秒\r\n" + this.textBox4.Text; ;
                        MessageBox.Show("模型训练已经结束，并已写入数据库！", "操作信息提示");
                    }
                    else
                    {
                        MessageBox.Show("数据库中不存在您配置的【" + train_data_table + "】训练数据表，请确认配置信息！", "操作信息提示", MessageBoxButtons.OK);
                    }
                }
                #endregion
                this.timer2.Enabled = true;//关闭自动分析计时器
            }
            #region 一开始的处理方式，被抛弃
            //MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
            //conn.Open();
            //MySqlTransaction trans = conn.BeginTransaction();
            //MySqlCommand cmd = conn.CreateCommand();
            //cmd.Transaction = trans;
            //try
            //{
            //    #region 先删除所有的配置信息
            //    cmd.CommandText = "delete from Turbine_Train";
            //    int x = cmd.ExecuteNonQuery();
            //    #endregion
            //    #region 先读取所有的配置信息
            //    string sql = "select * from Turbine_Conf";
            //    DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
            //    if (ds != null && ds.Tables[0].Rows.Count > 0)//有配置信息
            //    {
            //        //首先获取工况范围
            //        ConfReadAndModify conf = new ConfReadAndModify();
            //        string StatusRange = "2,4";
            //        string statusconf = conf.getConfigSetting("StatusRange").Trim();
            //        if (!statusconf.Equals(""))
            //        {
            //            StatusRange = statusconf;
            //        }
            //        string[] status = StatusRange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            //        #region 原始配置文件的解析
            //        int recordid = 0;
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            string[] conditionrange = ds.Tables[0].Rows[i]["conditionrange"].ToString().Replace("[", "").Replace("]", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //            List<string> list = new List<string>();
            //            #region 获取所有的工况区间
            //            if (conditionrange.Length == 1)
            //            {
            //                list.Add("[-inf," + conditionrange[0] + "]");
            //                list.Add("[" + conditionrange[0] + ",inf]");
            //            }
            //            else if (conditionrange.Length > 1)
            //            {
            //                for (int j = 0; j < conditionrange.Length; j++)
            //                {
            //                    if (j == 0)//第一个
            //                    {
            //                        list.Add("[-inf," + conditionrange[j] + "]");
            //                    }
            //                    else if (j == conditionrange.Length - 1)//最后一个
            //                    {
            //                        list.Add("[" + conditionrange[j - 1] + "," + conditionrange[j] + "]");
            //                        list.Add("[" + conditionrange[j] + ",inf]");
            //                    }
            //                    else//中间的部分
            //                    {
            //                        list.Add("[" + conditionrange[j - 1] + "," + conditionrange[j] + "]");
            //                    }
            //                }
            //            }
            //            #endregion

            //            #region 开始逐条写入工况范围

            //            for (int k = 0; k < list.Count; k++)
            //            {
            //                for (int m = 0; m < status.Length; m++)//按照状态写入
            //                {
            //                    List<string> ComputeResult = ComputeThreshold(DateTime.Parse(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Parse(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " 23:59:59", ds.Tables[0].Rows[i]["TurbineID"].ToString(), status[m], ds.Tables[0].Rows[i]["AnalysisVar"].ToString(), ds.Tables[0].Rows[i]["ConditionVar"].ToString(), list[k]);
            //                    cmd.CommandText = "insert into Turbine_Train(recordid,turbineid,status,analysisvar,conditionvar,conditionrange,conditioncenter,standarddev) values(" + recordid + ",'" + ds.Tables[0].Rows[i]["TurbineID"].ToString() + "','" + status[m] + "','" + ds.Tables[0].Rows[i]["AnalysisVar"].ToString() + "','" + ds.Tables[0].Rows[i]["ConditionVar"].ToString() + "','" + list[k] + "'," + ComputeResult[0] + ", " + ComputeResult[1] + ")";
            //                    int y = cmd.ExecuteNonQuery();
            //                    recordid++;
            //                }
            //            }
            //            #endregion
            //        }
            //        #endregion
            //    }
            //    #endregion
            //    trans.Commit();
            //    conn.Close();
            //    BindingTrainInfo();
            //    this.textBox4.Text = "模型训练完成，并已写入数据库！\r\n" + this.textBox4.Text; ;
            //    MessageBox.Show("模型训练完成，并已写入数据库！", "操作信息提示", MessageBoxButtons.OK);
            //}
            //catch (Exception ee)
            //{
            //    trans.Rollback();
            //    LogRecord.WriteLog(ee.ToString());
            //    this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
            //}
            #endregion
        }

        /// <summary>
        /// 分析变量的所有细分工况解析
        /// </summary>
        public void ConditionResolve()
        {
            MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            try
            {
                #region 先删除所有的解析信息
                cmd.CommandText = "delete from Turbine_Train";
                int x = cmd.ExecuteNonQuery();
                #endregion

                #region 读取所有的配置信息
                string sql = "select * from Turbine_Conf";
                DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                if (ds != null && ds.Tables[0].Rows.Count > 0)//有配置信息
                {
                    //首先获取工况范围
                    ConfReadAndModify conf = new ConfReadAndModify();
                    string StatusRange = "2,4";
                    string statusconf = conf.getConfigSetting("StatusRange").Trim();
                    if (!statusconf.Equals(""))
                    {
                        StatusRange = statusconf;
                    }
                    string[] status = StatusRange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    #region 原始配置文件的解析
                    int recordid = 0;
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string[] conditionrange = ds.Tables[0].Rows[i]["conditionrange"].ToString().Replace("[", "").Replace("]", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
                        List<string> list = new List<string>();
                        #region 获取所有的工况区间
                        if (conditionrange.Length == 1)
                        {
                            list.Add("[-inf," + conditionrange[0] + "]");
                            list.Add("[" + conditionrange[0] + ",inf]");
                        }
                        else if (conditionrange.Length > 1)
                        {
                            for (int j = 0; j < conditionrange.Length; j++)
                            {
                                if (j == 0)//第一个
                                {
                                    list.Add("[-inf," + conditionrange[j] + "]");
                                }
                                else if (j == conditionrange.Length - 1)//最后一个
                                {
                                    list.Add("[" + conditionrange[j - 1] + "," + conditionrange[j] + "]");
                                    list.Add("[" + conditionrange[j] + ",inf]");
                                }
                                else//中间的部分
                                {
                                    list.Add("[" + conditionrange[j - 1] + "," + conditionrange[j] + "]");
                                }
                            }
                        }
                        #endregion

                        #region 开始逐条写入工况范围

                        for (int k = 0; k < list.Count; k++)
                        {
                            for (int m = 0; m < status.Length; m++)//按照状态写入
                            {
                                //List<string> ComputeResult = ComputeThreshold(DateTime.Parse(this.dateTimePicker1.Text).ToString("yyyy-MM-dd") + " 00:00:00", DateTime.Parse(this.dateTimePicker2.Text).ToString("yyyy-MM-dd") + " 23:59:59", ds.Tables[0].Rows[i]["TurbineID"].ToString(), status[m], ds.Tables[0].Rows[i]["AnalysisVar"].ToString(), ds.Tables[0].Rows[i]["ConditionVar"].ToString(), list[k]);
                                cmd.CommandText = "insert into Turbine_Train(turbineid,status,analysisvar,conditionvar,conditionrange) values('" + ds.Tables[0].Rows[i]["TurbineID"].ToString() + "','" + status[m] + "','" + ds.Tables[0].Rows[i]["AnalysisVar"].ToString() + "','" + ds.Tables[0].Rows[i]["ConditionVar"].ToString() + "','" + list[k] + "')";
                                int y = cmd.ExecuteNonQuery();
                                recordid++;
                            }
                        }
                        #endregion
                    }
                    #endregion
                }
                #endregion

                trans.Commit();
                conn.Close();
                BindingTrainInfo();
                this.textBox4.Text = "分析变量工况解析已完成，并已写入数据库！\r\n" + this.textBox4.Text; ;
            }
            catch (Exception ee)
            {
                trans.Rollback();
                LogRecord.WriteLog(ee.ToString());
                this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
            }
        }

        /// <summary>
        /// 模型训练功能
        /// </summary>
        public void TrainModel(string starttime, string endtime)
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string createtable = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Train_Model`  (  `recordid` int NOT NULL AUTO_INCREMENT COMMENT '记录ID',  `turbineid` varchar(100) NULL COMMENT '风机ID', `status` varchar(10) NULL COMMENT '风机状态标识',`analysisvar` varchar(150) NULL COMMENT '分析变量',  `conditionvar` varchar(300) NULL COMMENT '工况变量',  `conditionrange` varchar(200) NULL COMMENT '工况区间',  `conditioncenter` float NULL COMMENT '分析变量中心值',  `standarddev` float NULL COMMENT '标准差',  PRIMARY KEY (`recordid`));";

            string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Train_Model';";

            DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);
            if (ds_isexist == null || ds_isexist.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, createtable, null);
            }

            MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            try
            {
                #region 先删除所有的模型训练结果
                cmd.CommandText = "delete from Turbine_Train_Model";
                int x = cmd.ExecuteNonQuery();
                #endregion

                #region 找到所有的配置信息
                string sql = "select distinct TurbineID,AnalysisVar from Turbine_Conf order by TurbineID";
                DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);
                #endregion

                if (ds != null && ds.Tables[0].Rows.Count > 0)//有配置信息，说明由要分析的机组和变量
                {
                    //首先获取工况范围
                    string StatusRange = "2,4";
                    string statusconf = conf.getConfigSetting("StatusRange").Trim();
                    if (!statusconf.Equals(""))
                    {
                        StatusRange = statusconf;
                    }
                    string[] status = StatusRange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    string TrainDataTable = conf.getConfigSetting("TrainDataTable");

                    string table_name_clone = TrainDataTable+"_temp";

                    string IsExist_temp = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='"+table_name_clone+"';";

                    DataSet ds_isexist_temp = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist_temp, null);
                    if (ds_isexist_temp != null)
                    {

                        if (ds_isexist_temp.Tables[0].Rows.Count>0)//存在这张表，删除
                        {
                            string sql_clone_drop = "drop table "+table_name_clone;
                            int sql_clone_drop_int = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, sql_clone_drop, null);
                        }
                    }

                    string sql_clone = "create table " + table_name_clone + " (select * from " + TrainDataTable + " where status in (" + StatusRange + ") and datatime between '" + starttime + "' and '" + endtime + "')";//克隆数据表，目的是在训练的过程中删除训练数据，加快训练过程
                    int clone = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, sql_clone, null);

                    string turbineid_temp = ""; //用于临时存储当前的机组编号
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)//开始遍历每一台机组，并开始每一台机组模型的训练
                    {
                        string TurbineID = ds.Tables[0].Rows[i]["TurbineID"].ToString();
                        if (!TurbineID.Equals(turbineid_temp))//是一台新机组，之前老机组的数据可以删除了
                        {
                            #region 
                            string sql_delete_cone = "delete from " + table_name_clone + " where Turbineid='" + turbineid_temp + "' ";//直接删除所有数据
                            int delete_clone = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, sql_delete_cone, null);
                            #endregion
                        }
                       
                        turbineid_temp = TurbineID;
                        string analysisvar = ds.Tables[0].Rows[i]["AnalysisVar"].ToString();

                        this.textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": 正在训练机组【" + TurbineID + "】【" + analysisvar + "】的分析模型......\r\n" + this.textBox4.Text;
                        Application.DoEvents();

                        #region 开始寻找每一个分析变量的工况变量
                        string sql_conditionvar = "select distinct ConditionVar from Turbine_Conf where TurbineID='" + TurbineID + "' and AnalysisVar='" + analysisvar + "'  ";
                        DataSet ds_conditionvar = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_conditionvar, null);

                        string conditionvar = "";

                        List<string>[] list = new List<string>[ds_conditionvar.Tables[0].Rows.Count];

                        List<string> conditionvarname = new List<string>();//用于存储该分析变量的所有工况变量名称
                        for (int j = 0; j < ds_conditionvar.Tables[0].Rows.Count; j++)//开始读取每一个工况变量
                        {
                            list[j] = new List<string>();
                            conditionvar = conditionvar + ":" + ds_conditionvar.Tables[0].Rows[j][0].ToString();
                            conditionvarname.Add(ds_conditionvar.Tables[0].Rows[j][0].ToString());
                            #region 寻找该工况的划分区间
                            string conditionvar_1 = ds_conditionvar.Tables[0].Rows[j][0].ToString();
                            string sql_conditon_range = "select distinct conditionrange from Turbine_Train where TurbineID='" + TurbineID + "' and analysisvar='" + analysisvar + "' and conditionvar='" + conditionvar_1 + "'  order by RecordID";
                            DataSet ds_conditionrange = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_conditon_range, null);
                            for (int k = 0; k < ds_conditionrange.Tables[0].Rows.Count; k++)
                            {
                                list[j].Add(ds_conditionrange.Tables[0].Rows[k][0].ToString());
                            }
                            #endregion
                        }
                        List<string> ConditionCombintions = GetAllConditions(list);

                        for (int n = 0; n < ConditionCombintions.Count; n++)//每一个细分工况都要计算
                        {
                            //TurbineID 风机ID
                            //analysisvar 分析变量
                            //conditionvar 工况变量
                            //ConditionCombintions[n] 工况组合
                            for (int m = 0; m < status.Length; m++)//每个工况都写入
                            {
                                //ststus[m] 风机状态
                                List<string> ThresholdCompu = GetThreshold(TurbineID, status[m], analysisvar, conditionvar, ConditionCombintions[n], starttime, endtime,table_name_clone);//获取不同工况下的阈值中心和方差
                                string sql_insert = "insert into Turbine_Train_Model(turbineid, status, analysisvar, conditionvar, conditionrange, conditioncenter,standarddev) values('" + TurbineID + "','" + status[m] + "','" + analysisvar + "','" + conditionvar + "','" + ConditionCombintions[n] + "', " + ThresholdCompu[0] + ", " + ThresholdCompu[1] + ")";
                                try
                                {
                                    cmd.CommandText = sql_insert;
                                    int xxxx = cmd.ExecuteNonQuery();
                                }
                                catch (Exception eeee)
                                {
                                    LogRecord.WriteLog(eeee.ToString());
                                    this.textBox4.Text = eeee.ToString() + "\r\n" + this.textBox4.Text;
                                }
                            }
                        }
                        #endregion

                        this.textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ": 机组【" + TurbineID + "】【" + analysisvar + "】的分析模型已经训练完成，等待写入数据库......\r\n" + this.textBox4.Text;
                        Application.DoEvents();
                    }
                    #region 所有的分析变量处理已经结束，临时表已完成使命，可以删除
                    string sql_drop_clone = "drop table " + table_name_clone;
                    int drop = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, sql_drop_clone, null);
                    #endregion
                }

                trans.Commit();
                conn.Close();
                BindingTrainInfo();
            }
            catch (Exception ee)
            {

                trans.Rollback();
                LogRecord.WriteLog(ee.ToString());
                this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
            }
        }

        /// <summary>
        /// 一键分析
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button5_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作会覆盖已有的判断结果，您确定要重新分析时间段\r\n【" + DateTime.Parse(this.dateTimePicker3.Text).ToString("yyyy-MM-dd") + " 00:00:00】~【" + DateTime.Parse(this.dateTimePicker4.Text).ToString("yyyy-MM-dd") + " 23:59:59】\r\n内的数据吗？", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                this.timer2.Enabled = false;//先关闭自动分析计时器
                DateTime time_start = DateTime.Now;//模型训练开始的时间

                //TurbinePointComparion(DateTime.Parse("2021-06-01"), DateTime.Parse("2021-06-10"));//测试用
                TurbinePointComparion(DateTime.Parse(this.dateTimePicker3.Text), DateTime.Parse(this.dateTimePicker4.Text));

                DateTime time_end = DateTime.Now;//模型训练结束的时间
                TimeSpan tp = time_end - time_start;
                int time = Convert.ToInt32(tp.Days * 24 * 60 * 60 + tp.Hours * 3600 + tp.Minutes * 60 + tp.Seconds);
                this.textBox4.Text = "所有机组的数据已经分析完成，并已写入数据库！耗时：" + time + "秒\r\n" + this.textBox4.Text; ;
                this.timer2.Enabled = true;//打开自动分析计时器
                MessageBox.Show("所有机组时间段内的监测数据分析已完成，结果已写入数据库！\r\n分析耗时：【" + time + "】秒", "操作信息提示", MessageBoxButtons.OK);
            }
            #region 最早的处理流程，已抛弃
            //#region 先创建数据表
            //ConfReadAndModify conf = new ConfReadAndModify();
            //string create_sql = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Var`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,  `analysisvar` varchar(100) NULL,  `isok` int NULL,  `fault_rate` float NULL, PRIMARY KEY (`recordid`));";
            //string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Var';";

            //DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);

            //if (ds_isexist == null || ds_isexist.Tables[0].Rows.Count == 0)//不存在创建之
            //{
            //    int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql, null);
            //}

            //string create_sql2 = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Point`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,  `analysisvar` varchar(100) NULL, `conditionvar` varchar(100) NULL, `isok` int NULL,  PRIMARY KEY (`recordid`));";
            //string IsExist2 = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Point';";

            //DataSet ds_isexist2 = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist2, null);

            //if (ds_isexist2 == null || ds_isexist2.Tables[0].Rows.Count == 0)//不存在创建之
            //{
            //    int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql2, null);
            //}


            //#endregion

            //#region 先找到要分析的机组
            //string sql_turbine = "SELECT  distinct turbineid FROM  turbine_conf";
            //DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_turbine, null);
            //List<string> startdate = new List<string>();
            //List<string> enddate = new List<string>();
            //DateTime starttime = DateTime.Parse(this.dateTimePicker3.Text);
            //DateTime endtime = DateTime.Parse(this.dateTimePicker4.Text);

            //while (starttime <= endtime)
            //{
            //    startdate.Add(starttime.ToString("yyyy-MM-dd 00:00:00"));
            //    enddate.Add(starttime.ToString("yyyy-MM-dd 23:59:59"));
            //    starttime = starttime.AddDays(1);
            //}
            //if (ds.Tables[0].Rows.Count > 0)
            //{
            //    MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
            //    conn.Open();
            //    MySqlTransaction trans = conn.BeginTransaction();
            //    MySqlCommand cmd = conn.CreateCommand();
            //    cmd.Transaction = trans;
            //    try
            //    {
            //        #region 逐台机组进行分析
            //        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
            //        {
            //            #region 找到当前机组的分析变量
            //            string sql_analysisvar = "select distinct analysisvar from Turbine_Conf where turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "'  ";
            //            DataSet ds_analysis = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_analysisvar, null);//当前机组的分析变量
            //            #endregion

            //            for (int j = 0; j < startdate.Count; j++)//获取每一天的数据
            //            {
            //                //先删掉该机组在改时间已有的分析记录
            //                cmd.CommandText = "  delete from Turbine_Var where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime='" + startdate[j] + "' ";
            //                int x = cmd.ExecuteNonQuery();

            //                cmd.CommandText = "  delete from Turbine_Point where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime between '" + startdate[j] + "' and '" + enddate[j] + "' ";
            //                int xxx = cmd.ExecuteNonQuery();

            //                string sql_data = "select * from dap_tenmindata_7_202101 where turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime between '" + startdate[j] + "' and '" + enddate[j] + "' ";
            //                DataSet ds_data = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_data, null);//监测数据集


            //                #region 找出这一行的分析变量，所有分析变量都正常，这一行（风机的这个时刻才正常）
            //                List<int> result_ds_analysis = new List<int>();//分析变量是否正常
            //                string errorvar = "";

            //                for (int f = 0; f < ds_analysis.Tables[0].Rows.Count; f++)
            //                {
            //                    int errnum = 0;
            //                    for (int h = 0; h < ds_data.Tables[0].Rows.Count; h++)//每一行的数据都要判断
            //                    {
            //                        #region 找出分析变量的每一个工况变量，分析变量再所有工况下正常，分析变量才正常
            //                        string sql_condition = "select distinct conditionvar, conditionrange from turbine_conf where turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "' and analysisvar='" + ds_analysis.Tables[0].Rows[f][0].ToString() + "'";
            //                        DataSet ds_condtion = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_condition, null);//当前机组的分析变量
            //                        List<int> result_ds_condtion = new List<int>();
            //                        for (int g = 0; g < ds_condtion.Tables[0].Rows.Count; g++)
            //                        {
            //                            //先把带分析数据找出来
            //                            double analysisdata = double.Parse(ds_data.Tables[0].Rows[h]["" + ds_analysis.Tables[0].Rows[f][0].ToString() + ""].ToString());
            //                            //找到工况数据
            //                            double conditiondata = double.Parse(ds_data.Tables[0].Rows[h]["" + ds_condtion.Tables[0].Rows[g][0].ToString() + ""].ToString());
            //                            //状态
            //                            string status = ds_data.Tables[0].Rows[h]["status"].ToString();
            //                            //找到工况区间
            //                            string conditionrange = ds_condtion.Tables[0].Rows[g][1].ToString();
            //                            List<string> list_recog = Recognidata(ds.Tables[0].Rows[i][0].ToString(), status, ds_analysis.Tables[0].Rows[f][0].ToString(), analysisdata, ds_condtion.Tables[0].Rows[g][0].ToString(), conditiondata, conditionrange);

            //                            if (list_recog[0].Equals("1"))//至少一个工况下是异常的
            //                            {
            //                                result_ds_condtion.Add(1);

            //                                cmd.CommandText = "insert into Turbine_Point(datatime,turbineid,analysisvar,conditionvar, isok) values('" + ds_data.Tables[0].Rows[h]["datatime"].ToString() + "', '" + ds.Tables[0].Rows[i]["TurbineID"].ToString() + "','" + ds_analysis.Tables[0].Rows[f][0].ToString() + "','" + ds_condtion.Tables[0].Rows[g][0].ToString() + "', 1  )";
            //                                int tt = cmd.ExecuteNonQuery();
            //                            }
            //                        }
            //                        #endregion

            //                        if (result_ds_condtion.Count > 0)//至少一个工况下是异常的,说明这一行的分析变量是异常的
            //                        {
            //                            result_ds_analysis.Add(1);
            //                            errnum++;
            //                            errorvar += ds_analysis.Tables[0].Rows[f][0].ToString() + ";"; ;
            //                        }

            //                    }
            //                    int isok = 0;
            //                    double fault_rate = 0.0;
            //                    if (errnum > 0)
            //                    {
            //                        isok = 1;
            //                        fault_rate = Math.Round(double.Parse(errnum.ToString()) / ds_data.Tables[0].Rows.Count * 100.0, 4);
            //                    }
            //                    try
            //                    {
            //                        cmd.CommandText = "insert into Turbine_Var(datatime,turbineid,analysisvar,isok,fault_rate) values('" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "', '" + ds.Tables[0].Rows[i]["TurbineID"].ToString() + "','" + ds_analysis.Tables[0].Rows[f][0].ToString() + "'," + isok + "," + fault_rate + " )";
            //                        int y = cmd.ExecuteNonQuery();
            //                    }
            //                    catch (Exception eee)
            //                    {

            //                        LogRecord.WriteLog(eee.ToString());
            //                        this.textBox4.Text = eee.ToString() + "\r\n" + this.textBox4.Text;
            //                    }
            //                #endregion
            //                }
            //            }
            //        }
            //        #endregion
            //        trans.Commit();
            //        conn.Close();

            //        WriteTurbineFaultRate();
            //        this.textBox4.Text = "变量分析已完成，结果已写入数据库！\r\n" + this.textBox4.Text; ;
            //        MessageBox.Show("变量分析已完成，结果已写入数据库库！", "操作信息提示", MessageBoxButtons.OK);
            //    }
            //    catch (Exception ee)
            //    {

            //        trans.Rollback();
            //        LogRecord.WriteLog(ee.ToString());
            //        this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
            //    }
            //}
            //else
            //{
            //    MessageBox.Show("当前数据库中无配置信息，请检查后操作！", "操作信息提示", MessageBoxButtons.OK);
            //}
            //#endregion
            #endregion

        }

        /// <summary>
        /// 逐点进行判异
        /// </summary>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">结束时间</param>
        public void TurbinePointComparion(DateTime starttime, DateTime endtime)
        {
            #region 先创建数据表

            #region Turbine_Var
            ConfReadAndModify conf = new ConfReadAndModify();
            string create_sql = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Var`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,  `analysisvar` varchar(100) NULL,  `fault_rate` float NULL, PRIMARY KEY (`recordid`));";
            string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Var';";
            DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);

            if (ds_isexist == null || ds_isexist.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql, null);
            }
            #endregion

            #region Turbine_Var_Point
            string create_sql_var_point = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Var_Point`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,  `analysisvar` varchar(100) NULL,  `isok` int NULL,  `result` float NULL, PRIMARY KEY (`recordid`));";
            string IsExist3 = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Var_Point';";

            DataSet ds_isexist3 = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist3, null);

            if (ds_isexist3 == null || ds_isexist3.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql_var_point, null);
            }
            #endregion

            #region Turbine_Point
            string create_sql2 = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Point`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,  `status` int NULL,  `isok` int NULL,  PRIMARY KEY (`recordid`));";

            string IsExist2 = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Point';";

            DataSet ds_isexist2 = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist2, null);

            if (ds_isexist2 == null || ds_isexist2.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql2, null);
            }
            #endregion

            #region Turbine_Fault_Rate

            string create_sql_turbine_fault = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Fault_Rate`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,    `turbine_fault_rate` float NULL, PRIMARY KEY (`recordid`));";
            string IsExist4 = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Fault_Rate';";

            DataSet ds_isexist4 = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist4, null);

            if (ds_isexist4 == null || ds_isexist4.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql_turbine_fault, null);
            }

            #endregion
            #endregion

            string StatusRange = "2,4";
            string statusconf = conf.getConfigSetting("StatusRange").Trim();
            if (!statusconf.Equals(""))
            {
                StatusRange = statusconf;
            }

            #region 先找到要分析的机组
            string sql_turbine = "SELECT  distinct turbineid FROM  turbine_conf";
            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_turbine, null);
            List<string> startdate = new List<string>();
            List<string> enddate = new List<string>();

            while (starttime <= endtime)
            {
                startdate.Add(starttime.ToString("yyyy-MM-dd") + " 00:00:00");
                enddate.Add(starttime.ToString("yyyy-MM-dd") + " 23:59:59");
                starttime = starttime.AddDays(1);
            }

            if (ds.Tables[0].Rows.Count > 0)
            {
                MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
                conn.Open();
                MySqlTransaction trans = conn.BeginTransaction();
                MySqlCommand cmd = conn.CreateCommand();
                cmd.Transaction = trans;
                try
                {
                    #region 逐台机组进行分析
                    for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                    {
                        string turbineid = ds.Tables[0].Rows[i][0].ToString();
                        #region 找到当前机组的分析变量
                        string sql_analysisvar = "select distinct analysisvar from Turbine_Conf where turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "'  ";
                        DataSet ds_analysis = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_analysisvar, null);//当前机组的分析变量
                        #endregion

                        for (int j = 0; j < startdate.Count; j++)//获取每一天的数据
                        {
                            this.textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "开始分析机组【" + turbineid + "】【" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "】的数据...\r\n" + this.textBox4.Text;
                            Application.DoEvents();
                            //先删掉该机组在改时间已有的分析记录
                            cmd.CommandText = "  delete from Turbine_Var where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime='" + startdate[j] + "' ";
                            int x = cmd.ExecuteNonQuery();

                            cmd.CommandText = "  delete from Turbine_Point where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime between '" + startdate[j] + "' and '" + enddate[j] + "' ";
                            int xxx = cmd.ExecuteNonQuery();

                            cmd.CommandText = "  delete from Turbine_Var_Point where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime between '" + startdate[j] + "' and '" + enddate[j] + "'  ";
                            int xxxxxx = cmd.ExecuteNonQuery();

                            cmd.CommandText = "  delete from Turbine_Fault_Rate where TurbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' and datatime='" + startdate[j] + "' ";
                            int xxxx = cmd.ExecuteNonQuery();

                            string sql_data = "select * from " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "' and status in (" + StatusRange + ") and datatime between '" + startdate[j] + "' and '" + enddate[j] + "' ";
                            DataSet ds_data = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_data, null);//监测数据集

                            int[] err_num_analysiavar = new int[ds_analysis.Tables[0].Rows.Count];//用于存储每一个分析变量异常点的个数
                            for (int z = 0; z < err_num_analysiavar.Length; z++)
                            {
                                err_num_analysiavar[z] = 0;
                            }

                            int point_num = 0;//每一天中异常的点的个数
                            for (int m = 0; m < ds_data.Tables[0].Rows.Count; m++)//开始个分析变量进行判断
                            {
                                string isok = "0";
                                #region 开始分析每一行中的每一个分析变量

                                for (int n = 0; n < ds_analysis.Tables[0].Rows.Count; n++)
                                {

                                    string analysisvar = ds_analysis.Tables[0].Rows[n][0].ToString();
                                    string analysisvar_data = (ds_data.Tables[0].Rows[m]["" + analysisvar + ""].ToString().Equals("") || ds_data.Tables[0].Rows[m]["" + analysisvar + ""].ToString().Equals("0")) ? "0" : ds_data.Tables[0].Rows[m]["" + analysisvar + ""].ToString();//要进行判定的数值

                                    //找出分析变量的每一个工况变量，分析变量再所有工况下正常，分析变量才正常
                                    string sql_condition = "select distinct conditionvar from turbine_conf where turbineid='" + turbineid + "' and analysisvar='" + analysisvar + "'";
                                    DataSet ds_condtion = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_condition, null);//当前机组的分析变量
                                    string condition_range_train = "";

                                    #region 找到阈值区间条件
                                    for (int h = 0; h < ds_condtion.Tables[0].Rows.Count; h++)
                                    {
                                        string conditionvar = ds_condtion.Tables[0].Rows[h][0].ToString();
                                        string condtionvar_data = ds_data.Tables[0].Rows[m]["" + conditionvar + ""].ToString();//要进行判定的数值

                                        string sql_conf = "select conditionrange from Turbine_Conf where Turbineid='" + turbineid + "' and analysisvar='" + analysisvar + "' and conditionvar='" + conditionvar + "' ";
                                        DataSet ds_condtiontange = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_conf, null);//当前机组的分析变量
                                        string conditionrange = ds_condtiontange.Tables[0].Rows[0][0].ToString();
                                        string conditiondata = ds_data.Tables[0].Rows[m]["" + conditionvar + ""].ToString();

                                        string conditionrange_temp = GetConditionRangeBasedConditionData(conditionrange, double.Parse(conditiondata));
                                        condition_range_train = condition_range_train + conditionrange_temp + ":";
                                    }
                                    condition_range_train = condition_range_train.Substring(0, condition_range_train.Length - 1);//去掉最后一个冒号
                                    #endregion
                                    //某一行某一个分析变量的判断结果
                                    List<string> compare_result = RecognitionPoint(turbineid, ds_data.Tables[0].Rows[m]["status"].ToString(), analysisvar, condition_range_train, double.Parse(analysisvar_data));
                                    if (compare_result[0].Equals("1"))
                                    {
                                        err_num_analysiavar[n]++;
                                        isok = "1";//只要有一个分析变量异常，该行数据就异常
                                        #region 写分析变量的判定结果
                                        cmd.CommandText = "insert into Turbine_Var_Point(datatime,turbineid,analysisvar,isok,result) values('" + ds_data.Tables[0].Rows[m]["datatime"].ToString() + "', '" + turbineid + "','" + analysisvar + "'," + compare_result[0] + "," + compare_result[1] + " )";
                                        int y = cmd.ExecuteNonQuery();
                                        #endregion
                                    }
                                }
                                #endregion
                                if (!isok.Equals("0"))
                                {
                                    point_num++;
                                }
                                cmd.CommandText = "insert into Turbine_Point(datatime,turbineid, status, isok) values('" + ds_data.Tables[0].Rows[m]["datatime"].ToString() + "','" + turbineid + "'," + ds_data.Tables[0].Rows[m]["status"].ToString() + ", " + isok + "  )";
                                int tt = cmd.ExecuteNonQuery();
                            }

                            #region 写当天分析变量的异常率
                            for (int xx = 0; xx < ds_analysis.Tables[0].Rows.Count; xx++)
                            {
                                double fault_rate_analysisvar = 0.0;
                                if (ds_data.Tables[0].Rows.Count != 0)
                                {
                                    fault_rate_analysisvar = Math.Round(double.Parse(err_num_analysiavar[xx].ToString()) / ds_data.Tables[0].Rows.Count * 100, 4);
                                }
                                cmd.CommandText = "insert into Turbine_Var(datatime,turbineid,analysisvar,fault_rate) values('" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "', '" + turbineid + "','" + ds_analysis.Tables[0].Rows[xx][0].ToString() + "'," + fault_rate_analysisvar + " )";
                                int y = cmd.ExecuteNonQuery();
                            }
                            #endregion

                            #region 写整机的异常率

                            double turbine_fault_rate = 0.0;
                            if (ds_data.Tables[0].Rows.Count != 0)
                            {
                                turbine_fault_rate = Math.Round(double.Parse(point_num.ToString()) / ds_data.Tables[0].Rows.Count * 100, 4);
                            }
                            cmd.CommandText = "insert into Turbine_Fault_Rate(datatime,turbineid,turbine_fault_rate) values('" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "', '" + turbineid + "', " + turbine_fault_rate + ")";
                            int yyy = cmd.ExecuteNonQuery();
                            #endregion

                            this.textBox4.Text = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "机组【" + turbineid + "】【" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "】的数据分析已完成，等待写入数据库...\r\n\r\n" + this.textBox4.Text;
                            Application.DoEvents();
                        }
                    }
                    #endregion
                    trans.Commit();
                    conn.Close();

                    this.textBox4.Text = "所有机组时间段内的监测数据分析已完成，结果已写入数据库！\r\n" + this.textBox4.Text; ;
                }
                catch (Exception ee)
                {
                    trans.Rollback();
                    LogRecord.WriteLog(ee.ToString());
                    this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
                }
            }
            else
            {
                MessageBox.Show("当前数据库中无配置信息，请检查后操作！", "操作信息提示", MessageBoxButtons.OK);
            }
            #endregion
        }

        /// <summary>
        /// 根据工况数据找到对应的工况区间
        /// </summary>
        /// <param name="conditionrange">工况区间（从配置文件中获得）</param>
        /// <param name="conditiondata">工况数据</param>
        /// <returns></returns>
        public string GetConditionRangeBasedConditionData(string conditionrange, double conditiondata)
        {
            string result = "";
            #region 首先解析工况区间
            string[] range = conditionrange.Replace("[", "").Replace("]", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            #region 获取所有的工况区间
            if (range.Length == 1)
            {
                list.Add("[-inf," + range[0] + "]");
                list.Add("[" + range[0] + ",inf]");
            }
            else if (range.Length > 1)
            {
                for (int j = 0; j < range.Length; j++)
                {
                    if (j == 0)//第一个
                    {
                        list.Add("[-inf," + range[j] + "]");
                    }
                    else if (j == range.Length - 1)//最后一个
                    {
                        list.Add("[" + range[j - 1] + "," + range[j] + "]");
                        list.Add("[" + range[j] + ",inf]");
                    }
                    else//中间的部分
                    {
                        list.Add("[" + range[j - 1] + "," + range[j] + "]");
                    }
                }
            }
            #endregion
            #endregion

            #region 判断当前工况变量的工况范围
            #region 判断当前工况的范围
            for (int i = 0; i < list.Count; i++)
            {
                string[] tempvalue = list[i].Replace("inf", "100000000").Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                if (conditiondata > double.Parse(tempvalue[0]) && conditiondata <= double.Parse(tempvalue[1]))//找到了工况区间
                {
                    result = list[i];
                    break;
                }
            }
            #endregion
            #endregion
            return result;
        }

        /// <summary>
        /// 按照工况对每一个工况下分析变量的值进行判断
        /// </summary>
        /// <param name="turbineid">风机ID</param>
        /// <param name="status">风机状态</param>
        /// <param name="analysisvar">分析变量</param>
        /// <param name="condition_range_train">工况区间</param>
        /// <param name="analysisvar_data">分析变量监测值</param>
        /// <returns></returns>
        public List<string> RecognitionPoint(string turbineid, string status, string analysisvar, string condition_range_train, double analysisvar_data)
        {
            List<string> result = new List<string>();
            string sql = "select conditioncenter,standarddev from turbine_train_model where turbineid='" + turbineid + "' and status='" + status + "' and analysisvar='" + analysisvar + "' and conditionrange='" + condition_range_train + "'";
            DataSet ds_threshold = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql, null);//获取工况下对应分析变量的阈值中心和标准差
            try
            {
                double conditioncenter = double.Parse(ds_threshold.Tables[0].Rows[0][0].ToString());
                double standarddev = double.Parse(ds_threshold.Tables[0].Rows[0][1].ToString());
                double conditioncenter_1 = conditioncenter + standarddev;
                double conditioncenter_2 = conditioncenter + 2 * standarddev;
                double conditioncenter_3 = conditioncenter + 3 * standarddev;
                double conditioncenter_4 = conditioncenter + 4 * standarddev;
                double conditioncenter_5 = conditioncenter + 5 * standarddev;
                double conditioncenter_6 = conditioncenter + 6 * standarddev;

                #region 开始判断
                if (analysisvar_data <= conditioncenter)//是正常数据
                {
                    result.Add("0");
                    result.Add("0");
                }
                else if (analysisvar_data > conditioncenter && analysisvar_data <= conditioncenter_1)//落到了1西格玛区间
                {
                    result.Add("0");
                    result.Add("1");
                }
                else if (analysisvar_data > conditioncenter_1 && analysisvar_data <= conditioncenter_2)//落到了2西格玛区间
                {
                    result.Add("0");
                    result.Add("2");
                }
                else if (analysisvar_data > conditioncenter_2 && analysisvar_data <= conditioncenter_3)//落到了3西格玛区间
                {
                    result.Add("0");
                    result.Add("3");
                }
                else if (analysisvar_data > conditioncenter_3 && analysisvar_data <= conditioncenter_4)//落到了4西格玛区间外
                {
                    result.Add("1");
                    result.Add("4");
                }
                else if (analysisvar_data > conditioncenter_4 && analysisvar_data <= conditioncenter_5)//落到了5西格玛区间外
                {
                    result.Add("1");
                    result.Add("5");
                }
                else if (analysisvar_data > conditioncenter_5 && analysisvar_data <= conditioncenter_6)//落到了6西格玛区间外
                {
                    result.Add("1");
                    result.Add("6");
                }
                else if (analysisvar_data > conditioncenter_6)//落到了6西格玛区间外
                {
                    result.Add("1");
                    result.Add("7");
                }
                #endregion
            }
            catch (Exception e)
            {
                LogRecord.WriteLog(e.ToString());
                this.textBox4.Text = e.ToString() + "\r\n" + this.textBox4.Text;
            }


            return result;
        }

        /// <summary>
        /// 根据条件计算分析变量再工况组合下的阈值
        /// </summary>
        /// <param name="TurbineID">风机ID</param>
        /// <param name="status">风机状态</param>
        /// <param name="analysisvar">分析变量</param>
        /// <param name="conditionvar">工况变量</param>
        /// <param name="conditionrange">工况区间</param>
        /// <param name="starttime">开始时间</param>
        /// <param name="endtime">截止时间</param>
        /// <returns>list[0]阈值中心list[1]标准差</returns>
        public List<string> GetThreshold(string TurbineID, string status, string analysisvar, string conditionvar, string conditionrange, string starttime, string endtime, string table_name_clone)
        {
            List<string> threshold = new List<string>();
            string sql_where = "select " + analysisvar + " from " + table_name_clone + " where Turbineid='" + TurbineID + "' and status='" + status + "' ";//用于拼接工况的组合查询条件

            string[] conditionname = conditionvar.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);//获取所有的分析变量名称
            string[] conditionname_range = conditionrange.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);//获取所有的分析变量对应的区间
            for (int i = 0; i < conditionname.Length; i++)
            {
                sql_where = sql_where + " and " + conditionname[i];
                if (conditionname_range[i].Contains("[-inf"))//小于区间
                {
                    sql_where = sql_where + " <" + conditionname_range[i].Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
                else if (conditionname_range[i].Contains("inf]"))//大于区间
                {
                    sql_where = sql_where + " >" + conditionname_range[i].Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0];
                }
                else
                {
                    string[] temp_range = conditionname_range[i].Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    sql_where = sql_where + " between " + temp_range[0] + " and " + temp_range[1];
                }
            }

            //sql_where = sql_where + " and datatime between '"+starttime+"' and '"+endtime+"' "; //2023.1.4，由于前面在克隆表的时候已经限制了训练数据的时间段，所以这里所有的数据都是时间段以内的
            DataSet ds_analysis_data = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_where, null);//获取分析变量在条件下的数据
            threshold = GetConditionCenter(ds_analysis_data.Tables[0]);
            return threshold;
        }

        /// <summary>
        /// 计算每一个工况下的分析变量阈值和标准差
        /// </summary>
        /// <param name="starttime">训练数据开始时间</param>
        /// <param name="endtime">训练数据结束时间</param>
        /// <param name="status">风机状态</param>
        /// <param name="analysisvar">分析变量</param>
        /// <param name="condtionvar">工况变量</param>
        /// <param name="conditionrange">工况区间</param>
        /// <returns></returns>
        public List<string> ComputeThreshold(string starttime, string endtime, string turbineid, string status, string analysisvar, string conditionvar, string conditionrange)
        {
            List<string> result = new List<string>();
            #region 构建区间的查询条件
            string condition = conditionvar;
            if (conditionrange.Contains("[-inf"))//说明是个左值
            {
                condition += " < " + conditionrange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", "").Replace("]", "");
            }
            else if (conditionrange.Contains("inf]"))//说明是右值
            {
                condition += " > " + conditionrange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("[", "").Replace("]", "");
            }
            else//说明是中间值
            {
                condition += " between  " + conditionrange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0].Replace("[", "").Replace("]", "") + " and " + conditionrange.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[1].Replace("[", "").Replace("]", "") + "  ";
            }
            #endregion
            #region 构建时间的查询条件
            string time = " DataTime between '" + starttime + "' and '" + endtime + "' ";
            #endregion
            string querysql = "select " + analysisvar + " from " + GetDynamicMonitoringDataTableName.GetMonitoringDataTable() + " where turbineid='" + turbineid + "' and status=" + int.Parse(status) + " and " + condition + " and " + time;

            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, querysql, null);
            if (ds != null && ds.Tables[0].Rows.Count > 0)
            {
                //result.Add(GetConditionCenter(ds.Tables[0]));
                //result.Add(GetStandarddev(ds.Tables[0]));
                result = GetConditionCenter(ds.Tables[0]);
            }
            else
            {
                result.Add("0");
                result.Add("0");
            }
            return result;
        }

        /// <summary>
        /// 计算阈值中心和标准差
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public List<string> GetConditionCenter(DataTable dt)
        {
            List<string> list = new List<string>();
            double sum = 0.0;
            double numcount = dt.Rows.Count;//重新定义一个计数器，目的是吧中间值为0和空的数据在训练时剔除
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i][0].ToString().Equals("") || dt.Rows[i][0].ToString().Equals("0"))//不参与计数，不参与训练
                {
                    numcount--;
                }
                else
                {
                    sum += double.Parse(dt.Rows[i][0].ToString());
                }
            }
            if (numcount!=0)
            {
                double average = sum / numcount;
                list.Add(average.ToString());

                double variance = 0.0;
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (!dt.Rows[i][0].ToString().Equals("") && !dt.Rows[i][0].ToString().Equals("0"))
                    {
                        variance += Math.Pow(double.Parse(dt.Rows[i][0].ToString()) - average, 2);
                    }
                }
                list.Add(Math.Sqrt(variance / numcount).ToString());
            }
            else
            {
                list.Add("0");
                list.Add("0");
            }
            
            return list;
        }

        /// <summary>
        /// 判断特定分析变量再特定工况下异常与否
        /// </summary>
        /// <param name="turbineid">风机ID</param>
        /// <param name="status">风机状态</param>
        /// <param name="analysisvar">分析变量</param>
        /// <param name="analysisdata">分析数据</param>
        /// <param name="conditionvar">工况变量</param>
        /// <param name="condiyiondata">工况数据</param>
        /// <param name="conditionrange">工况区间</param>
        /// <returns></returns>
        public List<string> Recognidata(string turbineid, string status, string analysisvar, double analysisdata, string conditionvar, double conditiondata, string conditionrange)
        {
            List<string> result = new List<string>();
            #region 首先解析工况区间
            string[] range = conditionrange.Replace("[", "").Replace("]", "").Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            List<string> list = new List<string>();
            #region 获取所有的工况区间
            if (range.Length == 1)
            {
                list.Add("[-inf," + range[0] + "]");
                list.Add("[" + range[0] + ",inf]");
            }
            else if (range.Length > 1)
            {
                for (int j = 0; j < range.Length; j++)
                {
                    if (j == 0)//第一个
                    {
                        list.Add("[-inf," + range[j] + "]");
                    }
                    else if (j == range.Length - 1)//最后一个
                    {
                        list.Add("[" + range[j - 1] + "," + range[j] + "]");
                        list.Add("[" + range[j] + ",inf]");
                    }
                    else//中间的部分
                    {
                        list.Add("[" + range[j - 1] + "," + range[j] + "]");
                    }
                }
            }
            #endregion
            #endregion
            ConfReadAndModify conf = new ConfReadAndModify();
            string[] status_conf = conf.getConfigSetting("StatusRange").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            if (!status_conf.Contains(status))
            {
                result.Add("-1");
                result.Add("-1");
            }
            else
            {
                #region 判断当前工况的范围
                for (int i = 0; i < list.Count; i++)
                {
                    string[] tempvalue = list[i].Replace("inf", "100000000").Replace("[", "").Replace("]", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    if (conditiondata > double.Parse(tempvalue[0]) && conditiondata < double.Parse(tempvalue[1]))//找到了工况区间
                    {
                        conditionrange = list[i];
                        break;
                    }
                }
                #endregion

                #region 开始查看训练模型
                string sql_model = "select conditioncenter,standarddev from Turbine_Train where turbineid='" + turbineid + "'  and status='" + status + "' and analysisvar='" + analysisvar + "' and conditionvar='" + conditionvar + "' and conditionrange='" + conditionrange + "'";
                DataSet ds_model = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_model, null);//当前机组的分析变量
                double conditioncenter = double.Parse(ds_model.Tables[0].Rows[0][0].ToString());
                double conditioncenter_1 = double.Parse(ds_model.Tables[0].Rows[0][0].ToString()) + double.Parse(ds_model.Tables[0].Rows[0][1].ToString());
                double conditioncenter_2 = double.Parse(ds_model.Tables[0].Rows[0][0].ToString()) + 2 * double.Parse(ds_model.Tables[0].Rows[0][1].ToString());
                double conditioncenter_3 = double.Parse(ds_model.Tables[0].Rows[0][0].ToString()) + 3 * double.Parse(ds_model.Tables[0].Rows[0][1].ToString());

                if (ds_model.Tables[0].Rows.Count > 0)
                {
                    if (analysisdata < conditioncenter)//是正常数据
                    {
                        result.Add("0");
                        result.Add("0");
                    }
                    else if (analysisdata > conditioncenter && analysisdata < conditioncenter_1)//落到了1西格玛区间
                    {
                        result.Add("0");
                        result.Add("1");
                    }
                    else if (analysisdata > conditioncenter_1 && analysisdata < conditioncenter_2)//落到了2西格玛区间
                    {
                        result.Add("0");
                        result.Add("2");
                    }
                    else if (analysisdata > conditioncenter_2 && analysisdata < conditioncenter_3)//落到了3西格玛区间
                    {
                        result.Add("0");
                        result.Add("3");
                    }
                    else if (analysisdata > conditioncenter_3)//落到了3西格玛区间外
                    {
                        result.Add("1");
                        result.Add("4");
                    }
                }
                #endregion
            }
            //string statusconf=


            return result;
        }

        public int GetMaxID(string tablename)
        {
            string sql_turbine = "SELECT  max(recordid) FROM  " + tablename;
            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_turbine, null);

            if (ds.Tables[0].Rows[0][0].ToString().Equals(""))
            {
                return 1;
            }
            else
            {
                return int.Parse(ds.Tables[0].Rows[0][0].ToString()) + 1;
            }
        }

        public void WriteTurbineFaultRate()
        {
            #region 先创建数据表
            ConfReadAndModify conf = new ConfReadAndModify();
            string create_sql = "CREATE TABLE `" + conf.getConfigSetting("DataBaseName") + "`.`Turbine_Fault_Rate`  (  `recordid` int NOT NULL AUTO_INCREMENT,  `datatime` datetime NULL,  `turbineid` varchar(100) NULL,    `turbine_fault_rate` float NULL, PRIMARY KEY (`recordid`));";
            string IsExist = "SELECT table_name FROM information_schema.TABLES WHERE table_name ='Turbine_Fault_Rate';";

            DataSet ds_isexist = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, IsExist, null);

            string sql_turbine = "SELECT  distinct turbineid FROM  turbine_conf";
            DataSet ds = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_turbine, null);

            if (ds_isexist == null || ds_isexist.Tables[0].Rows.Count == 0)//不存在创建之
            {
                int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, create_sql, null);
            }


            #endregion
            MySqlConnection conn = new MySqlConnection(MYSQLHelper.connectionStringManager);
            conn.Open();
            MySqlTransaction trans = conn.BeginTransaction();
            MySqlCommand cmd = conn.CreateCommand();
            cmd.Transaction = trans;
            try
            {

                List<string> startdate = new List<string>();
                List<string> enddate = new List<string>();
                DateTime starttime = DateTime.Parse(this.dateTimePicker3.Text);
                DateTime endtime = DateTime.Parse(this.dateTimePicker4.Text);

                while (starttime <= endtime)
                {
                    startdate.Add(starttime.ToString("yyyy-MM-dd 00:00:00"));
                    enddate.Add(starttime.ToString("yyyy-MM-dd 23:59:59"));
                    starttime = starttime.AddDays(1);
                }
                for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                {
                    for (int j = 0; j < startdate.Count; j++)//获取每一天的数据
                    {
                        cmd.CommandText = "delete from Turbine_Fault_Rate where datatime='" + startdate[j] + "' and turbineID='" + ds.Tables[0].Rows[i][0].ToString() + "' ";
                        int z = cmd.ExecuteNonQuery();

                        string sql_turbine_var = "SELECT  * FROM  turbine_Var where datatime='" + startdate[j] + "' and turbineid='" + ds.Tables[0].Rows[i][0].ToString() + "' ";
                        DataSet ds_var = MYSQLHelper.GetDataSet(MYSQLHelper.connectionStringManager, sql_turbine_var, null);

                        if (ds_var.Tables[0].Rows.Count > 0)
                        {
                            string turbinefault = ComputeTurbibeFault(ds_var.Tables[0]);
                            cmd.CommandText = "insert into Turbine_Fault_Rate(datatime,turbineid,turbine_fault_rate) values('" + DateTime.Parse(startdate[j]).ToString("yyyy-MM-dd") + "', '" + ds.Tables[0].Rows[i][0].ToString() + "', " + turbinefault + ")";
                            int y = cmd.ExecuteNonQuery();
                        }
                    }
                }


                trans.Commit();
                conn.Close();
                this.textBox4.Text = "整机故障率分析已完成，结果已写入数据库！\r\n" + this.textBox4.Text; ;
                MessageBox.Show("整机故障率分析已完成，结果已写入数据库库！", "操作信息提示", MessageBoxButtons.OK);
            }
            catch (Exception ee)
            {

                trans.Rollback();
                LogRecord.WriteLog(ee.ToString());
                this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
            }
        }

        public string ComputeTurbibeFault(DataTable dt)
        {
            string result = "";
            double sum = 0.0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                sum += double.Parse(dt.Rows[i]["fault_rate"].ToString());
            }
            result = Math.Round(sum / dt.Rows.Count, 4).ToString();
            return result;
        }

        private void dataGridView1_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right)
                {
                    this.dataGridView1.CurrentCell = dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    this.dataGridView1.ClearSelection();
                    this.dataGridView1.Rows[e.RowIndex].Selected = true;
                    selectrowid = this.dataGridView1.Rows[e.RowIndex].Cells[0].Value.ToString();
                    selectrow = e.RowIndex;
                    contextMenuStrip1.Show(MousePosition.X, MousePosition.Y);
                }
            }
        }

        #region 右键菜单功能区
        /// <summary>
        /// 修改某一条配置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 修改配置信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigEditing edit = new ConfigEditing(selectrowid);
            edit.ShowDialog();
            BindingConfInfo();
        }

        /// <summary>
        /// 删除某一条配置信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 删除配置信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请确认要删除该条配置信息吗？\r\n\r\n风机:" + this.dataGridView1.Rows[selectrow].Cells["编号"].Value.ToString() + "\r\n\r\n分析变量:" + this.dataGridView1.Rows[selectrow].Cells["分析变量"].Value.ToString() + "\r\n\r\n工况变量:" + this.dataGridView1.Rows[selectrow].Cells["工况变量"].Value.ToString() + "\r\n\r\n", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                try
                {
                    string del_sql = " delete from Turbine_Conf where recordid=" + selectrowid;
                    int result = MYSQLHelper.ExecuteNonQuery(MYSQLHelper.connectionStringManager, CommandType.Text, del_sql, null);
                    this.textBox4.Text = "风机【" + this.dataGridView1.Rows[selectrow].Cells["编号"].Value.ToString() + "】【" + this.dataGridView1.Rows[selectrow].Cells["分析变量"].Value.ToString() + "】【" + this.dataGridView1.Rows[selectrow].Cells["工况变量"].Value.ToString() + "】的配置信息删除成功！\r\n" + this.textBox4.Text; ;
                    MessageBox.Show("风机【" + this.dataGridView1.Rows[selectrow].Cells["编号"].Value.ToString() + "】【" + this.dataGridView1.Rows[selectrow].Cells["分析变量"].Value.ToString() + "】【" + this.dataGridView1.Rows[selectrow].Cells["工况变量"].Value.ToString() + "】的配置信息删除成功！", "操作信息提示", MessageBoxButtons.OK);
                }
                catch (Exception ee)
                {
                    LogRecord.WriteLog(ee.ToString());
                    this.textBox4.Text = ee.ToString() + "\r\n" + this.textBox4.Text;
                }
            }
        }
        #endregion

        /// <summary>
        /// 获取所有工况变量的区间组合
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<string> GetAllConditions(List<string>[] list)
        {
            List<string> conditions = new List<string>();
            #region 1一个工况变量
            if (list.Length == 1)//只有一个工况变量
            {
                conditions = list[0];
            }
            #endregion

            #region 2个工况变量
            if (list.Length == 2)//2个工况变量，2层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString());
                    }
                }
            }
            #endregion

            #region 3个工况变量
            if (list.Length == 3)//3个工况变量，3层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString());
                        }
                    }
                }
            }
            #endregion

            #region  4个工况变量
            if (list.Length == 4)//4个工况变量，3层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString());
                            }
                        }
                    }
                }
            }
            #endregion

            #region  5个工况变量
            if (list.Length == 5)//5个工况变量，5层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString());
                                }
                            }
                        }
                    }
                }
            }

            #endregion

            #region  6个工况变量
            if (list.Length == 6)//6个工况变量，6层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    for (int i5 = 0; i5 < list[5].Count; i5++)
                                    {
                                        conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString() + ":" + list[5][i5].ToString());
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region  7个工况变量
            if (list.Length == 7)//7个工况变量，7层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    for (int i5 = 0; i5 < list[5].Count; i5++)
                                    {
                                        for (int i6 = 0; i6 < list[6].Count; i6++)
                                        {
                                            conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString() + ":" + list[5][i5].ToString() + ":" + list[6][i6].ToString());
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region  8个工况变量
            if (list.Length == 8)//8个工况变量，8层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    for (int i5 = 0; i5 < list[5].Count; i5++)
                                    {
                                        for (int i6 = 0; i6 < list[6].Count; i6++)
                                        {
                                            for (int i7 = 0; i7 < list[7].Count; i7++)
                                            {
                                                conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString() + ":" + list[5][i5].ToString() + ":" + list[6][i6].ToString() + ":" + list[7][i7].ToString());
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region  9个工况变量
            if (list.Length == 9)//8个工况变量，8层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    for (int i5 = 0; i5 < list[5].Count; i5++)
                                    {
                                        for (int i6 = 0; i6 < list[6].Count; i6++)
                                        {
                                            for (int i7 = 0; i7 < list[7].Count; i7++)
                                            {
                                                for (int i8 = 0; i8 < list[8].Count; i8++)
                                                {
                                                    conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString() + ":" + list[5][i5].ToString() + ":" + list[6][i6].ToString() + ":" + list[7][i7].ToString() + ":" + list[8][i8].ToString());
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            #region  10个工况变量
            if (list.Length == 10)//8个工况变量，8层循环
            {
                for (int i0 = 0; i0 < list[0].Count; i0++)
                {
                    for (int i1 = 0; i1 < list[1].Count; i1++)
                    {
                        for (int i2 = 0; i2 < list[2].Count; i2++)
                        {
                            for (int i3 = 0; i3 < list[3].Count; i3++)
                            {
                                for (int i4 = 0; i4 < list[4].Count; i4++)
                                {
                                    for (int i5 = 0; i5 < list[5].Count; i5++)
                                    {
                                        for (int i6 = 0; i6 < list[6].Count; i6++)
                                        {
                                            for (int i7 = 0; i7 < list[7].Count; i7++)
                                            {
                                                for (int i8 = 0; i8 < list[8].Count; i8++)
                                                {
                                                    for (int i9 = 0; i9 < list[9].Count; i9++)
                                                    {
                                                        conditions.Add(list[0][i0].ToString() + ":" + list[1][i1].ToString() + ":" + list[2][i2].ToString() + ":" + list[3][i3].ToString() + ":" + list[4][i4].ToString() + ":" + list[5][i5].ToString() + ":" + list[6][i6].ToString() + ":" + list[7][i7].ToString() + ":" + list[8][i8].ToString() + ":" + list[9][i9].ToString());
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            #endregion

            return conditions;
        }  

        private void 关于ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox ab = new AboutBox();
            ab.Show();
        }

        #region 自动执行任务功能区
        /// <summary>
        /// 时间实时显示计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer1_Tick(object sender, EventArgs e)
        {
            this.toolStripStatusLabel2.Text = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            Application.DoEvents();
        }

        /// <summary>
        /// 数据自动分析计时器
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            ConfReadAndModify conf = new ConfReadAndModify();
            string AutoAnalysis=conf.getConfigSetting("AutoAnalysis");
            string AutoAnalysisTime = conf.getConfigSetting("AutoAnalysisTime");
            string LatestAutoAnalysisDate = conf.getConfigSetting("LatestAutoAnalysisDate");//获取已经自动分析的最新日期

            DateTime dt = DateTime.Now;//用于锁定时间

            if (dt.Hour == int.Parse(AutoAnalysisTime) && AutoAnalysis.Trim().Equals("true"))//已开启自动分析，且到了指定的自动分析时间
            {
                #region 开始自动分析前一天的数据
                //if (!dt.AddDays(-1).ToString("yyyy-MM-dd").Equals("LatestAutoAnalysisDate"))//前一天的数据还没有分析则自动分析前一天的数据
                //{
                //    this.timer2.Enabled = false;//计时器先暂定
                //    DateTime time_start = DateTime.Now;//模型训练开始的时间
                //    //分析前一天的数据
                //    TurbinePointComparion(dt.AddDays(-1), dt.AddDays(-1));
                //    DateTime time_end = DateTime.Now;//模型训练结束的时间
                //    TimeSpan tp = time_end - time_start;
                //    int time = Convert.ToInt32(tp.Days * 24 * 60 * 60 + tp.Hours * 3600 + tp.Minutes * 60 + tp.Seconds);

                //    //分析完成后，吧最新的分析日期保存
                //   bool success= conf.modifyConfigSetting("LatestAutoAnalysisDate", dt.AddDays(-1).ToString("yyyy-MM-dd"));
                //   if (success)
                //   {
                //       this.textBox4.Text = dt.AddDays(-1).ToString("yyyy-MM-dd") + "的数据已经自动分析完成，耗时【" + time + "】秒，并以写入数据库！";
                //   }
                //   this.timer2.Enabled = true;
                //}
                #endregion
            }
        }
        #endregion

        #region 数据库工具功能区
        /// <summary>
        /// 数据备份配置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 备份配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataBackupSetting dbs = new DataBackupSetting();
            dbs.Show();
        }

        /// <summary>
        /// 数据备份
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void 备份数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataBackup dbu = new DataBackup();
            dbu.Show();
        }

        private void 数据库配置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DataBaseSetting dbs = new DataBaseSetting();
            dbs.Show();
        }
        #endregion

        #region 系统设置功能区
        private void 退出ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }

        private void 界面风格设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SkinSelection ss = new SkinSelection();
            ss.ShowDialog();
        }
        #endregion

        #region 风机健康状态智能诊断功能区

        private void 计数器设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AutoAnalysisSetting aas = new AutoAnalysisSetting();
            aas.Show();
        }

        private void 模型参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ParameterConfiguration pf = new ParameterConfiguration();
            pf.Show();
        }

        private void 模式训练ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ImmuneTraining it = new ImmuneTraining();
            it.Show();
        }

        private void 模式中心ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ModeCenterView mcv = new ModeCenterView();
            mcv.Show();
        }

        private void 原始数据浏览ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OriginalDataView odv = new OriginalDataView();
            odv.Show();
        }

        private void 配置数据查看ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ConfigurationView cv = new ConfigurationView();
            cv.Show();
        }

        private void 训练结果查看ToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            TrainInfoView tv = new TrainInfoView();
            tv.Show();
        }

        private void 整机异常率天ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            TurbineFaultView tfv = new TurbineFaultView();
            tfv.Show();
        }

        private void 变量异常率天ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalysisVarFault avf = new AnalysisVarFault();
            avf.Show();
        }

        private void 变量异常率点ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalysisVarPoint avp = new AnalysisVarPoint();
            avp.Show();
        }

        private void 单时刻判异结果ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointView pv = new PointView();
            pv.Show();
        }
        #endregion

        #region 海缆可靠性分析功能区
        private void 海缆数据预处理ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ElecLineDataAnalysis eld = new ElecLineDataAnalysis();
            eld.Show();
        }

        private void 海缆分析设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SettingForElecLine sfe = new SettingForElecLine();
            sfe.Show();
        }
        #endregion
    }
}
