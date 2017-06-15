using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using System.Diagnostics;
using System.Net;
using System.Runtime.Remoting;
using System.IO;
using System.Xml;

namespace UtilityKit {
    public partial class UtilityKit : Form {
        [DllImport("kernel32.dll")]
        static extern uint SetThreadExecutionState(ExecutionFlag flags);

        [Flags]
        enum ExecutionFlag : uint {
            System = 0x00000001,
            Display = 0x00000002,
            Continus = 0x80000000,
        }

        public UtilityKit() {
            InitializeComponent();
            updateIPStatus();
            updateQuickStartItems();
            runtimeTimer.Start();
            unitLog.logDebug("runtimeTimer Started");
        }

        /* mainform event BEGIN */
        private void UtilityKit_Shown(object sender, EventArgs e) {
            this.Visible = false;
        }

        private void UtilityKit_Deactivate(object sender, EventArgs e) {
            if (this.WindowState == FormWindowState.Minimized) {
                this.Visible = false;
            }
        }

        protected override void OnClosing(CancelEventArgs e) {
            e.Cancel = true;
            base.OnClosing(e);
            this.Visible = false;
        }

        private void Submit_Click(object sender, EventArgs e) {
            if (Properties.Settings.Default.isAutoStart) {
                if (!checkBox1.Checked) {           //取消开机自启动
                    MessageBox.Show("将修改注册表以取消开机自启动", "提示");
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.DeleteValue("UtilityKit", false);
                    rk2.Close();
                    rk.Close();
                    Properties.Settings.Default.isAutoStart = false;
                }
            } else {
                if (checkBox1.Checked) {            //设置开机自启动
                    MessageBox.Show("将修改注册表以设置开机自启动", "提示");
                    string path = Application.ExecutablePath;
                    RegistryKey rk = Registry.LocalMachine;
                    RegistryKey rk2 = rk.CreateSubKey(@"Software\Microsoft\Windows\CurrentVersion\Run");
                    rk2.SetValue("UtilityKit", path);
                    rk2.Close();
                    rk.Close();
                    Properties.Settings.Default.isAutoStart = true;
                }
            }
            if (Properties.Settings.Default.isDebug) {
                if (!checkBox2.Checked) {           //取消调试模式
                    Properties.Settings.Default.isDebug = false;
                }
            } else {
                if (checkBox2.Checked) {            //设置调试模式
                    Properties.Settings.Default.isDebug = true;
                }
            }

            Properties.Settings.Default.oaIPAddress = textBox1.Text;
            Properties.Settings.Default.oaGateway = textBox2.Text;
            Properties.Settings.Default.oaMask = textBox3.Text;
            Properties.Settings.Default.oaDNS1 = textBox4.Text;
            Properties.Settings.Default.oaDNS2 = textBox5.Text;
            Properties.Settings.Default.kfIPAddress = textBox6.Text;
            Properties.Settings.Default.kfGateway = textBox7.Text;
            Properties.Settings.Default.kfMask = textBox8.Text;
            Properties.Settings.Default.kfDNS1 = textBox9.Text;
            Properties.Settings.Default.kfDNS2 = textBox10.Text;

            Properties.Settings.Default.Save();
            this.Visible = false;
            updateIPStatus();
        }

        private void Cancel_Click(object sender, EventArgs e) {
            this.Visible = false;
        }
        private void textBox1_TextChanged(object sender, EventArgs e) {
            string[] oaIPAddress = textBox1.Text.Split('.');
            string oaGateway = "";
            string oaMask = "";
            string oaDNS1 = "80.32.241.1";
            string oaDNS2 = "84.32.145.2";

            string kfIPAddress = "";
            string kfGateway = "";
            string kfMask = "";
            string kfDNS1 = "80.32.241.1";
            string kfDNS2 = "84.32.145.2";

            if (oaIPAddress.GetLength(0) == 1) {
                oaGateway = oaIPAddress[0];
                oaMask = "255";

                kfIPAddress = oaIPAddress[0];
                kfGateway = oaIPAddress[0];
                kfMask = "255";
            } else if (oaIPAddress.GetLength(0) == 2) {
                oaGateway = oaIPAddress[0] + "." + oaIPAddress[1];
                oaMask = "255.255";

                kfIPAddress = oaIPAddress[0] + "." + oaIPAddress[1];
                kfGateway = oaIPAddress[0] + "." + oaIPAddress[1];
                kfMask = "255.255";
            } else if (oaIPAddress.GetLength(0) == 3) {
                oaGateway = oaIPAddress[0] + "." + oaIPAddress[1] + "." + oaIPAddress[2];
                oaMask = "255.255.255";
                if (oaIPAddress[2] != "") {
                    kfIPAddress = oaIPAddress[0] + "." + oaIPAddress[1] + "." + (int.Parse(oaIPAddress[2]) - 2).ToString();
                    kfGateway = oaIPAddress[0] + "." + oaIPAddress[1] + "." + (int.Parse(oaIPAddress[2]) - 2).ToString();
                } else {
                    kfIPAddress = oaIPAddress[0] + "." + oaIPAddress[1] + ".";
                    kfGateway = oaIPAddress[0] + "." + oaIPAddress[1] + ".";
                }
                kfMask = "255.255.255";
            } else if (oaIPAddress.GetLength(0) == 4) {
                oaGateway = oaIPAddress[0] + "." + oaIPAddress[1] + "." + oaIPAddress[2] + ".254";
                oaMask = "255.255.255.0";
                kfIPAddress = oaIPAddress[0] + "." + oaIPAddress[1] + "." + (int.Parse(oaIPAddress[2]) - 2).ToString() + "." + oaIPAddress[3];
                kfGateway = oaIPAddress[0] + "." + oaIPAddress[1] + "." + (int.Parse(oaIPAddress[2]) - 2).ToString() + ".254";

                kfMask = "255.255.255.0";
            }
            textBox2.Text = oaGateway;
            textBox3.Text = oaMask;
            textBox4.Text = oaDNS1;
            textBox5.Text = oaDNS2;
            textBox6.Text = kfIPAddress;
            textBox7.Text = kfGateway;
            textBox8.Text = kfMask;
            textBox9.Text = kfDNS1;
            textBox10.Text = kfDNS2;
        }
        /* mainform event END */

