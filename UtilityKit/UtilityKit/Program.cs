using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Reflection;

namespace UtilityKit {
    static class Program {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Process instance = RunningInstance();
            if (instance == null){
                //没有实例在运行
                Application.Run(new UtilityKit());
            }else{
                //已经有一个实例在运行
                MessageBox.Show("已有一个实例在运行！", "提示信息", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            unitLog.logDebug("Success StartUp");
        }

        private static Process RunningInstance() {
            Process current = Process.GetCurrentProcess();
            Process[] processes = Process.GetProcessesByName(current.ProcessName);
            //遍历与当前进程名称相同的进程列表  
            foreach (Process process in processes) {
                //如果实例已经存在则忽略当前进程  
                if (process.Id != current.Id) {
                    //保证要打开的进程同已经存在的进程来自同一文件路径
                    if (Assembly.GetExecutingAssembly().Location.Replace("/", "\\") == current.MainModule.FileName) {
                        //返回已经存在的进程
                        return process;

                    }
                }
            }
            return null;
        }
    }
}
