using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using EightSphere.BlackLists.Models;
using EightSphere.BlackLists.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Mvc.Filters;

namespace EightSphere.BlackLists.Filters
{
    public class BlackListFilter : FilterProvider, IActionFilter
    {
        private readonly IBlackListsService _blackListsService;
        private readonly IOrchardServices _services;

        public BlackListFilter(IBlackListsService blackListsService, IOrchardServices services)
        {
            _blackListsService = blackListsService;
            _services = services;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            
            string ip = _blackListsService.GetIp();
            if (string.IsNullOrEmpty(ip))
            {
                return;
            }            
            _blackListsService.AddRequestToHistory();
            if (_blackListsService.IsInWiteList())
            {
                return;
            }
            if (_blackListsService.IsRequrestBlacklisted())
            {
                filterContext.Result = new HttpStatusCodeResult(200);                                
                return;
            }
            _blackListsService.LogRequest();
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}