using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;
using RFIDReaderNetwork_SerialSDK_ForCSharp.ExternalInterfaceLayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace BLL
{
    /// <summary>
    /// 物必连读写器 读取钥匙卡
    /// </summary>
    public class UHF2Service
    {
        private static RFIDServer m_rifdServer = null;
        private static RFIDClient m_rfidClientReader = null;//rfid读卡器
        private static int _port = Convert.ToInt32(ServerBase.XMLRead("UHF2", "ListenPort"));
        private static string ComPortName = ServerBase.XMLRead("UHF2", "ComPortName");
        private static string BaudRate = ServerBase.XMLRead("UHF2", "BaudRate");
        
        private static string m_strWorkReaderDeviceID = string.Empty;              //工作读写器的设备序列号
        private static Reader reader = null;
        private static string m_strLostConnectDeviceID = string.Empty;
        private static Reader m_selectedWorkReader = null;
        private delegate void InventoryResultListViewUpdate(INVENTORY_REPORT_RESULT inventoryListViewItem);
        public static Hashtable Listm_strEPC = new Hashtable();//单次盘点集合
        public static string  strEPC = "";//单次盘点集合
        public static bool IsOneCheckInv = false;
        /// <summary>
        /// COM口连接
        /// </summary>
        public static void ConnectCOM()
        {
            OperationResult nRetVal = OperationResult.FAIL;
            string strReaderIPOrCom = ComPortName;
            //先判断该是否已经连接了该读写器   
            Reader reader = ReaderManager.g_ReaderManager.GetRFIDReaderByReaderCom(strReaderIPOrCom);
            if (reader != null)
                Logo.sWriteLogo("读写器" + reader.m_rfidWorkReader.m_strDeviceID + ",序列号为" + reader.m_rfidWorkReader.m_strDeviceID + "已连接", 11);
            else
            {
                m_rfidClientReader = new RFIDClient();//服务端读写器
                nRetVal = m_rfidClientReader.ConnectSerial(strReaderIPOrCom, Convert.ToInt32(BaudRate));
                if (nRetVal == OperationResult.SUCCESS)
                {
                    m_rfidClientReader.m_OnDisconnect += new EventHandler<DisconnectEventArgs>(m_rfidClientReader_m_OnDisconnect);
                    m_rfidClientReader.m_OnErrorcallback += new EventHandler<ErrorReportEventArgs>(m_rifdServer_m_OnErrorOccured);
                    m_rfidClientReader.m_OnInventoryReport += new EventHandler<InventoryReportEventArgs>(m_rfidClientReader_m_OnInventoryReport);
                    reader = new Reader(m_rfidClientReader);
                    ReaderManager.g_ReaderManager.AddConnectDevice(m_rfidClientReader.m_strDeviceID, reader);
                    m_strWorkReaderDeviceID = m_rfidClientReader.m_strDeviceID;
                    //AddConnectReaderInListBox(m_rfidClientReader.m_strDeviceID, m_rfidClientReader.m_strReaderIP);
                    IniFile.g_IniFile.m_nPCWorkType = ReaderWorkType.COM;
                    IniFile.g_IniFile.m_strDefaultComPort = strReaderIPOrCom;
                    IniFile.g_IniFile.m_nComPortBaudRate = int.Parse(BaudRate);
                    IniFile.g_IniFile.SaveProfileInfo(1);
                    ReaderManager.g_ReaderManager.AddConnectDevice(m_rfidClientReader.m_strDeviceID, reader);
                    m_selectedWorkReader = reader;
                    m_selectedWorkReader.m_cOperateReader.m_nReaderWorkType = ReaderWorkType.COM;
                    m_selectedWorkReader.m_cOperateReader.m_strReaderIPOrCom = strReaderIPOrCom;

                    m_selectedWorkReader = reader;                  //把连接上的读写器设置当前工作读写器 

                    Logo.sWriteLogo("读写器" + m_rfidClientReader.m_strDeviceID + ",序列号为" + m_rfidClientReader.m_strDeviceID + "已连接", 11);
                }
                else
                {
                    Logo.sWriteLogo("读写器连接失败：原因" + nRetVal, 11);
                }
            }
        }
       /* public static void ConnectNetWork()
        {
            OperationResult nRetVal = OperationResult.FAIL;

            if (m_rifdServer == null)
            {
                m_rifdServer = new RFIDServer(_port);
                nRetVal = m_rifdServer.StartServer();                      //开启服务
                if (nRetVal == OperationResult.SUCCESS)
                {
                    m_rifdServer.m_OnErrorOccured += new EventHandler<ErrorReportEventArgs>(m_rifdServer_m_OnErrorOccured);
                    m_rifdServer.m_OnRegistered += new EventHandler<RegisteredEventArgs>(m_rifdServer_m_OnRegistered);
                    m_rifdServer.m_OnUnregistered += new EventHandler<UnregisteredEventArgs>(m_rifdServer_m_OnUnregistered);
                    m_rifdServer.m_OnInventoryReport += new EventHandler<InventoryReportEventArgs>(m_rfidClientReader_m_OnInventoryReport);
                }
            }
        }*/

        /// <summary>
        /// 根据天线获取标签数据
        /// </summary>
        public static void OneCheckInvnetoryWhile(int nAntnnaNumber)
        {

            if (!CheckReaderOnLine())
                return;
            OperationResult nRetVal = OperationResult.FAIL;
            TagReport tagReport = new TagReport();
            DateTime dt1 = DateTime.Now;
            while ((DateTime.Now - dt1).TotalMilliseconds < 3000)
            {
                if (!CheckReaderOnLine())
                    break;
                nRetVal = m_selectedWorkReader.m_rfidWorkReader.Inventory(nAntnnaNumber, ref tagReport);//单次盘点 
                if (nRetVal == OperationResult.SUCCESS)
                {
                    Tag tag = null;
                    for (int i = 0; i < tagReport.m_listTags.Count; ++i)
                    {
                        tag = tagReport.m_listTags[i];
                        if (!Listm_strEPC.Contains(tag.m_strEPC))
                            Listm_strEPC.Add(tag.m_strEPC, tag.m_strEPC);                        
                    }
                }
                Thread.Sleep(10);
            }
            m_selectedWorkReader.m_bIsInventory = true;
        }

        public static void OneCheckInvnetoryWhile2(int nAntnnaNumber)
        {

            if (!CheckReaderOnLine())
                return;
            OperationResult nRetVal = OperationResult.FAIL;
            TagReport tagReport = new TagReport();
            DateTime dt1 = DateTime.Now;
            while ((DateTime.Now - dt1).TotalMilliseconds < 3000)
            {
                if (!CheckReaderOnLine())
                    break;
                nRetVal = m_selectedWorkReader.m_rfidWorkReader.Inventory(nAntnnaNumber, ref tagReport);//单次盘点 
                if (nRetVal == OperationResult.SUCCESS)
                {
                    Tag tag = null;
                    for (int i = 0; i < tagReport.m_listTags.Count; ++i)
                    {
                        tag = tagReport.m_listTags[i];
                        if (!Listm_strEPC.Contains(tag.m_strEPC))
                            Listm_strEPC.Add(tag.m_strEPC, tag.m_strEPC);
                    }
                }
                Thread.Sleep(10);
            }
            m_selectedWorkReader.m_bIsInventory = true;
        }
        public static void StartPerioInventory()
        {
            OperationResult nRetVal = OperationResult.FAIL;
            nRetVal = m_selectedWorkReader.m_rfidWorkReader.StartPerioInventory();//开始连续盘点
            m_selectedWorkReader.m_bIsInventory = true;
        }
        public static void StopPerioInventory()
        {
            if (!CheckReaderOnLine())
                return;
            if (IniFile.g_IniFile.m_nHubEnable == 0)
            {
                OperationResult nRetVal = m_selectedWorkReader.m_rfidWorkReader.StopPeriodInventory();
            }
        }
        public static bool CheckReaderOnLine()
        {
            if (null == m_selectedWorkReader)
            {
                ConnectCOM();
                return true;
            }
            else
                return true;
        }
        /// <summary>
        /// 作为客户端的读写器发生错误事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void m_rifdServer_m_OnErrorOccured(object sender, ErrorReportEventArgs e)
        {
            if (e != null)
            {
                //这里有特殊情况,有相同设备序列号的读写器连到本地服务器m_szErrorCode为0x18,SAME_DEVICE_ID=24
                if (e.m_strErrorCode == "18")
                {
                    Logo.sWriteLogo("有相同设备序列号 + e.m_strDeviceID + 的读写器 + e.m_strMacError + 连接到本地服务器", 11);
                }
                else
                {
                    Logo.sWriteLogo("读写器 + e.m_strDeviceID + 发生错误,ErrorCode: + e.m_strErrorCode + ,MacCode: + e.m_strMacError", 11);
                }
            }
        }
        /// <summary>
        /// 作为客户端的读写器请求连接事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void m_rifdServer_m_OnRegistered(object sender, RegisteredEventArgs e)
        {
            if (e != null)
            {

                //在读写器管理类中查找是否存在该读写器
                if (null == ReaderManager.g_ReaderManager.GetRFIDReaderByReaderIP(e.m_strReaderIp))
                {
                    if (null == ReaderManager.g_ReaderManager.GetRFIDReaderByDeviceID(e.m_strDeviceID))
                    {
                        OperationResult nRetVal = OperationResult.FAIL;
                        RFIDServerModel rfidReader = new RFIDServerModel(e.m_strDeviceID);
                        m_strWorkReaderDeviceID = e.m_strDeviceID;
                        //把刚连接上来的读写器作为当前选中的读写器
                        reader = new Reader(rfidReader);
                        IniFile.g_IniFile.m_nPCWorkType = ReaderWorkType.TCP_CLIENT;
                        IniFile.g_IniFile.m_nListenPort = _port;
                        IniFile.g_IniFile.SaveProfileInfo(0);
                        ReaderManager.g_ReaderManager.AddConnectDevice(e.m_strDeviceID, reader);
                        reader.m_cOperateReader.m_nReaderWorkType = ReaderWorkType.TCP_CLIENT;
                        reader.m_cOperateReader.m_strReaderIPOrCom = e.m_strReaderIp;


                        //同步时间
                        DateTime day = DateTime.Now;
                        int month = day.Month;
                        int d = day.Day;
                        int h = day.Hour;
                        int mm = day.Minute;
                        int y = day.Year;
                        int s = day.Second;
                        string time;
                        time = String.Format("{0:D2}{1:D2}{2:D2}{3:D2}{4:D4}.{5:D2}", month, d, h, mm, y, s);
                        // nRetVal = reader.m_rfidWorkReader.SetCurrentTime(time);

                        m_selectedWorkReader = reader;


                        //if (nRetVal == OperationResult.SUCCESS)
                            //log.WriteLogo("同步时间成功...", 11);
                        //else
                            //log.WriteLogo(string.Format("同步时间失败：原因:{0}...", nRetVal.ToString()), 11);

                    }
                    else
                    {
                        //log.WriteLogo("有相同设备序列号" + e.m_strDeviceID + "的读写器" + e.m_strReaderIp + "连接到本地服务器", 11);
                    }
                }
                else
                {
                    //log.WriteLogo("读写器" + e.m_strReaderIp + "已经连接到本地服务器", 11);
                }
            }
        }
        /// <summary>
        /// 作为客户端的读写器断开连接事件处理函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void m_rifdServer_m_OnUnregistered(object sender, UnregisteredEventArgs e)
        {
            if (e != null)
            {
                Reader operateReader = ReaderManager.g_ReaderManager.GetRFIDReaderByDeviceID(e.m_strDeviceID);
                if (null != operateReader)
                {
                    m_strLostConnectDeviceID = e.m_strDeviceID;
                    Logo.sWriteLogo("读写器" + e.m_strDeviceID + "从本地服务器中断开", 11);
                    ReaderManager.g_ReaderManager.DisconnectDevice(e.m_strDeviceID);
                    ReaderManager.g_ReaderManager.RemoveConnectDevice(e.m_strDeviceID);
                }
            }
        }

        #region[连续盘点事件]
        private static void m_rfidClientReader_m_OnInventoryReport(object sender, InventoryReportEventArgs e)
        {
            if (e != null)
            {
                INVENTORY_REPORT_RESULT report = e.m_stInventoryResult;
                if (!string.IsNullOrEmpty(report.m_strEPC))
                {
                    Listm_strEPC.Add(report.m_strEPC, report.m_strEPC);
                }
                Logo.sWriteLogo(string.Format("读写器:{0},EPC:{1},TID:{2},天线号:{3},RSSI:{4},频点:{5},盘点时刻:{6}", report.m_strDeviceID, report.m_strEPC, report.m_strTID, report.m_strAntennaNo, report.m_strRSSI, report.m_strFrequency, report.m_strTimeStamp), 11);

                //统计不同读写器盘点数据
                //StatisticsInventoryData(report);
            }
        }
        private static void StatisticsInventoryData(INVENTORY_REPORT_RESULT stInventoryReportResult)
        {
            Reader pInventoryDataOwnerReader = ReaderManager.g_ReaderManager.GetRFIDReaderByDeviceID(stInventoryReportResult.m_strDeviceID);
            if (null == pInventoryDataOwnerReader)
            {
                return;
            }
            //保存到各自读写器对象中
            pInventoryDataOwnerReader.StatisticsInventoryData(stInventoryReportResult);
        }
        #endregion


        private static void m_rfidClientReader_m_OnDisconnect(object sender, DisconnectEventArgs e)
        {
            if (e != null)
            {
                Logo.sWriteLogo(string.Format("读写器:{0}断开连接", e.m_strDeviceID), 11);
                Reader operateReader = ReaderManager.g_ReaderManager.GetRFIDReaderByDeviceID(e.m_strDeviceID);
                if (null != operateReader)
                {
                    m_strLostConnectDeviceID = e.m_strDeviceID;
                    ReaderManager.g_ReaderManager.RemoveConnectDevice(e.m_strDeviceID);
                }
            }
        }
    }
}
