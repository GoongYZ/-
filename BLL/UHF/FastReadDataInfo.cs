using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //连续读数据
    public class FastReadDataInfo
    {
        public string m_strDeviceID;                            //设备序列号
        public string m_strEPC;                                 //标签EPC
        public string m_strReadData;                            //读取到的数据
        public float m_fSpeed;                                  //读取速度
        public int m_nAntennaNo;                                //天线号
        public int[] m_arrnReadCntSortByAntenna;                //根据天线号分类读取次数,元素0存放天线1的读取次数,元素1存放天线2的读取次数

        public FastReadDataInfo()
        {
            m_strDeviceID = string.Empty;
            m_strEPC = string.Empty;
            m_strReadData = string.Empty;
            m_fSpeed = 0.0f;
            m_arrnReadCntSortByAntenna = new int[4];
            m_nAntennaNo = 1;
            for (int i = 0; i < 4; ++i)
            {
                m_arrnReadCntSortByAntenna[i] = 0;
            }
        }

        public FastReadDataInfo(CircleReadTag tag)
        {
            m_strDeviceID = tag.m_strDeviceID;
            m_strEPC = tag.m_strEPC;
            m_strReadData = tag.m_strReadData;
            m_fSpeed = 0.0f;
            m_nAntennaNo = tag.m_nAntennaNum + 1;
            m_arrnReadCntSortByAntenna = new int[4];
            for (int i = 0; i < 4; ++i)
            {
                m_arrnReadCntSortByAntenna[i] = 0;
            }
        }

    }
}
