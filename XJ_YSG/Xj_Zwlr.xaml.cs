using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
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
using System.Windows.Threading;
using XJ_YSG;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Zwlr.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Zwlr : Window
    {
        Fingerprint fingerprint = new Fingerprint();
        public Xj_Zwlr()
        {
            InitializeComponent();
            if (ParameterModel.islj == true) 
            {
                CanShow();
            }
        }



        #region 实时绑定指纹
        DispatcherTimer disTimer = new DispatcherTimer();
        private void CanShow()
        {
            disTimer.Interval = new TimeSpan(0, 0, 0, 0, 100); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            disTimer.Tick += new EventHandler(disTimer_Tick_canShow); //每一秒执行的方法
            disTimer.Start();
        }
        void disTimer_Tick_canShow(object sender, EventArgs e)
        {
            //每秒中向绑定一次指纹图片 ,实时采集图像，并显示       
            int nRet = ParameterModel.ZKFPModule_GetFingerImage(ParameterModel.m_hDevice, ref ParameterModel.m_nWidth, ref ParameterModel.m_nHeight, ParameterModel.m_pImageBuffer, ref ParameterModel.m_nSize);
            if (nRet == 0)
            {
                MemoryStream ms = new MemoryStream();
                BitmapFormat.GetBitmap(ParameterModel.m_pImageBuffer, ParameterModel.m_nWidth, ParameterModel.m_nHeight, ref ms);
                if (ms != null)
                {
                    Bitmap bmp = new Bitmap(ms);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    this.pictureBox_FingerImg.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                    disTimer.Stop();
                }
            }
        }
        #endregion


        /// <summary>
        //用户注册按钮
        /// </summary>
        private void Button_lr_Click(object sender, RoutedEventArgs e)
        {
            //这里需要通过人脸识别或密码开启后获取的手机号码查询出id。
            int userid = 0;
            if (userid != 0)
            {
                fingerprint.Addzw(userid);
            }
        }

    }
}
