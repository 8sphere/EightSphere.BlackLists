using System;
using Orchard.ContentManagement;

namespace EightSphere.BlackLists.Models
{
    public class BlackListsSettingsPart : ContentPart
    {
        public string IpBlackList
        {
            get { return this.Retrieve(x => x.IpBlackList); }
            set { this.Store(x => x.IpBlackList, value); }
        }

        public string IpWhiteList
        {
            get { return this.Retrieve(x => x.IpWhiteList); }
            set { this.Store(x => x.IpWhiteList, value); }
        }

        public string RefererBlackList
        {
            get { return this.Retrieve(x => x.RefererBlackList); }
            set { this.Store(x => x.RefererBlackList, value); }
        }

        public string RefererWhiteList
        {
            get { return this.Retrieve(x => x.RefererWhiteList); }
            set { this.Store(x => x.RefererWhiteList, value); }
        }

        public bool EnableRefererBotDetector
        {
            get { return this.Retrieve(x => x.EnableRefererBotDetector); }
            set { this.Store(x => x.EnableRefererBotDetector, value); }
        }

        public bool EnableRequestLogging
        {
            get { return this.Retrieve(x => x.EnableRequestLogging); }
            set { this.Store(x => x.EnableRequestLogging, value); }
        }

        public bool AutomaticAddItemToBlacklist
        {
            get { return this.Retrieve(x => x.AutomaticAddItemToBlacklist); }
            set { this.Store(x => x.AutomaticAddItemToBlacklist, value); }
        }
    }    
}