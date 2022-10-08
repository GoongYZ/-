using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BLL
{
    public class Logo
    {
        /// <summary>
        /// 创建日志文件
        /// </summary>
        /// <returns></returns>
        private string SetLogoFile(int Type)
        {
            string Current = System.IO.Directory.GetCurrentDirectory();
            string Path = Current + "/Log";
            try
            {
                if (!Directory.Exists(Path))
                {
                    Directory.CreateDirectory(Path);
                }
            }
            catch (Exception ex)
            {

            }
            string FilePath = Path + "/" + string.Format("{0:yyyyMMdd}{1}.txt", DateTime.Now, getFileLx(Type));
            return FilePath;
        }
        private static string getFileLx(int type)
        {
            string rtn = "";
            switch (type)
            {
                case 0:
                    rtn = "_用户日志";
                    break;
                case 1:
                    rtn = "_错误日志";
                    break;
                case 2:
                    rtn = "_读卡日志";
                    break;
                case 3:
                    rtn = "_人脸识别";
                    break;
                case 4:
                    rtn = "_rfid";
                    break;
                case 5:
                    rtn = "_指纹日志";
                    break;
            }
            return rtn;
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text">日志内容</param>
        /// <param name="Type">0：数据信息/1:开柜门/2:人脸识别/3：二维码/4：RFID通道/5：门禁/6：工控机调用/7：视频监控/8：桌面读写器/9错误日志/10：语音播报/11:读写器</param>
        public void WriteLogo(string text, int Type)
        {
            try
            {
                text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + ":" + text;
                string FilePath = SetLogoFile(Type);
                if (File.Exists(FilePath))
                {
                    StreamWriter sw = new StreamWriter(FilePath, true);
                    sw.WriteLine(text);
                    sw.Close();
                }
                else
                {
                    FileStream myFs = new FileStream(FilePath, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.WriteLine(text);
                    mySw.Close();
                    myFs.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 写入日志
        /// </summary>
        /// <param name="text">日志内容</param>
        /// <param name="Type">0：数据信息/1:开柜门/2:人脸识别/3：二维码/4：RFID通道/5：门禁/6：工控机调用/7：视频监控/8：桌面读写器/9错误日志/10：语音播报/11:读写器</param>
        public static void sWriteLogo(string text, int Type)
        {
            try
            {
                text = string.Format("{0:yyyy-MM-dd HH:mm:ss}", DateTime.Now) + text;
                string FilePath = sSetLogoFile(Type);
                if (File.Exists(FilePath))
                {
                    StreamWriter sw = new StreamWriter(FilePath, true);
                    sw.WriteLine(text);
                    sw.Close();
                }
                else
                {
                    FileStream myFs = new FileStream(FilePath, FileMode.Create);
                    StreamWriter mySw = new StreamWriter(myFs);
                    mySw.WriteLine(text);
                    mySw.Close();
                    myFs.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }
        /// <summary>
        /// 创建日志文件
        /// </summary>
        /// <returns></returns>
        private static string sSetLogoFile(int Type)
        {
            string Current = System.IO.Directory.GetCurrentDirectory();
            string Path = Current + "/Log";
            if (!Directory.Exists(Path))
            {
                Directory.CreateDirectory(Path);
            }
            string FilePath = Path + "/" + string.Format("{0:yyyyMMdd}{1}.txt", DateTime.Now, getFileLx(Type));
            return FilePath;
        }
        /// <summary>
        /// 读取日志文件信息
        /// <param name="Type">0/1/2/3：所有RFID记录/非法RFID记录/读写器链接日志/温湿度记录</param>
        /// </summary>
        /// <param name="Type"></param>
        public string GetLogo(int Type)
        {
            string FilePath = SetLogoFile(Type);
            string returnStr = string.Empty;
            if (File.Exists(FilePath))
            {
                FileStream fs = new FileStream(FilePath, FileMode.Open, FileAccess.Read);
                StreamReader sr = new StreamReader(fs, System.Text.Encoding.GetEncoding("gb2312"));
                sr.BaseStream.Seek(0, SeekOrigin.Begin);
                string str = sr.ReadLine();
                returnStr += str + "$";
                while (str != null)
                {
                    str = sr.ReadLine();
                    returnStr += str + "$";
                }
                sr.Close();
                fs.Close();
            }
            return returnStr;
        }
    }
}
