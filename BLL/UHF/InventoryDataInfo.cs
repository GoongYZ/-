using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //盘点数据信息
    public class InventoryDataInfo
    {				
        public string m_strDeviceID;                     //设备ID  
        public string m_strTimeStamp;                    //盘点时刻
        public string m_strAntennaNo;                    //盘点天线号,从0开始编号
        public string m_strEPC;                          //标签EPC
        public string m_strRSSI;                         //接收灵敏度
        public string m_strTID;                          //标签TID
        public string m_strFrequency;                    //频点	
        public float m_fSpeed;                           //速度
        public int[] m_arrnInventoryCntSortByAntenna;    //根据天线号分类盘点次数,元素0存放天线1的盘点次数,元素1存放天线2的盘点次数

        public InventoryDataInfo()
        {
            m_strDeviceID = string.Empty;
            m_strTimeStamp = string.Empty;
            m_strAntennaNo = string.Empty;
            m_strEPC = string.Empty;
            m_strRSSI = string.Empty;
            m_strTID = string.Empty;
            m_strFrequency = string.Empty;
            m_arrnInventoryCntSortByAntenna = new int[100];
            for (int i = 0; i < 100;++i )
            {
                m_arrnInventoryCntSortByAntenna[i] = 0;
            }
            m_fSpeed = 0.0f;
        }

        public InventoryDataInfo(INVENTORY_REPORT_RESULT stInventoryResult)
        {
            m_strDeviceID = stInventoryResult.m_strDeviceID;
            m_strTimeStamp = stInventoryResult.m_strTimeStamp;
            m_strAntennaNo = stInventoryResult.m_strAntennaNo;
            m_strEPC = stInventoryResult.m_strEPC;
            m_strRSSI = stInventoryResult.m_strRSSI;
            m_strTID = stInventoryResult.m_strTID;
            m_strFrequency = stInventoryResult.m_strFrequency;
            m_arrnInventoryCntSortByAntenna = new int[100];
            for (int i = 0; i < 100; ++i)
            {
                m_arrnInventoryCntSortByAntenna[i] = 0;
            }            
            m_fSpeed = 0.0f;
        }

    }
}
