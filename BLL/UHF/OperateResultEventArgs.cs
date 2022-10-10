using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //操作结果事件参数
    public class OperateResultEventArgs : EventArgs
    {
        public OperationResult nRetVal = OperationResult.FAIL;


    }
}
