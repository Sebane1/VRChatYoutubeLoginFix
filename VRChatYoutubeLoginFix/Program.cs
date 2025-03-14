using System.Diagnostics;

namespace VRChatYoutubeLoginFix {
    internal class Program {
        // This code intercepts commands meant for yt-dlp so that we can add cookie login commands to whatever VR Chat is sending.
        static void Main(string[] args) {
            Guid guid = Guid.NewGuid();
            string applicationPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "yt-dlp-real.exe");
            string cookiesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cookies.txt");
            string cacheDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "cache");
            string logFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"logs\");
            Directory.CreateDirectory(logFolder);
            string logPath = Path.Combine(logFolder, guid.ToString() + "log.txt");
            string output = "";
            string error = "";
            string log = "";

            List<string> argsList = new List<string>();

            // Add our cookies
            argsList.Add("--cookies");
            argsList.Add(cookiesPath);
            
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

            // Sent the command arguments to the real yt-dlp
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
                Console.WriteLine(output);
            }
            foreach (var argument in argsList) {
                log += argument.ToString() + "\r\n";
            }
            log += output + "\r\n";
            log += error;

            // Write any errors to log file.
            if (!string.IsNullOrEmpty(error)) {
                File.WriteAllText(logPath, log);
            }
        }
    }
}
