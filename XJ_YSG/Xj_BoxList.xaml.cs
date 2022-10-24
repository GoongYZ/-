using BLL;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_BoxList.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_BoxList : Window
    {
        private static int gzsl = Convert.ToInt32(ServerBase.XMLRead("Count", "Ysg_gzsl")); //柜子格子数量
        DataTable tb = new DataTable();
        private Xj_BoxList BoxList = null;
        public static  string gh = "";
        public Xj_BoxList()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/Boxlist.png", UriKind.Absolute));
            this.Background = b3;
            if (BoxList == null)
            {
                BoxList = this;
            }
            Bindinfo();
        }
      

        private void Bindinfo()
        {
            //获取用户可以打开的柜号         
            tb.Columns.Add("UriSource");
            tb.Columns.Add("BH1");
            tb.Columns.Add("BH2");
            for (int i = 1; i <= gzsl; i++)
            {
                DataRow dr = tb.NewRow();
                dr["UriSource"] = "img/Boxlist_zaiku.png";
                dr["BH1"] = i;
                dr["BH2"] = "正常";
                tb.Rows.Add(dr);
            }
            s_1.DataContext = tb;
        }


        private void Button_zwlr_Click(object sender, RoutedEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog(); ;
        }

        private void Grid_b_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //遍历用户控件
            var d = sender as Grid;
            d.Focus();
            UIElementCollection Childrens = d.Children;
            foreach (UIElement ui in Childrens)
            {
                if (((Label)ui).Name == "BH")
                {
                    string dwm = ((Label)ui).Content.ToString();
                    if (!string.IsNullOrEmpty(dwm))
                    {
                        //开启柜门                                          
                        Xj_Ycsy Ycsy = new Xj_Ycsy(BoxList, dwm);                       
                        Ycsy.ShowDialog();
                    }
                }
            }
        }

       

        #region 语音播报
        private void speack(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpeechVoice.speack(text);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }
        #endregion

        private void s_1_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        private void btn_clos_TouchLeave(object sender, TouchEventArgs e)
        {
            MainBox.zwTimer.Start();
            MainBox.RfidTimer.Start();
            this.Close();
        }
    }
}
