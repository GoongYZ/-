using BLL;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Xj_BoxList.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_BoxList : Window
    {
        DataTable tb = new DataTable();
        private Xj_BoxList BoxList = null;
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
            for (int i = 0; i < 50; i++)
            {
                DataRow dr = tb.NewRow();
                dr["UriSource"] = "img/Boxlist_daifenpei.png";
                dr["BH1"] = "01";
                dr["BH2"] = "待分配";
                tb.Rows.Add(dr);
            }
            s_1.DataContext = tb;
        }


        private void Button_zwlr_Click(object sender, RoutedEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr();
            zwlr.Show();
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
                        Xj_Ycsy Ycsy = new Xj_Ycsy(BoxList);
                        Ycsy.ShowDialog();
                    }
                }
            }
        }

        private void btn_clos_Click(object sender, RoutedEventArgs e)
        {
            MainBox.issbzw = true;
            MainBox.issfsk = true;
            this.Close();           
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
    }
}
