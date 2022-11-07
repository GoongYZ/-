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
             
        private Xj_BoxList BoxList = null;
        private DataTable dtlist = new DataTable();
        Interaction_WebService Service = new Interaction_WebService();
        string sjh = "";
        public Xj_BoxList(string sjhm)
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
            sjh = sjhm;

            Bindinfo();


        }




        private void Bindinfo()
        {
            DataTable dt = Service.getListBox(sjh, MainBox.sbbm);
            if (dt.Rows.Count > 0 && dt != null)
            {
                //获取用户可以打开的柜号                   
                for (int i = 1; i <= dt.Rows.Count; i++)
                {
                    DataRow dr = dtlist.NewRow(); ;
                    dr["BH"] = dtlist.Rows[i]["BH"].ToString();
                    string zt = dtlist.Rows[i]["ZT"].ToString();
                    if (zt == "0")
                    {
                        dr["UriSource"] = "img/Boxlist_zaiku.png";
                        dr["ZT"] = "正常";
                    }
                    else if (zt == "1")
                    {
                        dr["UriSource"] = "img/Boxlist_chuche.png";
                        dr["ZT"] = "出车中";
                    }                   
                    MainBox.cllb.Rows.Add(dr);
                }
                s_1.DataContext = dt;
            }
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
            MainBox.cllb.Clear();
            this.Close();
        }
    }
}
