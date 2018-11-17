using FrpGUI.Model;
using FrpGUI.ViewModels.Page;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace FrpGUI
{

    class Frp
    {
        //路径
        static string path = AppDomain.CurrentDomain.BaseDirectory + "Resources\\";
        public static string MainConfig = path + "frpc.ini";
        public static string FullConfig = path + "frpc_full.ini";
        public static string Excute = path + "frpc.exe";

        IniFile MainFile;
        IniFile FullFile;

        //控制台进程
        Process p;
        //单例
        static Frp frp;
        public static Frp GetInstance()
        {
            if (null == frp)
            {
                frp = new Frp()
                {
                    MainFile = new IniFile(MainConfig),
                    FullFile = new IniFile(FullConfig)
            };
                
            }
            return frp;
        }
        //控制台输出
        public AppendString append;

        public IniSection GetCommon()
        {
            return MainFile.Section("common");
        }

        public void EditCommon(string value,string propertyName)
        {
            IniSection s = MainFile.Section("common");
            s.Set(propertyName, value);
            MainFile.Save(MainConfig);
        }


        /// <summary>
        /// 得到配置文件里面的每一个映射
        /// </summary>
        /// <param name="change">属性修改时对应的代理</param>
        /// <param name="type">是否是完整版本</param>
        /// <returns></returns>
        public ObservableCollection<Item> GetItems(CheckChange change, int type)
        {
            ObservableCollection<Item> Items = new ObservableCollection<Item>();


            List<String> temp = null;

            if (type == 0)
            {

                temp = new List<String>();

                foreach (IniSection section in MainFile.Sections)
                {
                    if (!section.Name.Equals("common"))
                    {
                        temp.Add(section.Name);
                    }
                }
            }


            foreach (IniSection section in FullFile.Sections)
            {
                if (!section.Name.Equals("common"))
                {
                    Item item = new Item()
                    {
                        Name = section.Name,
                        Ip = section.Get("local_ip"),
                        RemotePort = section.Get("remote_port"),
                        LocalPort = section.Get("local_port"),
                        Type = (TYPE)Enum.Parse(typeof(TYPE), section.Get("type")),
                        change = change
                    };
                    if (null != temp && temp.Contains(item.Name))
                    {
                        item.IsSelected = true;
                    }
                    Items.Add(item);
                }
            }
            return Items;
        }

        /// <summary>
        /// 编辑配置文件
        /// </summary>
        /// <param name="item"></param>
        public void EditConfig(Item item, string name)
        {
            IniFile ini;
            if (name.Equals(MainConfig))
            {
                ini = MainFile;
            }
            else
            {
                ini = FullFile;
            }
            //如果不存在则创建，因此直接赋值即可
            IniSection section = ini.Section(item.Name);
            section.Set("local_port", item.LocalPort);
            section.Set("remote_port", item.RemotePort);
            section.Set("local_ip", item.Ip);
            section.Set("type", Enum.GetName(typeof(TYPE), item.Type));
            ini.Save(name);
        }

        public void delete(string name, Item item)
        {
            IniFile ini;
            if (name.Equals(MainConfig))
            {
                ini = MainFile;
            }
            else
            {
                ini = FullFile;
            }
            ini.RemoveSection(item.Name);
            ini.Save(name);
        }

        #region 进程

        /// <summary>
        /// 重新加载frp的配置
        /// </summary>
        /// <param name="item">被修改的转发</param>
        public void ReloadConfig(Item item, string property)
        {
            //通过对主配置文件进行修改，如果转发不存在，则添加
            //如果转发存在且不被选择，那么就将其写进主文件
            if (true == item.IsSelected)
            {
                EditConfig(item, MainConfig);
            }
            else
            {
                delete(MainConfig, item);
            }

            Process process = new Process();
            //调用的程序名称，比如windows下的cmd，linux下的sh或者bash
            process.StartInfo.FileName = Excute;
            //设置一开始的参数
            process.StartInfo.Arguments = "reload " + "-c " + MainConfig;
            //后面要重定向输出，所以这里要设置为false，否则会报错
            process.StartInfo.UseShellExecute = false;
            //重定向输出，用来获取执行结果
            process.StartInfo.RedirectStandardOutput = true;
            //不创建窗口
            process.StartInfo.CreateNoWindow = true;

            process.Start();

            String s = process.StandardOutput.ReadToEnd();

            //MessageBox.Show(s);

        }

        public void FrpStart()
        {
            if (p != null)
            {
                MessageBox.Show("进程已存在");
                return;
            }
            p = new Process();
            //调用的程序名称，比如windows下的cmd，linux下的sh或者bash
            p.StartInfo.FileName = path + "frpc.exe";
            //设置一开始的参数
            p.StartInfo.Arguments = "-c " + path + "frpc.ini";
            //后面要重定向输出，所以这里要设置为false，否则会报错
            p.StartInfo.UseShellExecute = false;
            //重定向输入，用来输入要执行的命令
            p.StartInfo.RedirectStandardInput = true;
            //重定向输出，用来获取执行结果
            p.StartInfo.RedirectStandardOutput = true;
            //不创建窗口
            p.StartInfo.CreateNoWindow = true;
            //监听
            p.OutputDataReceived += new DataReceivedEventHandler((sender, e) =>
            {
                // Prepend line numbers to each line of the output.
                if (!string.IsNullOrEmpty(e.Data))
                {
                    append(e.Data);
                }
            });
            //输入要执行的命令
            //p.StandardInput.WriteLine("");
            //******这里如果是执行cmd的话，一定要输入exit，否则会一直卡住。******
            //p.StandardInput.WriteLine("exit");
            //启动
            p.Start();
            p.BeginOutputReadLine();

        }

        public void FrpStop()
        {
            try
            {
                p.CancelOutputRead();
                p.Kill();
            }
            catch (Exception expection)
            {
                MessageBox.Show("进程已经结束啦" + expection.Message);
            }
            finally
            {
                p = null;
            }
        }
        #endregion
    }
}
