using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //读取标签结果
    public class ReadTagResult
    {
        public string m_strTimeStamp;
        public Tag m_tag;

        public ReadTagResult()
        {
            m_strTimeStamp = string.Empty;
            m_tag = null;
        }

        public ReadTagResult(Tag tag)
        {
            DateTime currentTime = DateTime.Now;
            m_strTimeStamp = string.Format("{0:D4}-{1:D2}-{2:D2} {3:D2}:{4:D2}:{5:D2}", currentTime.Year, currentTime.Month, currentTime.Day, currentTime.Hour, currentTime.Minute, currentTime.Second);    
            m_tag = tag;
        }
    }

}
