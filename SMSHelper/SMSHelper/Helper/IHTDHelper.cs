using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SMSHelper.Helper
{
    internal interface IHTDHelper
    {
        Task<List<string>> Send(string phoneNumber, string txt);
        Task<string?> GetCredit();
    }
    
}
