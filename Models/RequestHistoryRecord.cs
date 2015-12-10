using System;

namespace EightSphere.BlackLists.Models
{
    internal class RequestHistoryRecord
    {
        public DateTime Date { get; set; }
        public string Ip { get; set; }
        public string Referer { get; set; }
        public string RawUrl { get; set; }

        public RequestHistoryRecord()
        {
            Date = DateTime.UtcNow;
        }
    }
}