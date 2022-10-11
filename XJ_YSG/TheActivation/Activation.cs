
using AForge.Video.DirectShow;
using ArcFaceSDK;
using ArcFaceSDK.Entity;
using ArcFaceSDK.SDKModels;
using ArcSoftFace.Utils;
using BLL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;

namespace FaceBox.TheActivation
{
    public class Activation
    {       
        #region 初始化引擎
        EntityModel entityModel = new EntityModel();
        Logo log = new Logo();
        /// <summary>
        /// 初始化引擎
        /// </summary>
        public string  InitEngines()
        {
            try
            {
                //读取配置文件
                AppSettingsReader reader = new AppSettingsReader();
                EntityModel.rgbCameraIndex = (int)reader.GetValue("RGB_CAMERA_INDEX", typeof(int));
                EntityModel.irCameraIndex = (int)reader.GetValue("IR_CAMERA_INDEX", typeof(int));
                EntityModel.frMatchTime = (int)reader.GetValue("FR_MATCH_TIME", typeof(int));
                EntityModel.liveMatchTime = (int)reader.GetValue("LIVENESS_MATCH_TIME", typeof(int));

                int retCode = 0;
                bool isOnlineActive = true;//true(在线激活) or false(离线激活)
                try
                {
                    if (isOnlineActive)
                    {
                        #region 读取在线激活配置信息
                        string appId = (string)reader.GetValue("APPID", typeof(string));
                        string sdkKey64 = (string)reader.GetValue("SDKKEY64", typeof(string));
                        string sdkKey32 = (string)reader.GetValue("SDKKEY32", typeof(string));
                        string activeKey64 = (string)reader.GetValue("ACTIVEKEY64", typeof(string));
                        string activeKey32 = (string)reader.GetValue("ACTIVEKEY32", typeof(string));
                        //判断CPU位数
                        var is64CPU = Environment.Is64BitProcess;
                        if (string.IsNullOrWhiteSpace(appId) || string.IsNullOrWhiteSpace(is64CPU ? sdkKey64 : sdkKey32) || string.IsNullOrWhiteSpace(is64CPU ? activeKey64 : activeKey32))
                        {
                            log.WriteLogo(string.Format("请在App.config配置文件中先配置APP_ID和SDKKEY{0}、ACTIVEKEY{0}!", is64CPU ? "64" : "32"), 1);
                            System.Environment.Exit(0);
                        }
                        #endregion
                        //在线激活引擎    如出现错误，1.请先确认从官网下载的sdk库已放到对应的bin中，2.当前选择的CPU为x86或者x64
                        retCode = EntityModel.imageEngine.ASFOnlineActivation(appId, is64CPU ? sdkKey64 : sdkKey32, is64CPU ? activeKey64 : activeKey32);
                    }
                    else
                    {
                        #region 读取离线激活配置信息
                        string offlineActiveFilePath = (string)reader.GetValue("OfflineActiveFilePath", typeof(string));
                        if (string.IsNullOrWhiteSpace(offlineActiveFilePath) || !File.Exists(offlineActiveFilePath))
                        {
                            string deviceInfo;
                            retCode = EntityModel.imageEngine.ASFGetActiveDeviceInfo(out deviceInfo);
                            if (retCode != 0)
                            {
                                log.WriteLogo("获取设备信息失败，错误码:" + retCode, 1);
                            }
                            else
                            {
                                File.WriteAllText("ActiveDeviceInfo.txt", deviceInfo);
                                log.WriteLogo("获取设备信息成功，已保存到运行根目录ActiveDeviceInfo.txt文件，请在官网执行离线激活操作，将生成的离线授权文件路径在App.config里配置后再重新运行", 1);
                            }
                            System.Environment.Exit(0);
                        }
                        #endregion
                        //离线激活
                        retCode = EntityModel.imageEngine.ASFOfflineActivation(offlineActiveFilePath);
                    }
                    if (retCode != 0 && retCode != 90114)
                    {
                        log.WriteLogo("激活SDK失败,错误码:" + retCode, 1);
                        System.Environment.Exit(0);
                    }
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("无法加载 DLL"))
                    {
                        log.WriteLogo("请将SDK相关DLL放入bin对应的x86或x64下的文件夹中!", 1);
                    }
                    else
                    {
                        log.WriteLogo("激活SDK失败,请先检查依赖环境及SDK的平台、版本是否正确!", 1);
                    }
                    System.Environment.Exit(0);
                }

                //初始化引擎
                DetectionMode detectMode = DetectionMode.ASF_DETECT_MODE_IMAGE;
                //Video模式下检测脸部的角度优先值
                ASF_OrientPriority videoDetectFaceOrientPriority = ASF_OrientPriority.ASF_OP_ALL_OUT;
                //Image模式下检测脸部的角度优先值
                ASF_OrientPriority imageDetectFaceOrientPriority = ASF_OrientPriority.ASF_OP_ALL_OUT;
                //最大需要检测的人脸个数
                int detectFaceMaxNum = 6;
                //引擎初始化时需要初始化的检测功能组合
                int combinedMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_AGE | FaceEngineMask.ASF_GENDER | FaceEngineMask.ASF_FACE3DANGLE | FaceEngineMask.ASF_IMAGEQUALITY | FaceEngineMask.ASF_MASKDETECT;
                //初始化引擎，正常值为0，其他返回值请参考http://ai.arcsoft.com.cn/bbs/forum.php?mod=viewthread&tid=19&_dsign=dbad527e
                retCode = EntityModel.imageEngine.ASFInitEngine(detectMode, imageDetectFaceOrientPriority, detectFaceMaxNum, combinedMask);
                Console.WriteLine("InitEngine Result:" + retCode);
                log.WriteLogo((retCode == 0) ? "图片引擎初始化成功!" : string.Format("图片引擎初始化失败!错误码为:{0}", retCode),3);
            

                //初始化视频模式下人脸检测引擎
                DetectionMode detectModeVideo = DetectionMode.ASF_DETECT_MODE_VIDEO;
                int combinedMaskVideo = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_FACELANDMARK;
                retCode = EntityModel.videoEngine.ASFInitEngine(detectModeVideo, videoDetectFaceOrientPriority, detectFaceMaxNum, combinedMaskVideo);
                log.WriteLogo((retCode == 0) ? "视频引擎初始化成功!" : string.Format("视频引擎初始化失败!错误码为:{0}", retCode), 3);
             

                //RGB视频专用FR引擎
                combinedMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_LIVENESS | FaceEngineMask.ASF_MASKDETECT;
                retCode = EntityModel.videoRGBImageEngine.ASFInitEngine(detectMode, videoDetectFaceOrientPriority, detectFaceMaxNum, combinedMask);
                log.WriteLogo((retCode == 0) ? "RGB处理引擎初始化成功!" : string.Format("RGB处理引擎初始化失败!错误码为:{0}", retCode), 3);
                //设置活体阈值
                EntityModel.videoRGBImageEngine.ASFSetLivenessParam(EntityModel.thresholdRgb);
                //IR视频专用FR引擎
                combinedMask = FaceEngineMask.ASF_FACE_DETECT | FaceEngineMask.ASF_FACERECOGNITION | FaceEngineMask.ASF_IR_LIVENESS;
                retCode = EntityModel.videoIRImageEngine.ASFInitEngine(detectModeVideo, videoDetectFaceOrientPriority, detectFaceMaxNum, combinedMask);
                log.WriteLogo((retCode == 0) ? "IR处理引擎初始化成功!\r\n" : string.Format("IR处理引擎初始化失败!错误码为:{0}\r\n", retCode), 3);
                //设置活体阈值
                EntityModel.videoIRImageEngine.ASFSetLivenessParam(EntityModel.thresholdRgb, EntityModel.thresholdIr);
                initVideo();
                return "1";
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
                log.WriteLogo("程序初始化异常,请在App.config中修改日志配置,根据日志查找原因!", 3);
                System.Environment.Exit(0);
                return "0";
            }
           
        }
        /// <summary>
        /// 摄像头初始化
        /// </summary>
        public void initVideo()
        {
            try
            {
                EntityModel.filterInfoCollection = new FilterInfoCollection(FilterCategory.VideoInputDevice);
                //如果没有可用摄像头，“启用摄像头”按钮禁用，否则使可用
                //btnStartVideo.IsEnabled = filterInfoCollection.Count != 0;
            }
            catch (Exception ex)
            {
                LogUtil.LogInfo(GetType(), ex);
            }
        }
        #endregion     
    }
}
