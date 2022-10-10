using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RFIDReaderNetwork_SerialSDK_ForCSharp.DataStructureLayer;

namespace BLL
{
    //读写器操作记录类
    public class ReaderOperate
    {
        public ReaderWorkType m_nReaderWorkType;							//读写器工作模式
        public string m_strReaderIPOrCom;							//读写器IP,应用程序作为客户端时该数据有效
        public bool m_bIsInventory;								//是否正在盘点
        public bool m_bIsReadCycle;								//是否正在循环读

        //状态信息TAB页
        //存储其他信息,如读写器状态信息,但不保存到XML文件中	
        public DEVICE_STATUS_RESULT m_stDeviceStatus;				//读写器状态信息
        public SWRInfo m_stSwrInfo;					//读写器驻波比信息
        public DEVICE_GPIO_STATUS_RESULT m_stDeviceGpiStatus;		//GPI状态信息
        public DEVICE_GPIO_STATUS_RESULT m_stDeviceGpoStatus;		//GPO状态信息
        public SET_GPO_PARAM m_stSetGPOParams;			//设置GPO信息
        public SET_ALLANTENNA_PARAM m_stDeviceSetAntennaInfo;			//设置天线信息
        public DEVICE_ANAINFO_STATUS_RESULT m_stDeviceGetAntennaInfo;	//获取天线信息

        public bool m_bIsGetDeviceStatusSuccess;	//获取读写器状态信息是否成功
        public bool m_bIsGetSWRInfoSuccess;			//获取驻波比信息是否成功
        public bool m_bIsGetGpiStatusSuccess;		//获取GPI信息是否成功
        public bool m_bIsGetGpoStatusSuccess;		//获取GPO信息是否成功
        public bool m_bIsSetGpoStatusSuccess;		//设置GPO信息是否成功
        public bool m_bIsSetAntennaInfoSuccess;		//设置天线信息是否成功
        public bool m_bIsGetAntennaInfoSuccess;		//获取天线信息是否成功
        public bool m_bIsSetAntennaInfoTimeLaterThanGetAntennaInfoTime;//设置天线信息时刻在获取天线时刻之后

        //6C快读操作TAB页
        public int m_nRecycleReadMenoryBank;		//循环读的内存区域
        public int m_nRecycleReadOffset;			//循环读的偏移
        public int m_nRecycleReadLength;			//循环读的长度
        public int m_nRecycleReadAntennaNo;		//循环读的天线号
        public bool m_bIsRecycleSortByEPCAndAntenna;//循环读EPC+Antenna方式排序

        //6B标签操作TAB页
        public string m_strSelectUID;					//选中的UID
        public int m_n6BTagAntennaIndex;			//天线号
        public int m_n6BTagReadOffset;				//读取偏移
        public int m_n6BTagReadLength;				//读取长度
        public int m_n6BTagWriteOffset;			//写入偏移
        public int m_n6BTabWriteLength;			//写入长度
        public bool m_bIs6BTagDataTypeIsHex;		//写入数据格式为HEX
        public string m_str6BWriteData;				//写入数据
        public ReadTagResult[] m_arrSt6BTagOperateReportResult;	//读取结果
        public int m_n6BTagReadResultArraySize;	//存放读取结果数组长度

        //6C标签操作TAB页
        public string m_strSelectEPC;					//选中的EPC
        public int m_n6CTagAntennaIndex;			//天线号
        public int m_n6CTagReadMemoryBank;			//读内存区域
        public int m_n6CTagReadOffset;				//读取偏移
        public int m_n6CTagReadLength;				//读取长度
        public string m_str6CTagReadAccessPassword;	//标签访问密码	
        public bool m_bIsCheckSig;					//是否验签
        public bool m_bIsDecryption;				//是否解密
        public int m_n6CTagWriteMemoryBank;		//写内存区域
        public int m_n6CTagWriteOffset;			//写入偏移
        public int m_n6CTagWriteLength;			//写入长度
        public string m_str6CTagWriteAccessPassword;	//标签访问密码
        public bool m_bIs6CTagDataTypeIsHex;		//写入数据格式为HEX
        public string m_str6CWriteData;				//写入数据
        public bool m_bIsSigure;					//是否签名	
        public bool m_bIsEncryption;				//是否加密
        public string m_str6CTagKillTagAcessPassword;	//销毁标签中的访问密码
        public string m_str6CTagKillTagKillPassword;	//销毁标签中的销毁密码
        public int m_n6CQTMemoryBank;				//QT操作内存区
        public int m_n6CQTReadWrite;				//QT操作读还是写
        public bool m_bIsEnableQTShortEnage;		//近场使能
        public int m_n6CQTSortType;				//存储方式
        public string m_str6CQTAccessPassword;		//QT访问密码
        public int m_n6CTagLockAccessPassword;		//锁标签访问密码
        public int m_n6CTagLockEPC;				//锁标签EPC区
        public int m_n6CTagLockUser;				//锁标签User区
        public int m_n6CTagLockKillPassword;		//锁标签销毁密码
        public int m_n6CTagLockTID;				//锁标签TID区
        public string m_str6CTagLockAccessPassword;	//锁标签访问密码
        public ReadTagResult[] m_arrSt6CTagOperateReportResult;	//读取结果
        public int m_n6CTagReadResultArraySize;	//存放读取结果数组长度

