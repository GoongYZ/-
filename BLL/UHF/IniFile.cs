using System.Text;
using System.IO;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;
using System.Reflection;

namespace BLL
{
    public class IniFile
    {
        /// <summary>
        /// 写入指定字段值
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="val">写入值</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        /// <summary>
        /// 读取指定字段值
        /// </summary>
        /// <param name="section">配置节</param>
        /// <param name="key">键名</param>
        /// <param name="def">默认值</param>
        /// <param name="retVal">返回值</param>
        /// <param name="size">读取长度</param>
        /// <param name="filePath">文件路径</param>
        /// <returns></returns>
        [System.Runtime.InteropServices.DllImport("kernel32")]
        private static extern int GetPrivateProfileString(string section, string key, string def, System.Text.StringBuilder retVal, int size, string filePath);

        private const string PROFILE_FILE_NAME = ".//Config.ini";
        private const string MODEL_INFO = "UHF";
        /// <summary>
        /// 配置文件的路径
        /// </summary>
        public string m_strFileNameAndPath = string.Empty;		
        /// <summary>
        /// 通信方式
        /// </summary>
        public ReaderWorkType m_nPCWorkType;			 	
        /// <summary>
        /// 监听端口
        /// </summary>
        public int m_nListenPort;						
        /// <summary>
        /// 和读写器串口通信默认的串口号
        /// </summary>
        public string m_strDefaultComPort = string.Empty;		 
        /// <summary>
        /// 串口通信波特率
        /// </summary>
        public int m_nComPortBaudRate;
        /// <summary>
        ///模块端口数
        /// </summary>
        public int m_nModulePortCount { set; get; }
        /// <summary>
        /// 分支器模式测试
        /// </summary>
        public int m_nHubEnable;

        public IniFile()                                       
        {                                                      
            m_nPCWorkType = ReaderWorkType.TCP_CLIENT;	          
            m_nListenPort = 7880;						       
            m_strDefaultComPort = "COM3";	                   
            m_nComPortBaudRate = 115200;
            m_nModulePortCount = 4;
        }                                                      
        /// <summary>                                          
        /// 从ini文件中得到指定字段的值【读写器服务类型】
        /// </summary>
        /// <param name="section">模块名</param>
        /// <param name="key">键名</param>
        /// <param name="nDefauleValue">默认值</param>
        /// <param name="strFilePath">文件名</param>
        /// <returns></returns>
        public int GetPrivateProfileInt(string section, string key, int nDefauleValue, string strFilePath)
        { 
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            int nValue = 0;
            try
            { 
                GetPrivateProfileString(section, key, "", temp, 255, strFilePath);
                string strValue = temp.ToString();
                nValue = int.Parse(strValue);
            }
            catch (System.Exception ex)
            {
                nValue = nDefauleValue;
            }
            return nValue;
        } 
        /// <summary>
        /// 从ini文件中得到指定字段的值
        /// </summary>
        /// <param name="section">模块名</param>
        /// <param name="key">键名</param>
        /// <param name="nDefauleValue">默认值</param>
        /// <param name="strFilePath">文件名</param>
        /// <returns></returns>
        public string GetPrivateProfileString(string section, string key, string strDefauleValue, string strFilePath)
        { 
            System.Text.StringBuilder temp = new System.Text.StringBuilder(255);
            string strValue = string.Empty;
            try
            { 
                GetPrivateProfileString(section, key, "", temp, 255, strFilePath);
                strValue = temp.ToString();
            }
            catch (System.Exception ex)
            {
                strValue = strDefauleValue;
            }
            return strValue;
        }
        /// <summary>
        /// 读取配置文件
        /// </summary>
        /// <param name="strErrorMessage">错误</param>
        /// <returns></returns>
        public bool LoadIniFile(ref string strErrorMessage)
        {
            string strModlePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetName().CodeBase);
            strModlePath = strModlePath.Replace("file:\\", "");//得到模块路径	        
            StringBuilder sb = new StringBuilder();
            sb.Append(strModlePath);
            sb.Append(PROFILE_FILE_NAME);
            m_strFileNameAndPath = sb.ToString();

