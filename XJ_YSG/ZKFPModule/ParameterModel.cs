using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace XJ_YSG
{

    /// <summary>
    /// 变量与DLL
    /// </summary>
    public class ParameterModel
    {



        #region 存放临时登记模板
        public static int m_UserId = -1;  //用户ID
        public  static IntPtr m_hDevice = IntPtr.Zero;  //设备句柄        
       
        const int MESSAGE_CAPTURED_OK = 0x0400 + 10;
        const int MESSAGE_IDENTIFY_OK = 0x0400 + 20;
        const int MESSAGE_IDENTIFY_FAIL = 0x0400 + 21;
        const int MESSAGE_REGISTER_COUNT = 0x0400 + 31;
        const int MESSAGE_REGISTER_OK = 0x0400 + 41;
        const int MESSAGE_REGISTER_FAIL = 0x0400 + 42;


        //是否继续识别
        public static bool issbzw = true;
        #endregion

        #region  dll
        [DllImport("user32.dll", EntryPoint = "SendMessageA")]
        public static extern int SendMessage(IntPtr hwnd, int wMsg, IntPtr wParam, IntPtr lParam);

        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr MemCopy(byte[] dest, byte[] src, UIntPtr count);

        [DllImport("ZKFPModule.dll")]
        public  extern static IntPtr ZKFPModule_Connect(string lpParams);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_Disconnect(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_EnrollUserByScan(IntPtr Handle, int nUserID);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_GetFingerImage(IntPtr Handle, ref int width, ref int heigth, byte[] imgData, ref int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ClearDB(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DisableDevice(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_EnableDevice(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_FreeScan(IntPtr Handle, ref int UserID, ref int Index);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_EnrollTemplateByImage(IntPtr Handle, int userID, byte[] data, int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_IdentifyByImage(IntPtr Handle, byte[] imgData, int dataSize, ref int userID, ref int index);


        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_SetTime(IntPtr Handle, int Year, int Month, int Day, int Hour, int Minute, int Second);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_GetTime(IntPtr Handle, ref int Year, ref int Month, ref int Day, ref int Hour, ref int Minute, ref int Second);
        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ScanTemplate(IntPtr Handle, byte[] m_szTemplate, ref int nLength);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ReadAllLogs(IntPtr Handle, byte[] logData, ref int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_GetStatus(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_GetParameter(IntPtr Handle, int flag, ref int value);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_SetParameter(IntPtr Handle, int flag, int value);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_SaveParameter(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DeleteAllUsers(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DeleteUser(IntPtr Handle, int userID);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ReadAllUser(IntPtr Handle, byte[] userData, ref int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_GetUser(IntPtr Handle, int userID, StringBuilder name, string password,
                                                ref ushort secLevel, ref UInt32 PIN2,
                                                    byte[] privilege, byte[] figerprintNum, byte[] Card);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ModifyUser(IntPtr Handle, int userID, string name, string password,
                                                ushort secLevel, UInt32 PIN2, byte privilege,
                                                byte figerprintNum, string Card);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_SetTemplates(IntPtr Handle, int userID, int flag, byte[] data, int dataSize);

        [DllImport("ZKFPModule.dll")]
         public extern static int ZKFPModule_DeleteTemplates(IntPtr Handle, int userID, int index, int flag);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DeleteAllTemplates(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ReadTemplates(IntPtr Handle, int userID, int index, int flag, byte[] data, ref int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ReadAllTemplates(IntPtr Handle, byte[] data, ref int dataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_Verify(IntPtr Handle, int userID);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DeleteAllLogs(IntPtr Handle);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_Reset(IntPtr Handle);


        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_Upgrade(IntPtr Handle, byte[] fw, int size);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_UploadTemplatesFileData(IntPtr Handle, byte[] tmpData, int size);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_UploadUserFileData(IntPtr Handle, byte[] userData, int size);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DownloadUserFileData(IntPtr Handle, byte[] userData, ref int size);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_DownloadTemplatesFileData(IntPtr Handle, byte[] tmpData, ref int size);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_TimeAnalyse(UInt32 date, UInt32 time,
                                                 ref int Year, ref int Month, ref int Day,
                                                 ref int Hour, ref int Minute, ref int Second);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_BufferToProtocol(int inDataType, byte[] data, int size, byte[] descData, ref int descDataSize);

        [DllImport("ZKFPModule.dll")]
        public extern static int ZKFPModule_ProtocolToBuffer(int inDataType, byte[] descData, byte[] data, ref int size);

        // typedef void (__stdcall* PCBEnrollStatus)(int index, void* pUserParam);
        public delegate void PCBEnrollStatus(int index, IntPtr pUserParam); //声明委托  

        [DllImport("ZKFPModule.dll")]
        public extern static void ZKFPModule_SetEnrollCallBack(IntPtr Handle, PCBEnrollStatus pCBFunc, IntPtr pUserParam);

        //typedef int (_stdcall *MyFun)(int n, string str);
        #endregion

    }
}
