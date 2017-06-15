using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace UtilityKit {
    class unitCmd {
        static Process p = new Process();
        public static void run(string[] cmd) {
            for (int i = 0; i < cmd.Length; i++) {
                run(cmd[i]);
            }
        }
        private static void run(string cmd) {       
            p.StartInfo.FileName = "cmd.exe";//要执行的程序名称 
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;//可能接受来自调用程序的输入信息 
            p.StartInfo.RedirectStandardOutput = true;//由调用程序获取输出信息 
            p.StartInfo.CreateNoWindow = true;//不显示程序窗口 
            p.Start();//启动程序 
            p.StandardInput.WriteLine(cmd);
        }
    }
}
