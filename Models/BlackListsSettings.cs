using System;
using System.Collections.Generic;
using System.Linq;
using Orchard.ContentManagement;

namespace EightSphere.BlackLists.Models
{
    public class BlackListsSettings : ContentPart
    {
        internal const string CacheKey = "EightSphere_BlackLists_BlackListsSettings";

        public BlackListsSettings(BlackListsSettingsPart part)
        {
            if (part == null)
                return;
            EnableRequestLogging = part.EnableRequestLogging;
            EnableRefererBotDetector = part.EnableRefererBotDetector;
            IpBlackList = part.IpBlackList;
            IpWhiteList = part.IpWhiteList;
            RefererWhiteList = part.RefererWhiteList;
            RefererBlackList = part.RefererBlackList;
            AutomaticAddItemToBlacklist = part.AutomaticAddItemToBlacklist;
            BotDetectorDistinctReferersCount = part.BotDetectorDistinctReferersCount;
            BotDetectorSessionLengthInSeconds = part.BotDetectorSessionLengthInSeconds;
        }

        public bool EnableRequestLogging { get; private set; }
        public bool EnableRefererBotDetector { get; private set; }
        public bool AutomaticAddItemToBlacklist { get; private set; }
        public string IpBlackList { get; private set; }
        public string IpWhiteList { get; private set; }
        public string RefererWhiteList { get; private set; }
        public string RefererBlackList { get; private set; }
        public int BotDetectorDistinctReferersCount { get; private set; }
        public int BotDetectorSessionLengthInSeconds { get; private set; }
    }
}