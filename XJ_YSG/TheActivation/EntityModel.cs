
using AForge.Video.DirectShow;
using ArcFaceSDK;
using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using ArcSoftFace.Entity;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FaceBox.TheActivation
{
    public class EntityModel
    {
        #region 参数定义
        /// <summary>
        /// 图像处理引擎对象
        /// </summary>
        public static FaceEngine imageEngine = new FaceEngine();

        /// <summary>
        /// 保存右侧图片路径
        /// </summary>
        public static string image1Path;

        /// <summary>
        /// 图片最大大小限制
        /// </summary>
        public static long maxSize = 1024 * 1024 * 2;

        /// <summary>
        /// 最大宽度
        /// </summary>
        public static int maxWidth = 1536;

        /// <summary>
        /// 最大高度
        /// </summary>
        public static int maxHeight = 1536;

        
        /// <summary>
        /// 人脸比对阈值
        /// </summary>
        public static float threshold = 0.8f;

        /// <summary>
        /// 红外（IR）活体阈值
        /// </summary>
        public static float thresholdIr = 0.7f;

        /// <summary>
        /// 可见光（RGB）活体阈值
        /// </summary>
        public static float thresholdRgb = 0.5f;

        /// <summary>
        /// 图像质量注册阈值
        /// </summary>
        public static float thresholdImgRegister = 0.63f;

        /// <summary>
        /// 图像质量识别戴口罩阈值
        /// </summary>
        public static float thresholdImgMask = 0.29f;

        /// <summary>
        /// 图像质量识别未戴口罩阈值
        /// </summary>
        public static float thresholdImgNoMask = 0.49f;
        /// <summary>
        /// 比对模型
        /// </summary>
        public static ASF_CompareModel compareModel = ASF_CompareModel.ASF_ID_PHOTO;
        /// <summary>
        /// 用于标记是否需要清除比对结果
        /// </summary>
        public static bool isCompare = false;

        #region 视频模式下相关
        /// <summary>
        /// 视频引擎对象
        /// </summary>
        public static FaceEngine videoEngine = new FaceEngine();

        /// <summary>
        /// RGB视频引擎对象
        /// </summary>
        public static FaceEngine videoRGBImageEngine = new FaceEngine();

        /// <summary>
        /// IR视频引擎对象
        /// </summary>
        public static FaceEngine videoIRImageEngine = new FaceEngine();

        /// <summary>
        /// 视频输入设备信息
        /// </summary>
        public static FilterInfoCollection filterInfoCollection;

        /// <summary>
        /// RGB摄像头设备
        /// </summary>
        public static VideoCaptureDevice rgbDeviceVideo;

        /// <summary>
        /// IR摄像头设备
        /// </summary>
        public static VideoCaptureDevice irDeviceVideo;

        /// <summary>
        /// 是否是双目摄像
        /// </summary>
        public static bool isDoubleShot = false;

        /// <summary>
        /// RGB 摄像头索引
        /// </summary>
        public static int rgbCameraIndex = 0;

        /// <summary>
        /// IR 摄像头索引
        /// </summary>
        public static int irCameraIndex = 0;

        /// <summary>
        /// 人员库图片选择 锁对象
        /// </summary>
        public static object chooseImgLocker = new object();

        /// <summary>
        /// RGB视频帧图像使用锁
        /// </summary>
        public static object rgbVideoImageLocker = new object();
        /// <summary>
        /// IR视频帧图像使用锁
        /// </summary>
        public static object irVideoImageLocker = new object();
        /// <summary>
        /// RGB视频帧图像
        /// </summary>
        public static Bitmap rgbVideoBitmap = null;
        /// <summary>
        /// IR视频帧图像
        /// </summary>
        public static Bitmap irVideoBitmap = null;
        /// <summary>
        /// RGB 摄像头视频人脸追踪检测结果
        /// </summary>
        public static  DictionaryUnit<int, FaceTrackUnit> trackRGBUnitDict = new DictionaryUnit<int, FaceTrackUnit>();

        /// <summary>
        /// RGB 特征搜索尝试次数字典
        /// </summary>
        public static DictionaryUnit<int, int> rgbFeatureTryDict = new DictionaryUnit<int, int>();

        /// <summary>
        /// RGB 活体检测尝试次数字典
        /// </summary>
        public static DictionaryUnit<int, int> rgbLivenessTryDict = new DictionaryUnit<int, int>();

        /// <summary>
        /// IR 视频最大人脸追踪检测结果
        /// </summary>
        public static FaceTrackUnit trackIRUnit = new FaceTrackUnit();

        /// <summary>
        /// VideoPlayer 框的字体
        /// </summary>
        public static  Font font = new Font(System.Drawing.FontFamily.GenericSerif, 10f, System.Drawing.FontStyle.Bold);

        /// <summary>
        /// 红色画笔
        /// </summary>
        public static SolidBrush redBrush = new SolidBrush(Color.Red);

        /// <summary>
        /// 绿色画笔
        /// </summary>
        public static SolidBrush greenBrush = new SolidBrush(Color.Green);

        /// <summary>
        /// 关闭FR线程开关
        /// </summary>
        public static  bool exitVideoRGBFR = false;

        /// <summary>
        /// 关闭活体线程开关
        /// </summary>
        public static bool exitVideoRGBLiveness = false;
        /// <summary>
        /// 关闭IR活体和FR线程线程开关
        /// </summary>
        public static bool exitVideoIRFRLiveness = false;
        /// <summary>
        /// FR失败重试次数
        /// </summary>
        public static int frMatchTime = 30;

        /// <summary>
        /// 活体检测失败重试次数
        /// </summary>
        public static  int liveMatchTime = 30;
        #endregion

        /// <summary>
        /// 比对人脸图片人脸特征
        /// </summary>
        public static List<FaceFeature> rightImageFeatureList = new List<FaceFeature>();

        /// <summary>
        /// 保存对比图片的列表
        /// </summary>
        public static List<string> imagePathList = new List<string>();

        /// <summary>
        /// 人脸库人脸特征列表
        /// </summary>
        public static List<FaceFeature> leftImageFeatureList = new List<FaceFeature>();

        /// <summary>
        /// 人脸库图片保存列表
        /// </summary>
        //public static List<string,Image> imageLists = new List<string,Image>();

        public static Dictionary<string, string > imageLists = new Dictionary<string, string>();
public static string sjhm = "";
        #endregion    
    }
}
