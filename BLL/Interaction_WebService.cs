using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Comm;
using System.Net;
using System.Xml;
using System.IO;
using System.Collections;
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

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
        /// <returns></returns>
        public Hashtable getInfoByEwm(string mm,string sbbm)
        {
            Hashtable outtb = new Hashtable();
            Hashtable p = new Hashtable();
            p.Add("mm", mm);
            p.Add("sbbm", sbbm);
            string Json=doCallingInterface_String("clgl", "checkysm", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    if (jsonObj["data"] != null)
                    {
                        JObject vmodel = (JObject)jsonObj["data"];
                        outtb.Add("YSBH", vmodel["YSBH"].ToString());
                        outtb.Add("YCSQDPK", vmodel["YCSQDPK"].ToString());
                        outtb.Add("RFID", vmodel["RFID"].ToString());
                        outtb.Add("PK", vmodel["PK"].ToString());                     
                        outtb.Add("WZM", vmodel["WZM"].ToString());                        
                    }
                }
                
            }          
            return outtb;
            
        }

        /// <summary>
        /// 获取用户信息
        /// </summary>
        /// <param name="sjhm">手机号码</param>
        /// <returns></returns>
        public Hashtable getUserInfo(string sjhm) 
        {
            Hashtable outtb = new Hashtable();
            Hashtable p = new Hashtable();
            p.Add("sjhm", sjhm);
            string Json = doCallingInterface_String("p01", "getuserinfo", p);

            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    if (jsonObj["data"] != null)
                    {
                        JObject vmodel = (JObject)jsonObj["data"];
                        outtb.Add("JGJC", vmodel["JGJC"].ToString());
                        outtb.Add("XM", vmodel["XM"].ToString());
                        outtb.Add("DLZH", vmodel["DLZH"].ToString());
                        outtb.Add("SFZH", vmodel["SFZH"].ToString());
                        outtb.Add("SJHM", vmodel["SJHM"].ToString());
                        outtb.Add("PK", vmodel["PK"].ToString());
                        outtb.Add("DEPTPK", vmodel["DEPTPK"].ToString());
                    }
                }
            }
            return outtb;
        }

        /// <summary>
        /// 获取钥匙柜列表
        /// </summary>
        /// <param name="sjhm">手机号码</param>
        /// <param name="sbbm">设备编码</param>
        /// <returns></returns>
        public DataTable getListBox(string sjhm,string sbbm)
        {
           
            Hashtable p = new Hashtable();
            p.Add("sjhm", sjhm);
            p.Add("sbbm", sbbm);
            DataTable tb_kglzb = new DataTable();
            JArray javascript = new JArray();
            string Json = doCallingInterface_String("clgl", "getclxxlist", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    tb_kglzb.TableName = "tb_boxlist";
                    tb_kglzb.Columns.Add("PK");
                    tb_kglzb.Columns.Add("XZMC");
                    tb_kglzb.Columns.Add("JSYXM");
                    tb_kglzb.Columns.Add("CXMC");
                    tb_kglzb.Columns.Add("CLHP");              
                    javascript = (JArray)jsonObj["rows"];
                    for (int i = 0; i < javascript.Count; i++)
                    {
                        JObject jsonobj = (JObject)javascript[i];
                        tb_kglzb.Rows.Add(new object[] { jsonobj["PK"].ToString(), jsonobj["XZMC"].ToString(), jsonobj["JSYXM"].ToString(), jsonobj["CLHP"].ToString() });
                    }                  
                }               
            }           
            return tb_kglzb;
        }


        /// <summary>
        /// 取钥匙
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public bool saveQys(string sbbm, string ycsqdpk)
        {
            Hashtable p = new Hashtable();
            p.Add("sbbm", sbbm);
            p.Add("ycsqdpk", ycsqdpk);
            string Json= doCallingInterface_String("clgl", "saveqys", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    return true;
                }
                else 
                {
                    return false;
                }
            }
            else 
            {
                return false;
            }
        }


        /// <summary>
        /// 根据钥匙编号获取用车申请单信息
        /// </summary>
        /// <param name="sbbm"></param>
        /// <param name="wzm"></param>
        /// <returns></returns>
        public Hashtable getycsqdpkInfo(string sbbm ,string wzm) 
        {

            Hashtable outtb = new Hashtable();
            Hashtable p = new Hashtable();
            p.Add("sbbm", sbbm);
            p.Add("wzm", wzm);
            string Json = doCallingInterface_String("clgl", "getycsqdinfo", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    if (jsonObj["data"] != null)
                    {
                        JObject vmodel = (JObject)jsonObj["data"];
                        outtb.Add("DH", vmodel["DH"].ToString());
                        outtb.Add("SFPL", vmodel["SFPL"].ToString());
                        outtb.Add("PK", vmodel["PK"].ToString());                      
                    }
                }               
            }
            return outtb;
        }


        /// <summary>
        /// 还钥匙
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public bool saveHys(string sbbm, string ycsqdpk, string clzkpj, string clwgpj, string clnspj)
        {
            Hashtable p = new Hashtable();
            p.Add("sbbm", sbbm);
            p.Add("ycsqdpk", ycsqdpk);
            p.Add("clzkpj", clzkpj);
            p.Add("clwgpj", clwgpj);
            p.Add("clnspj", clnspj);
            string Json = doCallingInterface_String("clgl", "savehys", p);
            if (!String.IsNullOrEmpty(Json))
            {
                JObject jsonObj = JsonConvert.DeserializeObject<JObject>(Json);
                string r = jsonObj["r"].ToString();
                if (r.ToLower() == "true")
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }


        ///

        public Hashtable saveYcsq() 
        {
            Hashtable p = new Hashtable();
            return null;
        }



























        /// <summary>
        /// 根据钥匙编码获取钥匙信息
        /// </summary>
        /// <param name="key">钥匙编码</param>
        /// <returns></returns>
        public Hashtable getInfoByKey(string key,string lx)
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

      
        public Hashtable savePcd(string ycsy, string mdd, string sjhm, string ysbh)
        {
            Hashtable p = new Hashtable();
            p.Add("ycsy", ycsy);
            p.Add("mdd", mdd);
            p.Add("sjhm", sjhm);
            p.Add("ysbh", ysbh);
            return doCallingInterface("clgl", "savepcd", p);
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