        /* menuitem event BEGIN */
        private void exitMenuItem_Click(object sender, EventArgs e) {
            notifyIcon.Visible = false;
            System.Environment.Exit(0);
        }

        private void dashboardMenuItem_Click(object sender, EventArgs e) {
            showDashboard();
        }
        private void monitorMenuItem_Click(object sender, EventArgs e) {
            if (monitorMenuItem.Checked) {
                monitorMenuItem.CheckState = CheckState.Unchecked;
                meetingTimer.Stop();
                unitLog.logDebug("Meeting Mode Stopped");
            } else {
                monitorMenuItem.CheckState = CheckState.Checked;
                meetingTimer.Start();
                unitLog.logDebug("Meeting Mode Started");
            }
        }
        private void chgIP2KFMenuItem_Click(object sender, EventArgs e) {
            if (!isIPSet(2)) {
                string[] cmd = new string[3];
                cmd[0] = "netsh interface ipv4 set address name=本地连接 static addr=" + Properties.Settings.Default.kfIPAddress
                    + " mask=" + Properties.Settings.Default.kfMask
                    + " gateway=" + Properties.Settings.Default.kfGateway + " gwmetric=1";
                cmd[1] = "netsh interface ipv4 set dnsservers name=本地连接 static addr=" + Properties.Settings.Default.kfDNS1 + " register=PRIMARY";
                cmd[2] = "netsh interface ipv4 add dnsservers name=本地连接 addr=" + Properties.Settings.Default.kfDNS2;
                unitCmd.run(cmd);
                chgIPItemState("KF");
            } else {
                MessageBox.Show("您设置的开发网段配置信息不全，点击确定进入设置", "提示");
                showDashboard();
            }
        }

        private void chgIP2OAMenuItem_Click(object sender, EventArgs e) {
            if (!isIPSet(1)) {
                string[] cmd = new string[3];
                cmd[0] = "netsh interface ipv4 set address name=本地连接 static addr=" + Properties.Settings.Default.oaIPAddress
                    + " mask=" + Properties.Settings.Default.oaMask
                    + " gateway=" + Properties.Settings.Default.oaGateway + " gwmetric=1";
                cmd[1] = "netsh interface ipv4 set dnsservers name=本地连接 static addr=" + Properties.Settings.Default.oaDNS1 + " register=PRIMARY";
                cmd[2] = "netsh interface ipv4 add dnsservers name=本地连接 addr=" + Properties.Settings.Default.oaDNS2;
                unitCmd.run(cmd);
                chgIPItemState("OA");
            } else {
                MessageBox.Show("您设置的办公网段配置信息不全，点击确定进入设置", "提示");
                showDashboard();
            }
        }

        private void chgIP2ATMenuItem_Click(object sender, EventArgs e) {
            string[] cmd = new string[3];
            cmd[0] = "netsh interface ipv4 set address name=本地连接 source=dhcp";
            cmd[1] = "netsh interface ipv4 set dnsservers name=本地连接 source=dhcp";
            cmd[2] = "";
            unitCmd.run(cmd);
            chgIPItemState("AT");
        }
        /* menuitem event END */

        /* timer event BEIGIN */
        private void meetingTimer_Tick(object sender, EventArgs e) {
            SetThreadExecutionState(ExecutionFlag.System | ExecutionFlag.Display);
            unitLog.logDebug("Success Wakeup");
        }
        /* timer event END */
        private void showDashboard() {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
            this.BringToFront();
            textBox1.Text = Properties.Settings.Default.oaIPAddress;
            textBox2.Text = Properties.Settings.Default.oaGateway;
            textBox3.Text = Properties.Settings.Default.oaMask;
            textBox4.Text = Properties.Settings.Default.oaDNS1;
            textBox5.Text = Properties.Settings.Default.oaDNS2;
            textBox6.Text = Properties.Settings.Default.kfIPAddress;
            textBox7.Text = Properties.Settings.Default.kfGateway;
            textBox8.Text = Properties.Settings.Default.kfMask;
            textBox9.Text = Properties.Settings.Default.kfDNS1;
            textBox10.Text = Properties.Settings.Default.kfDNS2;
            if (Properties.Settings.Default.isAutoStart) {
                checkBox1.CheckState = CheckState.Checked;
            } else {
                checkBox1.CheckState = CheckState.Unchecked;
            }
            if (Properties.Settings.Default.isDebug) {
                checkBox2.CheckState = CheckState.Checked;
            } else {
                checkBox2.CheckState = CheckState.Unchecked;
            }
        }

