using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;
using System.IO;
using System.Collections;  

namespace UtilityKit {

    class unitXml {

        private string _Path = "C:\\UtilityKitConfig.xml";
        private Hashtable menuInfo = new Hashtable();


        public bool FileExist() {
            if (File.Exists(_Path)) {
                return true;
            } else {
                return false;
            }
        }

        public void CreateXmlFile()  
        {
            XmlDocument xmlDoc = new XmlDocument();
            //创建类型声明节点  
            XmlNode node = xmlDoc.CreateXmlDeclaration("1.0", "utf-8", "");
            xmlDoc.AppendChild(node);
            //创建根节点  
            XmlNode root = xmlDoc.CreateElement("Configs");
            xmlDoc.AppendChild(root);

            XmlNode node1 = xmlDoc.CreateNode(XmlNodeType.Element, "Menu", null);
            CreateNode(xmlDoc, node1, "name", "");
            CreateNode(xmlDoc, node1, "path", "");
            CreateNode(xmlDoc, node1, "args", "");
            root.AppendChild(node1);

            XmlNode node2 = xmlDoc.CreateNode(XmlNodeType.Element, "Hotkey", null);
            CreateNode(xmlDoc, node2, "command", "");
            CreateNode(xmlDoc, node2, "keys", "");
            root.AppendChild(node2);

            try {
                xmlDoc.Save(_Path);
            } catch (Exception e) {
                MessageBox.Show("生成配置文件失败！");
                unitLog.logError("生成配置文件失败:" + e.Message);
            }
 
        }  
  
        public void CreateNode(XmlDocument xmlDoc,XmlNode parentNode,string name,string value)  
        {  
            XmlNode node = xmlDoc.CreateNode(XmlNodeType.Element, name, null);  
            node.InnerText = value;  
            parentNode.AppendChild(node);  
        }

        public void LoadAllConfig(ToolStripMenuItem ToolStripMenuItem) {
            XmlDocument xmldoc = new XmlDocument();

            try{
                xmldoc.Load(_Path);    
                XmlNode node = xmldoc.SelectSingleNode("Configs");
                if (node != null) {
                    int i = 0;
                    string[] itemInfo = new string[3];
                    foreach (XmlNode xnode in xmldoc.SelectNodes("Configs/Menu")) {
                        i = 0;
                        foreach (XmlNode xcnode in xnode.ChildNodes) {
                            itemInfo[i++] = xcnode.InnerText.Replace("\t", "").Replace("\r","").Replace("\n",""); 
                        }
                        ToolStripMenuItem toolItem = new ToolStripMenuItem();
                        toolItem.Text = itemInfo[0];
                        toolItem.Click += new EventHandler(item_Click);
                        if (itemInfo[0] != null && itemInfo[0].Length > 1) {
                            ToolStripMenuItem.DropDownItems.Add(toolItem);
                            menuInfo.Add(itemInfo[0],itemInfo[1] + " " + itemInfo[2]);
                        }
                    }
                    foreach (XmlNode xnode in xmldoc.SelectNodes("Configs/Hotkey")) {
                        i = 0;
                        foreach (XmlNode xcnode in xnode.ChildNodes) {
                            itemInfo[i++] = xcnode.InnerText.Replace("\t", "").Replace("\r", "").Replace("\n", "");
                        }
                        //unitHotKey hk = new unitHotKey();
                        //hk.Regist(this.Handle, (int)unitHotKey.HotkeyModifiers.Control + (int)unitHotKey.HotkeyModifiers.Alt, Keys.E, CallBack);
       

                    }
                }
            } catch (Exception e) {
                MessageBox.Show("加载配置文件失败！");
                unitLog.logError("加载配置文件失败:" + e.Message);
            }
        }
     
        public void item_Click(object sender, EventArgs e) {
            string[] cmd = new string[1];
            cmd[0] = "start " + menuInfo[sender.ToString()];
            unitLog.logDebug(cmd[0]);
            unitCmd.run(cmd);
        }

    }    
} 