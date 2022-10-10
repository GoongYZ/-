using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //循环读事件参数
    public class RecycleReadTagEventArgs : EventArgs
    {
        public ReportData m_readTagResult=null;

        public RecycleReadTagEventArgs()
        {
            m_readTagResult=new ReportData();
        }

        public RecycleReadTagEventArgs(ReportData result)
        {
            m_readTagResult = result;
        }

        public void SetInventoryResult(ReportData result)
        {
            m_readTagResult = result;
        }

        public ReportData GetInventoryResult()
        {
            return m_readTagResult;
        }
    }

}
