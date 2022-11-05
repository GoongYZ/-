using BLL;
using System;
using System.Windows;
using System.Windows.Controls;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Clpj.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Clpj : Window
    {
        SerialPortUtil port = new SerialPortUtil();
        string gh = "";
       
        public Xj_Clpj(MainBox mainbox, string wzm)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 255;
            gh = wzm;
            this.Loaded += ((s, e) =>
            {
                mainbox.markLayer.Visibility = Visibility.Visible;
            });
            this.Closed += ((s, e) =>
            {                
                UHFService.strEPC = "";
                mainbox.markLayer.Visibility = Visibility.Hidden;
            });
        }

        private void clpj_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void clols_Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }



        private void clzk_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clzk = sender as RadioButton;
            if (clzk.IsChecked == true)
            {
                MainBox.clzk=clzk.Tag.ToString();
            }
        }

        private void clwg_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clwg = sender as RadioButton;
            if (clwg.IsChecked == true)
            {
                MainBox.clwg=clwg.Tag.ToString();
            }
        }

        private void clns_RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            var clns = sender as RadioButton;
            if (clns.IsChecked == true)
            {
                MainBox.clns=clns.Tag.ToString();
            }
        }

       
    }
}
