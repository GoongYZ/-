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
        LockControl lockControl = new LockControl();
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
                Xj_BoxList.gh = "";               
                BoxList.markLayer.Visibility = Visibility.Hidden;
            });
        }

        private void open_Button_Click(object sender, RoutedEventArgs e)
        {
            if (gh != "") 
            {
                if (MainBox.port.IsOpen)
                {
                    lockControl.Send(gh);
                    lockControl.White_light(gh, false);
                    MainBox.locklis.Add(gh);
                    this.Close();
                }
            }
        }




     


        private void btn_tc_TouchLeave(object sender, TouchEventArgs e)
        {
            Xj_BoxList.gh = "";
            this.Close();
        }
    }
}
