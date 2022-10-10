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

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Ycsy.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Ycsy : Window
    {

        //柜号
        string gh = "";
        LockControl Lock = new LockControl();
        public Xj_Ycsy(string dwm )
        {
            InitializeComponent();
            gh = dwm;
            this.Left = 114;
            this.Top = 308;
        }


        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void btn_save_Click(object sender, RoutedEventArgs e)
        {
            Lock.Open();
            Lock.Send(gh);
            Lock.red_light(gh, false);
            Lock.Close();
            SpeechVoice.speack("柜门已打开，取后请关门");
            Close();
        }

    }
}
