using System;
using System.Collections.Generic;
using RFIDReaderNetwork_SerialSDK_ForCSharp.ExternalInterfaceLayer;
using System.Threading;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //读写器应用层类
    public class Reader
    {
        public OperatReader m_rfidWorkReader = null;                //工作读写器
        /// <summary>
        /// 是否正在盘点
        /// </summary>
        public bool m_bIsInventory=false;							//
        public bool m_bIsClickReadCycle = false;					//是否点击连续读
        public DateTime m_startOperateTime = DateTime.Now;          //操作开始时间
        public DateTime m_endOperateTime = DateTime.Now;            //操作结束时间
        public bool m_bRecycleThreadRunning = false;               //循环读线程运行标记位
        public bool m_bIsClickStopRecycleRead=false;		        //是否点击停止连续读按钮

        private AccessParameter m_recycleReadParams = null;         //循环读参数
        public int m_nCurTabIndex = 0;							//TAB页索引
        /// <summary>
        /// 盘点标签总数量
        /// </summary>
        public int	m_nInventoryTagTotalCnt=0;						 
        /// <summary>
        /// 盘点次数
        /// </summary>
	    public int m_nInventoryTotalCnt=0;							 
        /// <summary>
        /// 盘点速度
        /// </summary>
	    public float m_fInventorySpeed=0.0f;						 

        //盘点数据统计信息MAP
        public Dictionary<string, InventoryDataInfo> m_dictionaryInventoryStatisticsData = new Dictionary<string, InventoryDataInfo>();
        public object m_lockDictionaryInventoryStatisticsData = new object();//访问字典的锁
        //EPC信息字典
        public Dictionary<string, EPCDataInfo> m_dictionaryEPCData = new Dictionary<string, EPCDataInfo>();
        public object m_lockDictionaryEPCData = new object();//访问字典的锁
        //连续读数据统计信息字典
        public Dictionary<string, FastReadDataInfo> m_dictionaryRecycleReadStatisticsData = new Dictionary<string, FastReadDataInfo>();
        public object m_lockDictionaryRecycleReadStatisticsData = new object();//访问字典的锁 
        public string m_strSelectedEPC = string.Empty;              //选中的EPC 
        //循环读标签结果事件
        public event EventHandler<RecycleReadTagEventArgs> m_OnRecycleReadTagReport = null; 
        //停止连续读操作的结果事件
        public event EventHandler<OperateResultEventArgs> m_OnStopRecycleReadTagResult = null;

        public ReaderOperate m_cOperateReader;					//读写器操作类

        private DateTime m_timeLastReadTagTime = DateTime.Now;

        public Reader(OperatReader rfidWorkReader)
        {
            m_rfidWorkReader = rfidWorkReader;
            m_recycleReadParams = new AccessParameter();
            m_cOperateReader = new ReaderOperate();           
        }

        /// <summary>
        /// 计算盘点速度
        /// </summary>
        /// <param name="nNum">标签个数</param>
        /// <returns></returns>
        public float CalculateInventorySpeed(int nNum)
        {
            m_endOperateTime = DateTime.Now;
            var aaa = m_endOperateTime - m_startOperateTime; 
            //long span = m_endOperateTime- m_startOperateTime <= 0 ? 1 : m_endOperateTime.Ticks - m_startOperateTime.Ticks;
            return (float)nNum / (float)aaa.TotalSeconds;
        }

        /// <summary>
        /// 设置快读标志位
        /// </summary>
        /// <param name="bFlg">bool bFlg--------------标志位</param>
        public void SetFastReadFlg(bool bFlg)
        {
            //if (mainWindow==null)
            //{
            //    mainWindow = (Form1)Application.Current.MainWindow;
            //}
            m_bIsClickReadCycle = bFlg;            
            //开始快读操作,创建线程
            if (bFlg)
            {
                m_bRecycleThreadRunning = true;
                //创建连续读的线程
                Thread recycleReadThread = new Thread(new ThreadStart(ReadCycleThread)) { IsBackground = true };
                recycleReadThread.Start();
               // LogFile.g_logfile.WriteWarningLog("创建连续读的线程");                
            }
            else
            {
                m_bRecycleThreadRunning = false;
            }
        }

        /// <summary>
        /// 设置连续读标签的参数
        /// </summary>
        /// <param name="nMemoryBank">读取区域</param>
        /// <param name="nReadOffset">偏移</param>
        /// <param name="nReadLength">长度</param>
        public void SetReadTagCycleParams(int nMemoryBank, int nReadOffset, int nReadLength)
        {
            if (m_recycleReadParams!=null)
            {
                m_recycleReadParams.m_strTargetTag = "";//不指定标签
                m_recycleReadParams.m_nAntennaNo = 0;   //循环读不需要指定天线号
                m_recycleReadParams.m_bEncryptOrDecrypt = false;
                m_recycleReadParams.m_bSignatureOrCheck = false;
                m_recycleReadParams.m_enumBankType = (MemoryBank)nMemoryBank;
                m_recycleReadParams.m_nOffset = nReadOffset;
                m_recycleReadParams.m_nLength = nReadLength;
                m_recycleReadParams.m_strAccessPassword = "00000000";                
            }
        }

        /// <summary>
        /// 循环读线程的执行函数
        /// </summary>
        private void ReadCycleThread()
        {
            while (m_bRecycleThreadRunning)
            {
                Thread.Sleep(10);
                //ReadTagCycle();
            }
            //如果点击了停止连续读,则发送停止盘点操作
            if (m_bIsClickStopRecycleRead && m_rfidWorkReader!=null)
            {
                OperationResult nRetVal=m_rfidWorkReader.StopPeriodInventory();
                //返回停止连续读的结果
                if (m_OnStopRecycleReadTagResult!=null)
                {
                    OperateResultEventArgs args=new OperateResultEventArgs();
                    args.nRetVal = nRetVal;
                    m_OnStopRecycleReadTagResult(this, args);
                }
            }
        }
         
        /// <summary>
        /// 统计盘点数据
        /// </summary>
        /// <param name="stInventoryReportResult">盘点数据包</param>
        public void StatisticsInventoryData(INVENTORY_REPORT_RESULT stInventoryReportResult)
        {
            lock (m_lockDictionaryInventoryStatisticsData)
            {
                InventoryDataInfo inventoryDataInfo=null;
                if (!m_dictionaryInventoryStatisticsData.ContainsKey(stInventoryReportResult.m_strEPC))
                {
                    inventoryDataInfo=new InventoryDataInfo(stInventoryReportResult);
                    m_dictionaryInventoryStatisticsData.Add(stInventoryReportResult.m_strEPC, inventoryDataInfo);                     
                }
                else
                {
                    inventoryDataInfo = m_dictionaryInventoryStatisticsData[stInventoryReportResult.m_strEPC];
                }
                inventoryDataInfo.m_strFrequency = stInventoryReportResult.m_strFrequency;
                inventoryDataInfo.m_strRSSI = stInventoryReportResult.m_strRSSI;
                inventoryDataInfo.m_strTimeStamp = stInventoryReportResult.m_strTimeStamp;
                int nAntennaNum = int.Parse(stInventoryReportResult.m_strAntennaNo);
   //             if (nAntennaNum >= 0 && nAntennaNum < 4)
                {
                    ++inventoryDataInfo.m_arrnInventoryCntSortByAntenna[nAntennaNum];		//对应天线号的盘点到次数加1
                }
                inventoryDataInfo.m_fSpeed = CalculateInventorySpeed(inventoryDataInfo.m_arrnInventoryCntSortByAntenna[nAntennaNum]);                               
                m_dictionaryInventoryStatisticsData[stInventoryReportResult.m_strEPC] = inventoryDataInfo;
            }
        }

        /// <summary>
        /// 清除盘点到的EPCMap
        /// </summary>
        public void ClearInventoryEPCMap()
        {
            lock (m_lockDictionaryInventoryStatisticsData)
            {
                m_dictionaryInventoryStatisticsData.Clear();
            }
        }

        /// <summary>
        /// 统计连续读数据
        /// </summary>
        public void StatisticsRecycleReadData(CircleReadTag tag)
        {
            lock (m_lockDictionaryRecycleReadStatisticsData)
            {
                FastReadDataInfo fastReadTagDataInfo = null;
                if (!m_dictionaryRecycleReadStatisticsData.ContainsKey(tag.m_strEPC))
                {
                    fastReadTagDataInfo = new FastReadDataInfo(tag);
                    m_dictionaryRecycleReadStatisticsData.Add(tag.m_strEPC, fastReadTagDataInfo);
                }
                else
                {
                    fastReadTagDataInfo = m_dictionaryRecycleReadStatisticsData[tag.m_strEPC];
                }
                fastReadTagDataInfo.m_strReadData = tag.m_strReadData;                                
                if (tag.m_nAntennaNum >= 0 && tag.m_nAntennaNum < 4)
                {
                    ++fastReadTagDataInfo.m_arrnReadCntSortByAntenna[tag.m_nAntennaNum];		//对应天线号的盘点到次数加1
                    fastReadTagDataInfo.m_fSpeed = CalculateInventorySpeed(fastReadTagDataInfo.m_arrnReadCntSortByAntenna[tag.m_nAntennaNum]);
                }                
                m_dictionaryRecycleReadStatisticsData[tag.m_strEPC] = fastReadTagDataInfo;
            }
        }

        /// <summary>
        /// 清除连续读数据
        /// </summary>
        public void ClearRecycleReadData()
        {
            lock (m_lockDictionaryRecycleReadStatisticsData)
            {
                m_dictionaryRecycleReadStatisticsData.Clear();
            }
        }

        /// <summary>
        /// 把EPC加入到集合中
        /// </summary>
        /// <param name="strEPC">EPC数据</param>
        /// <param name="nCnt">个数</param>
        public void AddEPCToMap(string strEPC, int nCnt)
        {	
            lock(m_lockDictionaryEPCData)
            {
                EPCDataInfo epcInfo=null;
                if (m_dictionaryEPCData.ContainsKey(strEPC))
                {
                    epcInfo=m_dictionaryEPCData[strEPC];
                    if (epcInfo!=null&&nCnt>epcInfo.m_nCnt)
                    {
                        epcInfo.m_nCnt=nCnt;
                    }
                }
                else
                {
                    epcInfo=new EPCDataInfo();
                    epcInfo.m_strEPC=strEPC;
                    epcInfo.m_nCnt=nCnt;
                    m_dictionaryEPCData.Add(strEPC,epcInfo);
                }
            }	   
        }

        /// <summary>
        /// 从Map中移除EPC
        /// </summary>
        /// <param name="strEPC">EPC</param>
        public void RemoveEPCFromMap(string strEPC)
        {
            lock(m_lockDictionaryEPCData)
            {
                if (m_dictionaryEPCData.ContainsKey(strEPC))
                {
                    m_dictionaryEPCData.Remove(strEPC);
                }
            }
        }

        /// <summary>
        /// 清除EPC Map
        /// </summary>
        public void ClearEPCMap()
        {
            lock(m_lockDictionaryEPCData)
            {
                m_dictionaryEPCData.Clear();
            }	
        }

        /// <summary>
        /// 更新数据库中读写器信息
        /// </summary>
        /// <param name="strReaderIP"></param>
        /// <param name="strDeviceID"></param>
        /// <param name="strPosition"></param>
        public bool UpdateReaderInfoInDataBase(string strReaderIP, string strDeviceID)
        {
            bool bResult = false;
            if (
                string.IsNullOrEmpty(strReaderIP)
                ||
                string.IsNullOrEmpty(strDeviceID)               
                )
            {
                return bResult;
            }

            //判断数据库中是否存在该读写器的信息,没有则保存,有则更新数据库中的数据
            string strQuery = String.Format("select ReaderIP from ReaderInfoTable where ReaderIP='{0}'", strReaderIP);
           // LogFile.g_logfile.WriteWarningLog(strQuery); 

            return bResult;
        }

        /// <summary>
        /// 更新盘点数据信息
        /// </summary>
        /// <param name="strDeviceID"></param>
        /// <param name="strEPC"></param>
        /// <param name="strEnterTime"></param>
        /// <param name="strLeaveTime"></param>
        public bool UpdateInventoryDataInfo(string strDeviceID, string strEPC, string strTID, string strInventoryTime,int nAntennaNo,float fRssi,float fInventoryFrequency)
        {
            bool bResult = false;
            if (
                string.IsNullOrEmpty(strEPC)
                ||
                string.IsNullOrEmpty(strDeviceID)
                ||     
                strTID==null
                ||
                string.IsNullOrEmpty(strInventoryTime)
                )
            {
                return bResult;
            }
            
            //判断数据库中是否存在该读写器盘点数据的信息,没有则保存,有则更新数据库中的数据
            string strQuery = String.Format("select DeviceID,EPC from InventoryDataInfoTable where DeviceID='{0}'and EPC='{1}'", strDeviceID, strEPC);
           // LogFile.g_logfile.WriteWarningLog(strQuery);
            
            return bResult;
        }

    }
}
