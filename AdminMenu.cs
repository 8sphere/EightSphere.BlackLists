﻿using Orchard.Localization;
using Orchard.Security;
using Orchard.UI.Navigation;

namespace EightSphere.BlackLists
{
    public class AdminMenu : INavigationProvider
    {
        public Localizer T { get; set; }

        public string MenuName
        {
            get { return "admin"; }
        }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder
                .Add(T("Settings"), menu => menu
                    .Add(T("Black Lists"), "10.0",
                        subMenu =>
                            subMenu.Action("Index", "Admin", new {area = "EightSphere.BlackLists" })
                                .Permission(StandardPermissions.SiteOwner)
                    ));
        }
    }
}
