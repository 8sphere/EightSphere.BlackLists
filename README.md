# EightSphere.BlackLists
Orchard CMS module to block incoming request by IP or by referer. Can be helpful to prevent referer spam. 

## Key features
* IP black/white lists.
* Wildcards support in ip black/white lists. E.g. 192.168.1.* 
* Referer black/white lists.
* Incoming requests logging to file.
* Referer bot detector.


## Referer bot detector
IP recognized as bot's if it sends more then 2 requests with different non whitelisted refereres last 2 minutes.



