using Caliburn.Micro;
using FrpGUI.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace FrpGUI.ViewModels.Page
{
    

    class ConfigViewModel : Screen,IShell
    {
        static ConfigViewModel model;
        public static ConfigViewModel GetInstance()
        {
            if (null == model)
            {
                model = new ConfigViewModel();
            }
            return model;
        }


        bool ChangeAll = false;
        Frp frp = Frp.GetInstance();

        public ConfigViewModel()
        {
            Items = frp.GetItems(Change, 1);
            IsAllItemsSelected = false;
        }

        #region Bindings

        public ObservableCollection<Item> Items
        {
            get { return _items; }
            set
            {
                _items = value;
                NotifyOfPropertyChange(() => Items);
            }
        }
        private ObservableCollection<Item> _items;

        public bool? IsAllItemsSelected
        {
            get { return _isAllItemsSelected; }
            set
            {
                _isAllItemsSelected = value;
                NotifyOfPropertyChange(() => IsAllItemsSelected);
            }
        }
        private bool? _isAllItemsSelected;

        #endregion


        #region Event

        /// <summary>
        /// 添加新的映射
        /// </summary>
        public void Add()
        {
            Item item = new Item()
            {
                Name = "默认",
                Ip = "127.0.0.1",
                LocalPort = "6666",
                RemotePort = "6666",
                Type = TYPE.tcp,
                change = Change
            };
            Items.Insert(0, item);
            item.Name = "123";
            IsAllItemsSelected = false;
        }

        /// <summary>
        /// 删除映射
        /// </summary>
        public void Delete()
        {
            for (int i=Items.Count -1; i >=0; --i)
            {
                if (Items[i].IsSelected == true)
                {
                    frp.delete(Frp.FullConfig, Items[i]);
                    Items.RemoveAt(i);
                }
            }
            IsAllItemsSelected = false;
        }

        /// <summary>
        /// 全选的动作
        /// </summary>
        public void All()
        {
            
            if (IsAllItemsSelected == false)
            {
                IsAllItemsSelected = true;
            }
            else if (IsAllItemsSelected == true)
            {
                IsAllItemsSelected = false;
            }
            else
            {
                return;
            }
            if (_isAllItemsSelected.HasValue)
            {
                ChangeAll = true;
                foreach (Item item in Items)
                {
                    item.IsSelected = IsAllItemsSelected;
                }
                ChangeAll = false;
            }
        }

        /// <summary>
        /// 复选框确认的委托，分是否为全选的两种情况
        /// </summary>
        public void Change(Item i,string name)
        {
            if (null != name && !name.Equals("IsSelected"))
            {
                frp.EditConfig(i,Frp.FullConfig);
                return;
            }
            if (ChangeAll)
            {
                return;
            }
            if (IsAllItemsSelected.HasValue)
            {
                int count = 0;
                foreach (Item item in Items)
                {
                    bool result;
                    if (i == item)
                    {
                        result = i.IsSelected == false;
                    }
                    else
                    {
                        result = item.IsSelected == false;
                    }
                    if (result)
                    {
                        IsAllItemsSelected = false;
                        break;
                    }
                    
                    ++count;
                }
                if (count == Items.Count)
                {
                    IsAllItemsSelected = true;
                }
            }
        }

        #endregion

    }
}
