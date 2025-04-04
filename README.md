A last resort solution for when youtube demands you login to your account to watch content in VR Chat, and additionally adds some more robust solutions for mitigating youtube restricting access.
This application works by intercepting VR Chats command arguments to yt-dlp.exe and adding cookie login arguments to allow youtube playback to function again.

Additionally adds error logging whenever playback fails for whatever reason to "%user%\AppData\LocalLow\VRChat\VRChat\Tools\logs"

**HOW TO USE**

**Step 1:**
You will need to go to your VR Chat installation, find "launch.exe", right click and go to properties, and under compatiblility set it to run as Administrator from now on, and click apply. For whatever reason this fix fails to function when not running VR Chat as administrator.

**Step 2:** 
Download the latest version of this tool from this repository under "[Releases](https://github.com/Sebane1/VRChatYoutubeLoginFix/releases)", extract the zip file anywhere, and run VRChatYoutubeLoginFixWatcher.exe.
Keep it running for as long as VR Chat is running from now on, otherwise the fix will get replaced by VR Chats version of yt-dlp, which will undo the fix.

**Step 3:**
Get this chrome based extension to export your cookie data from youtube, or an equvalent extension. Make sure its in netscape mode and not json. https://chromewebstore.google.com/detail/get-cookiestxt-locally/cclelndahbckbenkjhflpdbgdldlbecc?hl=en

**Step 4:** 
With youtube open and logged in via an incognito browser tab, use the extension and click copy, or save a cookies.txt to "%user%\AppData\LocalLow\VRChat\VRChat\Tools" (copying to clipboard will auto create the cookies.txt file for you).

The official [github](https://github.com/yt-dlp/yt-dlp/wiki/Extractors#exporting-youtube-cookies) of yt-dlp does warn that overdownloading on an account could lead to account suspension, so potentially use a burner account or be mindful of how many videos are played at once.

**Step 5:** 
Youtube videos should now work again in VR Chat. If they stop working again, you will need to grab new cookies via step 4.
