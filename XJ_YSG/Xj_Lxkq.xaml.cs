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
using System.Windows.Threading;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Lxkq.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Lxkq : Window
    {
        DispatcherTimer ClossDjs = new DispatcherTimer();
        Interaction_WebService Service = new Interaction_WebService();
        int djstime = 30;
        public Xj_Lxkq(MainBox mainbox)
        {
            InitializeComponent();
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/xj_lxkq.png", UriKind.Absolute));
            this.Background = b3;
            this.Left = 201;
            this.Top = 315;
            this.Loaded += ((s, e) =>
            {
                mainbox.markLayer.Visibility = Visibility.Visible;
            });
            this.Closed += ((s, e) =>
            {              
                mainbox.markLayer.Visibility = Visibility.Hidden;
            });
            Djs();
        }

        public void Djs()
        {
            ClossDjs.Interval = new TimeSpan(0, 0, 0, 0, 1000); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            ClossDjs.Tick += new EventHandler(ClossDjs_Tick_canShow); //每一秒执行的方法
            ClossDjs.Start();
        }
        void ClossDjs_Tick_canShow(object sender, EventArgs e)
        {   
            lab_djs.Content = djstime-- ;
            if (djstime == 0) 
            {
                bool islx = Service.sendyxzk(MainBox.sbbm, MainBox.bbh);
                if (islx == true) 
                {
                    MainBox.islxkq = false;
                    MainBox.zwTimer.Stop();               
                    MainBox.yxzkTimer.Stop();
                    Close();
                }
                else 
                {
                    djstime = 30;
                }
            }
        }

        private void btn_lxkq_TouchDown(object sender, TouchEventArgs e)
        {
            Xj_BoxList BoxList = new Xj_BoxList();
            BoxList.ShowDialog();
        }
    }
}
