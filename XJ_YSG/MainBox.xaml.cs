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
using System.Windows.Input;
using System.Drawing;
using System.Windows.Threading;
using System.Windows.Media.Imaging;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;
using System.Collections;
using System.Data;

namespace XJ_YSG
{
    /// <summary>
    /// MainBox.xaml 的交互逻辑
    /// </summary>


    public partial class MainBox : Window
    {
        public static string bbh = "XJ_YSG_LSSJ_1.0";
        Activation activation = new Activation();
        Fingerprint fingerprint = new Fingerprint();
        Logo log = new Logo();       
        LockService lockService = new LockService();
        private MainBox mainbox = null;
        public static string sbbm = ServerBase.XMLRead("Ysg_sbbm", "sbbm").ToString(); //设备编码
        public static int gzsl = Convert.ToInt32(ServerBase.XMLRead("Count", "Ysg_gzsl")); //柜子格子数量
        private static string suocom = ServerBase.XMLRead("Lock", "COM_LOCK");  //锁
        public static DispatcherTimer zwTimer = new DispatcherTimer();//指纹计时器
        public static DispatcherTimer RfidTimer = new DispatcherTimer();//刷卡计时器
        DispatcherTimer ClossTimer = new DispatcherTimer();  //监控开门状态倒计时
        DispatcherTimer yxzkTimer = new DispatcherTimer(); //上报设备运行状况
        Interaction_WebService Service = new Interaction_WebService();
        public static List<string> locklis = new List<string>();// 监控箱门的数据集合1_0&1_1,柜号_取或还
        public static List<string> yjkqlis = new List<string>();// 应急开启还钥匙
        public static string HycsqdPK = "";  //还钥匙保存用车申请单pk
        public static string QycsqdPK = "";  //取钥匙保存用车申请单pk
        public static string clzk = "0";  //车辆状况
        public static string clwg = "0";  //车辆外观
        public static string clns = "0";  //车辆内饰       
        #region  指纹图像数据
        public static byte[] m_pImageBuffer = new byte[640 * 480];
        public static int m_nWidth = 0;
        public static int m_nHeight = 0;
        public static int m_nSize = 640 * 480;
        #endregion

        #region 用户信息和钥匙柜列表
        public static Hashtable usertable = new Hashtable(); //用户信息      
        public static Dictionary<string, string> ycsqdinfo = new Dictionary<string, string>();
        public static bool isyjkq = false;

        #endregion


