using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HFUTIEDisplay
{
    static class Program
    {
        static Mutex _mutex;
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool createNew;//一个电脑只能有一个程序运行，用于做判断
            //获取程序集Guid作为唯一标识
            Attribute guid_attr = Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(GuidAttribute));
            string guid = ((GuidAttribute)guid_attr).Value;
            _mutex = new Mutex(true, guid, out createNew);
            if (createNew)
            {
                _mutex.ReleaseMutex();
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);             
                Application.Run(new MainForm());
            }
            else  //发现重复进程
            {
                MessageBox.Show("已经运行了一个程序了，不能再运行另一个了。");
            }
        }
    }
}
