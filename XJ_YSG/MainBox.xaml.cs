using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using ArcFaceSDK.Utils;
using ArcSoftFace.Utils;
using FaceBox.TheActivation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;
using BLL;
using System.Drawing;
using XJ_YSG;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections;

namespace XJ_YSG
{
    /// <summary>
    /// MainBox.xaml 的交互逻辑
    /// </summary>


    public partial class MainBox : Window
    {
        Activation activation = new Activation();
        Fingerprint fingerprint = new Fingerprint();
        Logo log = new Logo();
        LockControl LockControl = new LockControl();
        public static SerialPortUtil port = new SerialPortUtil();
        private MainBox mainbox = null;
        public static string sbbm = ServerBase.XMLRead("Ysg_sbbm", "sbbm").ToString(); //设备编码
        public static DispatcherTimer zwTimer = new DispatcherTimer();//指纹计时器
        public static DispatcherTimer RfidTimer = new DispatcherTimer();//刷卡计时器
        public static DispatcherTimer ClossTimer = new DispatcherTimer();  //监控开门状态倒计时
        Interaction_WebService Service = new Interaction_WebService();
        public static List<string> locklis = new List<string>();// 监控箱门的数据集合1_0&1_1,柜号_取或还
        public static string HycsqdPK = "";  //还钥匙保存用车申请单pk
        public static string QycsqdPK = "";  //取钥匙保存用车申请单pk
        public static Hashtable hashtable = new Hashtable();  //车辆评价
        #region  指纹图像数据
        public byte[] m_pImageBuffer = new byte[640 * 480];
        public int m_nWidth = 0;
        public int m_nHeight = 0;
        public int m_nSize = 640 * 480;
        #endregion
        #region 用户信息和钥匙柜列表
        List<string> userlist = new List<string>();
        Dictionary<int, List<string>> gh = new Dictionary<int, List<string>>();
        #endregion


