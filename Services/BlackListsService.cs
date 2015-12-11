using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Hosting;
using EightSphere.BlackLists.Models;
using Orchard;
using Orchard.Caching;
using Orchard.Logging;
using Orchard.ContentManagement;

namespace EightSphere.BlackLists.Services
{
    public interface IBlackListsService : IDependency
    {
        string GetIp();
        bool IsRequrestBlacklisted();
        bool IsInWiteList();
      
        void LogRequest(string message = "OK");
        void AddRequestToHistory();
        void Ban(string ip = "", IEnumerable<string> referers = null, string banReason = "");
    }

    public class BlackListsService : IBlackListsService
    {
        private readonly IOrchardServices _services;
        private readonly ICacheManager _cacheManager;
        private readonly ISignals _signals;
        private static readonly object syncRoot = new object();
        private static List<RequestHistoryRecord> _requestHistory = new List<RequestHistoryRecord>(); 

        private static readonly string IpListFilesPath = "~/App_Data/blacklists";

        private static List<Regex> _ipBlackList;
        private static List<Regex> _ipWhiteList;
        private static List<Regex> _referersBlackList;
        private static List<Regex> _referersWhiteList;
        public ILogger Logger { get; set; }

        static BlackListsService()
        {               
        }

        BlackListsSettings _cacheSettings ;
        

        public BlackListsService(IOrchardServices services, ICacheManager cacheManager, ISignals signals)
        {
            _services = services;
            _cacheManager = cacheManager;
            _signals = signals;
            Logger = NullLogger.Instance;
            _cacheSettings = BlackListsSettings;
        }

        private BlackListsSettings BlackListsSettings
        {
            get
            {
                return _cacheSettings ?? (_cacheSettings = _cacheManager.Get(BlackListsSettings.CacheKey, true, context => {
                    context.Monitor(_signals.When(BlackListsSettings.CacheKey));
                    var blackListSettings = new BlackListsSettings(_services.WorkContext.CurrentSite.As<BlackListsSettingsPart>());
                    ReloadLists(blackListSettings);
                    return blackListSettings;
                }));
            }
        }

        public void ReloadLists(BlackListsSettings settings)
        {
            _ipBlackList = LoadList(settings.IpBlackList).Select(x => IpToRegex(x)).ToList();
            _ipWhiteList = LoadList(settings.IpWhiteList).Select(x => IpToRegex(x)).ToList();
            _referersBlackList = LoadList(settings.RefererBlackList).Select(x => HostToRegex(x)).ToList();
            _referersWhiteList = LoadList(settings.RefererWhiteList).Select(x => HostToRegex(x)).ToList();
        }

        private void Log(string message)
        {
            var log = EnsureFile("blacklist_log.txt");
            File.AppendAllLines(log, new string[] { "[" + DateTime.UtcNow.ToString("O") + "]: " + message + Environment.NewLine });
        }

        private static Regex IpToRegex(string ip)
        {
            //remove comments
            ip = Regex.Replace(ip, @"\s+#.+$", "");
            var x = "^" + ip.Trim().EscapeRegex().Replace("*", @"\d+").Trim() + "$";
            return new Regex(x, RegexOptions.Compiled);
        }

        // allow wildchar to code letters like  *.google.*
        private static Regex HostToRegex(string host)
        {
            //remove commets
            host = Regex.Replace(host, @"\s+#.+$", "");
            // convert to regex
            var x = "^" + host.Trim().EscapeRegex().Replace("*", ".*") + "$";           
            return new Regex(x, RegexOptions.Compiled);
        }

        public string GetReferer()
        {
            var referer =  HttpContext.Current.Request.UrlReferrer != null ? HttpContext.Current.Request.UrlReferrer.Host : "";
            referer = Regex.Replace(referer, @"^www\.", "");
            return referer.ToLower();
        }

        public string GetIp()
        {
            var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
            if (string.IsNullOrEmpty(ip))
            {
                ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
            }
            ip = Regex.Replace(ip, ":.*$", "");
            return ip;
        }

        public void LogRequest(string message = "OK")
        {
            if (!BlackListsSettings.EnableRequestLogging)
            {
                return;
            }
            var ip = GetIp();
            var log = EnsureFile($"requestLog-{DateTime.UtcNow.ToString("yyyyMMdd")}.csv");
            var logRecord = new[]
            {                
                DateTime.UtcNow.ToString("yyyy.MM.dd hh:mm:ss"),                
                ip,
                HttpContext.Current.Request.UserHostAddress,
                GetReferer(),
                HttpContext.Current.Request.RawUrl,
                message,
                HttpContext.Current.Request.UserAgent != null
                    ? HttpContext.Current.Request.UserAgent.Replace(";", ",")
                    : ""
            };

            File.AppendAllLines(log, new List<string>()
            {
                string.Join(";", logRecord)
            });
        }
               
        private static List<string> LoadList(string content)
        {
            var lines = content.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return lines
                .Select(x => Regex.Replace(x, @"\s+#.*$", ""))
                .Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
        }

