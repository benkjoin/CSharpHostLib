using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace bn.IniConfigHelper
{
    /// <summary>
    /// IniConfigHelper
    /// </summary>
    public class IniConfigHelper
    {
        private static string filePath = "";

        #region API函数声明
        [DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key,
            string val, string filePath);
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern long GetPrivateProfileString(string section, string key,
            string def, StringBuilder retVal, int size, string filePath);
        [DllImport("kernel32", EntryPoint = "GetPrivateProfileString")]
        private static extern uint GetPrivateProfileStringA(string section, string key,
            string def, Byte[] retVal, int size, string filePath);
        #endregion

        #region 读取Sections
        /// <summary>
        ///  ReadSections
        /// </summary>
        /// <param name="iniFilename">文件路径</param>
        /// <returns>集合</returns>
        [Description("读取所有的Section集合")]
        public static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(null, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }

        #endregion

        #region 读Keys
        /// <summary>
        /// ReadKeys
        /// </summary>
        /// <param name="SectionName">区域名称</param>
        /// <param name="iniFilename">路径</param>
        /// <returns>集合</returns>
        [Description("读取某个Section下的所有的Key的集合")]
        public static List<string> ReadKeys(string SectionName, string iniFilename)
        {
            List<string> result = new List<string>();
            Byte[] buf = new Byte[65536];
            uint len = GetPrivateProfileStringA(SectionName, null, null, buf, buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
            {
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            }
            return result;
        }
        #endregion

        #region 读Ini文件

        /// <summary>
        ///  ReadIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <returns>返回值</returns>
        public static string ReadIniData(string Section, string Key, string DefaultValue)
        {
            return ReadIniData(Section, Key, DefaultValue, filePath);
        }

        /// <summary>
        ///  ReadIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="DefaultValue">默认值</param>
        /// <param name="iniFilePath">路径</param>
        /// <returns>返回值</returns>
        /// 
        [Description("读取某个Section下某个Key对应的Value")]
        public static string ReadIniData(string Section, string Key, string DefaultValue, string iniFilePath)
        {
            if (File.Exists(iniFilePath))
            {
                StringBuilder temp = new StringBuilder(1024);
                GetPrivateProfileString(Section, Key, DefaultValue, temp, 1024, iniFilePath);
                return temp.ToString();
            }
            return string.Empty;
        }

        #endregion

        #region 写Ini文件

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <returns>是否成功</returns>
        public static bool WriteIniData(string Section, string Key, string Value)
        {
            return WriteIniData(Section, Key, Value, filePath);
        }

        /// <summary>
        ///  WriteIniData
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="Value">值</param>
        /// <param name="iniFilePath">路径</param>
        /// <returns>是否成功</returns>
        /// 
        [Description("写入某个Section下某个Key对应的值")]
        public static bool WriteIniData(string Section, string Key, string Value, string iniFilePath)
        {
            long OpStation = WritePrivateProfileString(Section, Key, Value, iniFilePath);
            if (OpStation == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 删除指定的Section。
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>是否成功</returns>
        [Description("删除指定的Section")]
        public static bool DeleteSection(string Section, string iniFilePath)
        {
            return WriteIniData(Section, null, null, iniFilePath);
        }

        /// <summary>
        /// 删除指定Section下的特定Key。
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>是否成功</returns>
        public static bool DeleteKey(string Section, string Key, string iniFilePath)
        {
            return WriteIniData(Section, Key, null, iniFilePath);
        }


        /// <summary>
        /// 检查指定的Section是否存在。
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>是否存在</returns>
        public static bool SectionExists(string Section, string iniFilePath)
        {
            List<string> sections = ReadSections(iniFilePath);
            return sections.Contains(Section);
        }

        /// <summary>
        /// 检查指定Section下的特定Key是否存在。
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="Key">键</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>是否存在</returns>
        public static bool KeyExists(string Section, string Key, string iniFilePath)
        {
            List<string> keys = ReadKeys(Section, iniFilePath);
            return keys.Contains(Key);
        }

        /// <summary>
        /// 读取某个Section下的所有键值对。
        /// </summary>
        /// <param name="Section">区域</param>
        /// <param name="iniFilePath">文件路径</param>
        /// <returns>键值对的字典</returns>
        public static Dictionary<string, string> ReadKeyValues(string Section, string iniFilePath)
        {
            var result = new Dictionary<string, string>();
            List<string> keys = ReadKeys(Section, iniFilePath);
            foreach (var key in keys)
            {
                string value = ReadIniData(Section, key, "", iniFilePath);
                result[key] = value;
            }
            return result;
        }

        #endregion

    }
   
}