            ////判断工作目录下是否有配置文件
            if (!File.Exists(m_strFileNameAndPath))//无效的句柄,即文件不存在
            {
                //MessageBox.Show("配置文件不存在。");
                return false;
            } 
            else
            { 
                System.IO.FileStream fileStream = null;
                bool bIsUsing = false;
                try
                {
                    fileStream = System.IO.File.Open(m_strFileNameAndPath, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite, System.IO.FileShare.None);
                    bIsUsing = false;
                }
                catch (System.IO.IOException ioEx)
                {
                    bIsUsing = true;
                }
                catch (System.Exception ex)
                {
                    bIsUsing = true;
                }
                finally
                {
                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
                //文件被占用
                if (bIsUsing)
                {
                   // MessageBox.Show("配置文件被占用。");
                    return false;
                }
            } 
            //工作方式
            int nWorkType = GetPrivateProfileInt(MODEL_INFO, "WorkType", 1, m_strFileNameAndPath);
            if (nWorkType >= 0 && nWorkType <= 2)
            {
                m_nPCWorkType = (ReaderWorkType)nWorkType;
            }
            else
            {
                strErrorMessage = string.Format("配置文件info.ini中工作方式WorkType为非法值{0}", nWorkType);
                return false;
            } 
            //PC和读写器串口通信的串口号
            string strComPort = GetPrivateProfileString(MODEL_INFO, "ComPortName", "COM1", m_strFileNameAndPath);
            if (strComPort.Length > 7)
            {
                strErrorMessage = string.Format("配置文件info.ini中串口号ComPortName为非法值{0}", strComPort);
                return false;
            }
            else
            {
                m_strDefaultComPort = strComPort;
            }
            //串口通信波特率
            int nBaudRate = GetPrivateProfileInt(MODEL_INFO, "BaudRate", 115200, m_strFileNameAndPath);
            if (nBaudRate <= 0)
            {
                strErrorMessage = string.Format("配置文件info.ini中串口波特率BaudRate为非法值{0}", nBaudRate);
                return false;
            }
            else
            {
                m_nComPortBaudRate = nBaudRate;
            }

            //分支器模式配置
            m_nHubEnable = GetPrivateProfileInt(MODEL_INFO, "HubEnable", 0, m_strFileNameAndPath);
           
            //监听端口
            int nListenPort = GetPrivateProfileInt(MODEL_INFO, "ListenPort", 7880, m_strFileNameAndPath);
            if (nListenPort <= 0)
            {
                strErrorMessage = string.Format("配置文件info.ini中监听端口ListenPort为非法值{0}", nListenPort);
                return false;
            }
            else
            {
                m_nComPortBaudRate = nBaudRate;
            }

            //监听端口
            m_nModulePortCount = GetPrivateProfileInt(MODEL_INFO, "ModbulePortCount", 4, m_strFileNameAndPath);
            if (m_nModulePortCount < 0)
            {
                strErrorMessage = string.Format("配置文件info.ini中监听端口ListenPort为非法值{0}", m_nModulePortCount);
                m_nModulePortCount = 4;
           //     return false;
            }
          

            return true;
        }
        /// <summary>
        /// 保存配置文件信息
        /// </summary>
        /// <param name="nSaveItemIndex">0:网络监听端口  1:和读写器通信的COM号  </param>
        public void SaveProfileInfo(int nSaveItemIndex)
        {
            string strWorkType = string.Format("{0}", (int)m_nPCWorkType);
            WritePrivateProfileString(MODEL_INFO, "WorkType", strWorkType, m_strFileNameAndPath);
            switch (nSaveItemIndex)
            {
                case 0:
                    //网络监听端口 
                    string strReaderServerPort = string.Format("{0}", m_nListenPort);
                    WritePrivateProfileString(MODEL_INFO, "ListenPort", strReaderServerPort, m_strFileNameAndPath);
                    break;
                case 1:
                    //和读写器通信的COM号
                    WritePrivateProfileString(MODEL_INFO, "ComPortName", m_strDefaultComPort, m_strFileNameAndPath);
                    string strComPortBaudRate = string.Format("{0}", m_nComPortBaudRate);
                    WritePrivateProfileString(MODEL_INFO, "BaudRate", strComPortBaudRate, m_strFileNameAndPath);
                    break;
                default:
                    break;
            }
        }


        //全局的配置文件帮助类
        public static IniFile g_IniFile = new IniFile();
    }
}
