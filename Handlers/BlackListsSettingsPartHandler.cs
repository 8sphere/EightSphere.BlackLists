using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web;
using EightSphere.BlackLists.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;

namespace EightSphere.BlackLists.Handlers
{

    public class BlackListsSettingsPartHandler : ContentHandler
    {
        public readonly List<string> DefaultHostsWhiteList = new List<string>
        {
            "google.*",
            "t.co",
            "twitter.com",
            "facebook.com",
            "*.yahoo.*",
            "baidu.*",
            "bing.com",
            "linkedin.com",
            "*.linkedin.com"
        };

        public BlackListsSettingsPartHandler()
        {
            Filters.Add(new ActivatingFilter<BlackListsSettingsPart>("Site"));            
            // Default settings values.
            OnInitializing<BlackListsSettingsPart>((context, part) =>
            {
                part.IpWhiteList = "127.0.0.1";
                part.RefererWhiteList = string.Join(Environment.NewLine, DefaultHostsWhiteList);
                part.IpBlackList = "";
                part.RefererBlackList = "";
                part.AutomaticAddItemToBlacklist = false;
                part.EnableRequestLogging = false;
                part.EnableRefererBotDetector = false;
                part.BotDetectorSessionLenghtInMinutes = 2;
                part.BotDetectorDistinctReferersCount = 3;
            });
        }
    }
}