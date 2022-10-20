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
    /// Xj_Clpj.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Clpj : Window
    {
        SerialPortUtil port = new SerialPortUtil();
        public Xj_Clpj(MainBox mainbox)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 255;
            
            this.Loaded += ((s, e) => {              
                mainbox.markLayer.Visibility = Visibility.Visible;               
            });
            this.Closed += ((s, e) =>
            {
                MainBox.zwTimer.Start();
                MainBox.RfidTimer.Start();
                UHFService.strEPC = "";
                mainbox.markLayer.Visibility = Visibility.Hidden;
            });
        }

        private void clpj_Button_Click(object sender, RoutedEventArgs e)
        {
            if (UHFService.strEPC != "") 
            {
                LockControl lockControl = new LockControl();
                if (port.IsOpen)                 
                {
                    Send("1");
                    //打开柜门              
                    this.Close();
                }               
                
            }                            
        }
        private void clols_Button_Click(object sender, RoutedEventArgs e)
        {          
            this.Close();
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

    }
}
