using BLL;
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
        Logo log = new Logo();

        #region  指纹图像数据
        public static byte[] m_pImageBuffer = new byte[640 * 480];
        public static int m_nWidth = 0;
        public static int m_nHeight = 0;
        public static int m_nSize = 640 * 480;
        int nRet = -1;
        #endregion


        public Xj_Zwlr(Xj_BoxList BoxList)
        {
            InitializeComponent();
            this.Left = 134;
            this.Top = 187;
            this.Loaded += ((s, e) => {
                BoxList.markLayer.Visibility = Visibility.Visible;
            });

            this.Closed += ((s, e) =>
            {               
                BoxList.markLayer.Visibility = Visibility.Hidden;
            });
            if (ParameterModel.m_hDevice != IntPtr.Zero)
            {
                CanShow();
            }
            else
            {
                fingerprint.ZW_Connection();
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
            nRet = ParameterModel.ZKFPModule_GetFingerImage(ParameterModel.m_hDevice, ref m_nWidth, ref m_nHeight, m_pImageBuffer, ref m_nSize);
            if (nRet == 0)
            {
                MemoryStream ms = new MemoryStream();
                BitmapFormat.GetBitmap(m_pImageBuffer, m_nWidth, m_nHeight, ref ms);
                if (ms != null)
                {
                    Bitmap bmp = new Bitmap(ms);
                    IntPtr hBitmap = bmp.GetHbitmap();
                    this.pictureBox_FingerImg.Source = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(hBitmap, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());                  
                }
            }

        }
        #endregion


        /// <summary>
        //用户注册按钮
        /// </summary>     
        private void Button_lr_TouchUp(object sender, TouchEventArgs e)
        {
            //这里需要通过人脸识别或密码开启后获取的手机号码查询出id。
            int userid = Convert.ToInt32( MainBox.usertable["ZWID"]);
            if (userid != 0)
            {
                // 登记用户模板(设备句柄，用户id)             
                int nRet = ParameterModel.ZKFPModule_EnrollTemplateByImage(ParameterModel.m_hDevice, userid, m_pImageBuffer, m_nSize);
                if (0 == nRet)
                {
                    speack("录入成功");
                    log.WriteLogo("录入成功USERID="+ userid, 5);
                    disTimer.Stop();
                    Close();
                }
                else
                {
                    string erro = fingerprint.Erroneous(nRet.ToString());
                    log.WriteLogo("录入失败!" + "错误原因:" + erro, 5);
                }
            }
        }

        private void speack(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpeechVoice.speack(text);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }

        private void Button_Clos_TouchUp(object sender, TouchEventArgs e)
        {
            disTimer.Stop();
            Close();
        }

        
    }
}
