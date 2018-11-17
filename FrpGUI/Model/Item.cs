using FrpGUI.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FrpGUI.Model
{
    public enum TYPE
    {
        tcp, http, udp
    }


    public delegate void CheckChange(Item i,string propertyName);

    /// <summary>
    /// 转发的model
    /// 通过change这个委托，来判断是修改配置还是重新加载配置
    /// </summary>
    public class Item : INotifyPropertyChanged
    {
        public IEnumerable<string> TYPE
        {
            get
            {
                yield return "TCP";
                yield return "HTTP";
                yield return "UDP";
            }
        }

        const string path = @"D:\Application\frp_0.21.0_windows_amd64\";

        //委托
        public CheckChange change;

        Frp frp = Frp.GetInstance();

        string _name;
        string _ip;
        string _localPort;
        TYPE _type;
        string _remotePort;
        bool? _isSelected = false;


        #region

        public bool? IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;
                OnPropertyChanged();
                //change?.Invoke(this);
                //if (null == change)
                //{
                //    try
                //    {
                //        frp.ReloadConfig(this);
                //    }
                //    catch (Exception e)
                //    {
                //        Console.WriteLine(e.Message);
                //    }
                //}
            }
        }
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name == value) return;
                _name = value;
                OnPropertyChanged();
            }
        }
        public TYPE Type
        {
            get { return _type; }
            set
            {
                if (_type == value) return;
                _type = value;
                OnPropertyChanged();
            }
        }
        public string Ip
        {
            get { return _ip; }
            set
            {
                if (_ip == value) return;
                _ip = value;
                OnPropertyChanged();
            }
        }
        public string LocalPort
        {
            get { return _localPort; }
            set
            {
                if (_localPort == value) return;
                _localPort = value;
                OnPropertyChanged();
            }
        }
        public string RemotePort
        {
            get { return _remotePort; }
            set
            {
                if (_remotePort == value) return;
                _remotePort = value;
                OnPropertyChanged();
            }
        }

        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {

            change?.Invoke(this,propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
