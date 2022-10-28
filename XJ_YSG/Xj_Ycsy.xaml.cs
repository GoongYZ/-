using BLL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Ycsy.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Ycsy : Window
    {
        SerialPortUtil port = new SerialPortUtil();
        string gh = "";
        public Xj_Ycsy(Xj_BoxList BoxList,string dwm)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 172;
            if (dwm != "")            
            {
                gh = dwm;
            }
            this.Loaded += ((s, e) => {
               BoxList.markLayer.Visibility = Visibility.Visible;
            });

            this.Closed += ((s, e) =>
            {
                Xj_BoxList.gh = "";               
                BoxList.markLayer.Visibility = Visibility.Hidden;
            });
        }

        private void open_Button_Click(object sender, RoutedEventArgs e)
        {
            if (gh != "") 
            {
                if (port.IsOpen)
                {
                    Send(gh);
                    White_light(gh, false);
                    MainBox.locklis.Add(gh);
                    this.Close();
                }
            }
        }


        /// <summary>
        /// 发送命令打开柜门
        /// </summary>
        /// <param name="gzh">格子号</param>
        public void Send(string gzh)
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
        /// 红灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>

        public void red_light(string gzh, bool Flashing)
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


        private void btn_tc_TouchLeave(object sender, TouchEventArgs e)
        {
            Xj_BoxList.gh = "";
            this.Close();
        }
    }
}
