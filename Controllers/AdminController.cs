using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EightSphere.BlackLists.Services;
using Orchard.UI.Admin;
using Orchard.ContentManagement;
using EightSphere.BlackLists.Models;
using EightSphere.BlackLists.ViewModel;
using Orchard;
using Orchard.Caching;

namespace EightSphere.BlackLists.Controllers
{
    [Admin]
    public class AdminController : Controller
    {
        private readonly IBlackListsService _blackListService;
        private readonly IOrchardServices _services;
        private readonly ISignals _signals;

        public AdminController(IBlackListsService blackListService, IOrchardServices services, ISignals signals)
        {
            _blackListService = blackListService;
            _services = services;
            _signals = signals;
        }

        // GET: Admin
        public ActionResult Index()
        {
            var settings = _services.WorkContext.CurrentSite.As<BlackListsSettingsPart>();
            var viewModel = new BlacklistUpdateViewModel
            {
                IpBlackList = settings.IpBlackList,
                IpWhiteList = settings.IpWhiteList,
                RefererBlackList = settings.RefererBlackList,
                RefererWhiteList = settings.RefererWhiteList,
                EnableRefererBotDetector = settings.EnableRefererBotDetector,
                EnableRequestLogging = settings.EnableRequestLogging,
                AutomaticAddItemToBlacklist = settings.AutomaticAddItemToBlacklist
            };
            
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Index(BlacklistUpdateViewModel model)
        {
            var settings = _services.WorkContext.CurrentSite.As<BlackListsSettingsPart>();
            settings.IpBlackList = model.IpBlackList;
            settings.IpWhiteList = model.IpWhiteList;

            settings.RefererBlackList = model.RefererBlackList;
            settings.RefererWhiteList = model.RefererWhiteList;

            settings.EnableRefererBotDetector = model.EnableRefererBotDetector;
            settings.EnableRequestLogging = model.EnableRequestLogging;
            settings.AutomaticAddItemToBlacklist = model.AutomaticAddItemToBlacklist;
            _signals.Trigger(BlackListsSettings.CacheKey);            
            return RedirectToAction("Index");
        }
    }
}