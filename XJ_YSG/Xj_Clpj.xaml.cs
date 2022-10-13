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
    /// Xj_Clpj.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Clpj : Window
    {
        public Xj_Clpj()
        {
            InitializeComponent();
        }

        private void clpj_Button_Click(object sender, RoutedEventArgs e)
        {
            LockControl lockControl = new LockControl();
            lockControl.Send("1");
            //打开柜门
        }
    }
}
