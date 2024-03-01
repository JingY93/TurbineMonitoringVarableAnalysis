using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;

namespace TurbineMonitoringVarableAnalysis.CommonCode
{
    public class ConfReadAndModify
    {
        public ConfReadAndModify() { }

        #region 读取配置文件 getConfigSetting(string confNode)
        /// <summary>
        /// 读取配置文件信息
        /// </summary>
        /// <param name="confNode">结点名称(key值)</param>
        /// <returns></returns>
        public string getConfigSetting(string confNode)
        {
            string configString = "";

            XmlDocument xml = new XmlDocument();

            string path = Application.StartupPath.ToString();
            DirectoryInfo di = new DirectoryInfo(path).Parent.Parent;

            xml.Load(di.FullName + "\\Files\\Conf\\config.xml");

            XmlNode xn = xml.SelectSingleNode("configSettings");
            XmlNodeList xnl = xn.ChildNodes;

            for (int i = 0; i < xnl.Count; i++)
            {
                XmlElement xe = (XmlElement)xnl[i];
                if (xe.GetAttribute("key").ToString().Equals(confNode))
                {
                    configString = xe.GetAttribute("value").ToString();
                    break;
                }
            }
            return configString;
        }
        #endregion

        #region 暂存系统配置信息 modifyConfigSetting(string confNode,string value)
        /// <summary>
        /// 暂存系统配置信息
        /// </summary>
        /// <param name="confNode">要更改的结点</param>
        /// <param name="value">更改后的值</param>
        /// <returns></returns>
        public bool modifyConfigSetting(string confNode, string value)
        {
            Control.CheckForIllegalCrossThreadCalls = false;
            XmlDocument xml = new XmlDocument();
            string path = Application.StartupPath.ToString();
            DirectoryInfo di = new DirectoryInfo(path).Parent.Parent;

            xml.Load(di.FullName + "\\Files\\Conf\\config.xml");
            bool success = false;
            XmlNode xn = xml.SelectSingleNode("configSettings");
            XmlNodeList xnl = xn.ChildNodes;

            try
            {
                for (int i = 0; i < xnl.Count; i++)
                {
                    XmlElement xe = (XmlElement)xnl[i];
                    if (xe.GetAttribute("key").ToString().Equals(confNode))
                    {
                        xe.SetAttribute("value", value);

                        string path2 = Application.StartupPath.ToString();
                        DirectoryInfo di2 = new DirectoryInfo(path2).Parent.Parent;

                        xml.Save(di2.FullName + "\\Files\\Conf\\config.xml");
                        success = true;
                    }
                }
            }
            catch (Exception)
            {
            }
            return success;

        #endregion
        }
    }
}
