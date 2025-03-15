using System.Diagnostics;
using static VRChatYoutubeLoginFix.Program;

namespace VRChatYoutubeLoginFix {
    internal class Program {
        // This code intercepts commands meant for yt-dlp so that we can add cookie login commands to whatever VR Chat is sending.
        static void Main(string[] args) {
            if (args.Length == 0) {
                args = new string[] {
                "https://youtu.be/OqjSg8gipBs?si=-EqZ5YHBlzfxQH5q"
                };
            }
            // We use this for logs currently.
            Guid guid = Guid.NewGuid();
            
            // Set up our paths.
            string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp-real.exe");
            string cookiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\");
            Directory.CreateDirectory(logFolder);
            string logPath = Path.Combine(logFolder, guid.ToString() + "log.txt");
            string log = "";
            // Try getting cookies from all common browsers until one works.
            // Personally haven't had anything other than raw exported cookies work on my own machine
            // but yt-dlp has browser cookie grabbing in its arguments, so maybe they can work for somebody else. 
            if (GetVideoPath(cookiesPath, args, applicationPath, ref log, BrowserType.RawCookies)) { 
                return;
            } else if (GetVideoPath(cookiesPath, args, applicationPath, ref log, BrowserType.Chrome)) {
                return;
            } else if (GetVideoPath(cookiesPath, args, applicationPath, ref log, BrowserType.Firefox)) {
                return;
            } else if (GetVideoPath(cookiesPath, args, applicationPath, ref log, BrowserType.Edge)) {
                return;
            } else if (GetVideoPath(cookiesPath, args, applicationPath, ref log, BrowserType.Brave)) {
                return;
            } else {
                // We have failed to get any of the data we needed. Save the log to disk.
                try { 
                    File.WriteAllText(logPath, log);
                } catch (Exception e) {
                }
            }
        }
        public enum BrowserType {
            RawCookies,
            Chrome,
            Brave,
            Safari,
            Firefox,
            Edge
        }
        private static bool GetVideoPath(string cookiesPath, string[] args,
            string applicationPath, ref string log, BrowserType browserType) {
            string output = "";
            string error = "";

            List<string> argsList = new List<string>();

            log += "Attempt getting youtube login cookie using " + browserType.ToString() + "\r\n";

            // Add our cookies
            if (browserType == BrowserType.RawCookies) {
                argsList.Add("--cookies");
                argsList.Add(cookiesPath);
            } else {
                argsList.Add("--cookies-from-browser");
                argsList.Add(browserType.ToString().ToLower());
            }

            // Filter and add the commands VR Chat wants to send.
            for (int i = 0; i < args.Length; i++) {
                bool validatingArgs = true;
                while (validatingArgs) {
                    // These command flags are not supported by the yt-dlp we are using, so we filter them out.
                    if (args[i] == "--exp-allow" || args[i] == "--wild-allow") {
                        i += 2;
                    } else {
                        validatingArgs = false;
                    }
                }
                argsList.Add(args[i]);
            }

            // Send the command arguments to yt-dlp-real.exe
            ProcessStartInfo processStartInfo = new ProcessStartInfo();
            processStartInfo.FileName = applicationPath;
            foreach (var argument in argsList) {
                processStartInfo.ArgumentList.Add(argument);
            }
            processStartInfo.UseShellExecute = false;
            processStartInfo.RedirectStandardOutput = true;
            processStartInfo.RedirectStandardError = true;
            try {
                Process process = new Process();
                process.StartInfo = processStartInfo;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                error = process.StandardError.ReadToEnd();
                process.WaitForExit();
            } catch (Exception e) {
                log += e.Message + "\r\n";
            }

            if (!string.IsNullOrEmpty(output)) {
                // VR Chat reads the video path we grabbed from yt-dlp-real.exe from here.
                Console.WriteLine(output);
            }
            foreach (var argument in argsList) {
                log += argument.ToString() + "\r\n";
            }
            log += output + "\r\n";
            log += error;
            log += "\r\n----------------------------------------------\r\n";

            bool hasErrors = !string.IsNullOrEmpty(error);
            return !hasErrors;
        }
    }
}
