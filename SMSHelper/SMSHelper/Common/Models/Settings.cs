using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSHelper.Common.Models
{
    public class Settings
    {
        public Guid Id { get; set; }
        public int AvailableSMSBalance { get; set; }
        public bool SendToExpired { get; set; }
        public bool SendToInactive15 { get; set; }
        public bool SendToInactive7 { get; set; }
        public string SendToExpiredTxt { get; set; }
        public string SendToInactive15Txt { get; set; }
        public string SendToInactive7Txt { get; set; }
    }
}
