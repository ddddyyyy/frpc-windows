using Caliburn.Micro;
using FrpGUI.Model;
using FrpGUI.ViewModels.Page;
using Hardcodet.Wpf.TaskbarNotification;
using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;

namespace FrpGUI.ViewModels
{

    [Export(typeof(IShell))]
    public class MainWindowViewModel : Conductor<IShell>.Collection.OneActive, IShell
    {
        private IWindowManager windowManager;
        private Views.MainWindowView window
        {
            get
            {
                foreach (var i in Views.Keys)
                {
                    return Views.GetValueOrDefault(i) as Views.MainWindowView;
                }
                return null;
            }
        }
        //ListBox的数据源
        public ObservableCollection<DemoItem> List
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => List);
            }
        }
        private ObservableCollection<DemoItem> _items;

        public WindowState State
        {
            set
            {
                if (value == WindowState.Minimized)
                {
                    window.Visibility = Visibility.Hidden;
                }
                _state = State;
            }
            get { return _state; }
        }
        private WindowState _state;

        public ImageSource IconSource
        {
            get
            {
                window.MyNotifyIcon.Icon = new System.Drawing.Icon(AppDomain.CurrentDomain.BaseDirectory + "Resources\\Icon.ico");
                return window.MyNotifyIcon.IconSource;
            }
        }
 

        [ImportingConstructor]
        public MainWindowViewModel(IWindowManager windowManager)
        {
            ActivateItem(new HomePageViewModel());
            List = new ObservableCollection<DemoItem>();

            List.Add(new DemoItem("主页"));
            List.Add(new DemoItem("运行页面"));
            List.Add(new DemoItem("配置页面"));
            
            this.windowManager = windowManager;
            
        }

       

        #region Action

        public void ChangeSelect()
        {
            switch (window.DemoItemsListBox.SelectedIndex)
            {
                case 0:
                    ActivateItem(new HomePageViewModel());
                    break;
                case 1:
                    ActivateItem(StartPageViewModel.GetInstance());
                    break;
                case 2:
                    ActivateItem(ConfigViewModel.GetInstance());
                    break;
            }
            
			window.MenuToggleButton.IsChecked = false;

			window.Show();
        }

        #endregion

        #region NotifyIcon
        

        public void Close(System.ComponentModel.CancelEventArgs e)
        {
            
            e.Cancel = true;
            window.Visibility = Visibility.Hidden;
        }


        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern void SwitchToThisWindow(IntPtr hWnd, bool fAltTab);

        public void NotifyIconClick()
        {
            window.Visibility = Visibility.Visible;
            SwitchToThisWindow(new WindowInteropHelper(window).Handle, true);
        }

        public void Exit()
        {
            Application.Current.Shutdown();
            Frp.GetInstance().FrpStop();
        }

        #endregion

    }
}