        private void updateIPStatus() {
            if (!isIPSet(0)) {
                IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());
                IPAddress[] ipAddr = ipHost.AddressList;
                for (int i = 0; i < ipAddr.Length; i++) {
                    if (ipAddr[i].ToString().Equals(Properties.Settings.Default.oaIPAddress)) {
                        chgIPItemState("OA");
                    } else if (ipAddr[i].ToString().Equals(Properties.Settings.Default.kfIPAddress)) {
                        chgIPItemState("KF");
                    }
                }
            }
        }
        private void chgIPItemState(string menuItem) {
            if (menuItem.Equals("AT")) {
                chgIP2OAMenuItem.Enabled = true;
                chgIP2KFMenuItem.Enabled = true;
                chgIP2ATMenuItem.Enabled = false;
            } else if (menuItem.Equals("OA")) {
                chgIP2OAMenuItem.Enabled = false;
                chgIP2KFMenuItem.Enabled = true;
                chgIP2ATMenuItem.Enabled = true;
            } else if (menuItem.Equals("KF")) {
                chgIP2OAMenuItem.Enabled = true;
                chgIP2KFMenuItem.Enabled = false;
                chgIP2ATMenuItem.Enabled = true;
            }
        }
        private bool isIPSet(int method) {
            bool isSet = true;
            switch (method) {
                case 0:
                    isSet = string.IsNullOrEmpty(Properties.Settings.Default.oaIPAddress) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaGateway) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaMask) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaDNS1) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaDNS2) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfIPAddress) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfGateway) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfMask) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfDNS1) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfDNS2);
                    break;
                case 1:
                    isSet = string.IsNullOrEmpty(Properties.Settings.Default.oaIPAddress) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaGateway) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaMask) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaDNS1) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.oaDNS2);
                    break;
                case 2:
                    isSet = string.IsNullOrEmpty(Properties.Settings.Default.kfIPAddress) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfGateway) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfMask) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfDNS1) ||
                           string.IsNullOrEmpty(Properties.Settings.Default.kfDNS2);
                    break;
                default:
                    break;
            }
            return isSet;

        }

        private void notifyIcon_MClick(object sender, MouseEventArgs e) {
            if (e.Button == MouseButtons.Left) {
                string msg = "当前IP状态:\t";
                if (chgIP2OAMenuItem.Enabled == false) {
                    msg += "办公网段";
                } else if (chgIP2KFMenuItem.Enabled == false) {
                    msg += "开发网段";
                } else if (chgIP2ATMenuItem.Enabled == false) {
                    msg += "DHCP获取网段";
                } else {
                    msg += "未知";
                }
                msg += "\n会议模式:\t";
                msg += monitorMenuItem.Checked == true ? "已开启" : "已关闭";
                msg += "\n当前版本:\t1.4\n";
                notifyIcon.ShowBalloonTip(1000, "系统运行状态", msg, new ToolTipIcon());

            } else if (e.Button == MouseButtons.Right) {

            } else if (e.Button == MouseButtons.Middle) {

            } else {

            }
        }

        private void updateQuickStartItems() {
            unitXml xml = new unitXml();
            if (!xml.FileExist()) {
                xml.CreateXmlFile();
            }
            quickStartMenuItem.DropDownItems.Clear();
            xml.LoadAllConfig(quickStartMenuItem);

            if (!quickStartMenuItem.HasDropDownItems) {
                quickStartMenuItem.Enabled = false;
                quickStartMenuItem.Text = "快速启动 - 未设置";
            } else {
                quickStartMenuItem.Enabled = true;
                quickStartMenuItem.Text = "快速启动";
            }
        }

        private void button1_Click(object sender, EventArgs e) {
            updateIPStatus();
            MessageBox.Show("已更新");
        }

        private void button2_Click(object sender, EventArgs e) {
            updateQuickStartItems();
            MessageBox.Show("已更新");
        }

        private void button3_Click(object sender, EventArgs e) {
           
        }

        private void button4_Click(object sender, EventArgs e) {

        }


        [DllImport("User32.dll", EntryPoint = "FindWindow")]
        private static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        [DllImport("User32.dll")]
        private static extern int SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);


        private void runtimeTimer_Tick(object sender, EventArgs e) {

            IntPtr hWnd = FindWindow(null, "Lotus Notes");
            if (hWnd != IntPtr.Zero) {
                SendMessage(hWnd, 0x0010, 0, 0);
                notifyIcon.ShowBalloonTip(500, "邮件提醒", "您有新的邮件到达", new ToolTipIcon());
            }

        }

    }
}
