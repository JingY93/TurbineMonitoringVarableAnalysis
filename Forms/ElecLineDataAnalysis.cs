using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using TurbineMonitoringVarableAnalysis.CommonCode;
using System.Windows.Forms.DataVisualization.Charting;

namespace TurbineMonitoringVarableAnalysis.Forms
{
    public partial class ElecLineDataAnalysis : Form
    {
        public ElecLineDataAnalysis()
        {
            InitializeComponent();
            //this.textBox1.Text = "F:\\研究数据\\海缆数据\\控制器1原始数据\\2021\\2021-04-30-16-12-42.txt";
            InitChart();
        }

        /// <summary>
        /// 解析数据文件按钮动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作会覆盖同名文件，您确认要执行解析操作吗？", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                this.textBox2.Text = "";
                Application.DoEvents();
                FolderBrowserDialog dilog = new FolderBrowserDialog();
                dilog.Description = "请选择文件夹";
                string filepath = "";
                if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
                {
                    filepath = dilog.SelectedPath;
                }
                else
                {
                    return;
                }

                this.textBox1.Text = filepath;
                DirectoryInfo di = new DirectoryInfo(filepath);

                FileInfo[] file_original = di.GetFiles();

                for (int i = 0; i < file_original.Length; i++)
                {
                    string result = ResolveOriginalFile(file_original[i]);
                    this.textBox2.Text = "【" + (i + 1) + "/" + file_original.Length + "】" + file_original[i].Name + "已经解析完成，保存至" + result + "\r\n\r\n" + this.textBox2.Text;
                    Application.DoEvents();
                }
                this.textBox2.Text = "文件夹【" + filepath + "】下的所有原始数据文件已解析完成！\r\n\r\n" + this.textBox2.Text;
                MessageBox.Show("文件夹【" + filepath + "】下的所有原始数据文件已解析完成！", "操作信息提示", MessageBoxButtons.OK);
            }
        }

        private void ElecLineDataAnalysis_Load(object sender, EventArgs e)
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
        /// 解析原始数据文件
        /// </summary>
        /// <param name="fi">原始数据文件</param>
        public string ResolveOriginalFile(FileInfo fi) 
        {
            #region 开始处理指定的原始数据文件
            FileStream fs = new FileStream(fi.FullName, FileMode.Open, FileAccess.Read);
            StreamReader sr = new StreamReader(fs);
            string strLine = sr.ReadLine();
            List<string> contentList = new List<string>();

            #region 读出所有的数据
            while (strLine != null)//原始的txt文件是按行存储的
            {
                if (!strLine.Contains("%"))
                {
                    int columncount = strLine.Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                    if (columncount > 0)
                    {
                        contentList.Add(strLine);
                    }
                }
                strLine = sr.ReadLine();
            }
            #endregion
            string data_type = "Temperature";
            //第一行是测点的位置0,1,2m，后面行是数据，只有两类测点，温度测点和应变测点
            string[] locations = contentList[0].Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            //每四行是一个时间点下的数据,温度数据是第二行，应变数据是第三行
            // ,2021-04-30T16:50:53Z,1,1
            //'Temperature 
            //'Strain profile',
            //'Peak frequency profile',

            List<string>[] datalist = new List<string>[(contentList.Count - 1) / 4];//用于存储数据
            List<string> Timelist = new List<string>();//用于存储时间

            for (int j = 2; j < contentList.Count; j = j + 4)//从第2行开始，第一行是时间，第二行是温度，第三行是应变
            {
                datalist[(j - 1) / 4] = new List<string>();
                Timelist.Add(contentList[j - 1].Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries)[0]);
                //温度数据
                int datacount = contentList[j].Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Length;
                string[] data=new string[locations.Length+1];
                if (datacount>1)//说明这一行是温度数据，则应该取第二行的数据
                {
                    data = contentList[j].Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                else//第二行没有温度数据，说明是应变数据，则应该去下一行的数据
                {
                    data_type = "Strain";
                    data = contentList[j+1].Replace(" ", "").Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                }
                for (int k = 1; k < data.Length; k++)//因为data[0]存储的是Temperature Profile或者Strain Profile，data[1]以后存储的才是数据
                {
                    datalist[(j - 1) / 4].Add(data[k]);
                }
            }
            #region 开始写入数据表
            DataTable dt = new DataTable();
            DataColumn dc = new DataColumn();
            dc.Caption = "location";
            dc.ColumnName = "location";
            dt.Columns.Add(dc);

            for (int i = 0; i < Timelist.Count; i++)
            {
                DataColumn dc1 = new DataColumn();
                dc1.Caption = Timelist[i];
                dc1.ColumnName = Timelist[i];
                dt.Columns.Add(dc1);
            }

            for (int i = 0; i < datalist[0].Count; i++)
            {
                List<string> list_temp = new List<string>();
                DataRow dr = dt.NewRow();
                list_temp.Add(locations[i].Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]);//写入位置
                for (int j = 0; j < datalist.Length; j++)//写入每一个时刻的值
                {
                    list_temp.Add(datalist[j][i]);
                }
                dr.ItemArray = list_temp.ToArray();
                dt.Rows.Add(dr);
            }
            #region 保存数据文件
            string save_path = fi.Directory.Parent.FullName + "\\" + fi.Directory.Name+ "_Proceed";
            DirectoryInfo di_save = new DirectoryInfo(save_path);
            if (!di_save.Exists)
            {
                di_save.Create();
            }
            string save_filename = fi.Name.Split(new char[] { '.' }, StringSplitOptions.RemoveEmptyEntries)[0]+"_"+data_type;
            string proceed_data_file = save_path + "\\" + save_filename + ".csv";

            #region 如果有同名文件，先删除
            FileInfo fi_exist = new FileInfo(proceed_data_file);
            if (fi_exist.Exists)
            {
                fi_exist.Delete();
            }
            #endregion


            string result = ExcelHelper.SaveCSV(dt, save_filename, save_path);
            return result;
            
            #endregion
            #endregion

            #endregion
        }

        /// <summary>
        /// 拼接处理后的数据文件，形成一个新的合成数据文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("该操作会覆盖同名文件，您确认要执行合并操作吗？", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
            {
                this.textBox2.Text = "";
                Application.DoEvents();
                FolderBrowserDialog dilog = new FolderBrowserDialog();
                dilog.Description = "请选择文件夹";
                string filepath = "";
                if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
                {
                    filepath = dilog.SelectedPath;
                }
                else
                {
                    return;
                }

                this.textBox3.Text = filepath;
                DirectoryInfo di = new DirectoryInfo(filepath);

                FileInfo[] file_original = di.GetFiles("*.csv");
                string save_path_temp = di.Parent.FullName + "\\" + di.Name + "_Combined";
                DirectoryInfo di_save_temp = new DirectoryInfo(save_path_temp);
                if (di_save_temp.Exists)//该文件夹存在，说明之前已经进行过拼接，那么这个时候需要进行时间的判断，如果已经拼接过了，就要跳过，不重复拼接数据，避免数据重复
                {
                    //获取合并文件的起止时间
                    FileInfo[] combined_fiels=di_save_temp.GetFiles("*.csv");
                    string combinedname = combined_fiels[0].Name;//格式为：20230112153209_CobinedData_2021-04-30-16-12-42_2021-05-13-09-39-19 _1.csv
                    string[] name_contents = combinedname.Split(new char[]{'_'},StringSplitOptions.RemoveEmptyEntries);

                    string startdate = name_contents[2];//2021-04-30-16-12-42
                    string startdate_for_combinedfile = startdate;
                    string[] startdate_splits = startdate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    startdate = startdate_splits[0] + "-" + startdate_splits[1] + "-" + startdate_splits[2] + " " + startdate_splits[3] + ":" + startdate_splits[4] + ":" + startdate_splits[5];//2021-04-30 16-12-42
                    
                    string enddate = name_contents[3];//2021-05-13-09-39-19
                    string[] enddate_splits = enddate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    enddate = enddate_splits[0] + "-" + enddate_splits[1] + "-" + enddate_splits[2] + " " + enddate_splits[3] + ":" + enddate_splits[4] + ":" + enddate_splits[5];//2021-04-30 16-12-42
                    
                    DateTime datetime_startdate = DateTime.Parse(startdate);
                    DateTime datetime_enddate = DateTime.Parse(enddate);

                    for (int i = 0; i < file_original.Length; i++)
                    {
                        //获取合并文件的起止时间
                        string filename = file_original[i].Name;
                        string file_date = filename.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];//2021-04-30-16-12-42

                        string[] file_date_splits = file_date.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                        file_date = file_date_splits[0] + "-" + file_date_splits[1] + "-" + file_date_splits[2] + " " + file_date_splits[3] + ":" + file_date_splits[4] + ":" + file_date_splits[5];//2021-04-30 16-12-42

                        DateTime datetime_file_date = DateTime.Parse(file_date);
                        if (datetime_file_date<=datetime_enddate)//说明文件数据的时间已经在拼接的数据组中了（至少是有数据重叠的，对于这一部分数据，不做处理）
                        {
                            file_original[i].Delete();//删除这个文件
                        }
                    }
                    #region 这是一个已有的过程，已经有过合并文件，要把新的数据，追加到已有合并文件的后边（列追加）
                    //经过校验之后，重新获取一下文件列表
                    file_original = di.GetFiles("*.csv");

                    if (file_original.Length > 0)
                    {
                        file_original = OrderFileByDate(file_original);
                        //由于线缆长度较长，为了后续处理速度，将文件拆分成10个文件,也即是同一个文件夹下的合并操作执行10次
                        string version = DateTime.Now.ToString("yyyyMMddHHmmss");//这个不需要了，最后会更新文件名

                        FileInfo[] file_combined = di_save_temp.GetFiles();//获取合并数据文件夹下的数据文件
                        file_combined = OrderFileByNum(file_combined);


                        DataTable dt_data_basic = ExcelHelper.OpenCSV2(file_original[0].FullName, 1);//先把第1个数据文件拿到，目的是要确定行数，计算每个文件的起始行

                        //DataTable dt_data_combined = ExcelHelper.OpenCSV2(file_combined[i].FullName, 1);//获取第i个拼接数据文件片段

                        int start_row = 0;
                        int end_row = 0;
                        int total_data_rows = dt_data_basic.Rows.Count;
                        int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

                        for (int m = 0; m < 10; m++)
                        {
                            #region 计算每一个子文件的起止行
                            if (m < 9)//非最后一个文件
                            {
                                #region 计算当前文件中数据的起止行
                                start_row = m * file_data_rows;
                                end_row = (m + 1) * file_data_rows - 1;
                                #endregion
                            }
                            else
                            {
                                #region 计算当前文件中数据的起止行
                                start_row = m * file_data_rows;
                                end_row = total_data_rows - 1;
                                #endregion
                            }
                            #endregion

                            DataTable combined_dt = ExcelHelper.OpenCSV2(file_combined[m].FullName, 1);//获取第i个拼接数据文件片段

                            #region 开始合并数据
                            for (int i = 1; i < file_original.Length; i++)
                            {
                                DataTable dt_data_new = ExcelHelper.OpenCSV2(file_original[i].FullName, 1);//要拼接的数据文件

                                DataTable combine_dt = CombinDataTable(combined_dt, GetRangeData(dt_data_new, start_row, end_row));

                                combined_dt.Rows.Clear();
                                combined_dt.Columns.Clear();
                                combined_dt = combine_dt.Copy();//把合并后的数据重新赋给dt_data_basic

                                this.textBox2.Text = "【" + (m + 1) + "/10】【" + i + "/" + (file_original.Length - 1) + "】 正在拼接" + file_original[i].Name + "...\r\n\r\n" + this.textBox2.Text;
                                Application.DoEvents();
                            }
                            #endregion

                            #region 开始写数据

                            string save_path = di.Parent.FullName + "\\" + di.Name + "_Combined";
                            DirectoryInfo di_save = new DirectoryInfo(save_path);
                            if (!di_save.Exists)
                            {
                                di_save.Create();
                            }

                            string save_filename = version + "_CobinedData_" + startdate_for_combinedfile + "_" + file_original[file_original.Length - 1].Name.Split(new char[] { '_' })[0] + "_" + file_original[0].Name.Split(new char[] { '_' })[1].Split(new char[] { '.' })[0] + "_" + (m + 1);
                            string result = ExcelHelper.SaveCSV(combined_dt, save_filename, save_path);
                            this.textBox2.Text = "第" + (m + 1) + "个合并文件【" + result + "】已拼接完成\r\n\r\n" + this.textBox2.Text;
                            #endregion
                        }

                        #region 处理后，对原有的合并数据文件进行删除，因为在新的文件中已经包含了原来的数据
                        for (int i = 0; i < file_combined.Length; i++)
                        {
                            file_combined[i].Delete();
                        }
                        #endregion

                        #region 处理完之后把所有的原始数据备份，当前文件夹清空
                        string path_backup = di.Parent.FullName + "\\" + di.Name + "_Backup";
                        DirectoryInfo di_backup = new DirectoryInfo(path_backup);
                        if (!di_backup.Exists)
                        {
                            di_backup.Create();
                        }
                        //备份
                        for (int i = 0; i < file_original.Length; i++)
                        {
                            string file_backip_path = path_backup + "\\" + file_original[i].Name;
                            file_original[i].CopyTo(file_backip_path, true);
                            this.textBox2.Text = "文件" + file_original[i].Name + "已备份成功!\r\n\r\n" + this.textBox2.Text;
                            Application.DoEvents();
                        }
                        //删除
                        for (int i = 0; i < file_original.Length; i++)
                        {
                            file_original[i].Delete();
                        }
                        #endregion
                    }
                    #endregion

                }
                else
                {
                    #region 是一个全新的处理，从无到有
                    //经过校验之后，重新获取一下文件列表
                    file_original = di.GetFiles("*.csv");

                    if (file_original.Length > 0)
                    {
                        file_original = OrderFileByDate(file_original);
                        //由于线缆长度较长，为了后续处理速度，将文件拆分成10个文件,也即是同一个文件夹下的合并操作执行10次
                        string version = DateTime.Now.ToString("yyyyMMddHHmmss");

                        for (int m = 0; m < 10; m++)
                        {
                            DataTable dt_data_basic = ExcelHelper.OpenCSV2(file_original[0].FullName, 1);//先把第1个数据文件拿到，目的是要确定行数，计算每个文件的起始行

                            int start_row = 0;
                            int end_row = 0;
                            int total_data_rows = dt_data_basic.Rows.Count;
                            int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

                            #region 计算每一个子文件的起止行
                            if (m < 9)//非最后一个文件
                            {
                                #region 计算当前文件中数据的起止行
                                start_row = m * file_data_rows;
                                end_row = (m + 1) * file_data_rows - 1;
                                #endregion
                            }
                            else
                            {
                                #region 计算当前文件中数据的起止行
                                start_row = m * file_data_rows;
                                end_row = total_data_rows - 1;
                                #endregion
                            }
                            #endregion

                            DataTable combined_dt = GetRangeData(dt_data_basic, start_row, end_row);

                            #region 开始合并数据
                            for (int i = 1; i < file_original.Length; i++)
                            {
                                DataTable dt_data_new = ExcelHelper.OpenCSV2(file_original[i].FullName, 1);//要拼接的数据文件

                                DataTable combine_dt = CombinDataTable(combined_dt, GetRangeData(dt_data_new, start_row, end_row));

                                combined_dt.Rows.Clear();
                                combined_dt.Columns.Clear();
                                combined_dt = combine_dt.Copy();//把合并后的数据重新赋给dt_data_basic

                                this.textBox2.Text = "【" + (m + 1) + "/10】【" + i + "/" + (file_original.Length - 1) + "】 正在拼接" + file_original[i].Name + "...\r\n\r\n" + this.textBox2.Text;
                                Application.DoEvents();
                            }
                            #endregion

                            #region 开始写数据

                            string save_path = di.Parent.FullName + "\\" + di.Name + "_Combined";
                            DirectoryInfo di_save = new DirectoryInfo(save_path);
                            if (!di_save.Exists)
                            {
                                di_save.Create();
                            }

                            string save_filename = version + "_CobinedData_" + file_original[0].Name.Split(new char[] { '_' })[0] + "_" + file_original[file_original.Length - 1].Name.Split(new char[] { '_' })[0] + "_" + file_original[0].Name.Split(new char[] { '_' })[1].Split(new char[] { '.' })[0] + "_" + (m + 1);
                            string result = ExcelHelper.SaveCSV(combined_dt, save_filename, save_path);
                            this.textBox2.Text = "第" + (m + 1) + "个合并文件【" + result + "】已拼接完成\r\n\r\n" + this.textBox2.Text;
                            #endregion
                        }

                        #region 处理完之后把所有的原始数据备份，当前文件夹清空
                        string path_backup = di.Parent.FullName + "\\" + di.Name + "_Backup";
                        DirectoryInfo di_backup = new DirectoryInfo(path_backup);
                        if (!di_backup.Exists)
                        {
                            di_backup.Create();
                        }
                        //备份
                        for (int i = 0; i < file_original.Length; i++)
                        {
                            string file_backip_path = path_backup + "\\" + file_original[i].Name;
                            file_original[i].CopyTo(file_backip_path, true);
                            this.textBox2.Text =  "文件"+file_original[i].Name+"已备份成功!\r\n\r\n"+this.textBox2.Text;
                            Application.DoEvents();
                        }
                        //删除
                        for (int i = 0; i < file_original.Length; i++)
                        {
                            file_original[i].Delete();
                        }
                        #endregion
                    }
                    #endregion
                }
                this.textBox2.Text = "文件夹【" + filepath + "】下的所有数据文件已合并完成！\r\n\r\n" + this.textBox2.Text;
                MessageBox.Show("文件夹【" + filepath + "】下的所有数据文件已合并完成！", "操作信息提示", MessageBoxButtons.OK);
            }
        }

        /// <summary>
        /// 合并文件夹下的数据文件
        /// </summary>
        /// <param name="filepath">文件夹路径</param>
        public void DoDataCombinding(string filepath)
        {
            DirectoryInfo di = new DirectoryInfo(filepath);

            FileInfo[] file_original = di.GetFiles("*.csv");
            string save_path_temp = di.Parent.FullName + "\\" + di.Name + "_Combined";
            DirectoryInfo di_save_temp = new DirectoryInfo(save_path_temp);

            
            if (di_save_temp.Exists)//该文件夹存在，说明之前已经进行过拼接，那么这个时候需要进行时间的判断，如果已经拼接过了，就要跳过，不重复拼接数据，避免数据重复
            {
                #region 对文件夹下的文件进行过滤，过滤掉已经处理过的数据文件，避免数据重复
                //获取合并文件的起止时间
                FileInfo[] combined_fiels = di_save_temp.GetFiles("*.csv");
                string combinedname = combined_fiels[0].Name;//格式为：20230112153209_CobinedData_2021-04-30-16-12-42_2021-05-13-09-39-19 _1.csv
                string[] name_contents = combinedname.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);

                string startdate = name_contents[2];//2021-04-30-16-12-42
                string[] startdate_splits = startdate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                startdate = startdate_splits[0] + "-" + startdate_splits[1] + "-" + startdate_splits[2] + " " + startdate_splits[3] + ":" + startdate_splits[4] + ":" + startdate_splits[5];//2021-04-30 16-12-42

                string enddate = name_contents[3];//2021-05-13-09-39-19
                string[] enddate_splits = enddate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                enddate = enddate_splits[0] + "-" + enddate_splits[1] + "-" + enddate_splits[2] + " " + enddate_splits[3] + ":" + enddate_splits[4] + ":" + enddate_splits[5];//2021-04-30 16-12-42

                DateTime datetime_startdate = DateTime.Parse(startdate);
                DateTime datetime_enddate = DateTime.Parse(enddate);

                for (int i = 0; i < file_original.Length; i++)
                {
                    //获取合并文件的起止时间
                    string filename = file_original[i].Name;
                    string file_date = filename.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];//2021-04-30-16-12-42

                    string[] file_date_splits = file_date.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
                    file_date = file_date_splits[0] + "-" + file_date_splits[1] + "-" + file_date_splits[2] + " " + file_date_splits[3] + ":" + file_date_splits[4] + ":" + file_date_splits[5];//2021-04-30 16-12-42

                    DateTime datetime_file_date = DateTime.Parse(file_date);
                    if (datetime_file_date <= datetime_enddate)//说明文件数据的时间已经在拼接的数据组中了（至少是有数据重叠的，对于这一部分数据，不做处理）
                    {
                        file_original[i].Delete();//删除这个文件
                    }
                }
                #endregion

                #region 这是一个已有的过程，已经有过合并文件，要把新的数据，追加到已有合并文件的后边（列追加）
                //经过校验之后，重新获取一下文件列表
                file_original = di.GetFiles("*.csv");

                if (file_original.Length > 0)
                {
                    file_original = OrderFileByDate(file_original);
                    //由于线缆长度较长，为了后续处理速度，将文件拆分成10个文件,也即是同一个文件夹下的合并操作执行10次
                    string version = DateTime.Now.ToString("yyyyMMddHHmmss");//这个不需要了，最后会更新文件名

                    FileInfo[] file_combined = di_save_temp.GetFiles();//获取合并数据文件夹下的数据文件
                    file_combined = OrderFileByNum(file_combined);


                    DataTable dt_data_basic = ExcelHelper.OpenCSV2(file_original[0].FullName, 1);//先把第1个数据文件拿到，目的是要确定行数，计算每个文件的起始行

                    //DataTable dt_data_combined = ExcelHelper.OpenCSV2(file_combined[i].FullName, 1);//获取第i个拼接数据文件片段

                    int start_row = 0;
                    int end_row = 0;
                    int total_data_rows = dt_data_basic.Rows.Count;
                    int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

                    for (int m = 0; m < 10; m++)
                    {
                        #region 计算每一个子文件的起止行
                        if (m < 9)//非最后一个文件
                        {
                            #region 计算当前文件中数据的起止行
                            start_row = m * file_data_rows;
                            end_row = (m + 1) * file_data_rows - 1;
                            #endregion
                        }
                        else
                        {
                            #region 计算当前文件中数据的起止行
                            start_row = m * file_data_rows;
                            end_row = total_data_rows - 1;
                            #endregion
                        }
                        #endregion

                        DataTable combined_dt = ExcelHelper.OpenCSV2(file_combined[m].FullName, 1);//获取第i个拼接数据文件片段

                        #region 开始合并数据
                        for (int i = 1; i < file_original.Length; i++)
                        {
                            DataTable dt_data_new = ExcelHelper.OpenCSV2(file_original[i].FullName, 1);//要拼接的数据文件

                            DataTable combine_dt = CombinDataTable(combined_dt, GetRangeData(dt_data_new, start_row, end_row));

                            combined_dt.Rows.Clear();
                            combined_dt.Columns.Clear();
                            combined_dt = combine_dt.Copy();//把合并后的数据重新赋给dt_data_basic

                            //this.textBox2.Text = "【" + (m + 1) + "/10】【" + i + "/" + (file_original.Length - 1) + "】 正在拼接" + file_original[i].Name + "...\r\n\r\n" + this.textBox2.Text;
                            //Application.DoEvents();
                        }
                        #endregion

                        #region 开始写数据

                        string save_path = di.Parent.FullName + "\\" + di.Name + "_Combined";
                        DirectoryInfo di_save = new DirectoryInfo(save_path);
                        if (!di_save.Exists)
                        {
                            di_save.Create();
                        }

                        string save_filename = version + "_CobinedData_" + combinedname + "_" + file_original[file_original.Length - 1].Name.Split(new char[] { '_' })[0] + "_" + file_original[0].Name.Split(new char[] { '_' })[1].Split(new char[] { '.' })[0] + "_" + (m + 1);
                        string result = ExcelHelper.SaveCSV(combined_dt, save_filename, save_path);
                        #endregion
                    }

                    #region 处理后，对原有的合并数据文件进行删除，因为在新的文件中已经包含了原来的数据
                    for (int i = 0; i < file_combined.Length; i++)
                    {
                        file_combined[i].Delete();
                    }
                    #endregion

                    #region 处理完之后把所有的原始数据备份，当前文件夹清空
                    string path_backup = di.Parent.FullName + "\\" + di.Name + "_Backup";
                    DirectoryInfo di_backup = new DirectoryInfo(path_backup);
                    if (!di_backup.Exists)
                    {
                        di_backup.Create();
                    }
                    //备份
                    for (int i = 0; i < file_original.Length; i++)
                    {
                        string file_backip_path = path_backup + "\\" + file_original[i].Name;
                        file_original[i].CopyTo(file_backip_path, true);
                    }
                    //删除
                    for (int i = 0; i < file_original.Length; i++)
                    {
                        file_original[i].Delete();
                    }
                    #endregion
                }
                #endregion
            }
            else//文件夹不存在，说明是第一次合并
            {
                #region 是一个全新的处理，从无到有
                //经过校验之后，重新获取一下文件列表
                file_original = di.GetFiles("*.csv");

                if (file_original.Length > 0)
                {
                    file_original = OrderFileByDate(file_original);
                    //由于线缆长度较长，为了后续处理速度，将文件拆分成10个文件,也即是同一个文件夹下的合并操作执行10次
                    string version = DateTime.Now.ToString("yyyyMMddHHmmss");

                    for (int m = 0; m < 10; m++)
                    {
                        DataTable dt_data_basic = ExcelHelper.OpenCSV2(file_original[0].FullName, 1);//先把第1个数据文件拿到，目的是要确定行数，计算每个文件的起始行

                        int start_row = 0;
                        int end_row = 0;
                        int total_data_rows = dt_data_basic.Rows.Count;
                        int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

                        #region 计算每一个子文件的起止行
                        if (m < 9)//非最后一个文件
                        {
                            #region 计算当前文件中数据的起止行
                            start_row = m * file_data_rows;
                            end_row = (m + 1) * file_data_rows - 1;
                            #endregion
                        }
                        else
                        {
                            #region 计算当前文件中数据的起止行
                            start_row = m * file_data_rows;
                            end_row = total_data_rows - 1;
                            #endregion
                        }
                        #endregion

                        DataTable combined_dt = GetRangeData(dt_data_basic, start_row, end_row);

                        #region 开始合并数据
                        for (int i = 1; i < file_original.Length; i++)
                        {
                            DataTable dt_data_new = ExcelHelper.OpenCSV2(file_original[i].FullName, 1);//要拼接的数据文件

                            DataTable combine_dt = CombinDataTable(combined_dt, GetRangeData(dt_data_new, start_row, end_row));

                            combined_dt.Rows.Clear();
                            combined_dt.Columns.Clear();
                            combined_dt = combine_dt.Copy();//把合并后的数据重新赋给dt_data_basic
                        }
                        #endregion

                        #region 开始写数据

                        string save_path = di.Parent.FullName + "\\" + di.Name + "_Combined";
                        DirectoryInfo di_save = new DirectoryInfo(save_path);
                        if (!di_save.Exists)
                        {
                            di_save.Create();
                        }

                        string save_filename = version + "_CobinedData_" + file_original[0].Name.Split(new char[] { '_' })[0] + "_" + file_original[file_original.Length - 1].Name.Split(new char[] { '_' })[0] + "_" + file_original[0].Name.Split(new char[] { '_' })[1].Split(new char[] { '.' })[0] + "_" + (m + 1);
                        string result = ExcelHelper.SaveCSV(combined_dt, save_filename, save_path);
                        #endregion
                    }

                    #region 处理完之后把所有的原始数据备份，当前文件夹清空
                    string path_backup = di.Parent.FullName + "\\" + di.Name + "_Backup";
                    DirectoryInfo di_backup = new DirectoryInfo(path_backup);
                    if (!di_backup.Exists)
                    {
                        di_backup.Create();
                    }
                    //备份
                    for (int i = 0; i < file_original.Length; i++)
                    {
                        string file_backip_path = path_backup + "\\" + file_original[i].Name;
                        file_original[i].CopyTo(file_backip_path, true);
                    }
                    //删除
                    for (int i = 0; i < file_original.Length; i++)
                    {
                        file_original[i].Delete();
                    }
                    #endregion
                }
                #endregion
            }
        }

        /// <summary>
        /// 按照问阿金名中包含的时间对文件进行排序
        /// 目的是保证在数据拼接的过程中是按照时间先后顺序拼接的
        /// </summary>
        /// <param name="Original_files"></param>
        /// <returns></returns>
        public FileInfo[] OrderFileByDate(FileInfo[] Original_files) 
        {
            DateTime[] list_file_date = new DateTime[Original_files.Length];
            for (int i = 0; i < Original_files.Length; i++)
            {
                //2021-04-30-16-12-42_Temperature.csv
                string file_name = Original_files[i].Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[0];//得到2021-04-30-16-12-42
                string[] date_detail = file_name.Split(new char[]{'-'},StringSplitOptions.RemoveEmptyEntries);
                string date_string = date_detail[0] + "-" + date_detail[1] + "-" + date_detail[2] + " " + date_detail[3] + ":" + date_detail[4] + ":" + date_detail[5];
                list_file_date[i]=DateTime.Parse(date_string);
            }
            for (int i = 0; i < list_file_date.Length-1; i++)
            {
                for (int j = 0; j < list_file_date.Length-1-i; j++)
                {
                    if (list_file_date[j]>list_file_date[j+1])
                    {
                        DateTime temp_date = list_file_date[j];
                        list_file_date[j] = list_file_date[j+1];
                        list_file_date[j + 1] = temp_date;

                        FileInfo temp_fi = Original_files[j];
                        Original_files[j] = Original_files[j + 1];
                        Original_files[j + 1] = temp_fi;
                    }
                }
            }
            return Original_files;
        }

        /// <summary>
        /// 按照合并文件的序号对文件进行排序
        /// 目的是保证在数据拼接的过程中是按照序号先后顺序拼接的
        /// </summary>
        /// <param name="Original_files"></param>
        /// <returns></returns>
        public FileInfo[] OrderFileByNum(FileInfo[] Original_files)
        {
            //DateTime[] list_file_date = new DateTime[Original_files.Length];
            int[] list_file_num=new int[Original_files.Length];
            for (int i = 0; i < Original_files.Length; i++)
            {
                //20230112212900_CobinedData_2021-04-30-16-12-42_2021-05-24-20-33-15_Temperature_1
                string file_name = Original_files[i].Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries)[5].Split(new char[]{'.'})[0];//得到序号：1
                list_file_num[i] = int.Parse(file_name);
            }
            for (int i = 0; i < list_file_num.Length - 1; i++)
            {
                for (int j = 0; j < list_file_num.Length - 1 - i; j++)
                {
                    if (list_file_num[j] > list_file_num[j + 1])
                    {
                        int temp_num = list_file_num[j];
                        list_file_num[j] = list_file_num[j + 1];
                        list_file_num[j + 1] = temp_num;

                        FileInfo temp_fi = Original_files[j];
                        Original_files[j] = Original_files[j + 1];
                        Original_files[j + 1] = temp_fi;
                    }
                }
            }
            return Original_files;
        }

        /// <summary>
        /// 获取数据表指定航范围内的数据
        /// </summary>
        /// <param name="original_dt">数据表</param>
        /// <param name="start_row">开始行</param>
        /// <param name="end_row">结束行</param>
        /// <returns></returns>
        public DataTable GetRangeData(DataTable original_dt, int start_row, int end_row) 
        {
            DataTable dt = original_dt.Clone();
            for (int i = start_row; i <= end_row; i++)
            {
                DataRow dr = original_dt.Rows[i];
                DataRow drr = dt.NewRow();
                drr.ItemArray = dr.ItemArray;
                dt.Rows.Add(drr);
            }
            return dt;
        }

        /// <summary>
        /// 横向合并两个数据表的数据
        /// </summary>
        /// <param name="dt_data_basic">第一张数据表</param>
        /// <param name="dt_data_new">第二张数据表</param>
        /// <returns></returns>
        public DataTable CombinDataTable(DataTable dt_data_basic, DataTable dt_data_new) 
        {
            DataTable dt = new DataTable();
            for (int i = 0; i < dt_data_basic.Columns.Count; i++)//第一个表的所有列
            {
                DataColumn dc = new DataColumn();
                dc.Caption = dt_data_basic.Columns[i].ColumnName;
                dc.ColumnName = dt_data_basic.Columns[i].ColumnName;
                dt.Columns.Add(dc);
            }
            for (int j = 1; j < dt_data_new.Columns.Count; j++)//第二张表除了第一列以后的所有列
            {
                DataColumn dc = new DataColumn();
                dc.Caption = dt_data_new.Columns[j].ColumnName;
                dc.ColumnName = dt_data_new.Columns[j].ColumnName;
                dt.Columns.Add(dc);
            }

            for (int k = 0; k < dt_data_basic.Rows.Count; k++)
            {
                List<string> list_data = new List<string>();
                DataRow dr = dt.NewRow();
                for (int i = 0; i < dt_data_basic.Columns.Count; i++)
                {
                    list_data.Add(dt_data_basic.Rows[k][i].ToString());
                }
                 for (int i = 1; i < dt_data_new.Columns.Count; i++)
                {
                    list_data.Add(dt_data_new.Rows[k][i].ToString());
                }

                dr.ItemArray=list_data.ToArray();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            string filepath = "F:\\研究数据\\海缆数据\\大丰2021年5月\\[-]\\Controller\\10219\\1\\2021_Proceed\\2021-04-30-16-12-42_Temperature.csv";
            FileInfo fi = new FileInfo(filepath);

            string startdate = "2021-04-30-16-12-42";
            string[] startdate_splits = startdate.Split(new char[] { '-' }, StringSplitOptions.RemoveEmptyEntries);
            startdate = startdate_splits[0] + "-" + startdate_splits[1] + "-" + startdate_splits[2] + " " + startdate_splits[3] + ":" + startdate_splits[4] + ":" + startdate_splits[5];//2021-04-30 16-12-42

            MessageBox.Show(fi.Name);
        }

        /// <summary>
        /// 单击选择合并数据文件夹的功能
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBox4_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog dilog = new FolderBrowserDialog();
            dilog.Description = "请选择文件夹";
            string filepath = "";
            if (dilog.ShowDialog() == DialogResult.OK || dilog.ShowDialog() == DialogResult.Yes)
            {
                filepath = dilog.SelectedPath;
            }
            else
            {
                return;
            }
            this.textBox4.Text = filepath;

            DirectoryInfo di = new DirectoryInfo(filepath);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件
            if (file_combined.Length>0)
            {
                file_combined = OrderFileByNum(file_combined);

                DataTable dt = ExcelHelper.OpenCSV2(file_combined[file_combined.Length-1].FullName,1);

                int rows=int.Parse(dt.Rows[dt.Rows.Count-1][0].ToString())+1;
                int cols = dt.Columns.Count-1;

                #region 初始化控件内容
                this.textBox5.Text = rows.ToString();
                this.textBox6.Text = cols.ToString();
                this.comboBox1.Items.Clear();
                this.comboBox2.Items.Clear();
                for (int i = 0; i < rows-1; i++)
                {
                    ComBoxItem item = new ComBoxItem();
                    item.Text = i.ToString();
                    item.Value = i.ToString();
                    this.comboBox1.Items.Add(item);
                }
                if (this.comboBox1.Items.Count>0)
                {
                    this.comboBox1.SelectedIndex = 0;
                }
                for (int i = 1; i < cols; i++)
                {
                    ComBoxItem item = new ComBoxItem();
                    item.Text = dt.Columns[i].ColumnName; 
                    item.Value =dt.Columns[i].ColumnName; 
                    this.comboBox2.Items.Add(item);
                }
                if (this.comboBox2.Items.Count > 0)
                {
                    this.comboBox2.SelectedIndex = 0;
                }

                this.textBox2.Text = "海缆基础数据读取成功\r\n\r\n"+this.textBox2.Text;
                Application.DoEvents();
                #endregion

                this.button3.Enabled = true;
            }
            else
            {
                return;
            }
        }

        /// <summary>
        /// 检索按钮事件动作
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click_1(object sender, EventArgs e)
        {
            string location = this.comboBox1.SelectedItem.ToString();
            string datadate = this.comboBox2.SelectedItem.ToString();

            DirectoryInfo di = new DirectoryInfo(this.textBox4.Text);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件

            file_combined = OrderFileByNum(file_combined);
            //用于存储每一个数据文件的起止行
            List<int> start_num = new List<int>();
            List<int> end_num = new List<int>();

            #region 获取每一个数据文件的起止行
            int total_data_rows = int.Parse(this.textBox5.Text);
            int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

            int start_row = 0;
            int end_row = 0;

            for (int i = 0; i < 10; i++)
            {
                if (i<9)
                {
                    start_row = i * file_data_rows;
                    end_row = (i + 1) * file_data_rows - 1;
                }
                else
                {
                    start_row = i * file_data_rows;
                    end_row = total_data_rows - 1;
                }
                start_num.Add(start_row);
                end_num.Add(end_row);
            }
            #endregion

            #region 判断要检索的位置位于第几个文件
            int int_location = int.Parse(location);
            string target_file = "";
            for (int i = 0; i < start_num.Count; i++)
            {
                if (int_location >= start_num[i] && int_location <= end_num[i])
                {
                    target_file = file_combined[i].FullName;
                    break;
                }
            }
            #endregion

            if (this.radioButton3.Checked)//按照位置检索
            {
                #region 如果按照位置检索，则返回该位置下的所有时刻数据，所以GridView的列是时间和值
                this.textBox2.Text = "正在检索位置【"+location+"】的数据，可能会耗时较长，请耐心等待...\r\n\r\n"+this.textBox2.Text;
                Application.DoEvents();
                this.dataGridView1.Columns.Clear();
                
                DataTable dtt = SearchDataByLocation(this.textBox4.Text,location);

                this.dataGridView1.DataSource = dtt;
                this.groupBox7.Text = "位置【" + location + "】的所有数据【" + dtt .Rows.Count+ "】";
                this.groupBox9.Text = "位置【" + location + "】的数据曲线";
                DrawChart(dtt);

                List<double> manmin_chart1 = GetMaxMinData(ConvertDataTableToList(dtt, 2));

                this.chart1.ChartAreas["ChartArea1"].AxisY.Minimum = manmin_chart1[0] * 0.95;
                this.chart1.ChartAreas["ChartArea1"].AxisY.Maximum = manmin_chart1[manmin_chart1.Count - 1] * 1.05;

                this.textBox2.Text = "位置【" + location + "】的数据，已检索和绘制完毕，请查看\r\n\r\n" + this.textBox2.Text;
                Application.DoEvents();
                #endregion
            }
            else if (this.radioButton4.Checked)//按照时间检索
            {
                #region 如果按照时间检索，则返回该时间下的所有数据，是一个数据文件拼接问题
                this.textBox2.Text = "正在检索时刻【" + datadate + "】的数据，可能会耗时较长，请耐心等待...\r\n\r\n" + this.textBox2.Text;
                Application.DoEvents();
                this.dataGridView1.Columns.Clear();

                DataTable dtt = SearchDataByDatatime(this.textBox4.Text,datadate);
                
                this.dataGridView1.DataSource = dtt;
                this.groupBox7.Text = "时刻【" + datadate + "】的所有数据【" + dtt.Rows.Count + "】";
                this.groupBox9.Text = "时刻【" + datadate + "】的数据曲线";
                DrawChart(dtt);
                this.textBox2.Text = "时刻【" + datadate + "】的数据，已检索和绘制完毕，请查看\r\n\r\n" + this.textBox2.Text;
                Application.DoEvents();
                #endregion
            }
            else//混合检索
            {
                #region 根据时间和位置混合查询
                this.textBox2.Text = "正在检索位置【"+location+"】在时刻【" + datadate + "】的数据，可能会耗时较长，请耐心等待...\r\n\r\n" + this.textBox2.Text;
                Application.DoEvents();
                this.dataGridView1.Columns.Clear();

                DataTable dtt = SearchDataByDatatimeAndLocation(this.textBox4.Text,datadate,location);

                this.groupBox7.Text = "位置【" + location + "】在时刻【"+datadate+"】的数据";
                this.groupBox9.Text = "数据曲线";

                this.dataGridView1.DataSource = dtt;

                this.textBox2.Text = "位置【" + location + "】在时刻【" + datadate + "】的数据，已检索和绘制完毕，请查看...\r\n\r\n" + this.textBox2.Text;
                Application.DoEvents();
                #endregion

                #region 弹出数据判异功能
                if (MessageBox.Show("您是否要分析位置【" + location + "】在时刻【" + datadate + "】时的异常情况？", "操作信息提示", MessageBoxButtons.OKCancel, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    ShowElectrLineData sed=new ShowElectrLineData(this.textBox4.Text,datadate,location,this.textBox5.Text, dtt.Rows[0][2].ToString());
                    sed.Show();
                }
                #endregion
            }
        }

        /// <summary>
        /// 根据位置进行数据查找
        /// </summary>
        /// <param name="direcorypath">数据文件地址</param>
        /// <param name="location">要查询的位置</param>
        /// <returns></returns>
        public DataTable SearchDataByLocation(string direcorypath,string location) 
        {
            string target_file = "";//用于存储找到的目标数据文件
            DirectoryInfo di = new DirectoryInfo(direcorypath);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件

            file_combined = OrderFileByNum(file_combined);


            //用于存储每一个数据文件的起止行
            List<int> start_num = new List<int>();
            List<int> end_num = new List<int>();

            #region 获取每一个数据文件的起止行
            int total_data_rows = int.Parse(this.textBox5.Text);
            int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

            int start_row = 0;
            int end_row = 0;

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                {
                    start_row = i * file_data_rows;
                    end_row = (i + 1) * file_data_rows - 1;
                }
                else
                {
                    start_row = i * file_data_rows;
                    end_row = total_data_rows - 1;
                }
                start_num.Add(start_row);
                end_num.Add(end_row);
            }
            #endregion

            #region 判断要检索的位置位于第几个文件
            int int_location = int.Parse(location);
            for (int i = 0; i < start_num.Count; i++)
            {
                if (int_location >= start_num[i] && int_location <= end_num[i])
                {
                    target_file = file_combined[i].FullName;
                    break;
                }
            }
            #endregion

            DataTable data_dt = ExcelHelper.OpenCSV2(target_file,1);

            DataRow[] dtRow = data_dt.Select("location='"+location+"' ");//根据查询条件，筛选出所有满足条件的行

            DataTable dt_temp = data_dt.Clone();
            foreach (DataRow item in dtRow)//把满足条件的所有行数据填入数据表
            {
                dt_temp.ImportRow(item);
            }

            DataTable dt_result = new DataTable();

            DataColumn datalocation = new DataColumn();
            datalocation.Caption = "位置";
            datalocation.ColumnName = "位置";

            DataColumn datatime = new DataColumn();
            datatime.Caption = "时间";
            datatime.ColumnName = "时间";

            DataColumn datavalue = new DataColumn();
            datavalue.Caption = "监测值";
            datavalue.ColumnName = "监测值";

            //dt_result.Columns.Add(datalocation);
            dt_result.Columns.Add(datatime);
            dt_result.Columns.Add(datavalue);

            for (int i = 1; i < dt_temp.Columns.Count; i++)
			{
                List<string> list=new List<string>();
                //list.Add(location);
                list.Add(dt_temp.Columns[i].ColumnName);
                list.Add(dt_temp.Rows[0][i].ToString());

                DataRow dr=dt_result.NewRow();
                dr.ItemArray=list.ToArray();
                dt_result.Rows.Add(dr);
			}

            return dt_result;
        }

        /// <summary>
        /// 根据时刻查询数据
        /// </summary>
        /// <param name="direcorypath">数据文件地址</param>
        /// <param name="datatime">要查询的时间</param>
        /// <returns></returns>
        public DataTable SearchDataByDatatime(string direcorypath,string datatime) 
        {
            DirectoryInfo di = new DirectoryInfo(direcorypath);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件

            file_combined = OrderFileByNum(file_combined);
            DataTable dt_result = new DataTable();
            DataColumn data_time = new DataColumn();
            data_time.ColumnName = "时间";
            data_time.Caption = "时间";

            DataColumn data_location = new DataColumn();
            data_location.ColumnName = "位置";
            data_location.Caption = "位置";

            DataColumn data_value = new DataColumn();
            data_value.ColumnName = "监测值";
            data_value.Caption = "监测值";

            //dt_result.Columns.Add(data_time);
            dt_result.Columns.Add(data_location);
            dt_result.Columns.Add(data_value);
            for (int i = 0; i < file_combined.Length; i++)
            {
                DataTable dt_temp = ExcelHelper.OpenCSV2(file_combined[i].FullName,1);

                for (int j = 0; j < dt_temp.Rows.Count; j++)
                {
                    DataRow dr = dt_result.NewRow();
                    List<string> list = new List<string>();
                   // list.Add(datatime);
                    list.Add(dt_temp.Rows[j][0].ToString());
                    list.Add(dt_temp.Rows[j][""+datatime+""].ToString());
                    dr.ItemArray = list.ToArray();
                    dt_result.Rows.Add(dr);
                }
            }

            return dt_result;
        }

        /// <summary>
        /// 根据位置和时刻混合查询数据
        /// </summary>
        /// <param name="direcorypath">数据文件地址</param>
        /// <param name="datadate">要查询的时间</param>
        /// <param name="location">要查询的位置</param>
        /// <returns></returns>
        public DataTable SearchDataByDatatimeAndLocation(string direcorypath, string datadate,string location) 
        {
            string target_file = "";//用于存储找到的目标数据文件
            DirectoryInfo di = new DirectoryInfo(direcorypath);
            FileInfo[] file_combined = di.GetFiles();//获取合并数据文件夹下的数据文件

            file_combined = OrderFileByNum(file_combined);


            //用于存储每一个数据文件的起止行
            List<int> start_num = new List<int>();
            List<int> end_num = new List<int>();

            #region 获取每一个数据文件的起止行
            int total_data_rows = int.Parse(this.textBox5.Text);
            int file_data_rows = (int)(Math.Floor(total_data_rows / 10.0));

            int start_row = 0;
            int end_row = 0;

            for (int i = 0; i < 10; i++)
            {
                if (i < 9)
                {
                    start_row = i * file_data_rows;
                    end_row = (i + 1) * file_data_rows - 1;
                }
                else
                {
                    start_row = i * file_data_rows;
                    end_row = total_data_rows - 1;
                }
                start_num.Add(start_row);
                end_num.Add(end_row);
            }
            #endregion

            #region 判断要检索的位置位于第几个文件
            int int_location = int.Parse(location);
            for (int i = 0; i < start_num.Count; i++)
            {
                if (int_location >= start_num[i] && int_location <= end_num[i])
                {
                    target_file = file_combined[i].FullName;
                    break;
                }
            }
            #endregion

            DataTable data_dt = ExcelHelper.OpenCSV2(target_file, 1);

            DataRow[] dtRow = data_dt.Select("location='" + location + "' ");//根据查询条件，筛选出所有满足条件的行

            DataTable dt_temp = data_dt.Clone();
            foreach (DataRow item in dtRow)//把满足条件的所有行数据填入数据表
            {
                dt_temp.ImportRow(item);
            }

            DataTable dt_result = new DataTable();

            DataColumn datalocation = new DataColumn();
            datalocation.Caption = "位置";
            datalocation.ColumnName = "位置";

            DataColumn datatime = new DataColumn();
            datatime.Caption = "时间";
            datatime.ColumnName = "时间";

            DataColumn datavalue = new DataColumn();
            datavalue.Caption = "监测值";
            datavalue.ColumnName = "监测值";

            dt_result.Columns.Add(datalocation);
            dt_result.Columns.Add(datatime);
            dt_result.Columns.Add(datavalue);

            for (int i = 1; i < dt_temp.Columns.Count; i++)
            {
                if (dt_temp.Columns[i].ColumnName.Equals(datadate))
                {
                    List<string> list = new List<string>();
                    list.Add(location);
                    list.Add(dt_temp.Columns[i].ColumnName);
                    list.Add(dt_temp.Rows[0][i].ToString());

                    DataRow dr = dt_result.NewRow();
                    dr.ItemArray = list.ToArray();
                    dt_result.Rows.Add(dr);
                }
            }

            return dt_result;
        }

        /// <summary>
        /// 初始化MSChart
        /// </summary>
        public void InitChart() 
        {
            this.chart1.Series.Clear();
            Series series = new Series();
            series.Color = Color.Blue;
            series.ChartType = SeriesChartType.Spline;
            series.IsVisibleInLegend = false;
            series.BorderWidth = 2;
            series.MarkerSize = 1;
            series.MarkerStyle = MarkerStyle.Circle;
            series.IsValueShownAsLabel = false;

            //chart1.ChartAreas["ChartArea1"].Position.X = 1;
            //chart1.ChartAreas["ChartArea1"].Position.Y = 15;
            //chart1.ChartAreas["ChartArea1"].Position.Height = 97;
            //chart1.ChartAreas["ChartArea1"].Position.Width = 97;

            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.X = 5;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Y = 3;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Width = 95;
            //chart1.ChartAreas["ChartArea1"].InnerPlotPosition.Height = 99;

            chart1.ChartAreas["ChartArea1"].AxisY.LineColor = Color.Black;
            chart1.ChartAreas["ChartArea1"].AxisY.LineWidth = 1;

            chart1.ChartAreas["ChartArea1"].AxisX.LineColor = Color.Black;
            chart1.ChartAreas["ChartArea1"].AxisX.LineWidth = 1;

            chart1.Visible = true;
            chart1.ChartAreas["ChartArea1"].AxisX.IsMarginVisible = false;
            chart1.ChartAreas["ChartArea1"].AxisX.LabelStyle.IsEndLabelVisible = true;
            chart1.ChartAreas["ChartArea1"].AxisX.Enabled = AxisEnabled.True;
            chart1.ChartAreas["ChartArea1"].AxisX.MajorGrid.Enabled = true;

            chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorGrid.Enabled = true;
            chart1.ChartAreas["ChartArea1"].AxisY.MajorTickMark.Enabled = false;

            this.chart1.ChartAreas[0].AxisX.ScrollBar.IsPositionedInside = false;
            this.chart1.ChartAreas[0].AxisX.ScrollBar.Enabled = true;
            this.chart1.Series.Add(series);
        }
        
        /// <summary>
        /// 根据检索的数据，绘制MSChart
        /// </summary>
        /// <param name="dt"></param>
        public void DrawChart(DataTable dt) 
        {
            this.chart1.Series[0].Points.Clear();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //this.chart1.Series[0].Points.AddXY(dt.Rows[i][1].ToString(), dt.Rows[i][2].ToString());
                this.chart1.Series[0].Points.AddXY(dt.Rows[i][0].ToString(), dt.Rows[i][1].ToString());
            }
        }

        public List<double> GetMaxMinData(List<double> data)
        {
            for (int i = 0; i < data.Count - 1; i++)
            {
                for (int j = 0; j < data.Count - 1 - i; j++)
                {
                    if (data[j] > data[j + 1])
                    {
                        double temp_num = data[j];
                        data[j] = data[j + 1];
                        data[j + 1] = temp_num;
                    }
                }
            }
            return data;
        }
        /// <summary>
        /// 将DataTable指定列进行大小排序
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="column_num"></param>
        /// <returns></returns>
        public List<double> ConvertDataTableToList(DataTable dt, int column_num)
        {
            List<double> result = new List<double>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                result.Add(double.Parse(dt.Rows[i][column_num - 1].ToString()));
            }
            return result;
        }
    }
}
