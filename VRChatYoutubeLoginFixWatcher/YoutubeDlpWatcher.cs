using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VRChatYoutubeLoginFixWatcher {
    public class YoutubeDlpWatcher {
        private string _watcherPath;
        private string _argumentInterceptor;
        private string _argumentInterceptorDll;
        private string _cookiesPath;
        private string _ytdlp;
        private FileSystemWatcher _fileWatcher;
        private string _previousCookies;

        public YoutubeDlpWatcher() {
            _watcherPath = Path.Combine(LocalLow.GetLocalLow(), "VRChat\\VRChat\\Tools");
            _argumentInterceptor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            _argumentInterceptorDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.dll");
            _cookiesPath = Path.Combine(LocalLow.GetLocalLow(), "cookies.txt");
            _ytdlp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp-real.exe");

            RefreshData();
            _fileWatcher = new FileSystemWatcher(_watcherPath, "yt-dlp");
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.Created += _fileWatcher_Changed;
            _fileWatcher.Changed += _fileWatcher_Changed;
        }

        void RefreshData() {
            AggressiveCopy(_argumentInterceptor);
            AggressiveCopy(_argumentInterceptorDll);
            AggressiveCopy(_ytdlp);
        }

        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e) {
            RefreshData();
        }

        private void AggressiveCopy(string path) {
            string output = Path.Combine(_watcherPath, Path.GetFileName(path));
            bool succeeded = false;
            while (!succeeded) {
                try {
                    File.Copy(path, output, true);
                    succeeded = true;
                } catch {
                    Thread.Sleep(1000);
                }
            }
        }
        public void SaveCookies(string cookies) {
            if (_previousCookies != cookies) {
                File.WriteAllText(_cookiesPath, cookies);
                _previousCookies = cookies;
            }
        }
    }
}
