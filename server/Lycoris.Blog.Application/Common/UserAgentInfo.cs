using System.Text.RegularExpressions;

namespace Lycoris.Blog.Application.Common
{
    /// <summary>
    /// 
    /// </summary>
    public class UserAgentInfo
    {
        public string Browser { get; set; } = string.Empty;

        public string BrowserIcon { get; set; } = string.Empty;

        public string OS { get; set; } = string.Empty;

        public string OSIcon { get; set; } = string.Empty;

        public string Device { get; set; } = string.Empty;

        public string DeviceIcon { get; set; } = string.Empty;

        public UserAgentInfo(string userAgent)
        {
            BrowserInit(userAgent);

            OSInit(userAgent);

            DeviceInit(userAgent);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAgent"></param>
        private void BrowserInit(string userAgent)
        {
            foreach (var item in UaRegex)
            {
                var match = Regex.Match(userAgent, item.Value);
                if (match.Success)
                {
                    Browser = item.Client;
                    BrowserIcon = item.ClientIcon;
                    break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAgent"></param>
        private void OSInit(string userAgent)
        {
            // 判断操作系统
            if (Regex.IsMatch(userAgent, @"Windows NT 11.0"))
            {
                OS = "Windows 11";
                OSIcon = "windows.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 10.0"))
            {
                OS = "Windows 10";
                OSIcon = "windows.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 6.2"))
            {
                OS = "Windows 8";
                OSIcon = "windows-8.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 6.1"))
            {
                OS = "Windows 7";
                OSIcon = "windows-7.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 6.0"))
            {
                OS = "Windows Vista";
                OSIcon = "windows-7.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 5.1"))
            {
                OS = "Windows XP";
                OSIcon = "windows-7.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT 5.0"))
            {
                OS = "Windows 2000";
                OSIcon = "windows-2000.png";
            }
            else if (Regex.IsMatch(userAgent, @"Mac OS X"))
            {
                OS = "Mac OS";
                OSIcon = "mac-os.png";
            }
            else if (Regex.IsMatch(userAgent, @"Linux"))
            {
                OS = "Linux";
                OSIcon = "linux.png";
            }
            else if (Regex.IsMatch(userAgent, @"Android"))
            {
                OS = "Android";
                OSIcon = "android.png";
            }
            else if (Regex.IsMatch(userAgent, @"iOS"))
            {
                OS = "IOS";
                OSIcon = "ios.png";
            }
            else
            {
                OS = "未知";
                OSIcon = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userAgent"></param>
        private void DeviceInit(string userAgent)
        {
            // 判断设备类型
            if (Regex.IsMatch(userAgent, @"Windows NT") && Regex.IsMatch(userAgent, @"Win64; x64"))
            {
                Device = "台式机";
                DeviceIcon = "desktop.png";
            }
            else if (Regex.IsMatch(userAgent, @"Windows NT"))
            {
                Device = "笔记本";
                DeviceIcon = "laptop.png";
            }
            else if (Regex.IsMatch(userAgent, @"Mobile"))
            {
                Device = "手机";
                DeviceIcon = "mobile.png";
            }
            else if (Regex.IsMatch(userAgent, @"iPad|Android|tablet"))
            {
                Device = "平板";
                DeviceIcon = "tablet.png";
            }
            else
            {
                Device = "未知设备";
                DeviceIcon = "";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private readonly List<UserAgentMap> UaRegex = new()
        {
            new UserAgentMap("猎豹浏览器", "liebao.png", "LBBROWSER"),
            new UserAgentMap("QQ浏览器", "qq.png", " QQBrowser"),
            new UserAgentMap("百度浏览器", "baidu.png", "BIDUBrowser|baidubrowser|BaiduHD"),
            new UserAgentMap("UC浏览器", "uc.png", "UBrowser|UCBrowser|UCWEB"),
            new UserAgentMap("小米浏览器", "android.png", "MiuiBrowser"),
            new UserAgentMap("微信", "android.png", "MicroMessenger"),
            new UserAgentMap("手机QQ", "qq.png", "Mobile/\\w{5,}\\sQQ/(\\d+[.\\d]+)"),
            new UserAgentMap("手机百度", "baidu.png", "baiduboxapp"),
            new UserAgentMap("火狐浏览器", "firefox.png", "Firefox"),
            new UserAgentMap("360安全浏览器", "360.png", "360SE"),
            new UserAgentMap("360极速浏览器", "360.png", "360EE"),
            new UserAgentMap("Opera", "opera.png", "Opera|OPR/(\\d+[.\\d]+)"),
            new UserAgentMap("Microsoft Edge", "edge.png", "Edg"),
            new UserAgentMap("安卓浏览器", "android.png", "Android.*Mobile\\sSafari|Android/(\\d[.\\d]+)\\sRelease/(\\d[.\\d]+)\\sBrowser/AppleWebKit(\\d[.\\d]+)"),
            new UserAgentMap("IE浏览器", "ie.png", "Trident|MSIE"),
            new UserAgentMap("Chrome", "chrome.png", "Chrome|CriOS"),
            new UserAgentMap("Safari", "safari.png", "Version[|/](\\w.+)(\\s\\w.+)?\\s?Safari|like\\sGecko\\)\\sMobile/\\w{3,}$")
        };

        private class UserAgentMap
        {
            public string Client { get; }
            public string ClientIcon { get; }
            public string Value { get; }

            public UserAgentMap(string client, string clientIcon, string value)
            {
                Client = client;
                ClientIcon = clientIcon;
                Value = value;
            }
        }
    }
}
