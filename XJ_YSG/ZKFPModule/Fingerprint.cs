using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XJ_YSG
{

    /// <summary>
    /// 指纹链接
    /// </summary>
    public class Fingerprint
    {
        Logo log = new Logo();

        /// <summary>
        /// 指纹设备链接
        /// </summary>
        /// <returns></returns>
        public string  ZW_Connection()
        {           
            if (ParameterModel.m_hDevice == IntPtr.Zero)
            {
                ParameterModel.m_hDevice = ParameterModel.ZKFPModule_Connect("protocol=USB,vendor-id=6997,product-id=289");               
                log.WriteLogo("指纹激活成功", 5);
                return "ok";
            }
            else
            {
                return "erro";
            }
               
        }

        /// <summary>
        /// 指纹设备断开
        /// </summary>
        /// <returns></returns>
        public string Zw_Disconnect() 
        {
            if (ParameterModel.m_hDevice != IntPtr.Zero)
            {

                int nRet = ParameterModel.ZKFPModule_Disconnect(ParameterModel.m_hDevice);
                if (nRet==0)
                {
                
                    ParameterModel.m_hDevice = IntPtr.Zero;                  
                    return "ok";
                }
                else
                {
                    return "erro";
                }
            }
            else 
            {
                return "ok";
            }
        }
        /// <summary>
        /// 添加指纹
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string Addzw(int userid)
        {
            // 登记用户模板(设备句柄，用户id)             
            int nRet = ParameterModel.ZKFPModule_EnrollTemplateByImage(ParameterModel.m_hDevice, userid, ParameterModel.m_pImageBuffer, ParameterModel.m_nSize);
            if (0 == nRet)
            {
                //想窗体发送消息（//初始化为零的指针，常量，用户id）                 
                //MessageBox.Show("录入成功");
                log.WriteLogo("录入成功", 5);
                return "ok";

            }
            else
            {
                string erro = Erroneous(nRet.ToString());              
                log.WriteLogo("录入失败!" + "错误原因:" + erro, 5);
                return "erro";
            }
        }


        /// <summary>
        /// 删除指纹
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string delzw(int userid)
        {
            if (ParameterModel.m_hDevice != IntPtr.Zero)
            {
                int nRet = ParameterModel.ZKFPModule_DeleteUser(ParameterModel.m_hDevice, userid);
                if (0 == nRet)
                {
                    return "ok";
                }
                else
                {
                    return "erro";
                }
            }
            else
            {
                return "erro";
            }
        }


        /// <summary>
        /// 错误码
        /// </summary>
        /// <param name="Iserro"></param>
        /// <returns></returns>
        public string Erroneous(string  Iserro) 
        {
            string erro = "";
            switch (Iserro)
            {
                case "0":
                    erro = "操作成功";
                    break;
                case "-1":
                    erro = "操作失败";
                    break;
                case "-2":
                    erro = "参数无效/错误";
                    break;            
                case "-3":
                    erro = "空指针";
                    break;
                case "-4":
                    erro = "内存不足，分配失败";
                    break;
                case "-5":
                    erro = "无效协议类型";
                    break;
                case "-6":
                    erro = "协议不支持";
                    break;
                case "-7":
                    erro = "连接设备失败";
                    break;
                case "-8":
                    erro = "设备未连接";
                    break;
                case "-9":
                    erro = "无效句柄";
                    break;
                case "-10":
                    erro = "超时";
                    break;
                case "-11":
                    erro = "通信失败";
                    break;
                case "-12":
                    erro = "接口不支持";
                    break;
                case "-13":
                    erro = "内存分配不足";
                    break;                   
                case "-14":
                    erro = "无效图像";
                    break;
                case "-15":
                    erro = "采集模板失败";
                    break;
                case "-10000":
                    erro = "固件错误编码";
                    break;
                case "-10052":
                    erro= "系统正在忙  ";
                    break;
                case "-10099":
                    erro = "失败";
                    break;
                case "-10100":
                    erro = "超时";                  
                    break;
                case "-10104":
                    erro = "参数错误";
                    break;
                case "-10105":
                    erro = "找不到或不支持";
                    break;
                case "-10109":
                    erro = "存储容量超出";
                    break;
                case "-10114":
                    erro = "超出允许登记的最大指纹数量";
                    break;
                case "-10118":
                    erro = "无效的ID号";
                    break;
                case "-10129":
                    erro = "执行的命令被取消";
                    break;
                case "-10130":
                    erro = "传输数据错误";
                    break;
                case "-10134":
                    erro = "该指纹已存在";
                    break;
                case "-10255":
                    erro = "数据头校验失败（自定义）";
                    break;
                case "-10254":
                    erro = "数据校验失败（自定义）";
                    break;
                case "-10253":
                    erro = "数据头无效（自定义）";
                    break;
               
                default:
                    erro = "其他错误";
                    break;
            }
            return erro;
        }
    }
}