        public ReaderOperate()
        {
            m_nReaderWorkType = ReaderWorkType.TCP_SERVER;//读写器工作模式,默认为服务端	
            m_strReaderIPOrCom = "192.168.1.100";//读写器IP,应用程序作为客户端时该数据有效
            m_bIsInventory = false;//是否正在盘点
            m_bIsReadCycle = false;//是否正在循环读

            m_bIsGetDeviceStatusSuccess = false;		//获取读写器状态信息是否成功
            m_bIsGetSWRInfoSuccess = false;			//获取驻波比信息是否成功
            m_bIsGetGpiStatusSuccess = false;			//获取GPI信息是否成功
            m_bIsGetGpoStatusSuccess = false;			//获取GPO信息是否成功
            m_bIsSetGpoStatusSuccess = false;			//设置GPO信息是否成功
            m_bIsSetAntennaInfoSuccess = false;		//设置天线信息是否成功
            m_bIsGetAntennaInfoSuccess = false;		//获取天线信息是否成功
            m_bIsSetAntennaInfoTimeLaterThanGetAntennaInfoTime = false;//设置天线信息时刻在获取天线时刻之后

            m_nRecycleReadMenoryBank = 3;		//循环读的内存区域
            m_nRecycleReadOffset = 0;			//循环读的偏移
            m_nRecycleReadLength = 1;			//循环读的长度
            m_nRecycleReadAntennaNo = 0;		//循环读的天线号
            m_bIsRecycleSortByEPCAndAntenna = true;			//循环读EPC+Antenna方式排序

            m_strSelectUID = "";
            m_n6BTagAntennaIndex = 0;				//天线号
            m_n6BTagReadOffset = 0;				//读取偏移
            m_n6BTagReadLength = 4;				//读取长度
            m_n6BTagWriteOffset = 4;				//写入偏移
            m_n6BTabWriteLength = 1;				//写入长度
            m_bIs6BTagDataTypeIsHex = true;		//写入数据格式为HEX
            m_str6BWriteData = "";					//写入数据
            m_arrSt6BTagOperateReportResult = null;
            m_n6BTagReadResultArraySize = 0;

            m_strSelectEPC = "";							//选中的EPC
            m_n6CTagAntennaIndex = 0;						//天线号
            m_n6CTagReadMemoryBank = 0;					//读内存区域
            m_n6CTagReadOffset = 0;						//读取偏移
            m_n6CTagReadLength = 4;						//读取长度
            m_str6CTagReadAccessPassword = "00000000";	//标签访问密码	
            m_bIsCheckSig = false;						//是否验签
            m_bIsDecryption = false;						//是否解密
            m_n6CTagWriteMemoryBank = 0;					//写内存区域
            m_n6CTagWriteOffset = 0;						//写入偏移
            m_n6CTagWriteLength = 1;						//写入长度
            m_str6CTagWriteAccessPassword = "00000000";	//标签访问密码
            m_bIs6CTagDataTypeIsHex = true;				//写入数据格式为HEX
            m_str6CWriteData = "";						//写入数据
            m_bIsSigure = false;							//是否签名	
            m_bIsEncryption = false;						//是否加密
            m_str6CTagKillTagAcessPassword = "00000000";	//销毁标签中的访问密码
            m_str6CTagKillTagKillPassword = "00000000";	//销毁标签中的销毁密码
            m_n6CQTMemoryBank = 0;						//QT操作内存区
            m_n6CQTReadWrite = 0;							//QT操作读还是写
            m_bIsEnableQTShortEnage = false;				//近场使能
            m_n6CQTSortType = 0;							//存储方式
            m_str6CQTAccessPassword = "00000000";			//QT访问密码
            m_n6CTagLockAccessPassword = 4;				//锁标签访问密码
            m_n6CTagLockEPC = 4;							//锁标签EPC区
            m_n6CTagLockUser = 4;							//锁标签User区
            m_n6CTagLockKillPassword = 4;					//锁标签销毁密码
            m_n6CTagLockTID = 4;							//锁标签TID区
            m_str6CTagLockAccessPassword = "00000000";	//锁标签访问密码
            m_arrSt6CTagOperateReportResult = null;
            m_n6CTagReadResultArraySize = 0;
                        
        }


    }
}