        public MainBox()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            ImageBrush b3 = new ImageBrush();
            b3.ImageSource = new BitmapImage(new Uri("pack://application:,,,/Img/main.jpg", UriKind.Absolute));
            this.Background = b3;
            if (mainbox == null)
            {
                mainbox = this;
            }
            Logo.sWriteLogo("系统启动：" + bbh + "_" + DateTime.Now.ToString(), 8);           
            speack("欢迎使用智能钥匙管理柜");
            UHFService.ConnectCOM();   //刷卡
            UHF2Service.ConnectCOM();   //读钥匙
            activation.InitEngines(); //人脸识别引擎开启           
            ChooseMultiImg();  //人脸注册
            fingerprint.ZW_Connection();//打开指纹端口
            lockService.initCom(suocom);  //连接锁
            Csh_yskp(); //初始化钥匙柜卡片
            zwthan();   //开始指纹验证
            Rfidthan(); //实时RFID读信息卡           
            LockDjs(); //监控箱门
            Sbyxzt(); //上报状态
            this.Closed += MainWindow_Closed;
        }
        #region 输入钥匙码按钮     
        private void Smkey_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            Xj_Mima xj_Mima = new Xj_Mima();
            xj_Mima.ShowDialog();
        }

        #endregion


        #region 开启人脸识别按钮     
        private void Facekey_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            Xj_Rlsb xj_Rlsb = new Xj_Rlsb();
            xj_Rlsb.ShowDialog();
        }    
        #endregion


        #region 应急开启
        private void Emergency_open_TouchUp(object sender, System.Windows.Input.TouchEventArgs e)
        {
            zwTimer.Stop();
            RfidTimer.Stop();
            isyjkq = true;
            Xj_Mima mima = new Xj_Mima();
            mima.ShowDialog();
        }
        #endregion



        #region 指纹识别
        /// <summary>
        /// 指纹识别倒计时
        /// </summary>
        public void zwthan()
        {
            
            zwTimer.Interval = new TimeSpan(0, 0, 0, 0, 500); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
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
                    if (UserID != 0)
                    {
                        usertable = Service.getUserInfo(UserID.ToString());
                        if (usertable != null)
                        {
                            //将手机号码传给新页面
                            Xj_BoxList boxList = new Xj_BoxList();
                            this.Close();
                            boxList.ShowDialog();
                        }
                    }
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
        public static  void speack(string text)
        {

            try
            {
                SpeechVoice.speack(text);
            }
            catch (Exception e)
            {
                Logo.sWriteLogo(e.Message, 9);
            }
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

            UHFService.strEPC = "";
            UHFService.OneCheckInvnetoryWhile(0);
            string kp = UHFService.strEPC;
            if (kp != "")
            {
                Sk(kp);
            }
        }

        public void Sk(string kp) 
        {
            try
            {
                //读取txt文件;
                string txt = log.GetYsgGh();
                txt = txt.Substring(0, txt.Length - 1);
                var wzm = txt.Split(',').Where(t => t.Contains(kp)).FirstOrDefault().ToString().Split('_')[0];
                if (wzm != "")
                {
                    zwTimer.Stop();
                    RfidTimer.Stop();
                    //调用用车申请单接口
                    var hash = Service.getycsqdpkInfo(sbbm, wzm);
                    if (hash != null && hash.Count > 0)
                    {
                        string PK = hash["PK"].ToString(); //用车申请单pk                     
                        HycsqdPK = PK;               //保存用车申请单pk
                        if (hash["SFPL"].ToString() == "1") //是否评论
                        {
                            Xj_Clpj xj_Clpj = new Xj_Clpj(mainbox, wzm);
                            xj_Clpj.ShowDialog();
                        }
                    }                   
                    LockService.Send(wzm);  //开门
                    LockService.red_light(wzm, true);
                    speack("柜门已打开");
                    locklis.Add(wzm + "_skhys"); //添加到箱门状态集合中                           
                }
                else
                {
                    speack("该卡无效");
                    zwTimer.Start();
                    RfidTimer.Start();
                }
            }
            catch (Exception ex)
            {
                log.WriteLogo(ex.Message, 8);
            }
           
        }


        #endregion

      


        #region 监控箱门状态
        /// <summary>
        /// 监控开门状态倒计时
        /// </summary>
        /// 
        public void LockDjs()
        {

            ClossTimer.Interval = new TimeSpan(0, 0, 0, 0, 500); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            ClossTimer.Tick += new EventHandler(ClossTimer_Tick_canShow); //每一秒执行的方法
            ClossTimer.Start();
        }
        void ClossTimer_Tick_canShow(object sender, EventArgs e)
        {
            try
            {
                if (locklis.Count > 0) //判断是否有柜门打开
                {
                    foreach (string item in locklis)
                    {
                        //位置码_操作模式
                        string wzm = item.Split('_')[0];
                        string czms = item.Split('_')[1];
                        //A6 A8 00 01 00 0A 00 0A 08 01 03 6F
                        //A6 A8 00 01 00 0A 00 0A 03 01 03 6A
                        //A6A80001000A000A0301036A
                        string lockzt = LockService.State(wzm);
                        string ztjy = lockzt.Substring(0, 16);
                        bool zt = false;
                        if (ztjy == "A6A80001000A000A") 
                        {
                            if (Convert.ToInt32(lockzt.Substring(19, 1)) == 0) 
                            {
                                zt = true;
                            }
                        }                        
                        //状态 1 表示开的状态，状态 0 表示关的状态。
                        if (zt)
                        {
                            switch (czms)
                            {
                                case "skhys": //刷卡还钥匙                              
                                    UHF2Service.strEPC = "";
                                    UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签                           
                                    if (UHF2Service.strEPC != "")   //卡片是否读到 
                                    {
                                        Service.saveHys(sbbm, HycsqdPK, clzk, clwg, clns);      //调还钥匙接口
                                        LockService.Blue_light(wzm, false);
                                        speack("归还成功");
                                    }
                                    else
                                    {
                                        LockService.red_light(wzm, false);
                                    }
                                    locklis.Remove(item);   //移除对该柜号的监控
                                    if (HycsqdPK != "")
                                    {
                                        HycsqdPK = "";
                                    }
                                    clzk = "0";
                                    clwg = "0";
                                    clns = "0";
                                    zwTimer.Start();
                                    RfidTimer.Start();
                                    break;
                                case "mmqys": //密码取钥匙
                                    UHF2Service.strEPC = "";
                                    UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签                          
                                    if (UHF2Service.strEPC == "")   //取走
                                    {
                                        Service.saveQys(sbbm, QycsqdPK);
                                        LockService.red_light(wzm, false);
                                    }
                                    else
                                    {
                                        LockService.Blue_light(wzm, false);
                                    }
                                    locklis.Remove(item);   //移除对该柜号的监控
                                    if (HycsqdPK != "")
                                    {
                                        HycsqdPK = "";
                                    }
                                    zwTimer.Start();
                                    RfidTimer.Start();
                                    break;
                                case "qtqys": //应急，人脸，指纹，取钥匙
                                    UHF2Service.strEPC = "";
                                    UHF2Service.OneCheckInvnetoryWhile(Convert.ToInt32(wzm) - 1); //读标签  
                                    if (UHF2Service.strEPC == "")   // 取走
                                    {
                                        Service.saveYcsq(sbbm, wzm, usertable["PK"].ToString(), ycsqdinfo["yclxpk"].ToString(), ycsqdinfo["ycsypk"].ToString(), ycsqdinfo["mddspk"].ToString(), ycsqdinfo["mddxpk"].ToString(), ycsqdinfo["yjghsjpk"].ToString());
                                        LockService.red_light(wzm, false);
                                    }
                                    else  //未取走
                                    {
                                        LockService.Blue_light(wzm, false);
                                    }
                                    locklis.Remove(item);   //移除对该柜号的监控
                                    if (HycsqdPK != "")
                                    {
                                        HycsqdPK = "";    //清除用车申请
                                    }
                                    if (usertable != null)
                                    {
                                        usertable.Clear(); //清除用车申请单
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.WriteLogo(ex.Message, 8);
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
                    UHF2Service.strEPC = "";
                    LockService.Ice_Blue(i.ToString(), false);
                    UHF2Service.OneCheckInvnetoryWhile(i - 1);
                    if (UHF2Service.strEPC != "")
                    {
                        gh_kp += "" + i + "_" + UHF2Service.strEPC + "," + "\r";
                        LockService.Blue_light(i.ToString(), false);
                    }
                    else
                    {
                        LockService.red_light(i.ToString(), false);
                    }
                }
                string FilePath = Logo.sSetYsgGh();
                if (!File.Exists(FilePath))
                {
                    Logo.sWriteYsgh(gh_kp.Substring(0, gh_kp.Length - 1));
                }               
                speack("智能钥匙管理柜初始化成功");
            });
        }
        #endregion



        #region 上报钥匙柜运行状态
        public void Sbyxzt()
        {
            yxzkTimer.Interval = new TimeSpan(0, 2, 0, 0, 0); //参数分别为：天，小时，分，秒。此方法有重载，可根据实际情况调用。
            yxzkTimer.Tick += new EventHandler(yxzkTimer_Tick_canShow); //每一秒执行的方法
            yxzkTimer.Start();
        }
        void yxzkTimer_Tick_canShow(object sender, EventArgs e)
        {
            try
            {
                Service.sendyxzk(sbbm, bbh);
                Logo.sWriteLogo("运行正常" + bbh + "_" + DateTime.Now.ToString(), 8);
            }
            catch (Exception ex)
            {

                Logo.sWriteLogo(ex.Message + "_" + DateTime.Now.ToString(), 8);
            }


        }
        #endregion



        private void Emergency_clos_PreviewTouchUp(object sender, TouchEventArgs e)
        {
            if (MessageBox.Show("您确定退出？", "警告", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                Close();
            }
        }
        private void MainWindow_Closed(object sender, EventArgs e)
        {          
                //释放网络资源
                Environment.Exit(0);           
                     
        }

     
    }
}
