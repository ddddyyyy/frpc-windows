using Caliburn.Micro;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FrpGUI.ViewModels.Page
{
    public class HomePageViewModel : Screen, IShell
    {
        static HomePageViewModel model;
        public HomePageViewModel GetInstance()
        {
            if (null == model) model = new HomePageViewModel();
            return model;
        }
    }
}
