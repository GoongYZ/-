
using BLL;
using System;
using System.Collections.Generic;

namespace XJ_YSG
{
    public class LockControl
    {
        private SerialPortUtil port = new SerialPortUtil();
        private static string ComPortName = ServerBase.XMLRead("Lock", "COM_LOCK");

        /// <summary>
        /// 监控箱门的数据集合
        /// </summary>
        public static List<string> locklis = new List<string>();
        /// <summary>
        /// 打开串口
        /// </summary>
        public void Open()
        {
            try
            {
                port.PortName = ComPortName;
                port.BaudRate = SerialPortBaudRates.BaudRate_115200;
                port.OpenPort();
            }
            catch (Exception)
            {
                
            }
        }


        public void Close() 
        {
            port.ClosePort();
        }

        /// <summary>
        /// 发送命令打开柜门
        /// </summary>
        /// <param name="gzh">格子号</param>
        public void Send(string  gzh)      
        {
            int gz = Convert.ToInt32(gzh);
            string rtn = "";
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x05;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (94 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
            port.WriteData(data, ref rtn);
            bw = rtn;
        }


        /// <summary>
        /// 发送命令获取柜门状态
        /// </summary>
        /// <param name="gzh">格子号</param>
        public string  State_lock(string gzh )
        {
            int gz = Convert.ToInt32(gzh);          
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x05;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (94 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string rut = "";
            port.WriteData(data, ref rut);
            return rut;                     
        }



        //灭灯
        public void Destroy(string gzh)
        {
            //string str1 = "A6 A8 01 00 00 0A 00 01 01 00 00 5B";
            //port.WriteData_HEX(str1);
            //获取格子号
            int gz = Convert.ToInt32(gzh);
            string rtn = "";
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x01;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (90 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
            port.WriteData(data, ref rtn);
            bw = rtn;
        }

        /// <summary>
        /// 红灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>

        public void red_light(string gzh ,bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 01 00 5C=92";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x01;
                data[10] = 0x00;
                string gzbs = (91 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 81 00 DC";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x81;
                data[10] = 0x00;
                string gzbs = (219 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }

        }


        /// <summary>
        /// 绿灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>
        public void green_light(string gzh , bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01
                //
                //00 00 0A 00
                //01 01 04 00
                //5F";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;

                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x04;
                data[10] = 0x00;
                string gzbs = (94 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 84 00 DF";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x84;
                data[10] = 0x00;
                string gzbs = (222 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }


        /// <summary>
        /// 蓝灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public void Blue_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 02 00 5D"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x02;
                data[10] = 0x00;
                string gzbs = (92 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 82 00 DD";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x82;
                data[10] = 0x00;
                string gzbs = (221 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }



        /// <summary>
        /// 白灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public void White_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 07 00 62"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x07;
                data[10] = 0x00;
                string gzbs = (97 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 87 00 E2";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x87;
                data[10] = 0x00;
                string gzbs = (225 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }




        /// <summary>
        /// 冰蓝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void Ice_Blue(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 06 00 61"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x06;
                data[10] = 0x00;
                string gzbs = (96 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 86 00 E1";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x86;
                data[10] = 0x00;
                string gzbs = (224 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }































    }
}
