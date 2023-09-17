using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSHelper.Common
{
    public class MessagesContent
    {
        [Required]
        public string Expired { get; set; }
        [Required]

        public string Inactive7 { get; set; }
        [Required]

        public string  Inactive15 { get; set; }

    }

}
