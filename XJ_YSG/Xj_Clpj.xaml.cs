using BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Clpj.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Clpj : Window
    {
        SerialPortUtil port = new SerialPortUtil();
        string gh = "";
       
        public Xj_Clpj(MainBox mainbox, string wzm)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 255;
            gh = wzm;
            this.Loaded += ((s, e) =>
            {
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

            if (port.IsOpen)
            {
                if (gh != "")
                {
                    Send(gh);
                    MainBox.locklis.Add(gh);
                    speack("柜门已打开");                    //打开柜门                   
                    UHFService.strEPC = ""; //清除刷卡信息，下次刷卡使用
                }
                else
                {

                    this.Close();
                    speack("归还失败，请重新归还");
                }
            }
            else
            {
                this.Close();
                speack("归还失败，请重新归还");
            }
        }
        private void clols_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        #region 语音播报
        public void speack(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpeechVoice.speack(text);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }
        #endregion

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

        private void clzk_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clzk = sender as RadioButton;
            if (clzk.IsChecked == true)
            {
                MainBox.hashtable.Add("clzk", clzk.Tag.ToString());
            }
        }

        private void clwg_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clwg = sender as RadioButton;
            if (clwg.IsChecked == true)
            {
                MainBox.hashtable.Add("clwg", clwg.Tag.ToString());
            }
        }

        private void clns_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clns = sender as RadioButton;
            if (clns.IsChecked == true)
            {
                MainBox.hashtable.Add("clns", clns.Tag.ToString());
            }
        }
    }
}
