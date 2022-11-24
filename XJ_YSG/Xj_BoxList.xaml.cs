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

            cllb.Columns.Add("BH");
            cllb.Columns.Add("UriSource");
            cllb.Columns.Add("ZT");
            if (MainBox.isyjkq)
            {
                DataTable dt = Service.getListBox("", MainBox.sbbm);
                //应急开启展示所有柜子                  
                for (int i = 1; i <= MainBox.gzsl; i++)  //按规格显示
                {
                    DataRow dr = cllb.NewRow();
                    dr["BH"] = i.ToString();                  
                    var DataRowArr = dt.Select("WZM = '" + i + "'");
                    if (DataRowArr.Length > 0)
                    {
                        DataRow New_dr = DataRowArr[0];
                        if (Convert.ToInt32(New_dr["ZT"]) == 0)
                        {
                            dr["UriSource"] = "img/Boxlist_zaiku.png";
                            dr["ZT"] = "正常";
                        }
                        else 
                        {
                            dr["UriSource"] = "img/Boxlist_chuche.png";
                            dr["ZT"] = "出车中";
                        }                       
                    }
                    else
                    {
                        dr["ZT"] = "未绑定";
                        dr["UriSource"] = "img/Boxlist_daifenpei.png";
                        
                    }    
                    cllb.Rows.Add(dr);

                }
               
                s_1.DataContext = cllb;
            }
            else
            {              
                DataTable dt = Service.getListBox(MainBox.usertable["SJHM"].ToString(), MainBox.sbbm);
                if (dt.Rows.Count > 0 && dt != null)
                {                 
                    for (int i = 1; i <= cllb.Rows.Count; i++)
                    {
                        DataRow dr = dt.NewRow();
                        dr["BH"] = dt.Rows[i]["WZM"].ToString();
                        string zt = dt.Rows[i]["ZT"].ToString();
                        if (zt == "0" && zt=="1")
                        {
                            dr["UriSource"] = "img/Boxlist_zaiku.png";
                            dr["ZT"] = "正常";
                        }
                        else if (zt == "1")
                        {
                            dr["UriSource"] = "img/Boxlist_chuche.png";
                            dr["ZT"] = "出车中";
                        }
                        cllb.Rows.Add(dr);
                    }
                    s_1.DataContext = cllb;
                }


            }
        }

       



        private void Button_zwlr_TouchUp(object sender, TouchEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog();
        }
       

       
        private void Grid_b_TouchUp(object sender, TouchEventArgs e)
        {
            //遍历用户控件
            var d = sender as Grid;
            d.Focus();
            UIElementCollection Childrens = d.Children;
            string dwm = "";
            string zt = "";
            foreach (UIElement ui in Childrens)
            {
                if (((Label)ui).Name == "lab_BH")
                {
                    dwm = ((Label)ui).Content.ToString();
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
                Xj_Ycsy Ycsy = new Xj_Ycsy(BoxList, dwm);
                Ycsy.ShowDialog();
            }
            if (zt == "未绑定")
            {
                MainBox.Send(dwm);
            }
            if (zt == "出车中")
            {
                speack("该车俩已出车");
            }
        }

        private void s_1_ManipulationBoundaryFeedback(object sender, ManipulationBoundaryFeedbackEventArgs e)
        {
            e.Handled = true;
        }




        //页面关闭时执行的方法  
        private void btn_clos_TouchUp(object sender, TouchEventArgs e)
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
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Xj_Zwlr zwlr = new Xj_Zwlr(BoxList);
            zwlr.ShowDialog();
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
    }
}
