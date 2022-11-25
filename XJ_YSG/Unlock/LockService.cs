
using BLL;
using System;
using XJ_YSG;

namespace XJ_YSG
{
    public class LockService
    {
        public LockService()
        {

        }

        private static SerialPortUtil2 port = new SerialPortUtil2();
        public bool initCom(string com)
        {
            bool trn = false;
            try
            {
                port.PortName = com;
                port.BaudRate = SerialPortBaudRates.BaudRate_115200;
                port.OpenPort();
                Logo.sWriteLogo("锁板连接成功", 6);
                trn = true;
            }
            catch (Exception e)
            {
                trn = false;
            }
            return trn;
        }
        public void closeCom()
        {
            port.ClosePort();
        }
        /// <summary>
        /// 发送命令打开柜门
        /// </summary>
        /// <param name="gzh">格子号</param>
        public static void Send(string gzh)
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
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
            string rtn = "";
            port.WriteData2(data, ref rtn);
            Logo.sWriteLogo("接受报文：" + rtn.ToString(), 6);
        }
        public byte Get_CheckXor(byte[] data)
        {
            byte CheckCode = 0;
            int len = data.Length;
            for (int i = 0; i < len; i++)
            {
                CheckCode ^= data[i];
            }
            return CheckCode;
        }
        /// <summary>
        /// 发送命令获取柜门状态
        /// </summary>
        /// <param name="gzh">格子号</param>
        public static string State(string gzh)
        {
            //A6 A8 01 00 00 08 00 09 01 60 发送命令1号格子
            //A6 A8 00 01 00 0A 00 0A 01(柜号) 00(状态) 03 67   返回命令
            int gz = Convert.ToInt32(gzh);
            string rtn = "";
            byte[] data = new byte[10];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x08;
            data[6] = 0x00;
            data[7] = 0x09;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);  //柜号
            string gzbs = (96 + gz).ToString("X2");
            data[9] = Convert.ToByte(gzbs, 16);
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
            port.WriteData2(data, ref rtn);
            Logo.sWriteLogo("接受报文：" + rtn.ToString(), 6);
            return rtn;
        }



       
       


        /// <summary>
        /// 灭灯
        /// </summary>
        /// <param name="gzh"></param>
        public static void Destroy(string gzh)
        {
            //string str1 = "A6 A8 01 00 00 0A 00 01 01 00 00 5B";
            //port.WriteData_HEX(str1);
            //获取格子号
            int gz = Convert.ToInt32(gzh);
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
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
            port.SendCommand(data);
        }

        /// <summary>
        /// 红灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>

        public static void red_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 01 00 5C=92";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 81 00 DC";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }

        }


        /// <summary>
        /// 绿灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>
        public static void green_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01
                //
                //00 00 0A 00
                //01 01 04 00
                //5F";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 84 00 DF";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
        }


        /// <summary>
        /// 蓝灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public static void Blue_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 02 00 5D"；
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 82 00 DD";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
        }


        /// <summary>
        /// 白灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public static void White_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 07 00 62"；
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 87 00 E2";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
        }


        /// <summary>
        /// 冰蓝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Ice_Blue(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 06 00 61"；
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 86 00 E1";
                int gz = Convert.ToInt32(gzh);
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
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 6);
                port.SendCommand(data);
            }
        }       
    }
}
