using BLL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading;
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
    /// Xj_Mima.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Mima : Window
    {
        Interaction_WebService service = new Interaction_WebService();      
        public Xj_Mima()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/xj_mima.png", UriKind.Absolute));          
            this.Background = b3;
        }



        /// <summary>
        /// 输入密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_mima_TouchUp(object sender, TouchEventArgs e)
        {
            Button button = (Button)sender;
            mm.Content += button.Tag.ToString();
        }



        /// <summary>
        /// 删除密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Button_Delete_TouchUp(object sender, TouchEventArgs e)
        {
            if (mm.Content.ToString().Length > 0)
            {
                mm.Content = mm.Content.ToString().Substring(0, mm.Content.ToString().Length - 1);
            }
        }


        /// <summary>
        /// 密码输入完成后点击确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>     
        private void Button_Save_TouchUp(object sender, TouchEventArgs e)
        {
            if (mm.Content.ToString().Length == 6)
            {
                if (MainBox.isyjkq)
                {
                    bool seccss = service.Checkmanaemm(MainBox.sbbm, mm.Content.ToString());
                    if (seccss)
                    {
                        MainBox.usertable.Add("PK", "1");
                        Xj_BoxList boxList = new Xj_BoxList();
                        Close();
                        boxList.ShowDialog();
                    }
                    else
                    {
                        speack("密码错误");
                    }
                }
                else
                {
                    if (mm.Content.ToString().Length == 6)
                    {
                        Hashtable tb = service.getInfoByEwm(mm.Content.ToString(), MainBox.sbbm);
                        if (tb != null && tb.Count > 0)
                        {
                            string wzm = tb["WZM"].ToString();
                            MainBox.QycsqdPK = tb["YCSQDPK"].ToString();
                            MainBox.Send(wzm);
                            MainBox.red_light(wzm, true);
                            speack("柜门已打开，取后请关门");
                            MainBox.locklis.Add(wzm + "_mmqys");
                            Close();
                        }
                        else
                        {
                            speack("验证码不正确，请重新输入");
                        }
                    }
                }

            }
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
        /// 取消退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void Shut_down_TouchUp(object sender, TouchEventArgs e)
        {
            MainBox.zwTimer.Start();
            MainBox.RfidTimer.Start();
            if (MainBox.usertable.Count >= 0 || MainBox.usertable == null) 
            {
                MainBox.usertable.Clear();
            }
            if (MainBox.isyjkq) 
            {
                MainBox.isyjkq = false;
            }           
            Close();
        }

       

        
    }
}
