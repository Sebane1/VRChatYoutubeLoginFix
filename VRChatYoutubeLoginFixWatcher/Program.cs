namespace VRChatYoutubeLoginFixWatcher {
    internal static class Program {
        private static YoutubeDlpWatcher _youtubeDlpWatcher;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main() {
            _youtubeDlpWatcher = new YoutubeDlpWatcher();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();

            // Periodically check the clipboard for new cookies.
            while (true) {
                if (Clipboard.ContainsText(TextDataFormat.Text)) {
                    string clipboardText = Clipboard.GetText(TextDataFormat.Text);
                    // Make sure the data is cookies
                    if (clipboardText.Contains("# Netscape") && clipboardText.Contains("youtube")) {
                        // Save the cookie data to disk.
                        _youtubeDlpWatcher.SaveCookies(clipboardText);
                    }
                }
                Thread.Sleep(2000);
            }
        }
    }
}