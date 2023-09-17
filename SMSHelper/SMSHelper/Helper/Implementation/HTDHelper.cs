using Amazon.Runtime;
using Amazon.Runtime.Internal;
using Microsoft.Extensions.Configuration;
using SMSHelper.Common;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace SMSHelper.Helper.Implementation
{

    public class HTDHelper : IHTDHelper
    {
        IConfiguration _configuration;

        public HTDHelper(IConfiguration configuration)
        {
            _configuration = configuration;


        }


        public async Task<string?> GetCredit()
        {
            List<string> successPhones = new();
            HTDParams htdParams = _configuration.GetSection("HTDParams").Get<HTDParams>();

            using (HttpClient client = new HttpClient())
            {
                string url = htdParams.CreditURL!;


                // Send the POST request
                HttpResponseMessage response = await client.GetAsync(url);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    var  responseContent = await response.Content.ReadAsStringAsync();

                   return  responseContent;



                }
                return null;

            }


        }

        public async Task<List<string>> Send(string phoneNumbers, string txt)
        {
            List<string> successPhones = new();
            HTDParams htdParams = _configuration.GetSection("HTDParams").Get<HTDParams>();

            using (HttpClient client = new HttpClient())
            {
                string url = htdParams.URL!;

                // Create a dictionary of parameters that you want to send in the request
                var parameters = new Dictionary<string, string>
                {
                    { "id", htdParams.Id!},
                    { "sender", htdParams.Sender! },
                    { "to", phoneNumbers },
                    { "msg", txt },
                    { "mode", "1" }
                };


                var content = new FormUrlEncodedContent(parameters);

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync(url, content);

                // Check if the response was successful
                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();


                    var res = responseContent.Substring(3, responseContent.Length - 3);
                    string[] tokens1 = res.Split(';');
                    foreach (string token in tokens1)
                    {
                        string[] tokens2 = token.Split(':');
                        if (tokens2[1] != "INVALID")
                        {
                            successPhones.Add(tokens2[0].Substring(2));
                        }

                    }

                }

                return successPhones;
            }

        }
    }
}
