using AForge.Video.DirectShow;
using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using ArcSoftFace.Entity;
using ArcSoftFace.Utils;
using BLL;
using FaceBox.TheActivation;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace XJ_YSG
{
    /// <summary>
    /// Xj_Rlsb.xaml 的交互逻辑
    /// </summary>
    public partial class Xj_Rlsb : Window
    {

        Activation activation = new Activation();
        Logo log = new Logo();
        public Xj_Rlsb()
        {
            InitializeComponent();
            this.Left = 0;
            this.Top = 0;
            btnStartVideo_Click();
        }

        //启用摄像头
        private void btnStartVideo_Click()
        {
            try
            {
                //在点击开始的时候再坐下初始化检测，防止程序启动时有摄像头，在点击摄像头按钮之前将摄像头拔掉的情况
                activation.initVideo();
                //必须保证有可用摄像头
                if (EntityModel.filterInfoCollection.Count == 0)
                {
                    log.WriteLogo("未检测到摄像头，请确保已安装摄像头或驱动!", 1);
                    return;
                }
                //显示摄像头控件
                rgbVideoSource.Show();
                //获取filterInfoCollection的总数
                int maxCameraCount = EntityModel.filterInfoCollection.Count;
                //如果配置了两个不同的摄像头索引
                if (maxCameraCount != 0)
                {
                    //RGB摄像头加载
                    EntityModel.rgbDeviceVideo = new VideoCaptureDevice(EntityModel.filterInfoCollection[EntityModel.rgbCameraIndex < maxCameraCount ? EntityModel.rgbCameraIndex : 0].MonikerString);
                    rgbVideoSource.VideoSource = EntityModel.rgbDeviceVideo;
                    rgbVideoSource.Start();
                }
                //启动两个检测线程
                EntityModel.exitVideoRGBFR = false;
                EntityModel.exitVideoRGBLiveness = false;
                videoRGBLiveness();
                videoRGBFR();
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }






        /// <summary>
        /// 活体检测线程
        /// </summary>
        private void videoRGBLiveness()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                while (true)
                {
                    if (EntityModel.exitVideoRGBLiveness)
                    {
                        return;
                    }
                    if (EntityModel.rgbLivenessTryDict.GetDictCount() <= 0)
                    {
                        continue;
                    }
                    try
                    {
                        if (EntityModel.rgbVideoBitmap == null)
                        {
                            continue;
                        }
                        List<int> faceIdList = new List<int>();
                        faceIdList.AddRange(EntityModel.rgbLivenessTryDict.GetAllElement().Keys);
                        //遍历人脸Id，进行活体检测
                        foreach (int tempFaceId in faceIdList)
                        {
                            //待处理队列中不存在，移除
                            if (!EntityModel.rgbLivenessTryDict.ContainsKey(tempFaceId))
                            {
                                continue;
                            }
                            //大于尝试次数，移除
                            int tryTime = EntityModel.rgbLivenessTryDict.GetElementByKey(tempFaceId);
                            if (tryTime >= EntityModel.liveMatchTime)
                            {
                                continue;
                            }
                            tryTime += 1;
                            //无对应的人脸框信息
                            if (!EntityModel.trackRGBUnitDict.ContainsKey(tempFaceId))
                            {
                                continue;
                            }
                            FaceTrackUnit tempFaceTrack = EntityModel.trackRGBUnitDict.GetElementByKey(tempFaceId);

                            //RGB活体检测
                            Console.WriteLine(string.Format("faceId:{0},活体检测第{1}次\r\n", tempFaceId, tryTime));
                            SingleFaceInfo singleFaceInfo = new SingleFaceInfo();
                            singleFaceInfo.faceOrient = tempFaceTrack.FaceOrient;
                            singleFaceInfo.faceRect = tempFaceTrack.Rect;
                            singleFaceInfo.faceDataInfo = tempFaceTrack.FaceDataInfo;
                            Bitmap bitmapClone = null;
                            try
                            {
                                lock (EntityModel.rgbVideoImageLocker)
                                {
                                    if (EntityModel.rgbVideoBitmap == null)
                                    {
                                        break;
                                    }
                                    bitmapClone = (Bitmap)EntityModel.rgbVideoBitmap.Clone();
                                }
                                int retCodeLiveness = -1;
                                LivenessInfo liveInfo = FaceUtil.LivenessInfo_RGB(EntityModel.videoRGBImageEngine, bitmapClone, singleFaceInfo, out retCodeLiveness);
                                //更新活体检测结果
                                if (retCodeLiveness.Equals(0) && liveInfo.num > 0 && EntityModel.trackRGBUnitDict.ContainsKey(tempFaceId))
                                {
                                    EntityModel.trackRGBUnitDict.GetElementByKey(tempFaceId).RgbLiveness = liveInfo.isLive[0];
                                    if (liveInfo.isLive[0].Equals(1))
                                    {
                                        tryTime = EntityModel.liveMatchTime;
                                    }
                                }
                            }
                            catch (Exception ee)
                            {
                                LogUtil.LogInfo(GetType(), ee);
                            }
                            finally
                            {
                                if (bitmapClone != null)
                                {
                                    bitmapClone.Dispose();
                                }
                            }
                            EntityModel.rgbLivenessTryDict.UpdateDictionaryElement(tempFaceId, tryTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogInfo(GetType(), ex);
                    }
                }
            }));
        }



        /// <summary>
        /// 特征提取和搜索线程
        /// </summary>
        private void videoRGBFR()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(delegate
            {
                while (true)
                {
                    if (EntityModel.exitVideoRGBFR)
                    {
                        return;
                    }
                    if (EntityModel.rgbFeatureTryDict.GetDictCount() <= 0)
                    {
                        continue;
                    }
                    //左侧人脸库为空时，不用进行特征搜索
                    if (EntityModel.leftImageFeatureList.Count <= 0)
                    {
                        continue;
                    }
                    try
                    {
                        if (EntityModel.rgbVideoBitmap == null)
                        {
                            continue;
                        }
                        List<int> faceIdList = new List<int>();
                        faceIdList.AddRange(EntityModel.rgbFeatureTryDict.GetAllElement().Keys);
                        foreach (int tempFaceId in faceIdList)
                        {
                            //待处理队列中不存在，移除
                            if (!EntityModel.rgbFeatureTryDict.ContainsKey(tempFaceId))
                            {
                                continue;
                            }
                            //大于尝试次数，移除
                            int tryTime = EntityModel.rgbFeatureTryDict.GetElementByKey(tempFaceId);
                            if (tryTime >= EntityModel.frMatchTime)
                            {
                                continue;
                            }
                            //无对应的人脸框信息
                            if (!EntityModel.trackRGBUnitDict.ContainsKey(tempFaceId))
                            {
                                continue;
                            }
                            FaceTrackUnit tempFaceTrack = EntityModel.trackRGBUnitDict.GetElementByKey(tempFaceId);
                            tryTime += 1;
                            //特征搜索
                            int faceIndex = -1;
                            float similarity = 0f;
                            Console.WriteLine(string.Format("faceId:{0},特征搜索第{1}次\r\n", tempFaceId, tryTime));
                            //提取人脸特征
                            SingleFaceInfo singleFaceInfo = new SingleFaceInfo();
                            singleFaceInfo.faceOrient = tempFaceTrack.FaceOrient;
                            singleFaceInfo.faceRect = tempFaceTrack.Rect;
                            singleFaceInfo.faceDataInfo = tempFaceTrack.FaceDataInfo;
                            Bitmap bitmapClone = null;
                            try
                            {
                                lock (EntityModel.rgbVideoImageLocker)
                                {
                                    if (EntityModel.rgbVideoBitmap == null)
                                    {
                                        break;
                                    }
                                    bitmapClone = (Bitmap)EntityModel.rgbVideoBitmap.Clone();
                                }
                                FaceFeature feature = FaceUtil.ExtractFeature(EntityModel.videoRGBImageEngine, bitmapClone, singleFaceInfo);
                                if (feature == null || feature.featureSize <= 0)
                                {
                                    break;
                                }
                                //特征搜索
                                faceIndex = compareFeature(feature, out similarity);
                                //更新比对结果
                                if (EntityModel.trackRGBUnitDict.ContainsKey(tempFaceId))
                                {
                                    EntityModel.trackRGBUnitDict.GetElementByKey(tempFaceId).SetFaceIndexAndSimilarity(faceIndex, similarity.ToString("#0.00"));
                                    if (faceIndex > -1)
                                    {
                                        tryTime = EntityModel.frMatchTime;
                                    }
                                }
                            }
                            catch (Exception ee)
                            {
                                LogUtil.LogInfo(GetType(), ee);
                            }
                            finally
                            {
                                if (bitmapClone != null)
                                {
                                    bitmapClone.Dispose();
                                }
                            }
                            EntityModel.rgbFeatureTryDict.UpdateDictionaryElement(tempFaceId, tryTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        LogUtil.LogInfo(GetType(), ex);
                    }
                }
            }));
        }




        private int compareFeature(FaceFeature feature, out float similarity)
        {
            int result = -1;
            similarity = 0f;
            try
            {
                //如果人脸库不为空，则进行人脸匹配
                if (EntityModel.leftImageFeatureList != null && EntityModel.leftImageFeatureList.Count > 0)
                {
                    for (int i = 0; i < EntityModel.leftImageFeatureList.Count; i++)
                    {
                        //调用人脸匹配方法，进行匹配
                        EntityModel.videoRGBImageEngine.ASFFaceFeatureCompare(feature, EntityModel.leftImageFeatureList[i], out similarity, EntityModel.compareModel);
                        if (similarity >= EntityModel.threshold)
                        {
                            result = i;
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
            return result;
        }

        public static int ii = 0;

        /// <summary>
        /// RGB摄像头Paint事件，图像显示到窗体上，得到每一帧图像，并进行处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VideoSource_Paint(object sender, PaintEventArgs e)
        {
            try
            {
                if (!rgbVideoSource.IsRunning)
                {
                    return;
                }
                //得到当前RGB摄像头下的图片
                lock (EntityModel.rgbVideoImageLocker)
                {
                    EntityModel.rgbVideoBitmap = rgbVideoSource.GetCurrentVideoFrame();
                }
                Bitmap bitmapClone = null;
                try
                {
                    lock (EntityModel.rgbVideoImageLocker)
                    {
                        if (EntityModel.rgbVideoBitmap == null)
                        {
                            return;
                        }
                        bitmapClone = (Bitmap)EntityModel.rgbVideoBitmap.Clone();
                    }
                    if (bitmapClone == null)
                    {
                        return;
                    }
                    //检测人脸，得到Rect框
                    MultiFaceInfo multiFaceInfo = FaceUtil.DetectFaceAndLandMark(EntityModel.videoEngine, bitmapClone);
                    //未检测到人脸
                    if (multiFaceInfo.faceNum <= 0)
                    {
                        EntityModel.trackRGBUnitDict.ClearAllElement();
                        return;
                    }
                    Graphics g = e.Graphics;
                    float offsetX = rgbVideoSource.Width * 1f / bitmapClone.Width;
                    float offsetY = rgbVideoSource.Height * 1f / bitmapClone.Height;
                    List<int> tempIdList = new List<int>();
                    for (int faceIndex = 0; faceIndex < multiFaceInfo.faceNum; faceIndex++)
                    {
                        MRECT mrect = multiFaceInfo.faceRects[faceIndex];
                        float x = mrect.left * offsetX;
                        float width = mrect.right * offsetX - x;
                        float y = mrect.top * offsetY;
                        float height = mrect.bottom * offsetY - y;
                        int faceId = multiFaceInfo.faceID[faceIndex];
                        FaceTrackUnit currentFaceTrack = EntityModel.trackRGBUnitDict.GetElementByKey(faceId);
                        //根据Rect进行画框
                        //将上一帧检测结果显示到页面上
                        lock (EntityModel.rgbVideoImageLocker)
                        {
                            if (multiFaceInfo.pointAyy != null && multiFaceInfo.pointAyy.Length > 0)
                            {
                                ASF_FaceLandmark[] markAyy = multiFaceInfo.pointAyy[faceIndex];
                                if (markAyy != null && markAyy.Length > 0)
                                {
                                    PointF[] points = new PointF[markAyy.Length];
                                    if (markAyy.Length > 0)
                                    {
                                        for (int markIndex = 0; markIndex < markAyy.Length; markIndex++)
                                        {
                                            points[markIndex].X = markAyy[markIndex].x * offsetX;
                                            points[markIndex].Y = markAyy[markIndex].y * offsetY;
                                        }
                                    }
                                    // g.DrawPolygon(Pens.Blue, points);
                                }
                            }
                            //绘制人脸框并显示相似度信息
                            if (currentFaceTrack != null)
                            {
                                g.DrawRectangle(currentFaceTrack.CertifySuccess() ? Pens.Green : Pens.Red, x, y, width, height);
                                if (!string.IsNullOrWhiteSpace(currentFaceTrack.GetCombineMessage()) && x > 0 && y > 0)
                                {
                                    //将相似度信息实时显示     
                                    //g.DrawString(currentFaceTrack.GetCombineMessage(), EntityModel.font, currentFaceTrack.CertifySuccess() ? EntityModel.greenBrush : EntityModel.redBrush, x, y - 15);
                                    //14号:0.91|RGB:真人|faceId:1
                                    string similarityindex = currentFaceTrack.GetCombineMessage().Split('|')[0];
                                    string index = similarityindex.Split(':')[0];
                                    string similarity = similarityindex.Split(':')[1];
                                    if (Convert.ToDouble(similarity) >= 0.88)
                                    {
                                        string aimil = "";
                                        EntityModel.imageLists.TryGetValue(index, out aimil);
                                        //截取aimil 拿到手机号码  D:\ixjkj\synchro\龚于诏_19888925110.jpg
                                        string sjhm = System.Text.RegularExpressions.Regex.Replace(aimil, @"[^0-9]+", "");
                                        //将手机号码传给新页面
                                        Xj_BoxList boxList = new Xj_BoxList(sjhm);
                                        this.Close();
                                        boxList.ShowDialog();
                                    }
                                }
                            }
                            else
                            {
                                //只显示人脸框
                                g.DrawRectangle(Pens.Red, x, y, width, height);
                            }
                        }
                        if (faceId >= 0)
                        {
                            //判断faceid是否加入待处理队列
                            if (!EntityModel.rgbFeatureTryDict.ContainsKey(faceId))
                            {
                                EntityModel.rgbFeatureTryDict.AddDictionaryElement(faceId, 0);
                            }
                            if (!EntityModel.rgbLivenessTryDict.ContainsKey(faceId))
                            {
                                EntityModel.rgbLivenessTryDict.AddDictionaryElement(faceId, 0);
                            }
                            if (EntityModel.trackRGBUnitDict.ContainsKey(faceId))
                            {
                                EntityModel.trackRGBUnitDict.GetElementByKey(faceId).Rect = mrect;
                                EntityModel.trackRGBUnitDict.GetElementByKey(faceId).FaceOrient = multiFaceInfo.faceOrients[faceIndex];
                                EntityModel.trackRGBUnitDict.GetElementByKey(faceId).FaceDataInfo = multiFaceInfo.faceDataInfoList[faceIndex];
                            }
                            else
                            {
                                EntityModel.trackRGBUnitDict.AddDictionaryElement(faceId, new FaceTrackUnit(faceId, mrect, multiFaceInfo.faceOrients[faceIndex], multiFaceInfo.faceDataInfoList[faceIndex]));
                            }
                            tempIdList.Add(faceId);
                        }

                    }
                    //初始化及刷新待处理队列,移除出框的人脸
                    EntityModel.rgbFeatureTryDict.RefershElements(tempIdList);
                    EntityModel.rgbLivenessTryDict.RefershElements(tempIdList);
                    EntityModel.trackRGBUnitDict.RefershElements(tempIdList);
                }
                catch (Exception ee)
                {
                    LogUtil.LogInfo(GetType(), ee);
                }
                finally
                {
                    if (bitmapClone != null)
                    {
                        bitmapClone.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            rgbVideoSource.SignalToStop();
            rgbVideoSource.Hide();

            EntityModel.exitVideoRGBFR = true;
            EntityModel.exitVideoRGBLiveness = true;
        }








    }
}
