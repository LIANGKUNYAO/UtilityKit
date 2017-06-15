using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;

namespace UtilityKit {
    class unitLog{
        public static void logDebug(string message) {
            if (Properties.Settings.Default.isDebug) {
                log(message, "DEBUG");
            }
        }
        public static void logError(string message) {
            log(message, "ERROR");
        }
        private static void log(string message, string level) {
            File.AppendAllText("C:\\UtilityKitLog.log", level + " "+ DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss ") + message + Environment.NewLine);
        }
    }
}
