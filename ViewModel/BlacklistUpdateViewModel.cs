using System.ComponentModel.DataAnnotations;

namespace EightSphere.BlackLists.ViewModel
{
    public class BlacklistUpdateViewModel
    {
        public string IpBlackList { get; set; }
        public string IpWhiteList { get; set; }
        public string RefererBlackList { get; set; }
        public string RefererWhiteList { get; set; }
        public bool EnableRefererBotDetector { get; set; }
        public bool EnableRequestLogging { get; set; }
        public bool AutomaticAddItemToBlacklist { get; set; }
        public int BotDetectorDistinctReferersCount { get; set; }
        public int BotDetectorSessionLengthInSeconds { get; set; }
    }
}