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
        DataTable cllb = new DataTable();  //车两列表
        public double heig = 0;       
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


        /// <summary>
        /// 绑定列表
        /// </summary>
        private void Bindinfo()
        {
            try
            {
                cllb.Columns.Add("BH");
                cllb.Columns.Add("UriSource");
                cllb.Columns.Add("ZT");
                if (MainBox.islxkq == true)
                {
                    for (int i = 1; i <= MainBox.gzsl; i++)  //按规格显示
                    {
                        DataRow drclib = cllb.NewRow();
                        drclib["BH"] = i.ToString();
                        drclib["UriSource"] = "img/Boxlist_daifenpei.png";
                        drclib["ZT"] = "未联网";
                        cllb.Rows.Add(drclib);
                    }
                    s_1.DataContext = cllb;
                }
                else
                {
                    if (MainBox.isyjkq)
                    {

                        DataTable dt = Service.getListBox("", MainBox.sbbm);
                        //应急开启展示所有柜子                  
                        for (int i = 1; i <= MainBox.gzsl; i++)  //按规格显示
                        {
                            DataRow drclib = cllb.NewRow();
                            drclib["BH"] = i.ToString();
                            var DataRowArr = dt.Select("WZM = '" + i + "'");
                            if (DataRowArr.Length > 0)
                            {
                                DataRow New_dr = DataRowArr[0];
                                if (Convert.ToInt32(New_dr["ZT"]) == 0)
                                {
                                    drclib["UriSource"] = "img/Boxlist_zaiku.png";
                                    drclib["ZT"] = "正常";
                                }
                                else
                                {
                                    drclib["UriSource"] = "img/Boxlist_chuche.png";
                                    drclib["ZT"] = "出车中";
                                }
                            }
                            else
                            {
                                drclib["ZT"] = "未绑定";
                                drclib["UriSource"] = "img/Boxlist_daifenpei.png";

                            }
                            cllb.Rows.Add(drclib);
                        }
                        s_1.DataContext = cllb;
                    }
                    else
                    {
                        DataTable dt = Service.getListBox(MainBox.usertable["SJHM"].ToString(), MainBox.sbbm);
                        if (dt.Rows.Count > 0 && dt != null)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                DataRow drclib = cllb.NewRow();
                                drclib["BH"] = dt.Rows[i]["WZM"].ToString();
                                string zt = dt.Rows[i]["ZT"].ToString();
                                if (zt == "0")
                                {
                                    drclib["UriSource"] = "img/Boxlist_zaiku.png";
                                    drclib["ZT"] = "正常";
                                }
                                else if (zt == "1")
                                {
                                    drclib["UriSource"] = "img/Boxlist_chuche.png";
                                    drclib["ZT"] = "出车中";
                                }
                                cllb.Rows.Add(drclib);
                            }
                            DataTable sortTable = cllb.Clone();

                            DataView dv = cllb.DefaultView;
                            dv.Sort = "BH asc";
                            sortTable = dv.ToTable();
                            s_1.DataContext = sortTable;
                        }
                    }
                }
            }
            catch (Exception ex)
            {              
                cllb.Clear();
            }
           

           
        }
       



        private void Button_zwlr_TouchUp(object sender, TouchEventArgs e)
        {
            if (MainBox.islxkq == false)
            {
                Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
                zwlr.ShowDialog();
            }
            else 
            {
                MainBox.speack("系统无网络，请稍后在试");
            }
            
        }
       

       

        /// <summary>
        /// 打开柜子
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Grid_b_TouchUp(object sender, TouchEventArgs e)
        {
            //遍历用户控件
            var d = sender as Grid;
            d.Focus();
            UIElementCollection Childrens = d.Children;
            string wzm = "";
            string zt = "";
            foreach (UIElement ui in Childrens)
            {
                if (((Label)ui).Name == "lab_BH")
                {
                    wzm = ((Label)ui).Content.ToString();
                }
            }
            foreach (UIElement ui in Childrens)
            {
                if (((Label)ui).Name == "lab_ZT")
                {
                    zt = ((Label)ui).Content.ToString();
                }
            }
            if (zt == "正常")
            {
                //开启柜门
                if (MainBox.islxkq)
                {
                    LockService.Send(wzm);
                    LockService.red_light(wzm, true);
                }
                else                
                {
                    Xj_Ycsy Ycsy = new Xj_Ycsy(BoxList, wzm);
                    Ycsy.ShowDialog();
                }
             
            }
            if (zt == "未绑定")
            {
                LockService.Send(wzm);
            }
            if (zt == "出车中")
            {
                MainBox.speack("该车俩已出车");
            }
        }



        /// <summary>
        /// 防止滑动到最低时窗口抖动
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void s_1_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }



        /// <summary>
        /// 页面关闭时执行的方法  
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_clos_TouchUp(object sender, TouchEventArgs e)
        {
            if (MainBox.islxkq == false)
            {
                if (MainBox.usertable.Count != 0 || MainBox.usertable == null)
                {
                    MainBox.usertable.Clear();
                }
                if (MainBox.isyjkq)
                {
                    MainBox.isyjkq = false;
                }
                MainBox.zwTimer.Start();
                MainBox.RfidTimer.Start();
            }
            else 
            {
                Close();
            }
            
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog();
        }

       

        private void top_page_Click(object sender, RoutedEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild(s_1, 0) as Decorator;
            if (border != null)
            {
                // Get scrollviewer
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                if (scrollViewer != null)
                {
                    heig -= scrollViewer.ScrollableHeight / 3.0;
                    scrollViewer.ScrollToVerticalOffset(heig);
                    if (heig < 0)
                    {
                        heig = 0;
                    }
                }
            }

        }

        private void down_page_Click(object sender, RoutedEventArgs e)
        {
            Decorator border = VisualTreeHelper.GetChild(s_1, 0) as Decorator;
            if (border != null)
            {
                // Get scrollviewer
                ScrollViewer scrollViewer = border.Child as ScrollViewer;
                if (scrollViewer != null)
                {
                    heig += scrollViewer.ScrollableHeight / 3.0;
                    scrollViewer.ScrollToVerticalOffset(heig);
                    if (heig > scrollViewer.ScrollableHeight)
                    {
                        heig = scrollViewer.ScrollableHeight;
                    }
                }
            }
        }
    }
}
