using Caliburn.Micro;
using FrpGUI.Model;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.ViewModels.Page
{
    public delegate void AppendString(string str);

    class StartPageViewModel : Screen,IShell
    {
        static StartPageViewModel model;

        public static StartPageViewModel GetInstance()
        {
            if(model == null)model = new StartPageViewModel();
            return model;
        }


        private string _ip;
        private string _port;
        private ObservableCollection<Item> _items;
        private string _str;
        private Frp frp;

        public StartPageViewModel()
        {
            
            frp = Frp.GetInstance();
            IniSection s = frp.GetCommon();
            _port = s.Get("server_port");
            _ip = s.Get("server_addr");
            _password = s.Get("privilege_token");
            frp.append = AddString ;
            _str = "";
            Items = frp.GetItems(frp.ReloadConfig, 0);
        }

        #region Binding

        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }
        
        public string Ip
        {
            get { return _ip; }
            set
            {
                _ip = value;
                NotifyOfPropertyChange( () => Ip );
                frp.EditCommon(value, "server_addr");
            }
        }

        public string Port
        {
            get { return _port; }
            set
            {
                _port = value;
                NotifyOfPropertyChange(() => Port);
                frp.EditCommon(value, "server_port");
            }
        }

        string _password;
        public string Password
        {
            get { return _password; }
            set
            {
                _password = value;
                NotifyOfPropertyChange(() => Password);
                frp.EditCommon(value, "privilege_token");
            }
        }
        public string Str
        {
            get { return _str; }
            set
            {
                _str = value;
                NotifyOfPropertyChange(() => Str);
            }
        }



        #endregion

        #region Event

        public void Start()
        {
            frp.FrpStart();
        }

        public void AddString(string str)
        {
            Str = Str + "\n" + str;
        }

        public void Stop()
        {
            frp.FrpStop();
        }

        public void Reload()
        {
            Items = frp.GetItems(frp.ReloadConfig, 0);
        }

        public void Empty()
        {
            Str = "";
        }

        #endregion

    }
}
