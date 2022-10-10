using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace BLL
{
    //连接上读写器基本信息
    public class ReaderDeviceIDInfo : INotifyPropertyChanged
    {
        private string m_strDeviceID;       //读写器设备序列号
        private string m_strReaderIPOrCom;  //读写器IP地址或者Com号
        public ReaderDeviceIDInfo()
        {
            m_strDeviceID = string.Empty;
            m_strReaderIPOrCom = string.Empty;
        }
        public ReaderDeviceIDInfo(string strDeviceID,string strReaderIP)
        {
            m_strDeviceID = strDeviceID;
            m_strReaderIPOrCom = strReaderIP;
        }
        //设备序列号
        public string DeviceID
        {
            get { return m_strDeviceID; }
            set
            {
                m_strDeviceID = value;
                OnPropertyChanged(new PropertyChangedEventArgs("DeviceID"));
            }
        }
        //IP地址或者Com号
        public string ReaderIPOrCom
        {
            get { return m_strReaderIPOrCom; }
            set
            {
                m_strReaderIPOrCom = value;
                OnPropertyChanged(new PropertyChangedEventArgs("ReaderIPOrCom"));
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, e);
            }
        }

        //重写ToString方法,返回列表中的EPC
        public override string ToString()
        {
            return m_strDeviceID;
        }
    }
}