        private static string EnsureFile(string fileName)
        {
            var folder = HostingEnvironment.MapPath(IpListFilesPath);
            if (folder == null) { return ""; }
            var path = Path.Combine(folder, fileName);
            if (!File.Exists(path))
            {
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                File.Create(path).Dispose();
            }
            return path;
        }

        public void AddRequestToHistory()
        {
            var ip = GetIp();
            var referer = GetReferer();
            _requestHistory.Add(new RequestHistoryRecord
            {
                Ip = ip,
                Referer = referer,
                RawUrl = HttpContext.Current.Request.RawUrl
            });
        }

        public bool IsRequrestBlacklisted()
        {
            var ip = GetIp();
            var referer = GetReferer();
            var rawUrl = HttpContext.Current.Request;
            // check ip
            if (_ipBlackList.Any(x => x.IsMatch(ip)))
            {
                LogRequest("BLOCKED BY IP " + ip);
                if (!string.IsNullOrWhiteSpace(referer) && BlackListsSettings.AutomaticAddItemToBlacklist)
                {
                    Ban(referers: new[] {referer}, banReason: $"Blocked by ip {ip}");
                }
                return true;
            }

            // skip referer check if it is empty
            if (string.IsNullOrWhiteSpace(referer))
            {
                return false;
            }

            // check referer
            if (_referersBlackList.Any(x => x.IsMatch(referer)))
            {
                LogRequest("BLOCKED BY Referer " + referer);
                if (BlackListsSettings.AutomaticAddItemToBlacklist)
                {
                    Ban(ip: ip, banReason: $"Blocked by referer {referer}");
                }
                return true;
            }
            if (BlackListsSettings.EnableRefererBotDetector)
            {
                var botFound = DetectRefererBot(ip);
                if (botFound)
                {
                    return true;
                }
            }
            return false;

        }

        private bool DetectRefererBot(string ip)
        {
            var thisIpLog = _requestHistory.Where(x => x.Ip == ip && !string.IsNullOrWhiteSpace(x.Referer)).ToList();
            // check if this ip has whitelisted referers in history
            var pastRef = thisIpLog.FirstOrDefault(x => _referersWhiteList.Any(m => m.IsMatch(x.Referer)));
            if (pastRef != null)
            {
                //Do not block guest
                LogRequest("OK: has whitelisted refs in the past " + pastRef.Referer);
                return false;
            }

            //try to detect multi referer bot (one ip many referers)
            var referers =
                thisIpLog.Where(x => x.Date > DateTime.UtcNow.AddMinutes(-2))
                    .Select(x => x.Referer)
                    .Distinct()
                    .ToList();
            if (referers.Count > 1)
            {
                var banReason = "Referer bot: " + string.Join(", ", referers);
                LogRequest("BANNED:" + banReason);
                lock (syncRoot)
                {
                    Ban(ip, referers, banReason);
                }
            }

            return true;
        }

        public void Ban(string ip = "", IEnumerable<string> referers = null, string banReason = "")
        {
            if (!string.IsNullOrWhiteSpace(banReason))
            {
                banReason = $" # {banReason}";
            }
            var settings = _services.WorkContext.CurrentSite.As<BlackListsSettingsPart>();
            bool settingsChanged = false;
            if (!string.IsNullOrWhiteSpace(ip) && !_ipBlackList.Any(x => x.IsMatch(ip)) && !_ipWhiteList.Any(x => x.IsMatch(ip)))
            {
                _ipBlackList.Add(IpToRegex(ip));                
                settings.IpBlackList = AddLines(settings.IpBlackList, ip + banReason);
                settingsChanged = true;
            }

            if (referers != null)
            {
                var refsToBan = referers.Where(referer =>
                    !_referersBlackList.Any(x => x.IsMatch(referer)) &&
                    !_ipWhiteList.Any(x => x.IsMatch(referer))
                    ).ToList();
                if (refsToBan.Any())
                {
                    foreach (var refToBan in refsToBan)
                    {
                        _referersBlackList.Add(HostToRegex(refToBan));
                    }

                    settings.RefererBlackList = AddLines(settings.RefererBlackList, refsToBan.Select(x => x + banReason).ToArray());
                    settingsChanged = true;
                }
            }
            if (settingsChanged)
            {
                _signals.Trigger(BlackListsSettings.CacheKey);
            }
        }

        private string AddLines(string content, params string[] lines)
        {
            var sb = new StringBuilder(content);
            if (!content.EndsWith(Environment.NewLine))
            {
                sb.Append(Environment.NewLine);
            }
            foreach (var line in lines)
            {
                sb.AppendLine(line);
            }
            return sb.ToString();
        }

        public bool IsInWiteList()
        {
            var ip = GetIp();
            if (_ipWhiteList.Any(x => x.IsMatch(ip)))
            {
                LogRequest("WL by ip " + ip);
                return true;
            }
            var referer = GetReferer();
            if (_referersWhiteList.Any(x => x.IsMatch(referer)))
            {
                LogRequest("WL by referer " + referer);
                return true;
            }
            return false;
        }
    }
}