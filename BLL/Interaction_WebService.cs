﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Comm;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BLL
{
    public class Interaction_WebService
    {
        public static string _url = null;
        public Interaction_WebService()
        {
            if (_url == null)
            {
                _url = ServerBase.XMLRead("Database", "WebService");

            }
        }  
        /// <summary>
        /// 根据取钥匙码验证
        /// </summary>
        /// <param name="mm">取钥匙码</param>
        /// <param name="sbbm">钥匙柜编码</param>
        /// <returns></returns>
        public Hashtable getInfoByEwm(string mm,string sbbm)
        {
            Hashtable p = new Hashtable();
            p.Add("mm", mm);
            p.Add("sbbm", sbbm);
            return doCallingInterface("clgl", "checkysm", p);
        }




        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sjhm">手机号码</param>
        /// <returns></returns>
        public Hashtable getInfoUser(string sjhm) 
        {
            Hashtable p = new Hashtable();
            p.Add("sjhm", sjhm);
            return doCallingInterface("p01", "getuserinfo", p);
        }





        /// <summary>
        /// 根据钥匙编码获取钥匙信息
        /// </summary>
        /// <param name="key">钥匙编码</param>
        /// <returns></returns>
        public Hashtable getInfoByKey(string key, string lx)
        {
            Hashtable p = new Hashtable();
            if (lx == "1")
            {
                p.Add("key", key);
            }
            else
            {
                p.Add("ysbh", key);
            }
            return doCallingInterface("clgl", "getysgvmodel", p);
        }





        /// <summary>
        /// 将钥匙编号与RFID绑定
        /// </summary>
        /// <param name="ysbh"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Hashtable BindCode(string ysbh, string key)
        {
            Hashtable p = new Hashtable();
            p.Add("ysbh", ysbh);
            p.Add("key", key);
            return doCallingInterface("clgl", "savekeycode", p);
        }





        /// <summary>
        /// 绑定管理员卡
        /// </summary>
        /// <param name="ysbh"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public Hashtable BindCard(string key)
        {
            Hashtable p = new Hashtable();
            p.Add("key", key);
            return doCallingInterface("clgl", "saveadmincard", p);
        }





        /// <summary>
        /// 判断管理员卡是否正确
        /// </summary>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckCard(string key)
        {
            bool rtn = false;
            Hashtable p = new Hashtable();
            p.Add("key", key);
            Hashtable info = doCallingInterface("clgl", "checkadmincard", p);
            rtn = (info["r"].ToString() == "true");
            return rtn;
        }



        /// <summary>
        /// 判断财务密码是否正确
        /// </summary>
        /// <param name="password"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public bool CheckManageMM(string password)
        {
            bool rtn = false;
            Hashtable p = new Hashtable();
            p.Add("mm", password);
            Hashtable info = doCallingInterface("clgl", "checkmanagemm", p);
            rtn = (info["r"].ToString() == "true");
            return rtn;
        }
      



        /// <summary>
        /// 获取钥匙柜日志
        /// </summary>
        /// <returns></returns>
        public DataSet getLogList()
        {
            Hashtable p = new Hashtable();
            DataSet ds = new DataSet();
            JArray javascript = new JArray();
            string Json = doCallingInterface_String("clgl", "getysgloglist", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    DataTable tb_kglzb = new DataTable();
                    tb_kglzb.TableName = "tb_log";
                    tb_kglzb.Columns.Add("PK");
                    tb_kglzb.Columns.Add("YSBH");
                    tb_kglzb.Columns.Add("LX");
                    tb_kglzb.Columns.Add("CCJSJ");
                    javascript = (JArray)jsonObj["rows"];
                    for (int i = 0; i < javascript.Count; i++)
                    {
                        JObject jsonobj = (JObject)javascript[i];
                        tb_kglzb.Rows.Add(new object[] { jsonobj["PK"].ToString(), jsonobj["YSBH"].ToString(), jsonobj["LX"].ToString(), jsonobj["CCJSJ"].ToString() });
                    }
                    ds.Tables.Add(tb_kglzb);
                }                      
            }
            return ds;
        }



        /// <summary>
        /// 获取钥匙柜所有钥匙
        /// </summary>
        /// <returns></returns>
        public DataSet getYsgList(string sjhm)
        {
            Hashtable p = new Hashtable();
            DataSet ds = new DataSet();
            JArray javascript = new JArray();
            p.Add("sjhm", sjhm);
            string Json = doCallingInterface_String("clgl", "getysglist", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    DataTable tb_kglzb = new DataTable();
                    tb_kglzb.TableName = "tb_log";
                    tb_kglzb.Columns.Add("PK");
                    tb_kglzb.Columns.Add("YSBH");
                    tb_kglzb.Columns.Add("LX");
                    tb_kglzb.Columns.Add("CCJSJ");
                    javascript = (JArray)jsonObj["rows"];
                    for (int i = 0; i < javascript.Count; i++)
                    {                     
                        JObject jsonobj = (JObject)javascript[i];
                        tb_kglzb.Rows.Add(jsonobj["YSBH"].ToString(), jsonobj["CLHP"].ToString());
                    }
                    ds.Tables.Add(tb_kglzb);
                }
            }
            return ds;
        }




       


        /// <summary>
        /// 获取日志数量
        /// </summary>
        /// <returns></returns>
        public string getLogCount()
        {
            string rtn = "0";
            Hashtable p = new Hashtable();
            Hashtable info = doCallingInterface("clgl", "getlogcount", p);
            if (info["r"].ToString() == "true")
            {
                rtn = info["c"].ToString();
            }
            return rtn;
        }




        /// <summary>
        /// 插入日志
        /// </summary>
        /// <param name="key">钥匙编码</param>
        /// <param name="ysbh">钥匙编号</param>
        /// <param name="lx">1/2:取钥匙/还钥匙</param>
        /// <returns></returns>
        public Hashtable setLog(string key, string ysbh, string lx)
        {
            Hashtable p = new Hashtable();
            p.Add("ysbh", ysbh);
            p.Add("key", key);
            p.Add("lx", lx);
            return doCallingInterface("clgl", "savelog", p);
        }



        public Hashtable setLog(string key, string ysbh, string lx,string sjhm, string ycsqdpk)
        {
            Hashtable p = new Hashtable();
            p.Add("ysbh", ysbh);
            p.Add("key", key);
            p.Add("ycsqdpk", ycsqdpk);
            p.Add("sjhm", sjhm);
            p.Add("lx", lx);
            return doCallingInterface("clgl", "savelog", p);
        }



        /// <summary>
        /// 使用密码取钥匙时，回调接口
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public Hashtable saveQys(string mm,string pcdpk,string ycsqdpk)
        {
            Hashtable p = new Hashtable();
            p.Add("mm", mm);
            p.Add("pcdpk", pcdpk);
            p.Add("ycsqdpk", ycsqdpk);
            return doCallingInterface("clgl", "saveqys", p);
        }


        /// <summary>
        /// 使用验证码取钥匙
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public Hashtable saveQysLog(string ysbh, string sbbm)
        {
            Hashtable p = new Hashtable();
            p.Add("ysbh", ysbh);           
            p.Add("sbbm", sbbm);
            return doCallingInterface("clgl", "savehys", p);
        }


        /// <summary>
        /// 还钥匙，回调接口
        /// </summary>
        /// <param name="ysbh">钥匙编码(对应格子号)</param>
        /// <param name="sbbm">钥匙柜编码</param>
        /// <param name="yscqdpk">用车申请的PK</param>
        /// <param name="clzkpj">车辆状况评价值</param>
        /// <param name="clwgpj">车辆外观评价值</param>
        /// <param name="clnspj">车辆内饰评价值</param>
        /// <returns></returns>
        public Hashtable saveHys(string ysbh,string sbbm, string yscqdpk,string clzkpj,string clwgpj,string clnspj)
        {
            Hashtable p = new Hashtable();
            p.Add("ysbh", ysbh);
            p.Add("sbbm", sbbm);
            p.Add("yscqdpk", yscqdpk);
            p.Add("clzkpj", clzkpj);
            p.Add("clwgpj", clwgpj);
            p.Add("clnspj", clnspj);
            return doCallingInterface("clgl", "savehys", p);
        }


        /// <summary>
        /// 统一调用接口
        /// </summary>
        /// <param name="sys_info"></param>
        /// <param name="p">其他参数集合</param>
        /// <returns></returns>
        public Hashtable doCallingInterface(string sys_info, string method_name, Hashtable p)
        {
            Hashtable hb = new Hashtable();
            string returnXml = "";
            try
            {
                string strURL = _url;
                string sys_code = ServerBase.XMLRead("system", "sys_code");//柜号
                string postData = "params.sys_info=" + sys_info + "&params.method_name=" + method_name + "&params.gh=" + sys_code;
                foreach (DictionaryEntry de in p)
                {
                    string key = de.Key.ToString();
                    string val = de.Value.ToString();
                    postData += "&params." + key + "=" + val;
                }
                //创建Web访问对  象
                HttpWebRequest myRequest;
                if (sys_info == "5")
                {
                    myRequest = (HttpWebRequest)WebRequest.Create(strURL);
                    Encoding utf8 = Encoding.UTF8;
                    byte[] data = utf8.GetBytes(postData);
                    myRequest.Method = "POST";
                    myRequest.ContentType = "application/x-www-form-urlencoded";
                    myRequest.ContentLength = data.Length;
                    Stream newStream = myRequest.GetRequestStream();
                    //把请求数据写入请求流中
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                else
                {
                    myRequest = (HttpWebRequest)WebRequest.Create(strURL + "?" + postData);
                }
                //通过Web访问对象获取响应内容
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                reader.Close();
                myResponse.Close();
            }
            catch (Exception e)
            {

            }
            finally
            {
                hb = JsonToDataSet(returnXml);
            }
            return hb;
        }



        /// <summary>
        /// 统一调用接口
        /// </summary>
        /// <param name="sys_info"></param>
        /// <param name="p">其他参数集合</param>
        /// <returns></returns>
        public string doCallingInterface_String(string sys_info, string method_name, Hashtable p)
        {
            Hashtable hb = new Hashtable();
            string returnXml = "";
            try
            {
                string strURL = _url;
                string sys_code = ServerBase.XMLRead("system", "sys_code");//柜号
                string postData = "params.sys_info=" + sys_info + "&params.method_name=" + method_name + "&params.gh=" + sys_code;
                foreach (DictionaryEntry de in p)
                {
                    string key = de.Key.ToString();
                    string val = de.Value.ToString();
                    postData += "&params." + key + "=" + val;
                }
                //创建Web访问对  象
                HttpWebRequest myRequest;
                if (sys_info == "5")
                {
                    myRequest = (HttpWebRequest)WebRequest.Create(strURL);
                    Encoding utf8 = Encoding.UTF8;
                    byte[] data = utf8.GetBytes(postData);
                    myRequest.Method = "POST";
                    myRequest.ContentType = "application/x-www-form-urlencoded";
                    myRequest.ContentLength = data.Length;
                    Stream newStream = myRequest.GetRequestStream();
                    //把请求数据写入请求流中
                    newStream.Write(data, 0, data.Length);
                    newStream.Close();
                }
                else
                {
                    myRequest = (HttpWebRequest)WebRequest.Create(strURL + "?" + postData);
                }
                //通过Web访问对象获取响应内容
                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();
                //通过响应内容流创建StreamReader对象，因为StreamReader更高级更快
                StreamReader reader = new StreamReader(myResponse.GetResponseStream(), Encoding.UTF8);
                //string returnXml = HttpUtility.UrlDecode(reader.ReadToEnd());//如果有编码问题就用这个方法
                returnXml = reader.ReadToEnd();//利用StreamReader就可以从响应内容从头读到尾
                reader.Close();
                myResponse.Close();
            }
            catch (Exception e)
            {
            }
            return returnXml;
        }



        /// <summary>
        /// 将Json转化成DataSet
        /// </summary>
        /// <param name="Jsion"></param>
        /// <returns></returns>
        public Hashtable JsonToDataSet(string Json)
        {
           
            Hashtable ht = new Hashtable();
            if (!string.IsNullOrEmpty(Json))
            {
                string[] JS = Json.Replace("{", "").Replace("}", "").Split(',');
                for (int i = 0; i < JS.Length; i++)
                {
                    string key = JS[i].Replace("\":\"", "@").Split('@')[0].Replace("\"", "");
                    string val = JS[i].Replace("\":\"", "@").Split('@')[1].Replace("\"", "");
                    ht.Add(key, val);
                }
            }
            if (!ht.Contains("r"))
            {
                ht.Add("r", "false");
            }
            if (!ht.Contains("mess"))
            {
                ht.Add("mess", ServerBase.XMLRead("system", "mess"));
            }
            return ht;
        }
    }
}
