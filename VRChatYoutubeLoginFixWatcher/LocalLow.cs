using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace VRChatYoutubeLoginFixWatcher {
    public static class LocalLow {
        public static string GetLocalLow() {
            return Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).Replace("Roaming", "LocalLow");
        }
    }
}
