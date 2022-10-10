using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BLL
{
    //EPC数据包
    public class EPCDataInfo
    {
        public string m_strEPC;								//EPC
        public int m_nCnt;									//个数

        public EPCDataInfo()
        {
            m_strEPC = string.Empty;
            m_nCnt = 0;
        }

    }
}
