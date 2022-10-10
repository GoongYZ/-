using Comm;
using System;

namespace BLL
{
    public class ServerBase
    {
        /// <summary>
        /// 读取配置文件信息
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static string XMLRead(string Mode, string key)
        {
            string Current = System.IO.Directory.GetCurrentDirectory();//获取当前根目录
            IniHelper ini = new IniHelper(Current + "\\Config.ini");
            return ini.ReadValue(Mode, key);
        }

        /// <summary>
        /// 写入配置文件信息
        /// </summary>
        /// <param name="Mode"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public static void XMLWrite(string Mode, string key,string value)
        {
            string Current = System.IO.Directory.GetCurrentDirectory();//获取当前根目录
            IniHelper ini = new IniHelper(Current + "\\Config.ini");
            ini.Write(Mode, key, value);
        }
    }
}
