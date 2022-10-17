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
        public Xj_Ycsy(Xj_BoxList BoxList)
        {
            InitializeComponent();
            this.Left = 104;
            this.Top = 172;           
            this.Loaded += ((s, e) => {
                BoxList.markLayer.Visibility = Visibility.Visible;
            });

            this.Closed += ((s, e) =>
            {
                BoxList.markLayer.Visibility = Visibility.Hidden;
            });
        }      
    }
}
