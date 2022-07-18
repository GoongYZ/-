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
    /// Xj_Mima.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Mima : Window
    {
        public Xj_Mima()
        {
            this.Left = 0;
            this.Top = 0;
            InitializeComponent();
        }



        /// <summary>
        /// 输入密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_mima_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            mm.Content += button.Tag.ToString();
        }




        /// <summary>
        /// 删除密码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>

        private void btn_Delete_Click(object sender, RoutedEventArgs e)
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
        private void btn_Save_Click(object sender, RoutedEventArgs e)
        {
            doLogin();
        }


        /// <summary>
        /// 获取钥匙柜信息数据
        /// </summary>
        private void doLogin()
        {

           // Public.PublicMethods.sjhm = "19888925110";
            //Key_BoxList key_BoxList = new Key_BoxList();
           // key_BoxList.Show();
            //this.Close();

            //if (mm.Content.ToString().Length == 6)
            //{
            //    //Hashtable tb = service.getInfoByEwm(mm.Content.ToString());
            //    //if (tb.Count>0)
            //    //{
            //    //    Hashtable outtb = new Hashtable();
            //    //    DataSet ds = null;//service.getGrdjzbList("", out outtb);
            //    //    foreach (DataRow dr in ds.Tables[0].Rows)
            //    //    {
            //    //        string rfid = dr["RFID"].ToString();
            //    //        string zt = dr["ZT"].ToString();
            //    //        string boxid = dr["BOXID"].ToString();
            //    //        string diverid = dr["DIVERID"].ToString();
            //    //        if (rfid.Length > 0)
            //    //        {
            //    //            hashtable_rfid.Add(rfid, (zt == "正常" ? "0" : (zt == "出库中" ? "1" : "2")) + "&" + diverid + "_" + boxid);
            //    //        }
            //    //    }

            //        //柜门开关
            //        //if (elocker != null)
            //        //{
            //        //    isallclose = false;
            //        //    for (int i = 0; i < Box_Count; i++)
            //        //    {
            //        //        for (int j = 1; j < 7; j++)
            //        //        {
            //        //            eloc·ker.openBox(i, j);
            //        //            openBoxList.Add(i.ToString() + "_" + j.ToString());
            //        //        }
            //        //    }
            //        //    closedoorTimer.Start();
            //        //    speack("柜门已打开，取后请关门，谢谢！");
            //        //    message.Content = "柜门已打开，取后请关门，谢谢！";
            //        //    message.Visibility = Visibility.Visible;
            //        //}
            //        //else
            //        //{
            //        //    speack("柜门打开失败，请联系管理员，谢谢！");
            //        //    message.Content = "柜门打开失败，请联系管理员，谢谢！";
            //        //    message.Visibility = Visibility.Visible;
            //        //}
            //    }
            //    else
            //    {
            //        //speack(msg);
            //        //mm.Content = "";
            //        //message.Content = msg;
            //        //message.Visibility = Visibility.Visible;
            //    }
            //}
            //else
            //{
            //    //speack("请输入密码");
            //    //message.Content = "请输入密码!";
            //    //message.Visibility = Visibility.Visible;
            //}
        }


        /// <summary>
        /// 取消退出按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Shut_down_Click(object sender, RoutedEventArgs e)
        {           
            this.Close();
        }




    }
}
