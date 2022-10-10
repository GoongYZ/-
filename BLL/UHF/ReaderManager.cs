using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using RFIDReaderNetwork_SerialSDK_ForCSharp.ExternalInterfaceLayer;

namespace BLL
{
    //读写器管理类
    public class ReaderManager
    {
        //连接了读写器字典
        private Dictionary<string, Reader> m_dictionaryReader = new Dictionary<string, Reader>();
        //访问字典的锁
        private object m_objLock = new object();

        private  ReaderManager()
        {

        }

        /// <summary>
        /// 连接的读写器字典是否空
        /// </summary>
        /// <returns></returns>
        public bool IsConnectReaderDictionaryEmpty()
        {
            bool bResult = false;
            lock (m_objLock)
            {
                bResult=m_dictionaryReader.Count==0?true:false;
            }
            return bResult;
        }

        /// <summary>
        /// 添加已连接的读写器
        /// </summary>
        /// <param name="strDeviceID">设备ID</param>
        /// <param name="operateReader">读写器对象指针</param>
        public void AddConnectDevice(string strDeviceID, Reader operateReader)//1111
        {
            if (string.IsNullOrEmpty(strDeviceID) || operateReader == null)
                return;
            lock (m_objLock)
            {
                if (!m_dictionaryReader.ContainsKey(strDeviceID))
                {
                    m_dictionaryReader.Add(strDeviceID, operateReader);
                }
            }
        }

        /// <summary>
        /// 把读写器从MAP中移除
        /// </summary>
        /// <param name="strDeviceID">设备序列号</param>
        /// <returns>是否成功</returns>
        public bool RemoveConnectDevice(string strDeviceID)
        {
            if (string.IsNullOrEmpty(strDeviceID))
            {
                return false;
            }

            lock (m_objLock)
            {
                if (m_dictionaryReader.ContainsKey(strDeviceID))
                {
                    return m_dictionaryReader.Remove(strDeviceID);
                }
            }
            return false;
        }
        /// <summary>
        /// 把读写器从MAP中移除
        /// </summary>
        /// <returns></returns>
        public void RemoveAllConnectDevice()
        {
            lock (m_objLock)
            {
                m_dictionaryReader.Clear();
            }
        }


        /// <summary>
        /// 根据IP地址在MAP中查找读写器对象
        /// </summary>
        /// <param name="strReaderIP">读写器IP地址</param>
        /// <returns>读写器对象指针</returns>
        public Reader GetRFIDReaderByReaderIP(string strReaderIP)
        {
            Reader reader=null;
	        if (string.IsNullOrEmpty(strReaderIP))
	        {
                return reader;
	        }
            lock (m_objLock)
            {
                Dictionary<string, Reader>.ValueCollection values = m_dictionaryReader.Values;
                foreach (Reader value in values)
                {
                    if (value.m_rfidWorkReader.m_strReaderIP == strReaderIP)
                    {
                        reader=value;
                        break;
                    }
                }
            }
            return reader;
        }

        /// <summary>
        /// 根据COM号在MAP中查找读写器对象
        /// </summary>
        /// <param name="strComName">读写器COM号</param>
        /// <returns>读写器对象指针</returns>
        public Reader GetRFIDReaderByReaderCom(string strComName)
        {
            Reader reader = null;
            if (string.IsNullOrEmpty(strComName))
            {
                return reader;
            }
            lock (m_objLock)
            {
                Dictionary<string, Reader>.ValueCollection values = m_dictionaryReader.Values;
                foreach (Reader value in values)
                {
                    if (value.m_rfidWorkReader.m_strComName == strComName)
                    {
                        reader = value;
                        break;
                    }
                }
            }
            return reader;
        }


        /// <summary>
        /// 根据设备序列号在MAP中查找读写器对象
        /// </summary>
        /// <param name="strComName">设备序列号</param>
        /// <returns>读写器对象指针</returns>
        public Reader GetRFIDReaderByDeviceID(string strDeviceID)
        {
            Reader reader = null;
            if (string.IsNullOrEmpty(strDeviceID))
            {
                return reader;
            }
            lock (m_objLock)
            {
                foreach (KeyValuePair<string, Reader> keyValue in m_dictionaryReader)
                {
                    if (keyValue.Key == strDeviceID)
                    {
                        reader = keyValue.Value;
                        break;
                    }
                }             
            }
            return reader;
        }

        /// <summary>
        /// 断开所有读写器
        /// </summary>
        public void DisconnectAllDevices()
        {
            Reader reader = null;
            lock (m_objLock)
            {
                foreach (KeyValuePair<string, Reader> keyValue in m_dictionaryReader)
                {
                    reader = keyValue.Value;
                    if (reader!=null)
                    {
                        if (reader.m_bIsInventory)                  //正在盘点
                        {
                            reader.m_rfidWorkReader.StopPeriodInventory();
                            reader.m_bIsInventory = false;
                        }
                        if (reader.m_bIsClickReadCycle)             //正在连续读操作
                        {
                            reader.m_rfidWorkReader.StopPeriodInventory();
                            reader.m_bRecycleThreadRunning = false;
                        }
                        else
                        {
                            reader.m_rfidWorkReader.Disconnect();
                        }
                    }
                }
            }
        }
        public void DisconnectDevice(string strDeviceID)
        {
            Reader reader = null;
            lock (m_objLock)
            {
                reader = GetRFIDReaderByDeviceID(strDeviceID);
                if (reader != null)
                {
                    if (reader.m_bIsInventory)                  //正在盘点
                    {
                        reader.m_rfidWorkReader.StopPeriodInventory();
                        reader.m_bIsInventory = false;
                    }
                    if (reader.m_bIsClickReadCycle)             //正在连续读操作
                    {
                        reader.m_rfidWorkReader.StopPeriodInventory();
                        reader.m_bRecycleThreadRunning = false;
                    }
                    else
                        reader.m_rfidWorkReader.Disconnect();
                }
            }
        }

        //全局的读写器管理类
        public static ReaderManager g_ReaderManager = new ReaderManager();
    }
}
