using Microsoft.VisualBasic.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VRChatYoutubeLoginFixWatcher {
    public class YoutubeDlpWatcher {
        private string _watcherPath;
        private string _argumentInterceptor;
        private string _phantomJs;
        private string _argumentInterceptorDll;
        private string _cookiesFromBrowserFix;
        private string _cookiesPath;
        private string _ytdlp;
        private FileSystemWatcher _fileWatcher;
        private string _previousCookies;
        private bool refreshingData;

        public YoutubeDlpWatcher() {
            // Set up our paths.
            _watcherPath = Path.Combine(LocalLow.GetLocalLow(), "VRChat\\VRChat\\Tools");
            _argumentInterceptor = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.exe");
            _phantomJs = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "phantomjs.exe");
            _argumentInterceptorDll = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp.dll");
            _cookiesFromBrowserFix = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp-plugins");
            _cookiesPath = Path.Combine(_watcherPath, "cookies.txt");
            _ytdlp = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp-real.exe");
            UpdateYtDlp();
            // Copy over the data we need for things to function.
            CopyFilesRecursively(_cookiesFromBrowserFix, Path.Combine(_watcherPath, Path.GetFileNameWithoutExtension(_cookiesFromBrowserFix + ".folder")));

            AggressiveCopy(_phantomJs);
            AggressiveCopy(_argumentInterceptorDll);
            AggressiveCopy(_ytdlp);
            RefreshData();
            // Set up the file watcher. We need to replace yt-dlp.exe again if VR Chat puts its own back in.
            _fileWatcher = new FileSystemWatcher(_watcherPath, "yt-dlp.exe");
            _fileWatcher.EnableRaisingEvents = true;
            _fileWatcher.Created += _fileWatcher_Changed;
            _fileWatcher.Changed += _fileWatcher_Changed;
        }
        void UpdateYtDlp() {
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = _ytdlp;
            processStartInfo.ArgumentList.Add("-U");
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;

            try {
                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                Console.Write(process.StandardOutput.ReadToEnd());
                Console.Write(process.StandardError.ReadToEnd());
                process.WaitForExit();
            } catch (Exception e) {
                ;
            }
        }
        void RefreshData() {
            // Make sure we aren't already refreshing file data.
            if (!refreshingData) {
                refreshingData = true;
                // Copy the needed files.
                AggressiveCopy(_argumentInterceptor);
                refreshingData = false;
            }
        }

        private void _fileWatcher_Changed(object sender, FileSystemEventArgs e) {
            // If this has triggered, VR Chat has swapped out our fix with its original yt-dlp.exe.
            // Time to replace it with our fix again.
            RefreshData();
        }

        /// <summary>
        /// Attempts to copy a file until it succeeds.
        /// </summary>
        /// <param name="path"></param>
        private void AggressiveCopy(string path) {
            string output = Path.Combine(_watcherPath, Path.GetFileName(path));
            bool succeeded = false;
            if (File.Exists(path)) {
                while (!succeeded) {
                    try {
                        File.Copy(path, output, true);
                        succeeded = true;
                    } catch {
                        Thread.Sleep(1000);
                    }
                }
            }
        }
        private static void CopyFilesRecursively(string sourcePath, string targetPath) {
            Directory.CreateDirectory(targetPath);

            // Now Create all of the directories.
            foreach (string dirPath in Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)) {
                Directory.CreateDirectory(dirPath.Replace(sourcePath, targetPath));
            }

            // Copy all the files and replace any files with the same name.
            foreach (string newPath in Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories)) {
                File.Copy(newPath, newPath.Replace(sourcePath, targetPath), true);
            }
        }
        public void SaveCookies(string cookies) {
            // Check to see if we already saved this cookie data.
            if (_previousCookies != cookies) {
                // Save the cookies.
                Console.WriteLine("Youtube cookies saved locally for VR Chat use.");
                File.WriteAllText(_cookiesPath, cookies);
                _previousCookies = cookies;
            }
        }
    }
}
