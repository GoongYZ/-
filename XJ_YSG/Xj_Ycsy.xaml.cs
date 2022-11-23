using BLL;
using Data;
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
    /// Xj_Ycsy.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Ycsy : Window
    {
        Interaction_WebService Service = new Interaction_WebService();
        string gh = "";
        public Xj_Ycsy(Xj_BoxList BoxList,string dwm)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 172;
            if (dwm != "")            
            {
                gh = dwm;
            }
            this.Loaded += ((s, e) => {
               BoxList.markLayer.Visibility = Visibility.Visible;
            });

            this.Closed += ((s, e) =>
            {                   
                BoxList.markLayer.Visibility = Visibility.Hidden;
            });
            Bindate();
            yclx_cc.Checked += yclx_cc_Checked;
            yclx_rcwb.Checked += yclx_rcwb_Checked;
        }
        private void yclx_cc_Checked(object sender, RoutedEventArgs e)
        {
            if (yclx_rcwb.IsChecked==true) 
            {
                yclx_rcwb.IsChecked = false;
            }
           
        }
        private void yclx_rcwb_Checked(object sender, RoutedEventArgs e)
        {
            if (yclx_cc.IsChecked == true)
            {
                yclx_cc.IsChecked = false;
            }

        }

        public void Bindate() 
        {
            //用车
            List<YcsqModel> listycsy = new List<YcsqModel>();
            listycsy.Add(new YcsqModel { ID = "68ab5354-e866-483f-911e-240e0be4f4ce", Name = "执法执勤工作" });
            listycsy.Add(new YcsqModel { ID = "89a65287-baee-4466-aa2a-90c47429f8f1", Name = "警务督察、警务工作明查暗访" });
            listycsy.Add(new YcsqModel { ID = "e07d232f-fc1f-4818-ac9a-3765198a4797", Name = "处置涉警信访、舆情" });
            listycsy.Add(new YcsqModel { ID = "aea87fef-5599-4ccc-a3d4-7c7f2901f2ca", Name = "侦查、办案工作" });
            listycsy.Add(new YcsqModel { ID = "86e093ee-1e77-468e-a2c9-37bbaba7eafc", Name = "突发事件处置等工作" });
            listycsy.Add(new YcsqModel { ID = "ade49877-01d4-47bc-9a68-6d1c4cdff97c", Name = "安保、警卫工作" });
            listycsy.Add(new YcsqModel { ID = "1ba7a134-2e6d-4d95-86d7-baeed2330585", Name = "车辆修理" });
            listycsy.Add(new YcsqModel { ID = "c11d9d27-d99a-4ccd-99e3-c809c727982e  ", Name = "执法执纪检查和调研" });
            listycsy.Add(new YcsqModel { ID = "15b303a6-179c-4770-a9c9-f1fe4d9c875c", Name = "大型活动" });
            ycsy.ItemsSource = listycsy;
            ycsy.SelectedValuePath = "ID";//设置选择属性
            ycsy.DisplayMemberPath = "Name";//设置显示属性
            ycsy.SelectedIndex = 0;
            List<YcsqModel> listmdds = new List<YcsqModel>();
            listmdds.Add(new YcsqModel { ID = "85f89b0e-299f-43a0-a6fe-6d398ff7d6f8", Name = "县内" });
            listmdds.Add(new YcsqModel { ID = "a54ff4e5-2af9-47c5-baa8-1fe6e98710a0", Name = "杭嘉湖" });
            listmdds.Add(new YcsqModel { ID = "816b583e-fc07-4a67-8699-c29442b83464", Name = "其他" });
            mdds.ItemsSource = listmdds;
            mdds.SelectedValuePath = "ID";//设置选择属性
            mdds.DisplayMemberPath = "Name";//设置显示属性
            mdds.SelectedIndex = 0;
            List<YcsqModel> listyjghsj = new List<YcsqModel>();
            listyjghsj.Add(new YcsqModel { ID = "2", Name = "2小时" });
            listyjghsj.Add(new YcsqModel { ID = "4", Name = "4小时" });
            listyjghsj.Add(new YcsqModel { ID = "8", Name = "8小时" });
            listyjghsj.Add(new YcsqModel { ID = "12", Name = "12小时" });
            listyjghsj.Add(new YcsqModel { ID = "24", Name = "24小时" });
            listyjghsj.Add(new YcsqModel { ID = "48", Name = "48小时" });          
            yjghsj.ItemsSource = listyjghsj;
            yjghsj.SelectedValuePath = "ID";//设置选择属性
            yjghsj.DisplayMemberPath = "Name";//设置显示属性
            yjghsj.SelectedIndex = 0;
        }

        private void mdds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            string id = mdds.SelectedValue.ToString();
            switch (id)
            {
                case "85f89b0e-299f-43a0-a6fe-6d398ff7d6f8":
                    List<YcsqModel> listXN = new List<YcsqModel>();
                    listXN.Add(new YcsqModel { ID = "c2efe206-cdcd-4b61-8968-382a446990e1", Name = "武康" });
                    listXN.Add(new YcsqModel { ID = "8d4cd18a-f2cc-4b56-bcab-3f938009fd20", Name = "乾元" });
                    listXN.Add(new YcsqModel { ID = "cd7ff6c6-ad1c-4e60-a373-393bc943a68d", Name = "新市" });
                    listXN.Add(new YcsqModel { ID = "eccf9a46-96a8-46aa-a29e-00c1b45f1bd3", Name = "阜溪" });
                    listXN.Add(new YcsqModel { ID = "b7a6c8e2-724c-4aa9-a6d1-0389f202a989", Name = "钟管" });
                    listXN.Add(new YcsqModel { ID = "07562462-6479-4e52-8bcf-77974d8e0b08", Name = "洛舍" });
                    listXN.Add(new YcsqModel { ID = "e6e05404-d3aa-40be-b3f4-51af9b0b8edd", Name = "雷甸" });
                    listXN.Add(new YcsqModel { ID = "56a40b8b-b36e-434d-b8f6-4091adfae431", Name = "禹越" });
                    listXN.Add(new YcsqModel { ID = "7b8381f0-40ea-41d9-b959-505904335a25", Name = "新安" });
                    listXN.Add(new YcsqModel { ID = "8dc7b6e0-c8d9-4040-80cd-a5904e320d57", Name = "下渚湖" });
                    listXN.Add(new YcsqModel { ID = "d7715544-1fea-4a17-be3f-0c648ff685ae", Name = "莫干" });
                    listXN.Add(new YcsqModel { ID = "63847826-45a1-4011-93dd-acc7c6c11163", Name = "舞阳" });
                    listXN.Add(new YcsqModel { ID = "8e772a13-0732-46db-a7df-4c8982ec4466", Name = "康乾" });
                    listXN.Add(new YcsqModel { ID = "5d62fdf5-087f-4856-884d-73a46082fd20", Name = "莫干山" });
                    mddx.ItemsSource = listXN;
                    mddx.SelectedValuePath = "ID";//设置选择属性
                    mddx.DisplayMemberPath = "Name";//设置显示属性
                    mddx.SelectedIndex = 0;
                    break;
                case "a54ff4e5-2af9-47c5-baa8-1fe6e98710a0":
                    List<YcsqModel> listHJH = new List<YcsqModel>();
                    listHJH.Add(new YcsqModel { ID = "48923feb-dfa9-4092-b444-3791d3392353", Name = "杭州" });
                    listHJH.Add(new YcsqModel { ID = "11cf414d-a428-4ec4-ae34-4b0cf1b41ac5", Name = "嘉兴" });
                    listHJH.Add(new YcsqModel { ID = "dc4aa2bb-fdc3-4f47-8000-4efc30969f9d", Name = "湖州" });
                    mddx.ItemsSource = listHJH;
                    mddx.SelectedValuePath = "ID";//设置选择属性
                    mddx.DisplayMemberPath = "Name";//设置显示属性
                    mddx.SelectedIndex = 0;
                    break;
                case "816b583e-fc07-4a67-8699-c29442b83464":
                    List<YcsqModel> listQT = new List<YcsqModel>();
                    listQT.Add(new YcsqModel { ID = "8b2d2929-c190-4a62-a7ff-2cea57a6135a", Name = "宁波" });
                    listQT.Add(new YcsqModel { ID = "d110d3da-4a0e-497a-a869-19875824d5ac", Name = "温州" });
                    listQT.Add(new YcsqModel { ID = "7ad47637-8613-450e-86a6-772a3cea1420", Name = "绍兴" });
                    listQT.Add(new YcsqModel { ID = "c6a2d084-71dc-43b7-9973-c33f18a2617c", Name = "金华" });
                    listQT.Add(new YcsqModel { ID = "684564bc-24fb-4196-8942-5be4c491c0dd", Name = "衢州" });
                    listQT.Add(new YcsqModel { ID = "728b382b-facd-44e1-b641-0eaa929df20b", Name = "台州" });
                    listQT.Add(new YcsqModel { ID = "9afe6e3b-e621-46b5-a882-da7a0cb5f3e6", Name = "丽水" });
                    listQT.Add(new YcsqModel { ID = "1227f988-fdf7-4b96-8988-91aea13ba690", Name = "舟山" });
                    listQT.Add(new YcsqModel { ID = "400ba1d5-4c22-4f58-a49c-66e5226a006b", Name = "省外" });
                    mddx.ItemsSource = listQT;
                    mddx.SelectedValuePath = "ID";//设置选择属性
                    mddx.DisplayMemberPath = "Name";//设置显示属性
                    mddx.SelectedIndex = 0;
                    break;
                default:
                    break;
            }

        }

       

        private void open_Button_TouchUp(object sender, TouchEventArgs e)
        {        
            if (gh != "")
            {
                if (yclx_cc.IsChecked==true)
                {                  
                    MainBox.ycsqdinfo.Add("yclxpk", yclx_cc.Tag.ToString());
                }
                if (yclx_rcwb.IsChecked == true) 
                {                    
                    MainBox.ycsqdinfo.Add("yclxpk", yclx_rcwb.Tag.ToString());
                }                           
                MainBox.ycsqdinfo.Add("ycsypk", ycsy.SelectedValue.ToString());
                MainBox.ycsqdinfo.Add("mddspk", mdds.SelectedValue.ToString());
                MainBox.ycsqdinfo.Add("mddxpk", mddx.SelectedValue.ToString());
                MainBox.ycsqdinfo.Add("yjghsjpk", yjghsj.SelectedValue.ToString());
                if (MainBox.ycsqdinfo != null)
                {
                    MainBox.Send(gh);
                    MainBox.red_light(gh, true);
                    speack("柜门已打开,取后请关门");
                    MainBox.locklis.Add(gh + "_qtqys");
                    Close();
                }                             
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (gh != "")
            {
                string yclxpk = "";
                if (yclx_cc.IsChecked == true)
                {
                    yclxpk = yclx_cc.Tag.ToString();
                }
                if (yclx_rcwb.IsChecked == true)
                {
                    yclxpk = yclx_rcwb.Tag.ToString();
                }
                string ycsypk = ycsy.SelectedValue.ToString();
                string mddspk = mdds.SelectedValue.ToString();
                string mddxpk = mddx.SelectedValue.ToString();
                string yjghsjpk = yjghsj.SelectedValue.ToString();
                bool sesscc = Service.saveYcsq(MainBox.sbbm, gh, MainBox.usertable["PK"].ToString(), yclxpk, ycsypk, mddspk, mddxpk, yjghsjpk);
                if (sesscc)
                {
                    MainBox.Send(gh);
                    MainBox.red_light(gh, false);
                    speack("柜门已打开");
                    MainBox.locklis.Add(gh + "_rlzwqys");
                    Close();
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

        private void btn_tc_TouchUp(object sender, TouchEventArgs e)
        {
            this.Close();
        }

        
    }
}
