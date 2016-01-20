# EightSphere.BlackLists
Orchard CMS module to block incoming request by IP or by referer. Can be helpful to prevent referer spam. 

Developed and maintained by <a href="http://www.8sph.com">8th Sphere</a>

## Key features
* IP black/white lists.
* Wildcards support in ip black/white lists. E.g. 192.168.1.* 
* Referer black/white lists.
* Incoming requests logging to file.
* Referer bot detector.


## Referer bot detector
IP address will be marked as spam if it sends more than some number of requests with different non-whitelisted referrers during a given session

## Add ip/referer to black list programmatically

Use `IBlackListsService.Ban(string ip, IEnumerable<string> referers, string banReason);` method.

You can also access BlackListsSettings by following code:

```
var settings = _services.WorkContext.CurrentSite.As<BlackListsSettingsPart>();
settings.EnableRequestLogging = model.EnableRequestLogging;
            
_signals.Trigger(BlackListsSettings.CacheKey);  
```

