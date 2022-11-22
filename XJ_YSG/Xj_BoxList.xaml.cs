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
        Interaction_WebService Service = new Interaction_WebService();       
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
            this.Closed += Bolist_clos;
        }

        




        /// <summary>
        /// 绑定列表
        /// </summary>
        private void Bindinfo()
        {
            MainBox.cllb.Columns.Add("BH");
            MainBox.cllb.Columns.Add("UriSource");
            MainBox.cllb.Columns.Add("ZT");
            if (MainBox.usertable != null && MainBox.usertable.Count > 0)
            {               
                DataTable dt = Service.getListBox(MainBox.usertable["SJHM"].ToString(), MainBox.sbbm);
                if (dt.Rows.Count > 0 && dt != null)
                {
                    for (int i = 1; i <=MainBox.cllb.Rows.Count; i++)
                    {
                        DataRow dr = MainBox.cllb.NewRow();
                        dr["BH"] = MainBox.cllb.Rows[i]["BH"].ToString();
                        string zt = MainBox.cllb.Rows[i]["ZT"].ToString();
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
                    s_1.DataContext = MainBox.cllb;
                }               
            }
            else
            {
                //获取用户可以打开的柜号                   
                for (int i = 1; i <= MainBox.gzsl; i++)
                {
                    DataRow dr = MainBox.cllb.NewRow();                    
                    dr["BH"] = i.ToString();
                    string zt = "0";
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
                s_1.DataContext = MainBox.cllb;

            }
        }

       



        private void Button_zwlr_TouchUp(object sender, TouchEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog();
        }
       

        /// <summary>
        /// 点击格子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        //private void Grid_b_MouseDown(object sender, MouseButtonEventArgs e)
        //{
        //    //遍历用户控件
        //    var d = sender as Grid;
        //    d.Focus();
        //    UIElementCollection Childrens = d.Children;
        //    foreach (UIElement ui in Childrens)
        //    {
        //        if (((Label)ui).Name == "BH")
        //        {
        //            string dwm = ((Label)ui).Content.ToString();
        //            if (!string.IsNullOrEmpty(dwm))
        //            {
        //                //开启柜门                                          
        //                Xj_Ycsy Ycsy = new Xj_Ycsy(BoxList, dwm);                       
        //                Ycsy.ShowDialog();
        //            }
        //        }
        //    }
        //}


        private void Grid_b_TouchDown(object sender, TouchEventArgs e)
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




        private void s_1_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }

        //页面关闭时执行的方法
        private void Bolist_clos(object sender, EventArgs e)
        {
            MainBox.usertable.Clear();
            MainBox.cllb.Clear();
            MainBox.zwTimer.Start();
            MainBox.RfidTimer.Start();
        }   

        private void btn_clos_TouchUp(object sender, TouchEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog();
        }

       
    }
}
