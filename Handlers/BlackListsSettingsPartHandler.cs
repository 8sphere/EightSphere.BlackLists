using System.Globalization;
using EightSphere.BlackLists.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Handlers;
using Orchard.Core.Common.Models;

namespace EightSphere.BlackLists.Handlers
{
    public class BlackListsSettingsPartHandler : ContentHandler
    {


        public BlackListsSettingsPartHandler()
        {

            Filters.Add(new ActivatingFilter<BlackListsSettingsPart>("Site"));

            // Default settings values.
            OnInitializing<BlackListsSettingsPart>((context, part) =>
            {
                part.EnableRefererBotDetector = false;
                part.IpWhiteList = "127.0.0.1";
                part.RefererWhiteList = "google.*";
            });

        }
    }
}