        public MainBox()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/main.png", UriKind.Absolute));
            this.Background = b3;
            if (mainbox == null)
            {
                mainbox = this;
            }
            UHFService.ConnectCOM();   //刷卡
            UHF2Service.ConnectCOM();   //读钥匙
            port.OpenPort();  //连接锁

            
            if (activation.InitEngines() == "1")
            {
                ChooseMultiImg();  //激活人脸识别
            }
            if (fingerprint.ZW_Connection() == "ok")
            {
                zwthan();   //开始指纹验证
            }
            LockDjs();
            Rfidthan();   //实时RFID读信息卡          
            Csh_yskp(); //初始化钥匙柜卡片
            this.Closed += MainWindow_Closed;
        }
        #region 输入钥匙码按钮
        private void Smkey_Click(object sender, RoutedEventArgs e)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            Xj_Mima xj_Mima = new Xj_Mima();
            xj_Mima.ShowDialog();
        }
        #endregion



        #region 开启人脸识别按钮
        private void Facekey_Click(object sender, RoutedEventArgs e)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            Xj_Rlsb xj_Rlsb = new Xj_Rlsb();
            xj_Rlsb.ShowDialog();
        }
        #endregion



        #region 应急开启按钮
        private void Emergency_open_Click(object sender, RoutedEventArgs e)
        {

        }
        #endregion



        #region 指纹识别
        /// <summary>
        /// 指纹识别倒计时
        /// </summary>
        public void zwthan()
        {
            zwTimer.Interval = new TimeSpan(0, 0, 0, 0, 1000); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            zwTimer.Tick += new EventHandler(disTimer_Tick_canShow); //每一秒执行的方法
            zwTimer.Start();
        }


        void disTimer_Tick_canShow(object sender, EventArgs e)
        {
            int UserID = 0;
            int Index = 0;
            int nRet = -1;
            //图像数据
            nRet = ParameterModel.ZKFPModule_GetFingerImage(ParameterModel.m_hDevice, ref m_nWidth, ref m_nHeight, m_pImageBuffer, ref m_nSize);
            if (nRet == 0)
            {
                //根据图像数据进行比对
                nRet = ParameterModel.ZKFPModule_IdentifyByImage(ParameterModel.m_hDevice, m_pImageBuffer, m_nSize, ref UserID, ref Index);
                if (nRet == 0)
                {
                    zwTimer.Stop();
                    RfidTimer.Stop();
                    log.WriteLogo("指纹比对成功\r\n" + "id:" + UserID + "\r\n" + "index:" + Index, 5);
                    Xj_BoxList boxList = new Xj_BoxList();
                    boxList.Show();
                }
                else
                {
                    string erro = fingerprint.Erroneous(nRet.ToString());
                    log.WriteLogo("错误原因:" + erro, 5);
                }
            }
        }
        #endregion


        #region 语音播报
        public void speack(string text)
        {
            Dispatcher.BeginInvoke(new Action(() =>
            {
                SpeechVoice.speack(text);
            }), System.Windows.Threading.DispatcherPriority.Normal);
        }
        #endregion


        #region 人脸库注册
        /// <summary>
        /// 人脸库注册
        /// </summary>
        private void ChooseMultiImg()
        {
            try
            {
                lock (EntityModel.chooseImgLocker)
                {

                    string DVRSynchro = "D:\\ixjkj\\synchro\\";
                    if (!Directory.Exists(DVRSynchro))
                    {
                        Directory.CreateDirectory(DVRSynchro);
                    }
                    DirectoryInfo Synchro = new DirectoryInfo(@DVRSynchro);
                    string[] files = Directory.GetFiles(DVRSynchro, ".", SearchOption.AllDirectories);
                    List<string> imagePathListTemp = new List<string>();
                    var numStart = EntityModel.imagePathList.Count;
                    int isGoodImage = 0;
                    //人脸检测以及提取人脸特征
                    ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
                    {
                        foreach (string path in Directory.GetFiles(DVRSynchro))
                        {
                            //校验图片
                            if (CheckImage(path))
                            {
                                imagePathListTemp.Add(path);
                            }
                        }
                        //人脸检测和剪裁  遍历每张图片
                        for (int i = 0; i < imagePathListTemp.Count; i++)
                        {
                            Image image = ImageUtil.ReadFromFile(imagePathListTemp[i]);
                            //校验图片宽高
                            CheckImageWidthAndHeight(ref image);
                            if (image == null)
                            {
                                continue;
                            }
                            //调整图像宽度，需要宽度为4的倍数
                            if (image.Width % 4 != 0)
                            {
                                image = ImageUtil.ScaleImage(image, image.Width - (image.Width % 4), image.Height);
                            }
                            //提取特征判断
                            string featureResult = string.Empty;
                            bool isMask;
                            int retCode;
                            SingleFaceInfo singleFaceInfo = new SingleFaceInfo();
                            FaceFeature feature = FaceUtil.ExtractFeature(EntityModel.imageEngine, image, EntityModel.thresholdImgRegister, EntityModel.thresholdImgMask, ASF_RegisterOrNot.ASF_REGISTER, out singleFaceInfo, out isMask, ref featureResult, out retCode);
                            if (!string.IsNullOrEmpty(featureResult))
                            {
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    //MessageBox.Show(featureResult);
                                    log.WriteLogo(featureResult + image, 3);
                                }));
                                if (image != null)
                                {
                                    image.Dispose();


                                }
                                continue;
                            }
                            //人脸检测
                            MultiFaceInfo multiFaceInfo;
                            retCode = EntityModel.imageEngine.ASFDetectFacesEx(image, out multiFaceInfo);
                            //判断检测结果
                            if (retCode == 0 && multiFaceInfo.faceNum > 0)
                            {
                                //多人脸时，默认裁剪第一个人脸
                                EntityModel.imagePathList.Add(imagePathListTemp[i]);
                                MRECT rect = multiFaceInfo.faceRects[0];
                                image = ImageUtil.CutImage(image, rect.left, rect.top, rect.right, rect.bottom);
                            }
                            else
                            {
                                this.Dispatcher.Invoke(new Action(delegate
                                {
                                    log.WriteLogo("未检测到人脸", 3);
                                }));
                                if (image != null)
                                {
                                    image.Dispose();
                                }
                                continue;
                            }
                            //显示人脸
                            this.Dispatcher.Invoke(new Action(delegate
                            {
                                if (image == null)
                                {
                                    image = ImageUtil.ReadFromFile(imagePathListTemp[i]);
                                    //校验图片宽高
                                    CheckImageWidthAndHeight(ref image);
                                }
                                EntityModel.imageLists.Add((numStart + isGoodImage).ToString(), imagePathListTemp[i]);
                                log.WriteLogo(string.Format("已提取{0}号人脸特征值，[left:{1},right:{2},top:{3},bottom:{4},orient:{5},mask:{6}] ,PATH={7}", (numStart + isGoodImage), singleFaceInfo.faceRect.left, singleFaceInfo.faceRect.right, singleFaceInfo.faceRect.top, singleFaceInfo.faceRect.bottom, singleFaceInfo.faceOrient, isMask ? "mask" : "no mask", imagePathListTemp[i].ToString()), 3);
                                EntityModel.leftImageFeatureList.Add(feature);
                                isGoodImage++;
                                if (image != null)
                                {
                                    image.Dispose();
                                }
                                if (isGoodImage == imagePathListTemp.Count)
                                {
                                    //进行语音播报 "人脸识别初始化成功"
                                    speack("人脸识别初始化成功");
                                }
                            }));
                        }
                    }));
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }



        /// <summary>
        /// 校验图片
        /// </summary>
        /// <param name="imagePath"></param>
        /// <returns></returns>
        private bool CheckImage(string imagePath)
        {
            try
            {
                if (imagePath == null)
                {
                    log.WriteLogo("图片不存在，请确认后再导入", 3);
                    return false;
                }
                try
                {
                    //判断图片是否正常，如将其他文件把后缀改为.jpg，这样就会报错
                    Image image = ImageUtil.ReadFromFile(imagePath);
                    if (image == null)
                    {
                        throw new ArgumentException(" image is null");
                    }
                    else
                    {
                        image.Dispose();
                    }
                }
                catch
                {
                    log.WriteLogo(string.Format("{0} 图片格式有问题，请确认后再导入", imagePath), 3);
                    return false;
                }
                FileInfo fileCheck = new FileInfo(imagePath);
                if (!fileCheck.Exists)
                {
                    log.WriteLogo(string.Format("{0} 不存在", fileCheck.Name), 3);
                    return false;
                }
                else if (fileCheck.Length > EntityModel.maxSize)
                {
                    log.WriteLogo(string.Format("{0} 图片大小超过2M，请压缩后再导入", fileCheck.Name), 3);
                    return false;
                }
                else if (fileCheck.Length < 2)
                {
                    log.WriteLogo(string.Format("{0} 图像质量太小，请重新选择", fileCheck.Name), 3);
                    return false;
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
            return true;
        }


        /// <summary>
        /// 检查图片宽高
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        private void CheckImageWidthAndHeight(ref Image image)
        {
            if (image == null)
            {
                return;
            }
            try
            {
                if (image.Width > EntityModel.maxWidth || image.Height > EntityModel.maxHeight)
                {
                    image = ImageUtil.ScaleImage(image, EntityModel.maxWidth, EntityModel.maxHeight);
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }


        #endregion



        #region 刷卡还钥匙
        public void Rfidthan()
        {
            RfidTimer.Interval = new TimeSpan(0, 0, 0, 0, 500); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            RfidTimer.Tick += new EventHandler(RfidTimer_Tick_canShow); //每一秒执行的方法
            RfidTimer.Start();
        }
        void RfidTimer_Tick_canShow(object sender, EventArgs e)
        {
            UHFService.OneCheckInvnetoryWhile(0);
            string st = UHFService.strEPC;
            if (st != "")
            {
                Getycsqdpk(st);
            }
        }

        /// <summary>
        /// 根据钥匙编号获取用车申请单信息
        /// </summary>
        public void Getycsqdpk(string kp)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            //读取txt文件;
            string txt = log.GetYsgGh();
            txt = txt.Substring(0, txt.Length - 1);
            var wzm = txt.Split(',').Where(t => t.Contains(kp)).FirstOrDefault().ToString().Split('_')[0];
            //调用用车申请单接口
            var hash = Service.getycsqdpkInfo(sbbm, wzm);
            if (hash != null)
            {
                string PK = hash["PK"].ToString(); //用车申请单pk
                HycsqdPK = PK;               //保存用车申请单pk
                if (hash["SFPL"].ToString() == "1") //是否评论
                {
                    Xj_Clpj xj_Clpj = new Xj_Clpj(mainbox, wzm);
                    xj_Clpj.ShowDialog();

                }
                else
                {
                    Send(wzm);  //开门
                    speack("柜门已打开");
                    locklis.Add(wzm + "_0"); //添加到箱门状态集合中  
                    UHFService.strEPC = ""; //清楚刷卡信息，下次刷卡使用
                }
            }
            else
            {
                zwTimer.Start();
                RfidTimer.Start();
            }
        }
        #endregion



        #region 监控箱门状态
        /// <summary>
        /// 监控开门状态倒计时
        /// </summary>
        public void LockDjs()
        {
            ClossTimer.Interval = new TimeSpan(0, 0, 0, 0, 2000); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            ClossTimer.Tick += new EventHandler(ClossTimer_Tick_canShow); //每一秒执行的方法
            ClossTimer.Start();
        }
        void ClossTimer_Tick_canShow(object sender, EventArgs e)
        {
            if (locklis.Count > 0) //判断是否有柜门打开
            {
                Closedoor();
            }
        }

        /// <summary>
        /// 关门操作
        /// </summary>
        public  void Closedoor()
        {          
            foreach (string item in locklis)
            {
                string wzm = item.Split('_')[0];
                string isqh = item.Split('_')[1];
                string lockzt = LockControl.State_lock(wzm);
                string zt = lockzt.Substring(19, 1);
                //0关门
                if (zt == "0")
                {
                    MessageBox.Show("关门了");
                    if (isqh == "0")  //0 刷卡还钥匙   
                    {
                        UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签  
                        string yskp = string.Join(",", UHF2Service.strEPC.Split(',').Distinct().ToArray());
                        if (yskp != "")   //卡片是否读到 
                        {
                            if (hashtable != null||hashtable.Count>0)
                            {
                                Service.saveHys(sbbm, HycsqdPK, hashtable["clzk"].ToString(), hashtable["clwg"].ToString(), hashtable["clns"].ToString());      //调还钥匙接口
                            }
                            else
                            {
                                Service.saveHys(sbbm, HycsqdPK, "0", "0", "0");      //调还钥匙接口
                            }
                            locklis.Remove(item);   //归还成功就不需要对该柜号进行监控      
                            UHF2Service.strEPC = ""; //归还成功待下次还钥匙用                                                             
                            HycsqdPK = ""; //清除数据,待下次还钥匙用
                            hashtable.Clear();//归还成功后清除车辆评价数据
                        }
                    }
                    else if (isqh == "1") //1 验证码取钥匙
                    {
                        MessageBox.Show("我钥匙取走了");
                        UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签                          
                        if (UHF2Service.strEPC == "")   //卡片是否读到 
                        {
                           bool  issuccec=  Service.saveQys(sbbm, QycsqdPK);
                            if (issuccec)
                            {
                                locklis.Remove(item);   //取走就不需要对该柜号进行监控
                                QycsqdPK = ""; //清除数据,待下次取钥匙用
                                UHF2Service.strEPC = ""; //取走成功下次取钥匙用    
                            }                                             
                        }
                    }
                    else //2人脸或指纹取钥匙
                    {
                        UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签  
                        if (UHF2Service.strEPC == "")   //卡片是否读到 
                        {
                           Service.saveYcsq();     //保存用车申请单                         
                            locklis.Remove(item);   //取走就不需要对该柜号进行监控      
                            UHF2Service.strEPC = ""; //取走或归还成功
                        }
                    }
                }
            }
        }


        #endregion


        #region 初始化钥匙柜卡片
        public void Csh_yskp()
        {
            Task.Factory.StartNew(() =>
            {
                string gh_kp = "";  //柜号_卡片  
                int gzsl = Convert.ToInt32(ServerBase.XMLRead("Count", "Ysg_gzsl"));  //钥匙柜格子数量
                for (int i = 1; i <= gzsl; i++)
                {
                    UHF2Service.OneCheckInvnetoryWhile(i - 1);
                    string yskp = UHF2Service.strEPC;
                    if (yskp != "")
                    {
                        gh_kp += "" + i + "_" + yskp + "," + "\r";
                        UHF2Service.strEPC = "";
                    }
                }
                Logo.sWriteYsgh(gh_kp);
                speack("智能钥匙管理柜初始化成功");
            });
        }
        #endregion


       

        #region 上报钥匙柜运行状态


        #endregion

        private void Emergency_clos_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {
            Environment.Exit(0);
        }


        #region  锁

        /// <summary>
        /// 发送命令打开柜门
        /// </summary>
        /// <param name="gzh">格子号</param>
        public static void Send(string gzh)
        {
            int gz = Convert.ToInt32(gzh);
            string rtn = "";
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x05;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (94 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
            port.WriteData(data, ref rtn);
            bw = rtn;
        }


        /// <summary>
        /// 发送命令获取柜门状态
        /// </summary>
        /// <param name="gzh">格子号</param>
        public static string State_lock(string gzh)
        {
            int gz = Convert.ToInt32(gzh);
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x05;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (94 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string rut = "";
           port.WriteData(data, ref rut);
            return rut;
        }


        //灭灯
        public static void Destroy(string gzh)
        {
            //string str1 = "A6 A8 01 00 00 0A 00 01 01 00 00 5B";
            //port.WriteData_HEX(str1);
            //获取格子号
            int gz = Convert.ToInt32(gzh);
            string rtn = "";
            byte[] data = new byte[12];
            data[0] = 0xA6;
            data[1] = 0xA8;
            data[2] = 0x01;
            data[3] = 0x00;
            data[4] = 0x00;
            data[5] = 0x0A;
            data[6] = 0x00;
            data[7] = 0x01;
            data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
            data[9] = 0x00;
            data[10] = 0x00;
            string gzbs = (90 + gz).ToString("X2");
            data[11] = Convert.ToByte(gzbs, 16);
            string bw = port.ByteArrayToHexString2(data);
            Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
           port.WriteData(data, ref rtn);
            bw = rtn;
        }

        /// <summary>
        /// 红灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>

        public static void red_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 01 00 5C=92";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x01;
                data[10] = 0x00;
                string gzbs = (91 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
               port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 81 00 DC";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x81;
                data[10] = 0x00;
                string gzbs = (219 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw =port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
               port.WriteData(data, ref rtn);
                bw = rtn;
            }

        }


        /// <summary>
        /// 绿灯
        /// </summary>
        /// <param name="gzh">格子号</param>
        /// <param name="Flashing">是否闪烁</param>
        public static void green_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01
                //
                //00 00 0A 00
                //01 01 04 00
                //5F";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;

                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x04;
                data[10] = 0x00;
                string gzbs = (94 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
               port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 84 00 DF";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x84;
                data[10] = 0x00;
                string gzbs = (222 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
               port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }


        /// <summary>
        /// 蓝灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public static void Blue_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 02 00 5D"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x02;
                data[10] = 0x00;
                string gzbs = (92 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw =port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 82 00 DD";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x82;
                data[10] = 0x00;
                string gzbs = (221 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw =port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }


        /// <summary>
        /// 白灯
        /// </summary>
        /// <param name="gzh"></param>
        /// <param name="Flashing"></param>
        public static void White_light(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 07 00 62"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x07;
                data[10] = 0x00;
                string gzbs = (97 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
               port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 87 00 E2";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x87;
                data[10] = 0x00;
                string gzbs = (225 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }


        /// <summary>
        /// 冰蓝
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public static void Ice_Blue(string gzh, bool Flashing)
        {
            if (Flashing != true)
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 06 00 61"；
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x06;
                data[10] = 0x00;
                string gzbs = (96 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw = port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
            else
            {
                //string str = "A6 A8 01 00 00 0A 00 01 01 86 00 E1";
                int gz = Convert.ToInt32(gzh);
                string rtn = "";
                byte[] data = new byte[12];
                data[0] = 0xA6;
                data[1] = 0xA8;
                data[2] = 0x01;
                data[3] = 0x00;
                data[4] = 0x00;
                data[5] = 0x0A;
                data[6] = 0x00;
                data[7] = 0x01;
                data[8] = Convert.ToByte(gz.ToString("X2"), 16);   //格子号
                data[9] = 0x86;
                data[10] = 0x00;
                string gzbs = (224 + gz).ToString("X2");
                data[11] = Convert.ToByte(gzbs, 16);
                string bw =port.ByteArrayToHexString2(data);
                Logo.sWriteLogo("发送报文：" + bw.ToString(), 4);
                port.WriteData(data, ref rtn);
                bw = rtn;
            }
        }
        #endregion 

    }
}
